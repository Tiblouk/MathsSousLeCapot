using System.Globalization;
using System.Text;
using MathsSousLeCapot.Core.Mathematics.Numbers;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Valide une réponse selon le contrat porté par l'exercice.
/// </summary>
public static class ExerciseAnswerValidator
{
    /// <summary>
    /// Vérifie une réponse numérique ou une position décimale localisée.
    /// </summary>
    public static bool Matches(
        Exercise exercise,
        string? answer,
        string languageCode)
    {
        if (exercise.AnswerKind == ExerciseAnswerKind.Decimal)
        {
            return MatchesDecimal(exercise.CorrectAnswer, answer);
        }

        if (!int.TryParse(exercise.CorrectAnswer, out var expectedValue))
        {
            return false;
        }

        if (LocalizedNumberConverter.Matches(answer, expectedValue, languageCode))
        {
            return true;
        }

        return exercise.AnswerKind == ExerciseAnswerKind.PlaceValue
            && MatchesPlaceName(
                answer,
                languageCode,
                exercise.AnswerArguments?.FirstOrDefault());
    }

    /// <summary>
    /// Accepte la virgule ou le point comme séparateur décimal.
    /// </summary>
    private static bool MatchesDecimal(string expectedText, string? answer)
    {
        return decimal.TryParse(
                expectedText,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var expected)
            && decimal.TryParse(
                (answer ?? string.Empty).Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var actual)
            && actual == expected;
    }

    /// <summary>
    /// Accepte le nom de la colonne, au singulier ou au pluriel, dans la langue active.
    /// </summary>
    private static bool MatchesPlaceName(
        string? answer,
        string languageCode,
        string? placeText)
    {
        if (!int.TryParse(placeText, out var place)
            || !PlaceAliases.TryGetValue(
                GetLanguagePrefix(languageCode),
                out var aliases)
            || !aliases.TryGetValue(place, out var accepted))
        {
            return false;
        }

        var normalizedAnswer = Normalize(answer);
        return accepted.Any(alias => Normalize(alias) == normalizedAnswer);
    }

    /// <summary>
    /// Noms usuels des quatre premières positions décimales dans les langues fournies.
    /// </summary>
    private static readonly IReadOnlyDictionary<
        string,
        IReadOnlyDictionary<int, IReadOnlyList<string>>> PlaceAliases =
        new Dictionary<string, IReadOnlyDictionary<int, IReadOnlyList<string>>>(
            StringComparer.Ordinal)
        {
            ["fr"] = new Dictionary<int, IReadOnlyList<string>>
            {
                [1] = ["unité", "unités"],
                [10] = ["dizaine", "dizaines"],
                [100] = ["centaine", "centaines"],
                [1000] = ["millier", "milliers"]
            },
            ["en"] = new Dictionary<int, IReadOnlyList<string>>
            {
                [1] = ["unit", "units", "one", "ones", "ones place"],
                [10] = ["ten", "tens", "tens place"],
                [100] = ["hundred", "hundreds", "hundreds place"],
                [1000] = ["thousand", "thousands", "thousands place"]
            },
            ["es"] = new Dictionary<int, IReadOnlyList<string>>
            {
                [1] = ["unidad", "unidades"],
                [10] = ["decena", "decenas"],
                [100] = ["centena", "centenas"],
                [1000] = ["millar", "millares"]
            },
            ["it"] = new Dictionary<int, IReadOnlyList<string>>
            {
                [1] = ["unità", "unita"],
                [10] = ["decina", "decine"],
                [100] = ["centinaio", "centinaia"],
                [1000] = ["migliaio", "migliaia"]
            },
            ["ja"] = new Dictionary<int, IReadOnlyList<string>>
            {
                [1] = ["一の位", "1の位"],
                [10] = ["十の位", "10の位"],
                [100] = ["百の位", "100の位"],
                [1000] = ["千の位", "1000の位"]
            }
        };

    /// <summary>
    /// Extrait un préfixe de langue pris en charge.
    /// </summary>
    private static string GetLanguagePrefix(string? languageCode)
    {
        return string.IsNullOrWhiteSpace(languageCode) || languageCode.Length < 2
            ? "fr"
            : languageCode[..2].ToLowerInvariant();
    }

    /// <summary>
    /// Ignore les différences de casse, d'accent et de séparateur.
    /// </summary>
    private static string Normalize(string? text)
    {
        var decomposed = (text ?? string.Empty)
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character)
                    != UnicodeCategory.NonSpacingMark
                && char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}
