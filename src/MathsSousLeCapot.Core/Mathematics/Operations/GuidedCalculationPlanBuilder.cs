namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Décompose un calcul posé en décisions adaptées à la méthode de l'école primaire.
/// </summary>
public static class GuidedCalculationPlanBuilder
{
    /// <summary>
    /// Construit un guide sans exposer directement le résultat à l'interface.
    /// </summary>
    public static GuidedCalculationPlan Create(
        WrittenCalculationKind kind,
        int leftOperand,
        int rightOperand)
    {
        var calculation = WrittenCalculationBuilder.Create(
            kind,
            leftOperand,
            rightOperand);
        var steps = kind switch
        {
            WrittenCalculationKind.Addition => BuildAddition(calculation),
            WrittenCalculationKind.Subtraction => BuildSubtraction(calculation),
            WrittenCalculationKind.Multiplication => BuildMultiplication(calculation),
            WrittenCalculationKind.Division => BuildDivision(calculation),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        return new GuidedCalculationPlan(calculation, steps);
    }

    /// <summary>
    /// Demande le chiffre écrit puis l'éventuelle retenue de chaque colonne.
    /// </summary>
    private static IReadOnlyList<GuidedCalculationStep> BuildAddition(
        WrittenCalculation calculation)
    {
        var steps = new List<GuidedCalculationStep>();
        var columnCount = Math.Max(
            GetDigitCount(calculation.LeftOperand),
            GetDigitCount(calculation.RightOperand));
        var carry = 0;

        for (var column = 0; column < columnCount; column++)
        {
            var leftDigit = GetDigit(calculation.LeftOperand, column);
            var rightDigit = GetDigit(calculation.RightOperand, column);
            var total = leftDigit + rightDigit + carry;
            steps.Add(new GuidedCalculationStep(
                GuidedCalculationStepKind.AdditionDigit,
                total % 10,
                column,
                LeftValue: leftDigit,
                RightValue: rightDigit,
                IncomingValue: carry));

            carry = total / 10;
            if (carry > 0)
            {
                steps.Add(new GuidedCalculationStep(
                    column == columnCount - 1
                        ? GuidedCalculationStepKind.AdditionDigit
                        : GuidedCalculationStepKind.AdditionCarry,
                    carry,
                    column + 1));
            }
        }

        return steps;
    }

    /// <summary>
    /// Signale chaque emprunt avant de demander le chiffre de la différence.
    /// </summary>
    private static IReadOnlyList<GuidedCalculationStep> BuildSubtraction(
        WrittenCalculation calculation)
    {
        var steps = new List<GuidedCalculationStep>();
        var columnCount = GetDigitCount(calculation.LeftOperand);
        var resultColumnCount = GetDigitCount(calculation.Result);
        var borrowedFromCurrentColumn = 0;

        for (var column = 0; column < columnCount; column++)
        {
            var upperDigit = GetDigit(calculation.LeftOperand, column)
                - borrowedFromCurrentColumn;
            var lowerDigit = GetDigit(calculation.RightOperand, column);
            var requiresBorrow = upperDigit < lowerDigit;
            if (requiresBorrow)
            {
                steps.Add(new GuidedCalculationStep(
                    GuidedCalculationStepKind.SubtractionBorrow,
                    1,
                    column,
                    LeftValue: upperDigit,
                    RightValue: lowerDigit));
            }

            var availableUnits = requiresBorrow
                ? upperDigit + 10
                : upperDigit;
            if (column < resultColumnCount)
            {
                steps.Add(new GuidedCalculationStep(
                    GuidedCalculationStepKind.SubtractionDigit,
                    availableUnits - lowerDigit,
                    column,
                    LeftValue: availableUnits,
                    RightValue: lowerDigit));
            }

            borrowedFromCurrentColumn = requiresBorrow ? 1 : 0;
        }

        return steps;
    }

    /// <summary>
    /// Construit chaque produit partiel, ses retenues et leur décalage décimal.
    /// </summary>
    private static IReadOnlyList<GuidedCalculationStep> BuildMultiplication(
        WrittenCalculation calculation)
    {
        var steps = new List<GuidedCalculationStep>();
        var multiplicandDigits = GetDigitCount(calculation.LeftOperand);
        var multiplierDigits = GetDigitCount(calculation.RightOperand);
        var shiftedPartialProducts = new List<int>();

        for (var row = 0; row < multiplierDigits; row++)
        {
            var multiplierDigit = GetDigit(calculation.RightOperand, row);
            shiftedPartialProducts.Add(
                calculation.LeftOperand
                * multiplierDigit
                * GetPowerOfTen(row));
            if (multiplierDigit == 0)
            {
                steps.Add(new GuidedCalculationStep(
                    GuidedCalculationStepKind.MultiplicationDigit,
                    0,
                    RowIndex: row,
                    RightValue: multiplierDigit));
                continue;
            }

            var carry = 0;
            for (var column = 0; column < multiplicandDigits; column++)
            {
                var multiplicandDigit = GetDigit(
                    calculation.LeftOperand,
                    column);
                var total = multiplicandDigit * multiplierDigit + carry;
                steps.Add(new GuidedCalculationStep(
                    GuidedCalculationStepKind.MultiplicationDigit,
                    total % 10,
                    column,
                    row,
                    multiplicandDigit,
                    multiplierDigit,
                    carry));

                carry = total / 10;
                if (carry > 0)
                {
                    steps.Add(new GuidedCalculationStep(
                        column == multiplicandDigits - 1
                            ? GuidedCalculationStepKind.MultiplicationDigit
                            : GuidedCalculationStepKind.MultiplicationCarry,
                        carry,
                        column + 1,
                        row));
                }
            }
        }

        if (multiplierDigits > 1)
        {
            AddFinalProductSteps(steps, shiftedPartialProducts);
        }

        return steps;
    }

    /// <summary>
    /// Décompose l'addition des produits partiels en chiffres et retenues visibles.
    /// </summary>
    private static void AddFinalProductSteps(
        ICollection<GuidedCalculationStep> steps,
        IReadOnlyList<int> shiftedPartialProducts)
    {
        var columnCount = shiftedPartialProducts
            .Select(GetDigitCount)
            .DefaultIfEmpty(1)
            .Max();
        var carry = 0;

        for (var column = 0; column < columnCount; column++)
        {
            var total = carry + shiftedPartialProducts.Sum(
                product => GetDigit(product, column));
            steps.Add(new GuidedCalculationStep(
                GuidedCalculationStepKind.MultiplicationFinalDigit,
                total % 10,
                column));

            carry = total / 10;
            if (carry > 0)
            {
                steps.Add(new GuidedCalculationStep(
                    column == columnCount - 1
                        ? GuidedCalculationStepKind.MultiplicationFinalDigit
                        : GuidedCalculationStepKind.MultiplicationFinalCarry,
                    carry,
                    column + 1));
            }
        }
    }

    /// <summary>
    /// Vérifie chaque chiffre du quotient, produit soustrait et reste partiel.
    /// </summary>
    private static IReadOnlyList<GuidedCalculationStep> BuildDivision(
        WrittenCalculation calculation)
    {
        var divisionSteps = WrittenCalculationBuilder
            .GetDivisionSteps(calculation)
            .ToArray();
        var firstVisibleIndex = Array.FindIndex(
            divisionSteps,
            step => step.QuotientDigit > 0);
        if (firstVisibleIndex < 0)
        {
            firstVisibleIndex = divisionSteps.Length - 1;
        }

        var steps = new List<GuidedCalculationStep>();
        for (var index = firstVisibleIndex; index < divisionSteps.Length; index++)
        {
            var divisionStep = divisionSteps[index];
            var row = index - firstVisibleIndex;
            steps.Add(new GuidedCalculationStep(
                GuidedCalculationStepKind.DivisionQuotientDigit,
                divisionStep.QuotientDigit,
                RowIndex: row,
                LeftValue: divisionStep.PartialDividend,
                RightValue: calculation.RightOperand));
            steps.Add(new GuidedCalculationStep(
                GuidedCalculationStepKind.DivisionProduct,
                divisionStep.SubtractedValue,
                RowIndex: row,
                LeftValue: divisionStep.QuotientDigit,
                RightValue: calculation.RightOperand));
            steps.Add(new GuidedCalculationStep(
                GuidedCalculationStepKind.DivisionRemainder,
                divisionStep.Remainder,
                RowIndex: row,
                LeftValue: divisionStep.PartialDividend,
                RightValue: divisionStep.SubtractedValue));
        }

        return steps;
    }

    /// <summary>
    /// Lit le chiffre d'une colonne, avec les unités à l'indice zéro.
    /// </summary>
    private static int GetDigit(int value, int columnIndex)
    {
        for (var column = 0; column < columnIndex; column++)
        {
            value /= 10;
        }

        return value % 10;
    }

    /// <summary>
    /// Compte les chiffres d'un entier naturel, zéro occupant une colonne.
    /// </summary>
    private static int GetDigitCount(int value)
    {
        var count = 1;
        while (value >= 10)
        {
            value /= 10;
            count++;
        }

        return count;
    }

    /// <summary>
    /// Retourne la puissance de dix associée au décalage d'une ligne.
    /// </summary>
    private static int GetPowerOfTen(int exponent)
    {
        var value = 1;
        for (var index = 0; index < exponent; index++)
        {
            value *= 10;
        }

        return value;
    }
}
