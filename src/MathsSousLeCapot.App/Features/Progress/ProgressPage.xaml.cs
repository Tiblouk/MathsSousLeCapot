using System.Globalization;
using MathsSousLeCapot.App.Features.Challenges;
using MathsSousLeCapot.App.Features.Profiles;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Progress;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.Progress;

/// <summary>
/// Affiche la progression et l'historique du profil local actif.
/// </summary>
public partial class ProgressPage : ContentPage
{
    /// <summary>
    /// Nombre de sessions ajoutées à chaque étape du chargement progressif.
    /// </summary>
    private const int HistoryPageSize = 10;

    /// <summary>
    /// Service qui lit les fichiers du profil sélectionné.
    /// </summary>
    private readonly LocalStorageService _storage = new();

    /// <summary>
    /// Sessions triées utilisées pour le chargement progressif de l'historique.
    /// </summary>
    private IReadOnlyList<TrainingSession> _sessions = [];

    /// <summary>
    /// Nombre de sessions actuellement visibles.
    /// </summary>
    private int _visibleSessionCount = HistoryPageSize;

    /// <summary>
    /// Initialise la page de suivi local.
    /// </summary>
    public ProgressPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Recharge les données après chaque changement éventuel de profil.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _visibleSessionCount = HistoryPageSize;
        BuildProgress();
    }

    /// <summary>
    /// Calcule le résumé et reconstruit les listes de progression et d'historique.
    /// </summary>
    private void BuildProgress()
    {
        var text = TranslationService.Current;
        var profile = LocalProfileService.Current.ActiveProfile;
        var progress = _storage.GetCourseProgress();
        _sessions = _storage.GetTrainingSessions();
        var overview = LearningProgressCalculator.Calculate(progress, _sessions);

        ProfileLabel.Text = text.Format("profile.current", profile.Name);
        ConsultedValueLabel.Text = overview.Statistics.ConsultedCourseCount.ToString();
        CompletedValueLabel.Text = overview.Statistics.CompletedCourseCount.ToString();
        SessionsValueLabel.Text = overview.Statistics.TrainingSessionCount.ToString();
        AccuracyValueLabel.Text = text.Format(
            "progress.percent",
            overview.Statistics.AccuracyPercent);
        ReadCountValueLabel.Text = overview.Statistics.TotalReadCount.ToString();
        ScoreValueLabel.Text = overview.TotalScore.ToString();

        BuildCourseProgress(progress);
        BuildHistory();
    }

    /// <summary>
    /// Affiche les cours consultés en distinguant ceux qui sont terminés.
    /// </summary>
    private void BuildCourseProgress(IReadOnlyList<CourseProgress> progress)
    {
        CourseProgressPanel.Children.Clear();
        if (progress.Count == 0)
        {
            CourseProgressPanel.Children.Add(CreateEmptyLabel("progress.noCourses"));
            return;
        }

        foreach (var item in progress
            .OrderByDescending(value => value.FirstCompletedAt)
            .ThenByDescending(value => value.ReadCount))
        {
            CourseProgressPanel.Children.Add(CreateCourseProgressCard(item));
        }
    }

    /// <summary>
    /// Crée une carte qui résume les lectures et la première validation d'un cours.
    /// </summary>
    private static Border CreateCourseProgressCard(CourseProgress progress)
    {
        var text = TranslationService.Current;
        var title = GetCourseTitle(progress.CourseId);
        var statusKey = progress.FirstCompletedAt is null
            ? "progress.inProgress"
            : "progress.completedStatus";
        var statusColor = progress.FirstCompletedAt is null
            ? ThemeService.GetColor("Primary")
            : ThemeService.GetColor("Success");
        var details = progress.FirstCompletedAt is null
            ? text.Format("progress.readings", progress.ReadCount)
            : text.Format(
                "progress.completedDetails",
                progress.ReadCount,
                FormatDate(progress.FirstCompletedAt.Value));

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
                        Text = title
                    },
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        Text = text.Get(statusKey),
                        TextColor = statusColor
                    },
                    new Label
                    {
                        Text = details,
                        TextColor = ThemeService.GetColor("TextSecondary")
                    }
                }
            }
        };
    }

    /// <summary>
    /// Affiche une tranche récente de l'historique pour préserver la fluidité mobile.
    /// </summary>
    private void BuildHistory()
    {
        HistoryPanel.Children.Clear();
        if (_sessions.Count == 0)
        {
            HistoryPanel.Children.Add(CreateEmptyLabel("progress.noHistory"));
            LoadMoreButton.IsVisible = false;
            return;
        }

        foreach (var session in _sessions.Take(_visibleSessionCount))
        {
            HistoryPanel.Children.Add(CreateHistoryCard(session));
        }

        LoadMoreButton.IsVisible = _visibleSessionCount < _sessions.Count;
    }

    /// <summary>
    /// Crée une carte de session avec sa date, sa difficulté et son score.
    /// </summary>
    private static Border CreateHistoryCard(TrainingSession session)
    {
        var text = TranslationService.Current;
        var difficultyKey = session.Difficulty switch
        {
            TrainingDifficulty.Easy => "common.easy",
            TrainingDifficulty.Moderate => "common.moderate",
            TrainingDifficulty.Hard => "common.hard",
            _ => "common.moderate"
        };
        var scoreLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            HorizontalTextAlignment = TextAlignment.End,
            Text = text.Format(
                "progress.sessionScore",
                session.CorrectCount,
                session.Results.Count),
            TextColor = session.IncorrectCount == 0
                ? ThemeService.GetColor("Success")
                : ThemeService.GetColor("Primary")
        };
        Grid.SetColumn(scoreLabel, 1);

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
                ColumnSpacing = 12,
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 5,
                        Children =
                        {
                            new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 17,
                                Text = GetCourseTitle(session.CourseId)
                            },
                            new Label
                            {
                                Text = text.Format(
                                    "progress.sessionDetails",
                                    FormatDate(session.CompletedAt),
                                    text.Get(difficultyKey)),
                                TextColor = ThemeService.GetColor("TextSecondary")
                            }
                        }
                    },
                    scoreLabel
                }
            }
        };
    }

    /// <summary>
    /// Retourne le titre localisé d'un cours encore présent dans le catalogue.
    /// </summary>
    private static string GetCourseTitle(string courseId)
    {
        return CourseSearchCatalog.TryGet(courseId, out var entry)
            ? TranslationService.Current.Get(entry.TitleKey)
            : courseId;
    }

    /// <summary>
    /// Formate une date selon la langue actuellement sélectionnée.
    /// </summary>
    private static string FormatDate(DateTimeOffset value)
    {
        var culture = CultureInfo.GetCultureInfo(
            TranslationService.Current.CurrentLanguageCode.Replace('_', '-'));
        return value.ToLocalTime().ToString("g", culture);
    }

    /// <summary>
    /// Crée un texte discret lorsque la section ne possède encore aucune donnée.
    /// </summary>
    private static Label CreateEmptyLabel(string translationKey)
    {
        return new Label
        {
            Text = TranslationService.Current.Get(translationKey),
            TextColor = ThemeService.GetColor("TextSecondary")
        };
    }

    /// <summary>
    /// Ajoute la tranche suivante de l'historique.
    /// </summary>
    private void OnLoadMoreClicked(object? sender, EventArgs e)
    {
        _visibleSessionCount += HistoryPageSize;
        BuildHistory();
    }

    /// <summary>
    /// Ouvre la page des défis du profil actif.
    /// </summary>
    private async void OnOpenChallengesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ChallengesPage));
    }

    /// <summary>
    /// Ouvre la gestion des profils conservés sur l'appareil.
    /// </summary>
    private async void OnOpenProfilesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProfilesPage));
    }

    /// <summary>
    /// Retourne au menu principal.
    /// </summary>
    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
