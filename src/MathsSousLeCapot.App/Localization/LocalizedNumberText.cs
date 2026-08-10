using MathsSousLeCapot.Core.Mathematics.Numbers;

namespace MathsSousLeCapot.App.Localization;

/// <summary>
/// Écrit et reconnaît les nombres selon la langue active de l'interface.
/// </summary>
public static class LocalizedNumberText
{
    /// <summary>
    /// Écrit un entier compris entre zéro et 9 999 dans la langue active.
    /// </summary>
    public static string ToWords(int value)
    {
        return LocalizedNumberConverter.ToWords(
            value,
            TranslationService.Current.CurrentLanguageCode);
    }

    /// <summary>
    /// Vérifie qu'une réponse en chiffres ou en mots correspond à la valeur attendue.
    /// </summary>
    public static bool Matches(string? text, int expectedValue)
    {
        return LocalizedNumberConverter.Matches(
            text,
            expectedValue,
            TranslationService.Current.CurrentLanguageCode);
    }
}
