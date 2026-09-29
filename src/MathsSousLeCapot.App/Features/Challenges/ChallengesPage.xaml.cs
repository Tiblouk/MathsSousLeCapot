using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Progress;

namespace MathsSousLeCapot.App.Features.Challenges;

/// <summary>
/// Présente les défis et compare les scores des profils de l'appareil.
/// </summary>
public partial class ChallengesPage : ContentPage
{
    /// <summary>
    /// Service qui lit la progression de chaque profil.
    /// </summary>
    private readonly LocalStorageService _storage = new();

    /// <summary>
    /// Initialise la page des défis locaux.
    /// </summary>
    public ChallengesPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Recalcule les défis après chaque retour sur la page.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuildChallenges();
    }

    /// <summary>
    /// Construit le score actif, les défis et le classement local.
    /// </summary>
    private void BuildChallenges()
    {
        var text = TranslationService.Current;
        var activeProfile = LocalProfileService.Current.ActiveProfile;
        var overview = CalculateOverview(activeProfile.Id);

        ProfileLabel.Text = text.Format("profile.current", activeProfile.Name);
        TotalScoreLabel.Text = overview.TotalScore.ToString();
        ScoreDetailsLabel.Text = text.Format(
            "challenge.scoreDetails",
            overview.Statistics.TrainingScore,
            overview.ChallengeScore);

        ChallengesPanel.Children.Clear();
        foreach (var challenge in overview.Challenges)
        {
            ChallengesPanel.Children.Add(CreateChallengeCard(challenge));
        }

        BuildLeaderboard(activeProfile.Id);
    }

    /// <summary>
    /// Crée une carte avec objectif, récompense et barre de progression.
    /// </summary>
    private static Border CreateChallengeCard(LocalChallengeProgress challenge)
    {
        var text = TranslationService.Current;
        var statusColor = challenge.IsCompleted
            ? ThemeService.GetColor("Success")
            : ThemeService.GetColor("Primary");
        var progress = new ProgressBar
        {
            Progress = challenge.Definition.Target == 0
                ? 0
                : (double)challenge.DisplayValue / challenge.Definition.Target,
            ProgressColor = statusColor
        };
        var progressLabel = new Label
        {
            Text = text.Format(
                "challenge.progress",
                challenge.DisplayValue,
                challenge.Definition.Target),
            TextColor = statusColor
        };
        var rewardLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            Text = text.Format(
                challenge.IsCompleted
                    ? "challenge.rewardEarned"
                    : "challenge.reward",
                challenge.Definition.Reward),
            TextColor = statusColor
        };
        Grid.SetColumn(rewardLabel, 1);

        return new Border
        {
            Style = (Style)Application.Current!.Resources["Card"],
            Stroke = challenge.IsCompleted
                ? ThemeService.GetColor("Success")
                : ThemeService.GetColor("Border"),
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 19,
                        Text = text.Get(challenge.Definition.TitleKey)
                    },
                    new Label
                    {
                        Text = text.Get(challenge.Definition.DescriptionKey),
                        TextColor = ThemeService.GetColor("TextSecondary")
                    },
                    progress,
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto)
                        },
                        Children =
                        {
                            progressLabel,
                            rewardLabel
                        }
                    }
                }
            }
        };
    }

    /// <summary>
    /// Trie les profils uniquement à partir des scores enregistrés sur l'appareil.
    /// </summary>
    private void BuildLeaderboard(string activeProfileId)
    {
        LeaderboardPanel.Children.Clear();
        var ranking = LocalProfileService.Current.GetProfiles()
            .Select(profile => new
            {
                Profile = profile,
                Overview = CalculateOverview(profile.Id)
            })
            .OrderByDescending(item => item.Overview.TotalScore)
            .ThenBy(item => item.Profile.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        for (var index = 0; index < ranking.Length; index++)
        {
            var item = ranking[index];
            LeaderboardPanel.Children.Add(CreateRankingCard(
                index + 1,
                item.Profile,
                item.Overview,
                item.Profile.Id == activeProfileId));
        }
    }

    /// <summary>
    /// Crée une ligne du classement local en signalant le profil actif.
    /// </summary>
    private static Border CreateRankingCard(
        int rank,
        LocalProfile profile,
        LearningOverview overview,
        bool isActive)
    {
        var text = TranslationService.Current;
        var profileName = isActive
            ? text.Format("challenge.activeProfileName", profile.Name)
            : profile.Name;
        var nameLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 17,
            Text = profileName
        };
        var scoreLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            Text = text.Format("challenge.points", overview.TotalScore),
            TextColor = ThemeService.GetColor("Primary")
        };
        Grid.SetColumn(nameLabel, 1);
        Grid.SetColumn(scoreLabel, 2);
        return new Border
        {
            Style = (Style)Application.Current!.Resources["Card"],
            Stroke = isActive
                ? ThemeService.GetColor("Primary")
                : ThemeService.GetColor("Border"),
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 12,
                Children =
                {
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        Text = $"{rank}."
                    },
                    nameLabel,
                    scoreLabel
                }
            }
        };
    }

    /// <summary>
    /// Calcule l'aperçu d'un profil sans modifier son fichier de progression.
    /// </summary>
    private LearningOverview CalculateOverview(string profileId)
    {
        return LearningProgressCalculator.Calculate(
            _storage.GetCourseProgress(profileId),
            _storage.GetTrainingSessions(profileId));
    }

    /// <summary>
    /// Retourne au menu principal.
    /// </summary>
    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
