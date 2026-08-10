namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Applique une palette de couleurs complète aux ressources de l'application.
/// </summary>
public sealed class ThemeService
{
    /// <summary>
    /// Applique la palette demandée et ajuste les contrôles natifs au mode clair ou sombre.
    /// </summary>
    public void Apply(string themeCode)
    {
        var theme = ThemeCatalog.Get(themeCode);
        var palette = theme.Palette;
        var resources = Application.Current?.Resources
            ?? throw new InvalidOperationException("Les ressources de l'application ne sont pas disponibles.");

        SetColor(resources, "Primary", palette.Primary);
        SetColor(resources, "PrimaryDark", palette.PrimaryDark);
        SetColor(resources, "PrimaryLight", palette.PrimaryLight);
        SetColor(resources, "Background", palette.Background);
        SetColor(resources, "CardBackground", palette.CardBackground);
        SetColor(resources, "ControlBackground", palette.ControlBackground);
        SetColor(resources, "TextPrimary", palette.TextPrimary);
        SetColor(resources, "TextSecondary", palette.TextSecondary);
        SetColor(resources, "Border", palette.Border);
        SetColor(resources, "ExplanationBackground", palette.ExplanationBackground);
        SetColor(resources, "Success", palette.Success);
        SetColor(resources, "SuccessBackground", palette.SuccessBackground);
        SetColor(resources, "Error", palette.Error);
        SetColor(resources, "ErrorBackground", palette.ErrorBackground);

        Application.Current.UserAppTheme = theme.AppTheme;
    }

    /// <summary>
    /// Retourne une couleur de la palette active pour les éléments dessinés en C#.
    /// </summary>
    public static Color GetColor(string key)
    {
        return Application.Current?.Resources.TryGetValue(key, out var value) == true
            && value is Color color
                ? color
                : Colors.Transparent;
    }

    /// <summary>
    /// Remplace une couleur dans le dictionnaire global.
    /// </summary>
    private static void SetColor(ResourceDictionary resources, string key, string value)
    {
        resources[key] = Color.FromArgb(value);
    }

}
