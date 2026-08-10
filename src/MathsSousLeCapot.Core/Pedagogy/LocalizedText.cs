namespace MathsSousLeCapot.Core.Pedagogy;

/// <summary>
/// Référence un texte traduisible et les valeurs injectées dans ses paramètres.
/// </summary>
public sealed record LocalizedText(
    string Key,
    IReadOnlyList<string>? Arguments = null);
