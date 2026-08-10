using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;

namespace MathsSousLeCapot.App.Features.Settings;

/// <summary>
/// Permet de choisir la langue et la palette de couleurs de l'application.
/// </summary>
public partial class SettingsPage : ContentPage
{
    /// <summary>
    /// Service de stockage des choix de l'utilisateur.
    /// </summary>
    private readonly UserPreferencesService _preferences = new();

    /// <summary>
    /// Initialise les sélecteurs avec les valeurs actuellement actives.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();

        var text = TranslationService.Current;
        LanguagePicker.ItemsSource = text.Catalog.Languages
            .Select(language => language.Name)
            .ToArray();
        LanguagePicker.SelectedIndex = text.Catalog.Languages
            .Select((language, index) => (language, index))
            .First(item => item.language.Code == text.CurrentLanguageCode)
            .index;
        ThemePicker.ItemsSource = ThemeCatalog.Themes
            .Select(theme => text.Get(theme.NameKey))
            .ToArray();
        ThemePicker.SelectedIndex = Math.Max(
            0,
            ThemeCatalog.Themes
                .Select((theme, index) => (theme, index))
                .FirstOrDefault(item => item.theme.Code == _preferences.ThemeCode)
                .index);
    }

    /// <summary>
    /// Enregistre les choix, applique le thème et reconstruit le Shell pour retraduire le XAML.
    /// </summary>
    private async void OnApplyClicked(object? sender, EventArgs e)
    {
        var selectedLanguage = TranslationService.Current.Catalog.Languages[
            Math.Max(0, LanguagePicker.SelectedIndex)];
        var selectedTheme = ThemeCatalog.Themes[
            Math.Max(0, ThemePicker.SelectedIndex)].Code;

        _preferences.SetLanguage(selectedLanguage.Code);
        _preferences.SetTheme(selectedTheme);
        TranslationService.Current.SetLanguage(selectedLanguage.Code);
        new ThemeService().Apply(selectedTheme);

        await DisplayAlert(
            TranslationService.Current.Get("settings.title"),
            TranslationService.Current.Get("settings.applied"),
            TranslationService.Current.Get("common.ok"));

        Application.Current!.Windows[0].Page = new AppShell();
    }

    /// <summary>
    /// Retourne à la liste des cours sans modifier les préférences.
    /// </summary>
    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
