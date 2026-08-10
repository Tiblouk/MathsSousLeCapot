namespace MathsSousLeCapot.App.WinUI;

/// <summary>
/// Point d'entrée WinUI de la cible Windows.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initialise les ressources WinUI.
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Délègue la création de l'hôte à la configuration MAUI commune.
    /// </summary>
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }
}
