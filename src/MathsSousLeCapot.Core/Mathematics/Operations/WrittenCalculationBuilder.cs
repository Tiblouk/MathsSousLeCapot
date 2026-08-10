namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Construit et vérifie les données nécessaires aux calculs posés.
/// </summary>
public static class WrittenCalculationBuilder
{
    /// <summary>
    /// Crée un calcul posé à partir de deux entiers naturels.
    /// </summary>
    public static WrittenCalculation Create(
        WrittenCalculationKind kind,
        int leftOperand,
        int rightOperand)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(leftOperand);
        ArgumentOutOfRangeException.ThrowIfNegative(rightOperand);

        return kind switch
        {
            WrittenCalculationKind.Addition => new(
                kind,
                leftOperand,
                rightOperand,
                checked(leftOperand + rightOperand)),
            WrittenCalculationKind.Subtraction when rightOperand <= leftOperand => new(
                kind,
                leftOperand,
                rightOperand,
                leftOperand - rightOperand),
            WrittenCalculationKind.Subtraction => throw new ArgumentOutOfRangeException(
                nameof(rightOperand),
                "Le second opérande ne peut pas dépasser le premier."),
            WrittenCalculationKind.Multiplication => new(
                kind,
                leftOperand,
                rightOperand,
                checked(leftOperand * rightOperand)),
            WrittenCalculationKind.Division when rightOperand > 0 => new(
                kind,
                leftOperand,
                rightOperand,
                leftOperand / rightOperand,
                leftOperand % rightOperand),
            WrittenCalculationKind.Division => throw new DivideByZeroException(
                "Le diviseur d'un calcul posé doit être strictement positif."),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    /// <summary>
    /// Calcule les produits intermédiaires d'une multiplication posée.
    /// </summary>
    public static IReadOnlyList<int> GetPartialProducts(
        WrittenCalculation calculation)
    {
        EnsureKind(calculation, WrittenCalculationKind.Multiplication);
        var multiplier = calculation.RightOperand;
        var products = new List<int>();

        do
        {
            var digit = multiplier % 10;
            products.Add(calculation.LeftOperand * digit);
            multiplier /= 10;
        }
        while (multiplier > 0);

        return products;
    }

    /// <summary>
    /// Décompose une division selon les chiffres successivement abaissés.
    /// </summary>
    public static IReadOnlyList<WrittenDivisionStep> GetDivisionSteps(
        WrittenCalculation calculation)
    {
        EnsureKind(calculation, WrittenCalculationKind.Division);
        var divisor = calculation.RightOperand;
        var current = 0;
        var steps = new List<WrittenDivisionStep>();

        foreach (var character in calculation.LeftOperand.ToString())
        {
            var digit = character - '0';
            current = current * 10 + digit;
            var quotientDigit = current / divisor;
            var subtracted = quotientDigit * divisor;
            var remainder = current - subtracted;
            steps.Add(new WrittenDivisionStep(
                current,
                quotientDigit,
                subtracted,
                remainder));
            current = remainder;
        }

        return steps;
    }

    /// <summary>
    /// Retourne les retenues d'une addition, alignées des unités vers la gauche.
    /// </summary>
    public static IReadOnlyList<int> GetAdditionCarries(
        WrittenCalculation calculation)
    {
        EnsureKind(calculation, WrittenCalculationKind.Addition);
        var left = calculation.LeftOperand;
        var right = calculation.RightOperand;
        var carries = new List<int>();
        var carry = 0;

        do
        {
            var sum = left % 10 + right % 10 + carry;
            carry = sum / 10;
            carries.Add(carry);
            left /= 10;
            right /= 10;
        }
        while (left > 0 || right > 0);

        return carries;
    }

    /// <summary>
    /// Retourne les colonnes dans lesquelles une soustraction exige un emprunt.
    /// </summary>
    public static IReadOnlyList<int> GetSubtractionBorrowColumns(
        WrittenCalculation calculation)
    {
        EnsureKind(calculation, WrittenCalculationKind.Subtraction);
        var left = calculation.LeftOperand;
        var right = calculation.RightOperand;
        var borrowed = 0;
        var column = 0;
        var columns = new List<int>();

        while (left > 0 || right > 0)
        {
            var leftDigit = left % 10 - borrowed;
            var rightDigit = right % 10;
            borrowed = 0;
            if (leftDigit < rightDigit)
            {
                columns.Add(column);
                borrowed = 1;
            }

            left /= 10;
            right /= 10;
            column++;
        }

        return columns;
    }

    /// <summary>
    /// Vérifie qu'une méthode spécialisée reçoit le bon type de calcul.
    /// </summary>
    private static void EnsureKind(
        WrittenCalculation calculation,
        WrittenCalculationKind expectedKind)
    {
        if (calculation.Kind != expectedKind)
        {
            throw new ArgumentException(
                $"Le calcul doit être de type {expectedKind}.",
                nameof(calculation));
        }
    }
}

/// <summary>
/// Décrit une étape élémentaire de la division posée.
/// </summary>
public sealed record WrittenDivisionStep(
    int PartialDividend,
    int QuotientDigit,
    int SubtractedValue,
    int Remainder);
