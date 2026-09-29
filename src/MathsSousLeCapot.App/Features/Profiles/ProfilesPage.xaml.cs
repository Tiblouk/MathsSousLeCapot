using System.Globalization;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Progress;
using Microsoft.Maui.Layouts;

namespace MathsSousLeCapot.App.Features.Profiles;

/// <summary>
/// Permet de gérer plusieurs progressions locales sans compte ni réseau.
/// </summary>
public partial class ProfilesPage : ContentPage
{
    /// <summary>
    /// Service partagé qui conserve le profil actif.
    /// </summary>
    private readonly LocalProfileService _profiles = LocalProfileService.Current;

    /// <summary>
    /// Service utilisé pour afficher le score propre à chaque profil.
    /// </summary>
    private readonly LocalStorageService _storage = new();

    /// <summary>
    /// Initialise la page des profils locaux.
    /// </summary>
    public ProfilesPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Recharge la liste après chaque retour sur la page.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuildProfiles();
    }

    /// <summary>
    /// Reconstruit les cartes à partir du registre local actuel.
    /// </summary>
    private void BuildProfiles()
    {
        ProfilesPanel.Children.Clear();
        var activeId = _profiles.ActiveProfile.Id;
        foreach (var profile in _profiles.GetProfiles())
        {
            ProfilesPanel.Children.Add(CreateProfileCard(profile, profile.Id == activeId));
        }
    }

    /// <summary>
    /// Crée une carte de sélection, renommage et suppression pour un profil.
    /// </summary>
    private Border CreateProfileCard(LocalProfile profile, bool isActive)
    {
        var text = TranslationService.Current;
        var overview = LearningProgressCalculator.Calculate(
            _storage.GetCourseProgress(profile.Id),
            _storage.GetTrainingSessions(profile.Id));
        var selectButton = new Button
        {
            CommandParameter = profile.Id,
            IsEnabled = !isActive,
            Margin = new Thickness(0, 0, 8, 8),
            Text = isActive
                ? text.Get("profile.active")
                : text.Get("profile.select")
        };
        selectButton.Clicked += OnSelectClicked;
        var renameButton = new Button
        {
            CommandParameter = profile,
            Margin = new Thickness(0, 0, 8, 8),
            Style = (Style)Application.Current!.Resources["SecondaryButton"],
            Text = text.Get("profile.rename")
        };
        renameButton.Clicked += OnRenameClicked;
        var deleteButton = new Button
        {
            CommandParameter = profile,
            IsEnabled = _profiles.GetProfiles().Count > 1,
            Margin = new Thickness(0, 0, 8, 8),
            Style = (Style)Application.Current!.Resources["SecondaryButton"],
            Text = text.Get("profile.delete")
        };
        deleteButton.Clicked += OnDeleteClicked;

        return new Border
        {
            Style = (Style)Application.Current!.Resources["Card"],
            Stroke = isActive
                ? ThemeService.GetColor("Primary")
                : ThemeService.GetColor("Border"),
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 21,
                        Text = profile.Name
                    },
                    new Label
                    {
                        Text = text.Format(
                            "profile.cardDetails",
                            overview.Statistics.CompletedCourseCount,
                            overview.TotalScore),
                        TextColor = ThemeService.GetColor("TextSecondary")
                    },
                    new Label
                    {
                        FontSize = 12,
                        Text = text.Format(
                            "profile.createdAt",
                            FormatDate(profile.CreatedAt)),
                        TextColor = ThemeService.GetColor("TextSecondary")
                    },
                    new FlexLayout
                    {
                        Direction = FlexDirection.Row,
                        Wrap = FlexWrap.Wrap,
                        Children =
                        {
                            selectButton,
                            renameButton,
                            deleteButton
                        }
                    }
                }
            }
        };
    }

    /// <summary>
    /// Crée un profil avec le nom saisi et vide le champ après réussite.
    /// </summary>
    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        try
        {
            _profiles.CreateProfile(ProfileNameEntry.Text ?? string.Empty);
            ProfileNameEntry.Text = string.Empty;
            BuildProfiles();
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            await DisplayAlert(
                TranslationService.Current.Get("profile.errorTitle"),
                TranslationService.Current.Get("profile.invalidOrDuplicate"),
                TranslationService.Current.Get("common.ok"));
        }
    }

    /// <summary>
    /// Rend actif le profil associé au bouton sélectionné.
    /// </summary>
    private void OnSelectClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string profileId })
        {
            _profiles.SetActiveProfile(profileId);
            BuildProfiles();
        }
    }

    /// <summary>
    /// Demande puis applique un nouveau nom au profil choisi.
    /// </summary>
    private async void OnRenameClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: LocalProfile profile })
        {
            return;
        }

        var text = TranslationService.Current;
        var name = await DisplayPromptAsync(
            text.Get("profile.rename"),
            text.Get("profile.renamePrompt"),
            text.Get("profile.rename"),
            text.Get("actions.cancel"),
            initialValue: profile.Name,
            maxLength: 30);
        if (name is null)
        {
            return;
        }

        try
        {
            _profiles.RenameProfile(profile.Id, name);
            BuildProfiles();
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            await DisplayAlert(
                text.Get("profile.errorTitle"),
                text.Get("profile.invalidOrDuplicate"),
                text.Get("common.ok"));
        }
    }

    /// <summary>
    /// Supprime le profil choisi après une confirmation explicite.
    /// </summary>
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: LocalProfile profile })
        {
            return;
        }

        var text = TranslationService.Current;
        var confirmed = await DisplayAlert(
            text.Get("profile.deleteTitle"),
            text.Format("profile.deleteConfirmation", profile.Name),
            text.Get("profile.delete"),
            text.Get("actions.cancel"));
        if (!confirmed)
        {
            return;
        }

        _profiles.DeleteProfile(profile.Id);
        BuildProfiles();
    }

    /// <summary>
    /// Formate la date de création selon la langue active.
    /// </summary>
    private static string FormatDate(DateTimeOffset value)
    {
        var culture = CultureInfo.GetCultureInfo(
            TranslationService.Current.CurrentLanguageCode.Replace('_', '-'));
        return value.ToLocalTime().ToString("d", culture);
    }

    /// <summary>
    /// Retourne au menu principal.
    /// </summary>
    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
