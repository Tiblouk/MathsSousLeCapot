using System.Collections.Concurrent;
using System.Globalization;
using System.Text;

namespace MathsSousLeCapot.Core.Mathematics.Numbers;

/// <summary>
/// Écrit et reconnaît les entiers selon une langue explicitement demandée.
/// </summary>
public static class LocalizedNumberConverter
{
    /// <summary>
    /// Valeur maximale prise en charge par les cours actuels.
    /// </summary>
    public const int MaximumValue = 9999;

    /// <summary>
    /// Cache des formes normalisées acceptées pour chaque langue.
    /// </summary>
    private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, int>>
        ParseCaches = new(StringComparer.Ordinal);

    /// <summary>
    /// Écrit un entier compris entre -9 999 et 9 999 dans la langue demandée.
    /// </summary>
    public static string ToWords(int value, string languageCode)
    {
        if (value is < -MaximumValue or > MaximumValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        var languagePrefix = GetLanguagePrefix(languageCode);
        if (value < 0)
        {
            return $"{GetNegativeWord(languagePrefix)} {ToWords(-value, languageCode)}";
        }

        return languagePrefix switch
        {
            "en" => English(value),
            "es" => Spanish(value),
            "it" => Italian(value),
            "ja" => Japanese(value),
            _ => FrenchNumberNames.ToWords(value)
        };
    }

    /// <summary>
    /// Vérifie qu'une réponse en chiffres ou en mots correspond à la valeur attendue.
    /// </summary>
    public static bool Matches(string? text, int expectedValue, string languageCode)
    {
        if (expectedValue is < -MaximumValue or > MaximumValue)
        {
            return false;
        }

        var normalizedNumericText = NormalizeMinusSign(text?.Trim());
        if (int.TryParse(
            normalizedNumericText,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var numeric))
        {
            return numeric == expectedValue;
        }

        var languagePrefix = GetLanguagePrefix(languageCode);
        var cache = ParseCaches.GetOrAdd(languagePrefix, CreateParseCache);

        return cache.TryGetValue(Normalize(text), out var parsed) && parsed == expectedValue;
    }

    /// <summary>
    /// Extrait un préfixe ISO stable et revient au français pour un code invalide.
    /// </summary>
    private static string GetLanguagePrefix(string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode) || languageCode.Length < 2)
        {
            return "fr";
        }

        return languageCode[..2].ToLowerInvariant();
    }

    /// <summary>
    /// Génère les formes canoniques acceptées pour une langue.
    /// </summary>
    private static IReadOnlyDictionary<string, int> CreateParseCache(string languageCode)
    {
        var values = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var value = -MaximumValue; value <= MaximumValue; value++)
        {
            values[Normalize(ToWords(value, languageCode))] = value;
        }

        return values;
    }

    /// <summary>
    /// Convertit le signe moins mathématique en signe ASCII compris par l'analyseur .NET.
    /// </summary>
    private static string NormalizeMinusSign(string? text)
    {
        return (text ?? string.Empty).Replace('−', '-');
    }

    /// <summary>
    /// Retourne le mot qui introduit un entier négatif dans la langue active.
    /// </summary>
    private static string GetNegativeWord(string languageCode)
    {
        return languageCode switch
        {
            "en" => "minus",
            "es" => "menos",
            "it" => "meno",
            "ja" => "マイナス",
            _ => "moins"
        };
    }

    /// <summary>
    /// Ignore la casse, les accents et les séparateurs non significatifs.
    /// </summary>
    private static string Normalize(string? text)
    {
        var decomposed = (text ?? string.Empty)
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark
                && char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Formate un nombre anglais.
    /// </summary>
    private static string English(int value)
    {
        string[] small =
        [
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
            "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
            "seventeen", "eighteen", "nineteen"
        ];
        string[] tens =
        [
            "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty",
            "ninety"
        ];

        if (value < 20)
        {
            return small[value];
        }

        if (value < 100)
        {
            return value % 10 == 0
                ? tens[value / 10]
                : $"{tens[value / 10]}-{small[value % 10]}";
        }

        if (value < 1000)
        {
            var remainder = value % 100;
            return remainder == 0
                ? $"{small[value / 100]} hundred"
                : $"{small[value / 100]} hundred {English(remainder)}";
        }

        var rest = value % 1000;
        return rest == 0
            ? $"{English(value / 1000)} thousand"
            : $"{English(value / 1000)} thousand {English(rest)}";
    }

    /// <summary>
    /// Formate un nombre espagnol.
    /// </summary>
    private static string Spanish(int value)
    {
        string[] small =
        [
            "cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve",
            "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete",
            "dieciocho", "diecinueve", "veinte", "veintiuno", "veintidós", "veintitrés",
            "veinticuatro", "veinticinco", "veintiséis", "veintisiete", "veintiocho", "veintinueve"
        ];
        string[] tens =
        [
            "", "", "", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta",
            "noventa"
        ];
        string[] hundreds =
        [
            "", "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos",
            "seiscientos", "setecientos", "ochocientos", "novecientos"
        ];

        if (value < 30)
        {
            return small[value];
        }

        if (value < 100)
        {
            return value % 10 == 0
                ? tens[value / 10]
                : $"{tens[value / 10]} y {small[value % 10]}";
        }

        if (value == 100)
        {
            return "cien";
        }

        if (value < 1000)
        {
            return value % 100 == 0
                ? hundreds[value / 100]
                : $"{hundreds[value / 100]} {Spanish(value % 100)}";
        }

        var thousands = value / 1000;
        var prefix = thousands == 1 ? "mil" : $"{Spanish(thousands)} mil";
        return value % 1000 == 0 ? prefix : $"{prefix} {Spanish(value % 1000)}";
    }

    /// <summary>
    /// Formate un nombre italien.
    /// </summary>
    private static string Italian(int value)
    {
        string[] small =
        [
            "zero", "uno", "due", "tre", "quattro", "cinque", "sei", "sette", "otto", "nove",
            "dieci", "undici", "dodici", "tredici", "quattordici", "quindici", "sedici",
            "diciassette", "diciotto", "diciannove"
        ];
        string[] tens =
        [
            "", "", "venti", "trenta", "quaranta", "cinquanta", "sessanta", "settanta",
            "ottanta", "novanta"
        ];

        if (value < 20)
        {
            return small[value];
        }

        if (value < 100)
        {
            var unit = value % 10;
            var prefix = tens[value / 10];
            if (unit is 1 or 8)
            {
                prefix = prefix[..^1];
            }

            return unit == 0 ? prefix : $"{prefix}{small[unit]}";
        }

        if (value < 1000)
        {
            var hundreds = value / 100;
            var remainder = value % 100;
            var prefix = hundreds == 1 ? "cento" : $"{small[hundreds]}cento";
            if (remainder == 8 || remainder / 10 == 8)
            {
                prefix = prefix[..^1];
            }

            return remainder == 0 ? prefix : $"{prefix}{Italian(remainder)}";
        }

        var thousands = value / 1000;
        var thousandPrefix = thousands == 1 ? "mille" : $"{Italian(thousands)}mila";
        return value % 1000 == 0
            ? thousandPrefix
            : $"{thousandPrefix}{Italian(value % 1000)}";
    }

    /// <summary>
    /// Formate un nombre japonais avec les unités 十、百 et 千.
    /// </summary>
    private static string Japanese(int value)
    {
        string[] digits = ["零", "一", "二", "三", "四", "五", "六", "七", "八", "九"];
        if (value < 10)
        {
            return digits[value];
        }

        var builder = new StringBuilder();
        var places = new[] { (1000, "千"), (100, "百"), (10, "十") };

        foreach (var (place, suffix) in places)
        {
            var digit = value / place % 10;
            if (digit == 0)
            {
                continue;
            }

            if (digit > 1)
            {
                builder.Append(digits[digit]);
            }

            builder.Append(suffix);
        }

        if (value % 10 > 0)
        {
            builder.Append(digits[value % 10]);
        }

        return builder.ToString();
    }
}
