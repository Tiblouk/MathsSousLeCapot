using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace MathsSousLeCapot.App.Localization;

/// <summary>
/// Charge les traductions JSON embarquées et fournit les textes de l'interface.
/// </summary>
public sealed class TranslationService
{
    /// <summary>
    /// Nom logique du dossier qui contient les ressources de langue.
    /// </summary>
    private const string ResourceFolder = "Resources.Raw.lang";

    /// <summary>
    /// Options communes utilisées pour lire les fichiers JSON.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Noms des ressources embarquées, calculés une seule fois.
    /// </summary>
    private static readonly Lazy<string[]> ResourceNames =
        new(() => typeof(TranslationService).Assembly.GetManifestResourceNames());

    /// <summary>
    /// Instance partagée utilisée par le XAML et les pages créées en C#.
    /// </summary>
    public static TranslationService Current { get; } = new();

    /// <summary>
    /// Dictionnaire des traductions de la langue active.
    /// </summary>
    private IReadOnlyDictionary<string, string> _translations =
        new Dictionary<string, string>();

    /// <summary>
    /// Code chargé, vide tant qu'aucun texte n'a été demandé.
    /// </summary>
    private string _currentLanguageCode = string.Empty;

    /// <summary>
    /// Langues utilisées pour charger les modules complémentaires à la demande.
    /// </summary>
    private LanguageDefinition? _defaultLanguage;
    private LanguageDefinition? _selectedLanguage;
    private bool _supplementsLoaded;

    /// <summary>
    /// Initialise une nouvelle instance avec la langue par défaut du catalogue.
    /// </summary>
    private TranslationService()
    {
        Catalog = LoadJson<LanguageCatalog>("index.json");
    }

    /// <summary>
    /// Catalogue des langues disponibles dans l'application.
    /// </summary>
    public LanguageCatalog Catalog { get; }

    /// <summary>
    /// Code de la langue actuellement chargée.
    /// </summary>
    public string CurrentLanguageCode
    {
        get
        {
            EnsureLanguageLoaded();
            return _currentLanguageCode;
        }
        private set => _currentLanguageCode = value;
    }

    /// <summary>
    /// Charge le fichier JSON correspondant au code de langue demandé.
    /// </summary>
    /// <param name="languageCode">Code au format attendu par l'index, par exemple fr_FR.</param>
    public void SetLanguage(string languageCode)
    {
        // La définition relie le code public au nom réel du fichier JSON.
        var language = Catalog.Languages.FirstOrDefault(
            item => string.Equals(
                item.Code,
                languageCode,
                StringComparison.OrdinalIgnoreCase));

        // Une langue inconnue revient toujours vers la langue par défaut.
        language ??= Catalog.Languages.Single(
            item => string.Equals(
                item.Code,
                Catalog.DefaultLanguage,
                StringComparison.OrdinalIgnoreCase));

        if (_translations.Count > 0
            && string.Equals(
                CurrentLanguageCode,
                language.Code,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Le français fournit le contrat complet et sert de repli aux modules non traduits.
        var defaultLanguage = Catalog.Languages.Single(
            item => string.Equals(
                item.Code,
                Catalog.DefaultLanguage,
                StringComparison.OrdinalIgnoreCase));
        var translations = LoadJson<Dictionary<string, string>>(
            defaultLanguage.File);

        // La langue sélectionnée remplace uniquement les clés qu'elle possède déjà.
        if (!string.Equals(
            language.Code,
            defaultLanguage.Code,
            StringComparison.OrdinalIgnoreCase))
        {
            foreach (var pair in LoadJson<Dictionary<string, string>>(language.File))
            {
                translations[pair.Key] = pair.Value;
            }
        }

        _translations = translations;
        _defaultLanguage = defaultLanguage;
        _selectedLanguage = language;
        _supplementsLoaded = false;
        CurrentLanguageCode = language.Code;
    }

    /// <summary>
    /// Fusionne les catalogues complémentaires d'une langue dans le dictionnaire fourni.
    /// </summary>
    private static void LoadSupplements(
        LanguageDefinition language,
        IDictionary<string, string> translations)
    {
        foreach (var supplement in language.Supplements ?? [])
        {
            foreach (var pair in LoadJson<Dictionary<string, string>>(supplement))
            {
                translations[pair.Key] = pair.Value;
            }
        }
    }

    /// <summary>
    /// Retourne une traduction ou la clé elle-même lorsqu'elle est absente.
    /// </summary>
    /// <param name="key">Clé stable définie dans le fichier de langue.</param>
    public string Get(string key)
    {
        EnsureLanguageLoaded();
        if (!_translations.ContainsKey(key))
        {
            EnsureSupplementsLoaded();
        }

        return _translations.TryGetValue(key, out var value)
            ? value
            : $"[{key}]";
    }

    /// <summary>
    /// Formate une traduction paramétrée avec la culture de la langue active.
    /// </summary>
    /// <param name="key">Clé du modèle de texte.</param>
    /// <param name="arguments">Valeurs injectées dans les emplacements numérotés.</param>
    public string Format(string key, params object?[] arguments)
    {
        return string.Format(GetCulture(), Get(key), arguments);
    }

    /// <summary>
    /// Sélectionne la langue système lorsqu'elle existe, sinon la langue par défaut.
    /// </summary>
    private string FindBestLanguageCode()
    {
        // MAUI et .NET utilisent un tiret, tandis que les fichiers demandés utilisent un underscore.
        var systemLanguage = CultureInfo.CurrentUICulture.Name.Replace('-', '_');
        var matchingLanguage = Catalog.Languages.FirstOrDefault(
            item => string.Equals(
                item.Code,
                systemLanguage,
                StringComparison.OrdinalIgnoreCase));

        // Une culture régionale non déclarée, comme en-GB, utilise la langue disponible en-US.
        matchingLanguage ??= Catalog.Languages.FirstOrDefault(
            item => item.Code.StartsWith(
                $"{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}_",
                StringComparison.OrdinalIgnoreCase));

        return matchingLanguage?.Code ?? Catalog.DefaultLanguage;
    }

    /// <summary>
    /// Charge la langue système seulement si aucune préférence ne l'a déjà fait.
    /// </summary>
    private void EnsureLanguageLoaded()
    {
        if (_translations.Count == 0)
        {
            SetLanguage(FindBestLanguageCode());
        }
    }

    /// <summary>
    /// Charge les textes des modules seulement lors de leur première utilisation.
    /// </summary>
    private void EnsureSupplementsLoaded()
    {
        if (_supplementsLoaded
            || _defaultLanguage is null
            || _selectedLanguage is null)
        {
            return;
        }

        var translations = new Dictionary<string, string>(_translations);
        LoadSupplements(_defaultLanguage, translations);
        if (!string.Equals(
            _selectedLanguage.Code,
            _defaultLanguage.Code,
            StringComparison.OrdinalIgnoreCase))
        {
            LoadSupplements(_selectedLanguage, translations);
        }

        _translations = translations;
        _supplementsLoaded = true;
    }

    /// <summary>
    /// Convertit le code de langue actif en culture .NET pour le formatage.
    /// </summary>
    private CultureInfo GetCulture()
    {
        return CultureInfo.GetCultureInfo(CurrentLanguageCode.Replace('_', '-'));
    }

    /// <summary>
    /// Désérialise un fichier JSON embarqué dans l'assembly de l'application.
    /// </summary>
    /// <typeparam name="T">Type attendu après désérialisation.</typeparam>
    /// <param name="fileName">Nom du fichier présent dans le dossier lang.</param>
    private static T LoadJson<T>(string fileName)
    {
        // Le nom de ressource inclut l'espace de noms racine et le chemin du fichier.
        var normalizedFileName = fileName
            .Replace('\\', '.')
            .Replace('/', '.')
            // MSBuild remplace les tirets des dossiers par des underscores
            // lorsqu'il construit le nom logique d'une ressource embarquée.
            .Replace('-', '_');
        var resourceSuffix = $"{ResourceFolder}.{normalizedFileName}";
        var resourceName = ResourceNames.Value
            .Single(name => name.EndsWith(resourceSuffix, StringComparison.Ordinal));

        // Le flux reste local à l'application et ne nécessite aucune connexion.
        using var stream = typeof(TranslationService).Assembly
            .GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"La ressource de traduction '{fileName}' est introuvable.");

        // Une ressource invalide doit interrompre le démarrage plutôt que masquer des textes.
        return JsonSerializer.Deserialize<T>(stream, JsonOptions)
            ?? throw new InvalidOperationException(
                $"La ressource de traduction '{fileName}' est vide ou invalide.");
    }
}
