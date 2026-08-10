namespace MathsSousLeCapot.App;

using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;

/// <summary>
/// Point d'entrée commun qui crée la fenêtre principale.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Charge les ressources globales de l'application.
    /// </summary>
    public App()
    {
        InitializeComponent();

        // Les préférences sont appliquées avant la création de la première page.
        var preferences = new UserPreferencesService();
        if (!string.IsNullOrWhiteSpace(preferences.LanguageCode))
        {
            TranslationService.Current.SetLanguage(preferences.LanguageCode);
        }

        new ThemeService().Apply(preferences.ThemeCode);
    }

    /// <summary>
    /// Crée la fenêtre qui héberge le Shell de navigation.
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
