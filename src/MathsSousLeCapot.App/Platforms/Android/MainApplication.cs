using Android.App;
using Android.Runtime;

namespace MathsSousLeCapot.App;

/// <summary>
/// Point d'entrée Android chargé de construire l'application MAUI.
/// </summary>
[Application]
public class MainApplication : MauiApplication
{
    /// <summary>
    /// Initialise l'application Android avec le handle natif.
    /// </summary>
    public MainApplication(nint handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <summary>
    /// Délègue la création de l'hôte à la configuration commune.
    /// </summary>
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }
}
