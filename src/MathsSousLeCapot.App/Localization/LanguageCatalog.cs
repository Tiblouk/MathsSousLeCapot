namespace MathsSousLeCapot.App.Localization;

/// <summary>
/// Décrit le fichier d'index qui répertorie les langues embarquées.
/// </summary>
public sealed record LanguageCatalog(
    string DefaultLanguage,
    IReadOnlyList<LanguageDefinition> Languages);

/// <summary>
/// Décrit une langue et le fichier JSON qui contient ses traductions.
/// </summary>
public sealed record LanguageDefinition(
    string Code,
    string Name,
    string File,
    IReadOnlyList<string>? Supplements = null);
