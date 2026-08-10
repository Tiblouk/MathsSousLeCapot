namespace MathsSousLeCapot.App;

/// <summary>
/// Configure l'hôte .NET MAUI et les services de l'application.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Construit l'application MAUI commune à Windows et Android.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(_ => { });

        return builder.Build();
    }
}
