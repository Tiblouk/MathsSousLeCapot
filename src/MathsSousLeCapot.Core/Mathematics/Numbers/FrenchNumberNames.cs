namespace MathsSousLeCapot.Core.Mathematics.Numbers;

/// <summary>
/// Convertit les entiers du cours en leur écriture française.
/// </summary>
public static class FrenchNumberNames
{
    /// <summary>
    /// Noms irréguliers utilisés directement jusqu'à seize.
    /// </summary>
    private static readonly string[] Units =
    [
        "zéro",
        "un",
        "deux",
        "trois",
        "quatre",
        "cinq",
        "six",
        "sept",
        "huit",
        "neuf",
        "dix",
        "onze",
        "douze",
        "treize",
        "quatorze",
        "quinze",
        "seize"
    ];

    /// <summary>
    /// Préfixes réguliers des dizaines de dix à soixante.
    /// </summary>
    private static readonly string[] Tens =
    [
        string.Empty,
        "dix",
        "vingt",
        "trente",
        "quarante",
        "cinquante",
        "soixante"
    ];

    /// <summary>
    /// Convertit un entier compris entre zéro et 9 999.
    /// </summary>
    public static string ToWords(int value)
    {
        if (value is < 0 or > 9999)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Le cours prend en charge les nombres de 0 à 9 999.");
        }

        if (value < 100)
        {
            return BelowOneHundred(value);
        }

        if (value < 1000)
        {
            return BelowOneThousand(value);
        }

        var thousands = value / 1000;
        var remainder = value % 1000;
        var prefix = thousands == 1
            ? "mille"
            : $"{BelowOneHundred(thousands)} mille";

        return remainder == 0
            ? prefix
            : $"{prefix} {BelowOneThousand(remainder)}";
    }

    /// <summary>
    /// Convertit la partie inférieure à mille.
    /// </summary>
    private static string BelowOneThousand(int value)
    {
        if (value < 100)
        {
            return BelowOneHundred(value);
        }

        var hundreds = value / 100;
        var remainder = value % 100;
        var prefix = hundreds == 1
            ? "cent"
            : $"{Units[hundreds]} cent";

        if (remainder == 0)
        {
            return hundreds > 1 ? $"{prefix}s" : prefix;
        }

        return $"{prefix} {BelowOneHundred(remainder)}";
    }

    /// <summary>
    /// Convertit la partie inférieure à cent avec les règles françaises particulières.
    /// </summary>
    private static string BelowOneHundred(int value)
    {
        if (value <= 16)
        {
            return Units[value];
        }

        if (value < 20)
        {
            return $"dix-{Units[value - 10]}";
        }

        if (value < 70)
        {
            var tens = value / 10;
            var unit = value % 10;

            if (unit == 0)
            {
                return Tens[tens];
            }

            if (unit == 1)
            {
                return $"{Tens[tens]} et un";
            }

            return $"{Tens[tens]}-{Units[unit]}";
        }

        if (value < 80)
        {
            return value == 71
                ? "soixante et onze"
                : $"soixante-{BelowOneHundred(value - 60)}";
        }

        if (value == 80)
        {
            return "quatre-vingts";
        }

        return $"quatre-vingt-{BelowOneHundred(value - 80)}";
    }
}
