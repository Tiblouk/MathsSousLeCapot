using System.Globalization;
using System.Text;

namespace MathsSousLeCapot.Core.Mathematics.Numbers;

/// <summary>
/// Reconnaît un entier écrit en chiffres ou en toutes lettres françaises.
/// </summary>
public static class FrenchNumberParser
{
    /// <summary>
    /// Index normalisé des noms acceptés pour les valeurs du cours.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, int> ValuesByName =
        CreateValuesByName();

    /// <summary>
    /// Tente de convertir une réponse libre en entier.
    /// </summary>
    /// <param name="text">Réponse saisie par l'utilisateur.</param>
    /// <param name="value">Valeur reconnue lorsque la conversion réussit.</param>
    public static bool TryParse(string? text, out int value)
    {
        var trimmed = text?.Trim();
        if (int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
        {
            return value is >= 0 and <= 9999;
        }

        return ValuesByName.TryGetValue(Normalize(trimmed), out value);
    }

    /// <summary>
    /// Vérifie qu'une réponse libre représente la valeur décimale attendue.
    /// </summary>
    public static bool Matches(string? text, int expectedValue)
    {
        return TryParse(text, out var parsedValue) && parsedValue == expectedValue;
    }

    /// <summary>
    /// Construit une table à partir du générateur de noms afin de garder lecture et analyse cohérentes.
    /// </summary>
    private static IReadOnlyDictionary<string, int> CreateValuesByName()
    {
        var values = new Dictionary<string, int>(StringComparer.Ordinal);

        for (var value = 0; value <= 9999; value++)
        {
            var canonicalName = FrenchNumberNames.ToWords(value);
            values[Normalize(canonicalName)] = value;

            // Le pluriel final est facultatif dans une réponse pédagogique.
            values[Normalize(canonicalName.TrimEnd('s'))] = value;
        }

        return values;
    }

    /// <summary>
    /// Ignore accents, espaces, apostrophes et traits d'union lors de la comparaison.
    /// </summary>
    private static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var decomposed = text
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}
