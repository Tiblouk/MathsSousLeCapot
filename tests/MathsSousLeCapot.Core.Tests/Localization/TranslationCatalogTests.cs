using System.Text.Json;
using System.Text.RegularExpressions;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.Core.Tests.Localization;

/// <summary>
/// Vérifie automatiquement la cohérence de tous les catalogues de traduction.
/// </summary>
public sealed partial class TranslationCatalogTests
{
    /// <summary>
    /// Répertoire des fichiers de langue dans le dépôt.
    /// </summary>
    private static readonly string LanguageDirectory = FindLanguageDirectory();

    [Fact]
    public void Every_declared_translation_uses_keys_and_placeholders_from_french()
    {
        var index = Deserialize<LanguageIndex>(
            Path.Combine(LanguageDirectory, "index.json"));
        var defaultLanguage = index.Languages.Single(
            language => language.Code == index.DefaultLanguage);
        var reference = LoadTranslations(defaultLanguage);

        foreach (var language in index.Languages)
        {
            var path = Path.Combine(LanguageDirectory, language.File);
            Assert.True(File.Exists(path), $"Fichier de langue absent : {path}");

            var translations = LoadTranslations(language);
            Assert.Empty(translations.Keys.Except(reference.Keys));
            Assert.Empty(reference.Keys.Except(translations.Keys));

            foreach (var key in translations.Keys)
            {
                Assert.Equal(
                    GetPlaceholders(reference[key]),
                    GetPlaceholders(translations[key]));
            }
        }
    }

    /// <summary>
    /// Garantit que chaque découverte du lycée possède une vraie structure pédagogique.
    /// </summary>
    [Fact]
    public void French_high_school_discoveries_are_detailed_and_structured()
    {
        var index = Deserialize<LanguageIndex>(
            Path.Combine(LanguageDirectory, "index.json"));
        var french = index.Languages.Single(
            language => language.Code == index.DefaultLanguage);
        var translations = LoadTranslations(french);
        var discoveryKeys = HighSchoolCourseCatalog.Definitions
            .Select(definition => definition.DiscoveryKey)
            .Distinct()
            .ToArray();

        Assert.Equal(33, discoveryKeys.Length);
        AssertStructuredDiscoveries(
            translations,
            discoveryKeys,
            450,
            "Définition :",
            "Comment raisonner :",
            "À retenir :");
    }

    /// <summary>
    /// Garantit que chaque découverte du collège explique la notion et sa méthode.
    /// </summary>
    [Fact]
    public void French_middle_school_discoveries_are_detailed_and_structured()
    {
        var index = Deserialize<LanguageIndex>(
            Path.Combine(LanguageDirectory, "index.json"));
        var french = index.Languages.Single(
            language => language.Code == index.DefaultLanguage);
        var translations = LoadTranslations(french);
        var discoveryKeys = MiddleSchoolCourseCatalog.Definitions
            .Select(definition => definition.DiscoveryKey)
            .Distinct()
            .ToArray();

        Assert.Equal(31, discoveryKeys.Length);
        AssertStructuredDiscoveries(
            translations,
            discoveryKeys,
            400,
            "Définition :",
            "Méthode :",
            "À retenir :");
    }

    /// <summary>
    /// Garantit que chaque découverte du primaire reste détaillée et accessible.
    /// </summary>
    [Fact]
    public void French_primary_discoveries_are_detailed_and_structured()
    {
        var index = Deserialize<LanguageIndex>(
            Path.Combine(LanguageDirectory, "index.json"));
        var french = index.Languages.Single(
            language => language.Code == index.DefaultLanguage);
        var translations = LoadTranslations(french);
        var discoveryKeys = PrimaryCourseCatalog.Definitions
            .Select(definition => definition.Title.Replace(
                ".title",
                ".discovery",
                StringComparison.Ordinal))
            .Distinct()
            .ToArray();

        Assert.Equal(37, discoveryKeys.Length);
        AssertStructuredDiscoveries(
            translations,
            discoveryKeys,
            500,
            "Définition :",
            "Comment faire :",
            "À retenir :");
    }

    /// <summary>
    /// Vérifie la longueur et les blocs attendus d'une collection de découvertes.
    /// </summary>
    private static void AssertStructuredDiscoveries(
        IReadOnlyDictionary<string, string> translations,
        IEnumerable<string> discoveryKeys,
        int minimumLength,
        params string[] expectedHeadings)
    {
        foreach (var key in discoveryKeys)
        {
            Assert.True(translations.TryGetValue(key, out var content), key);
            Assert.True(
                content.Length >= minimumLength,
                $"Cours trop court : {key} ({content.Length} caractères)");
            Assert.Equal(3, ParagraphBreakRegex().Split(content).Length);

            foreach (var heading in expectedHeadings)
            {
                Assert.Contains(heading, content);
            }
        }
    }

    /// <summary>
    /// Fusionne le fichier principal et les catalogues de modules d'une langue.
    /// </summary>
    private static Dictionary<string, string> LoadTranslations(LanguageEntry language)
    {
        var translations = Deserialize<Dictionary<string, string>>(
            Path.Combine(LanguageDirectory, language.File));

        foreach (var supplement in language.Supplements ?? [])
        {
            var path = Path.Combine(
                LanguageDirectory,
                supplement.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Fichier de langue absent : {path}");
            foreach (var pair in Deserialize<Dictionary<string, string>>(path))
            {
                translations[pair.Key] = pair.Value;
            }
        }

        return translations;
    }

    /// <summary>
    /// Lit un fichier JSON fortement typé.
    /// </summary>
    private static T Deserialize<T>(string path)
    {
        return JsonSerializer.Deserialize<T>(
            File.ReadAllText(path),
            new JsonSerializerOptions(JsonSerializerDefaults.Web))
            ?? throw new InvalidOperationException($"JSON vide ou invalide : {path}");
    }

    /// <summary>
    /// Extrait les paramètres numérotés d'un texte traduit.
    /// </summary>
    private static string[] GetPlaceholders(string text)
    {
        return PlaceholderRegex()
            .Matches(text)
            .Select(match => match.Value)
            .Distinct()
            .OrderBy(value => value)
            .ToArray();
    }

    /// <summary>
    /// Remonte depuis le dossier de test jusqu'au répertoire de la solution.
    /// </summary>
    private static string FindLanguageDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(
                directory.FullName,
                "src",
                "MathsSousLeCapot.App",
                "Resources",
                "Raw",
                "lang");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Le dossier Resources/Raw/lang est introuvable.");
    }

    [GeneratedRegex(@"\{\d+\}")]
    private static partial Regex PlaceholderRegex();

    /// <summary>
    /// Repère les séparations entre les trois blocs pédagogiques d'une découverte.
    /// </summary>
    [GeneratedRegex(@"\r?\n\r?\n")]
    private static partial Regex ParagraphBreakRegex();

    private sealed record LanguageIndex(
        string DefaultLanguage,
        IReadOnlyList<LanguageEntry> Languages);

    private sealed record LanguageEntry(
        string Code,
        string Name,
        string File,
        IReadOnlyList<string>? Supplements);
}
