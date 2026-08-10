namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Centralise les thèmes disponibles afin que l'interface et le service partagent le même contrat.
/// </summary>
public static class ThemeCatalog
{
    /// <summary>
    /// Code du thème utilisé lorsqu'une préférence inconnue est rencontrée.
    /// </summary>
    public const string DefaultThemeCode = "light";

    /// <summary>
    /// Palettes proposées dans l'ordre affiché dans les paramètres.
    /// </summary>
    public static IReadOnlyList<ThemeDefinition> Themes { get; } =
    [
        new(
            "light",
            "settings.theme.light",
            new ThemePalette(
                "#46627A", "#334A5E", "#E9EEF2", "#FFFFFF", "#F3F4F6",
                "#FFFFFF", "#20252A", "#626A72", "#D5D9DD", "#F0F3F5",
                "#19713D", "#E5F5EB", "#B3261E", "#FCE8E6"),
            AppTheme.Light),
        new(
            "dark",
            "settings.theme.dark",
            new ThemePalette(
                "#8AB4D0", "#B7D3E4", "#263746", "#15191D", "#22282E",
                "#2A3138", "#F2F4F5", "#B8C0C7", "#46515A", "#252D34",
                "#75D69B", "#173C28", "#FF8A82", "#481E1B"),
            AppTheme.Dark),
        new(
            "sepia",
            "settings.theme.sepia",
            new ThemePalette(
                "#756043", "#59472F", "#E8DDCA", "#FBF5E9", "#F1E7D5",
                "#FFF9EE", "#332B22", "#6D6254", "#D6C7AF", "#EFE4D2",
                "#347047", "#E2F0E5", "#A63B32", "#F6E1DC"),
            AppTheme.Light)
    ];

    /// <summary>
    /// Retourne un thème par son code ou le thème par défaut.
    /// </summary>
    public static ThemeDefinition Get(string? code)
    {
        return Themes.FirstOrDefault(
            theme => string.Equals(
                theme.Code,
                code,
                StringComparison.OrdinalIgnoreCase))
            ?? Themes.Single(theme => theme.Code == DefaultThemeCode);
    }
}
