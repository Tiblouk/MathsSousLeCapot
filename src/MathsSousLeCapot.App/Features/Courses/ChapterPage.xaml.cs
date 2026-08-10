using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.App.Features.Assessments;
using MathsSousLeCapot.App.Features.Settings;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Features.Courses;

/// <summary>
/// Présente les cours par grand ensemble, classe et catégorie.
/// </summary>
public partial class ChapterPage : ContentPage
{
    /// <summary>
    /// Initialise la liste des cours.
    /// </summary>
    public ChapterPage()
    {
        InitializeComponent();
        BuildCourseList();
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
    private static void AddLevelSections(
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
    private static void AddCategorySections(
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
    private static Border CreateAvailableCourseCard(Course course)
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
                                Text = text.Get(course.Level),
                                TextColor = ThemeService.GetColor("Success")
                            },
                            new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 23,
                                Text = text.Get(course.Title)
                            },
                            new Label
                            {
                                Text = summary,
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
}
