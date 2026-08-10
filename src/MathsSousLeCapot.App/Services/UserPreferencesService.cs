using System.Text.Json;

namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Lit et enregistre les préférences générales de langue et de thème en JSON.
/// </summary>
public sealed class UserPreferencesService
{
    /// <summary>
    /// Nom du fichier de paramètres utilisateur.
    /// </summary>
    private const string SettingsFileName = "settings.json";

    /// <summary>
    /// Ancienne clé de la langue sélectionnée dans MAUI Preferences.
    /// </summary>
    private const string LanguageKey = "settings_language";

    /// <summary>
    /// Clé du thème sélectionné.
    /// </summary>
    private const string ThemeKey = "settings_theme";

    /// <summary>
    /// Options JSON utilisées pour les paramètres simples.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

    /// <summary>
    /// Retourne la langue enregistrée ou une chaîne vide pour utiliser la détection système.
    /// </summary>
    public string LanguageCode => Load().LanguageCode;

    /// <summary>
    /// Retourne le thème enregistré ou le thème clair par défaut.
    /// </summary>
    public string ThemeCode => Load().ThemeCode;

    /// <summary>
    /// Enregistre le code de langue choisi.
    /// </summary>
    public void SetLanguage(string languageCode)
    {
        Save(Load() with { LanguageCode = languageCode });
    }

    /// <summary>
    /// Enregistre le code de thème choisi.
    /// </summary>
    public void SetTheme(string themeCode)
    {
        Save(Load() with { ThemeCode = themeCode });
    }

    /// <summary>
    /// Charge les paramètres depuis le fichier JSON ou migre les anciennes préférences.
    /// </summary>
    private static UserSettings Load()
    {
        var path = AppDataPathService.GetDataFilePath(SettingsFileName);
        if (File.Exists(path))
        {
            try
            {
                return JsonSerializer.Deserialize<UserSettings>(
                        File.ReadAllText(path),
                        JsonOptions)
                    ?? UserSettings.Default;
            }
            catch (JsonException)
            {
                return UserSettings.Default;
            }
        }

        var settings = new UserSettings(
            Preferences.Default.Get(LanguageKey, string.Empty),
            Preferences.Default.Get(ThemeKey, ThemeCatalog.DefaultThemeCode));
        Save(settings);
        return settings;
    }

    /// <summary>
    /// Écrit les paramètres dans le dossier de données sauvegardable.
    /// </summary>
    private static void Save(UserSettings settings)
    {
        File.WriteAllText(
            AppDataPathService.GetDataFilePath(SettingsFileName),
            JsonSerializer.Serialize(settings, JsonOptions));
    }

    /// <summary>
    /// Représente les paramètres utilisateur persistés localement.
    /// </summary>
    private sealed record UserSettings(
        string LanguageCode,
        string ThemeCode)
    {
        /// <summary>
        /// Valeurs appliquées lors du premier lancement.
        /// </summary>
        public static UserSettings Default { get; } =
            new(string.Empty, ThemeCatalog.DefaultThemeCode);
    }
}
