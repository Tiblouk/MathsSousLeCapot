using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.App.Features.Assessments;
using MathsSousLeCapot.App.Features.Settings;
using MathsSousLeCapot.App.Features.Progress;
using MathsSousLeCapot.App.Features.Profiles;
using MathsSousLeCapot.App.Features.Challenges;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Progress;
using Microsoft.Maui.Layouts;

namespace MathsSousLeCapot.App.Features.Courses;

/// <summary>
/// Présente les cours par grand ensemble, classe et catégorie.
/// </summary>
public partial class ChapterPage : ContentPage
{
    /// <summary>
    /// Service qui filtre l'index local sans construire les pages de cours.
    /// </summary>
    private readonly CourseSearchService _searchService = new();

    /// <summary>
    /// Service qui lit la progression du profil actif lors de l'affichage du menu.
    /// </summary>
    private readonly LocalStorageService _storage = new();

    /// <summary>
    /// Meilleur accomplissement connu pour chaque cours du profil actif.
    /// </summary>
    private IReadOnlyDictionary<string, CourseAchievementStatus> _courseStatuses =
        new Dictionary<string, CourseAchievementStatus>(StringComparer.Ordinal);

    /// <summary>
    /// Vues déjà créées à rafraîchir après un entraînement ou un changement de profil.
    /// </summary>
    private readonly Dictionary<string, List<WeakReference<CourseStatusVisual>>>
        _courseStatusViews = new(StringComparer.Ordinal);

    /// <summary>
    /// Options traduites présentées par le filtre de tags.
    /// </summary>
    private IReadOnlyList<TagFilterOption> _tagOptions = [];

    /// <summary>
    /// Initialise la liste des cours.
    /// </summary>
    public ChapterPage()
    {
        InitializeComponent();
        InitializeSearch();
        BuildCourseList();
    }

    /// <summary>
    /// Actualise le profil affiché chaque fois que l'utilisateur revient au menu.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ActiveProfileLabel.Text = TranslationService.Current.Format(
            "profile.current",
            LocalProfileService.Current.ActiveProfile.Name);
        RefreshCourseStatuses();
    }

    /// <summary>
    /// Recalcule les badges puis actualise les cartes déjà matérialisées.
    /// </summary>
    private void RefreshCourseStatuses()
    {
        _courseStatuses = CourseAchievementCalculator.Calculate(
            _storage.GetCourseProgress(),
            _storage.GetPerfectTrainingAchievements());

        foreach (var courseViews in _courseStatusViews)
        {
            var status = GetCourseStatus(courseViews.Key);
            for (var index = courseViews.Value.Count - 1; index >= 0; index--)
            {
                if (courseViews.Value[index].TryGetTarget(out var visual))
                {
                    ApplyCourseStatus(visual, status);
                }
                else
                {
                    courseViews.Value.RemoveAt(index);
                }
            }
        }
    }

    /// <summary>
    /// Prépare le filtre avec une option globale suivie des tags connus.
    /// </summary>
    private void InitializeSearch()
    {
        var text = TranslationService.Current;
        _tagOptions =
        [
            new TagFilterOption(null, text.Get("search.allTags")),
            .. CourseSearchCatalog.Tags.Select(tag =>
                new TagFilterOption(tag.Id, text.Get(tag.LabelKey)))
        ];
        TagPicker.ItemsSource = _tagOptions.ToList();
        TagPicker.SelectedIndex = 0;
        SearchStatusLabel.Text = text.Get("search.hint");
    }

    /// <summary>
    /// Relance la recherche lors de la modification du texte.
    /// </summary>
    private void OnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateSearchResults();
    }

    /// <summary>
    /// Relance la recherche lors de la sélection d'un tag.
    /// </summary>
    private void OnTagChanged(object? sender, EventArgs e)
    {
        UpdateSearchResults();
    }

    /// <summary>
    /// Reconstruit uniquement les cartes correspondant aux critères actifs.
    /// </summary>
    private void UpdateSearchResults()
    {
        if (TagPicker.SelectedIndex < 0 || _tagOptions.Count == 0)
        {
            return;
        }

        var query = CourseSearchBar.Text;
        var tagId = _tagOptions[TagPicker.SelectedIndex].TagId;
        SearchResultsPanel.Children.Clear();
        if (string.IsNullOrWhiteSpace(query) && tagId is null)
        {
            SearchStatusLabel.Text = TranslationService.Current.Get("search.hint");
            return;
        }

        var results = _searchService.Search(query, tagId);
        SearchStatusLabel.Text = results.Count == 0
            ? TranslationService.Current.Get("search.noResult")
            : TranslationService.Current.Format("search.resultCount", results.Count);
        foreach (var result in results)
        {
            SearchResultsPanel.Children.Add(CreateSearchResultCard(result));
        }
    }

    /// <summary>
    /// Crée une carte de résultat depuis les seules métadonnées indexées.
    /// </summary>
    private Border CreateSearchResultCard(CourseSearchEntry entry)
    {
        var text = TranslationService.Current;
        var chapter = CourseCatalog.ChapterHeaders.Single(
            item => item.Id == entry.ChapterId);
        var openButton = new Button
        {
            CommandParameter = entry.CourseId,
            Text = text.Get("actions.open"),
            VerticalOptions = LayoutOptions.Center
        };
        openButton.Clicked += OnOpenCourseClicked;
        Grid.SetColumn(openButton, 1);

        var tags = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap
        };
        foreach (var tagId in entry.TagIds)
        {
            var definition = CourseSearchCatalog.Tags.Single(tag => tag.Id == tagId);
            tags.Children.Add(new Border
            {
                BackgroundColor = ThemeService.GetColor("PrimaryLight"),
                Stroke = ThemeService.GetColor("Border"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = 10
                },
                Padding = new Thickness(8, 4),
                Margin = new Thickness(0, 0, 6, 6),
                Content = new Label
                {
                    FontSize = 12,
                    Text = text.Get(definition.LabelKey)
                }
            });
        }

        var badge = CreateStatusBadge(out var badgeLabel);
        var details = new VerticalStackLayout
        {
            Spacing = 6,
            Children =
            {
                new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 13,
                    Text = text.Get(entry.LevelKey),
                    TextColor = ThemeService.GetColor("Success")
                },
                new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    Text = text.Get(entry.TitleKey)
                },
                badge,
                new Label
                {
                    Text = text.Get(chapter.Title),
                    TextColor = ThemeService.GetColor("TextSecondary")
                },
                tags
            }
        };

        var card = new Border
        {
            Style = (Style)Application.Current!.Resources["Card"],
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 14,
                Children =
                {
                    details,
                    openButton
                }
            }
        };
        RegisterCourseStatusView(entry.CourseId, card, badge, badgeLabel);
        return card;
    }

    /// <summary>
    /// Construit les accordéons repliés des ensembles, classes et catégories.
    /// </summary>
    private void BuildCourseList()
    {
        var chaptersById = CourseCatalog.ChapterHeaders.ToDictionary(
            chapter => chapter.Id,
            StringComparer.Ordinal);

        foreach (var schoolGroup in SchoolGroupCatalog.Definitions
            .OrderBy(group => group.Order))
        {
            var schoolContent = new VerticalStackLayout
            {
                IsVisible = false,
                Padding = new Thickness(12, 0, 0, 8),
                Spacing = 14
            };
            Button? schoolButton = null;
            schoolButton = CreateDisclosureButton(
                TranslationService.Current.Get(schoolGroup.Title),
                null,
                schoolContent,
                DisclosureKind.SchoolGroup,
                () =>
                {
                    var totalCount = 0;
                    foreach (var level in schoolGroup.Levels)
                    {
                        var courseCount = CourseCatalog.GetCourseCount(level);
                        if (courseCount == 0)
                        {
                            continue;
                        }

                        totalCount += courseCount;
                        if (schoolGroup.Levels.Count == 1)
                        {
                            AddLevelSections(
                                schoolContent,
                                level,
                                CourseCatalog.GetCoursesForLevel(level),
                                chaptersById);
                            continue;
                        }

                        var levelContent = new VerticalStackLayout
                        {
                            IsVisible = false,
                            Padding = new Thickness(12, 0, 0, 8),
                            Spacing = 12
                        };
                        var levelButton = CreateDisclosureButton(
                            TranslationService.Current.Get(level),
                            courseCount,
                            levelContent,
                            DisclosureKind.Level,
                            () => AddLevelSections(
                                levelContent,
                                level,
                                CourseCatalog.GetCoursesForLevel(level),
                                chaptersById));

                        schoolContent.Children.Add(levelButton);
                        schoolContent.Children.Add(levelContent);
                    }

                    if (schoolButton?.CommandParameter is DisclosureState state)
                    {
                        state.CourseCount = totalCount;
                    }
                });
            CoursesPanel.Children.Add(schoolButton);
            CoursesPanel.Children.Add(schoolContent);
        }
    }

    /// <summary>
    /// Ajoute les contrôles puis les catégories de cours d'une classe.
    /// </summary>
    private void AddLevelSections(
        VerticalStackLayout parent,
        string level,
        IEnumerable<Course> courses,
        IReadOnlyDictionary<string, Chapter> chaptersById)
    {
        var levelCourses = courses.ToArray();
        AddAssessmentSection(parent, level);
        AddCategorySections(parent, levelCourses, chaptersById);
    }

    /// <summary>
    /// Ajoute la section repliée des contrôles intermédiaires du niveau.
    /// </summary>
    private static void AddAssessmentSection(
        VerticalStackLayout parent,
        string level)
    {
        var text = TranslationService.Current;
        var assessmentCardsPanel = new VerticalStackLayout
        {
            Padding = new Thickness(16, 0, 0, 8),
            Spacing = 12
        };
        var assessmentContent = new VerticalStackLayout
        {
            IsVisible = false,
            Spacing = 8,
            Children =
            {
                new Label
                {
                    Text = text.Get("assessment.class.sectionIntro"),
                    TextColor = ThemeService.GetColor("TextSecondary")
                },
                assessmentCardsPanel
            }
        };
        var assessmentButton = CreateDisclosureButton(
            text.Get("assessment.class.sectionTitle"),
            1,
            assessmentContent,
            DisclosureKind.Category,
            () => assessmentCardsPanel.Children.Add(
                CreateAssessmentCard(level)));

        parent.Children.Add(assessmentButton);
        parent.Children.Add(assessmentContent);
    }

    /// <summary>
    /// Crée la carte ouvrant le contrôle général de la classe.
    /// </summary>
    private static Border CreateAssessmentCard(string level)
    {
        var text = TranslationService.Current;
        var openButton = new Button
        {
            CommandParameter = level,
            Text = text.Get("assessment.class.open"),
            VerticalOptions = LayoutOptions.Center
        };
        openButton.Clicked += OnOpenAssessmentClicked;
        Grid.SetColumn(openButton, 1);

        return new Border
        {
            Style = (Style)Application.Current!.Resources["Card"],
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 18,
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 13,
                                Text = text.Get(level),
                                TextColor = ThemeService.GetColor("Success")
                            },
                            new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 23,
                                Text = text.Get("assessment.class.cardTitle")
                            },
                            new Label
                            {
                                Text = text.Get("assessment.class.cardSummary"),
                                TextColor = ThemeService.GetColor("TextSecondary")
                            }
                        }
                    },
                    openButton
                }
            }
        };
    }

    /// <summary>
    /// Ajoute les catégories et leurs cartes de cours à un niveau.
    /// </summary>
    private void AddCategorySections(
        VerticalStackLayout parent,
        IEnumerable<Course> courses,
        IReadOnlyDictionary<string, Chapter> chaptersById)
    {
        foreach (var chapterGroup in courses.GroupBy(course => course.ChapterId))
        {
            var chapter = chaptersById[chapterGroup.Key];
            var courseCardsPanel = new VerticalStackLayout
            {
                Padding = new Thickness(16, 0, 0, 8),
                Spacing = 12
            };
            var categoryContent = new VerticalStackLayout
            {
                IsVisible = false,
                Spacing = 8,
                Children =
                {
                    new Label
                    {
                        Text = TranslationService.Current.Get(chapter.Description),
                        TextColor = ThemeService.GetColor("TextSecondary")
                    },
                    courseCardsPanel
                }
            };
            var categoryButton = CreateDisclosureButton(
                TranslationService.Current.Get(chapter.Title),
                chapterGroup.Count(),
                categoryContent,
                DisclosureKind.Category,
                () =>
                {
                    foreach (var course in chapterGroup)
                    {
                        courseCardsPanel.Children.Add(
                            course.IsAvailable
                                ? CreateAvailableCourseCard(course)
                                : CreatePlannedCourseCard(course));
                    }
                });

            parent.Children.Add(categoryButton);
            parent.Children.Add(categoryContent);
        }
    }

    /// <summary>
    /// Crée un bouton qui déplie ou replie le contenu associé.
    /// </summary>
    private static Button CreateDisclosureButton(
        string title,
        int? courseCount,
        VisualElement content,
        DisclosureKind kind,
        Action? loadContent = null)
    {
        var button = new Button
        {
            CommandParameter = new DisclosureState(
                title,
                courseCount,
                content,
                loadContent),
            FontAttributes = FontAttributes.Bold,
            FontSize = kind switch
            {
                DisclosureKind.SchoolGroup => 26,
                DisclosureKind.Level => 22,
                _ => 18
            },
            Style = (Style)Application.Current!.Resources[kind switch
            {
                DisclosureKind.SchoolGroup => "SchoolGroupDisclosureButton",
                DisclosureKind.Level => "LevelDisclosureButton",
                _ => "CategoryDisclosureButton"
            }]
        };
        UpdateDisclosureButton(button, false);
        button.Clicked += OnDisclosureClicked;
        return button;
    }

    /// <summary>
    /// Bascule la visibilité d'un niveau ou d'une catégorie.
    /// </summary>
    private static void OnDisclosureClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: DisclosureState state
            } button)
        {
            return;
        }

        state.EnsureLoaded();
        state.Content.IsVisible = !state.Content.IsVisible;
        UpdateDisclosureButton(button, state.Content.IsVisible);
    }

    /// <summary>
    /// Actualise le chevron, le compteur et la description accessible du bouton.
    /// </summary>
    private static void UpdateDisclosureButton(Button button, bool isExpanded)
    {
        if (button.CommandParameter is not DisclosureState state)
        {
            return;
        }

        var text = TranslationService.Current;
        var count = state.CourseCount is { } courseCount
            ? $" ({courseCount})"
            : string.Empty;
        button.Text = $"{(isExpanded ? "▾" : "▸")}  {state.Title}{count}";
        SemanticProperties.SetDescription(
            button,
            text.Format(
                isExpanded
                    ? "accessibility.collapseSection"
                    : "accessibility.expandSection",
                state.Title,
                state.CourseCount ?? 0));
    }

    /// <summary>
    /// Crée une carte ouvrant la fiche d'un cours disponible.
    /// </summary>
    private Border CreateAvailableCourseCard(Course course)
    {
        var text = TranslationService.Current;
        var summary = PrimaryCourseCatalog.TryGet(course.Id, out var definition)
            ? text.Format(
                "course.primary.summaryFormat",
                text.Get(definition.Title))
            : MiddleSchoolCourseCatalog.TryGet(course.Id, out var middleDefinition)
                ? text.Format(
                    "course.middle.summaryFormat",
                    text.Get(middleDefinition.Title))
                : HighSchoolCourseCatalog.TryGet(course.Id, out var highDefinition)
                    ? text.Format(
                        "course.high.summaryFormat",
                        text.Get(highDefinition.Title))
                    : text.Get(course.Summary);
        var openButton = new Button
        {
            CommandParameter = course.Id,
            Text = text.Get("actions.open"),
            VerticalOptions = LayoutOptions.Center
        };
        openButton.Clicked += OnOpenCourseClicked;
        Grid.SetColumn(openButton, 1);
        SemanticProperties.SetDescription(
            openButton,
            $"{text.Get("actions.open")} : {text.Get(course.Title)}");
        var badge = CreateStatusBadge(out var badgeLabel);
        var details = new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 13,
                    Text = text.Get(course.Level),
                    TextColor = ThemeService.GetColor("Success")
                },
                new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 23,
                    Text = text.Get(course.Title)
                },
                badge,
                new Label
                {
                    Text = summary,
                    TextColor = ThemeService.GetColor("TextSecondary")
                }
            }
        };

        var card = new Border
        {
            Style = (Style)Application.Current!.Resources["Card"],
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 18,
                Children =
                {
                    details,
                    openButton
                }
            }
        };
        RegisterCourseStatusView(course.Id, card, badge, badgeLabel);
        return card;
    }

    /// <summary>
    /// Crée un badge masqué qui sera renseigné selon l'historique du cours.
    /// </summary>
    private static Border CreateStatusBadge(out Label label)
    {
        label = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 12
        };
        return new Border
        {
            HorizontalOptions = LayoutOptions.Start,
            IsVisible = false,
            Padding = new Thickness(9, 4),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 10
            },
            Content = label
        };
    }

    /// <summary>
    /// Enregistre une carte et lui applique immédiatement le statut du cours.
    /// </summary>
    private void RegisterCourseStatusView(
        string courseId,
        Border card,
        Border badge,
        Label badgeLabel)
    {
        var visual = new CourseStatusVisual(card, badge, badgeLabel);
        card.BindingContext = visual;
        if (!_courseStatusViews.TryGetValue(courseId, out var views))
        {
            views = [];
            _courseStatusViews[courseId] = views;
        }

        views.Add(new WeakReference<CourseStatusVisual>(visual));
        ApplyCourseStatus(visual, GetCourseStatus(courseId));
    }

    /// <summary>
    /// Retourne le statut calculé ou l'état non consulté par défaut.
    /// </summary>
    private CourseAchievementStatus GetCourseStatus(string courseId)
    {
        return _courseStatuses.TryGetValue(courseId, out var status)
            ? status
            : CourseAchievementStatus.Unseen;
    }

    /// <summary>
    /// Applique la couleur et le badge correspondant au meilleur accomplissement.
    /// </summary>
    private static void ApplyCourseStatus(
        CourseStatusVisual visual,
        CourseAchievementStatus status)
    {
        var appearance = status switch
        {
            CourseAchievementStatus.Read => new CourseStatusAppearance(
                "course.status.read",
                "CourseRead",
                "CourseReadBackground"),
            CourseAchievementStatus.PerfectSession => new CourseStatusAppearance(
                "course.status.perfect",
                "Success",
                "SuccessBackground"),
            CourseAchievementStatus.PerfectHardSession => new CourseStatusAppearance(
                "course.status.perfectHard",
                "CourseMasteredHard",
                "CourseMasteredHardBackground"),
            _ => null
        };

        if (appearance is null)
        {
            visual.Card.BackgroundColor = ThemeService.GetColor("CardBackground");
            visual.Card.Stroke = ThemeService.GetColor("Border");
            visual.Badge.IsVisible = false;
            return;
        }

        var color = ThemeService.GetColor(appearance.ColorKey);
        visual.Card.BackgroundColor = ThemeService.GetColor(appearance.BackgroundKey);
        visual.Card.Stroke = color;
        visual.Badge.BackgroundColor = ThemeService.GetColor(appearance.BackgroundKey);
        visual.Badge.Stroke = color;
        visual.BadgeLabel.Text = TranslationService.Current.Get(appearance.LabelKey);
        visual.BadgeLabel.TextColor = color;
        visual.Badge.IsVisible = true;
        SemanticProperties.SetDescription(visual.Badge, visual.BadgeLabel.Text);
    }

    /// <summary>
    /// Crée une carte informative pour un cours non encore disponible.
    /// </summary>
    private static Border CreatePlannedCourseCard(Course course)
    {
        var text = TranslationService.Current;
        return new Border
        {
            Style = (Style)Application.Current!.Resources["Card"],
            Content = new VerticalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        Text = text.Get(course.Title)
                    },
                    new Label
                    {
                        Text = text.Get("chapter.planned"),
                        TextColor = ThemeService.GetColor("TextSecondary")
                    }
                }
            }
        };
    }

    /// <summary>
    /// Ouvre la route associée au cours sélectionné.
    /// </summary>
    private static async void OnOpenCourseClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string courseId }
            || !NavigationService.TryGetCourseEntryRoute(courseId, out var route))
        {
            return;
        }

        await Shell.Current.GoToAsync(route);
    }

    /// <summary>
    /// Ouvre le contrôle intermédiaire associé au niveau sélectionné.
    /// </summary>
    private static async void OnOpenAssessmentClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string level })
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ClassAssessmentPage)}?level={Uri.EscapeDataString(level)}");
    }

    /// <summary>
    /// Ouvre les paramètres de langue et de thème.
    /// </summary>
    private async void OnOpenSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    /// <summary>
    /// Ouvre le tableau de progression du profil actif.
    /// </summary>
    private async void OnOpenProgressClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProgressPage));
    }

    /// <summary>
    /// Ouvre la gestion des profils locaux.
    /// </summary>
    private async void OnOpenProfilesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProfilesPage));
    }

    /// <summary>
    /// Ouvre les défis et le classement stockés sur l'appareil.
    /// </summary>
    private async void OnOpenChallengesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ChallengesPage));
    }

    /// <summary>
    /// Distingue les trois étages visuels de l'accordéon.
    /// </summary>
    private enum DisclosureKind
    {
        SchoolGroup,
        Level,
        Category
    }

    /// <summary>
    /// Mémorise les informations nécessaires au dépliage d'une section.
    /// </summary>
    private sealed class DisclosureState
    {
        /// <summary>
        /// Initialise l'état et l'éventuelle fabrique de contenu différé.
        /// </summary>
        public DisclosureState(
            string title,
            int? courseCount,
            VisualElement content,
            Action? loadContent)
        {
            Title = title;
            CourseCount = courseCount;
            Content = content;
            _loadContent = loadContent;
        }

        /// <summary>
        /// Titre, compteur et conteneur associés au bouton.
        /// </summary>
        public string Title { get; }
        public int? CourseCount { get; set; }
        public VisualElement Content { get; }

        /// <summary>
        /// Fabrique exécutée au premier déploiement seulement.
        /// </summary>
        private Action? _loadContent;

        /// <summary>
        /// Matérialise le niveau suivant une seule fois.
        /// </summary>
        public void EnsureLoaded()
        {
            var loadContent = _loadContent;
            _loadContent = null;
            loadContent?.Invoke();
        }
    }

    /// <summary>
    /// Associe la valeur technique d'un tag à son libellé traduit dans le sélecteur.
    /// </summary>
    private sealed record TagFilterOption(string? TagId, string Label)
    {
        /// <summary>
        /// Retourne uniquement le libellé visible dans le contrôle Picker.
        /// </summary>
        public override string ToString() => Label;
    }

    /// <summary>
    /// Regroupe les éléments graphiques à actualiser pour un badge de cours.
    /// </summary>
    private sealed record CourseStatusVisual(
        Border Card,
        Border Badge,
        Label BadgeLabel);

    /// <summary>
    /// Associe un libellé aux couleurs sémantiques d'un statut.
    /// </summary>
    private sealed record CourseStatusAppearance(
        string LabelKey,
        string ColorKey,
        string BackgroundKey);
}
