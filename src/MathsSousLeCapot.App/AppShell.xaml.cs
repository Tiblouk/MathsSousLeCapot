using MathsSousLeCapot.App.Services;

namespace MathsSousLeCapot.App;

/// <summary>
/// Déclare les routes de navigation des chapitres, cours et entraînements.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// Initialise le Shell et enregistre toutes les pages accessibles par route.
    /// </summary>
    public AppShell()
    {
        InitializeComponent();
        NavigationService.RegisterRoutes();
    }
}
