using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

/// <summary>
/// Vérifie la progression pédagogique des quatre calculs posés guidés.
/// </summary>
public sealed class GuidedCalculationPlanBuilderTests
{
    /// <summary>
    /// Vérifie que chaque retenue d'une addition suit le chiffre de sa colonne.
    /// </summary>
    [Fact]
    public void Addition_alternates_result_digits_and_required_carries()
    {
        var plan = GuidedCalculationPlanBuilder.Create(
            WrittenCalculationKind.Addition,
            68,
            47);

        Assert.Collection(
            plan.Steps,
            step => AssertStep(step, GuidedCalculationStepKind.AdditionDigit, 5),
            step => AssertStep(step, GuidedCalculationStepKind.AdditionCarry, 1),
            step => AssertStep(step, GuidedCalculationStepKind.AdditionDigit, 1),
            step => AssertStep(step, GuidedCalculationStepKind.AdditionDigit, 1));
    }

    /// <summary>
    /// Vérifie qu'un emprunt traverse correctement une colonne contenant zéro.
    /// </summary>
    [Fact]
    public void Subtraction_guides_chained_borrows_without_skipping_a_column()
    {
        var plan = GuidedCalculationPlanBuilder.Create(
            WrittenCalculationKind.Subtraction,
            100,
            3);

        Assert.Collection(
            plan.Steps,
            step => AssertStep(step, GuidedCalculationStepKind.SubtractionBorrow, 1),
            step => AssertStep(step, GuidedCalculationStepKind.SubtractionDigit, 7),
            step => AssertStep(step, GuidedCalculationStepKind.SubtractionBorrow, 1),
            step => AssertStep(step, GuidedCalculationStepKind.SubtractionDigit, 9));
    }

    /// <summary>
    /// Vérifie le décalage visuel et la somme finale d'une multiplication par dix.
    /// </summary>
    [Fact]
    public void Multiplication_guides_partial_products_shift_and_final_sum()
    {
        var plan = GuidedCalculationPlanBuilder.Create(
            WrittenCalculationKind.Multiplication,
            12,
            10);

        Assert.Collection(
            plan.Steps,
            step => AssertStep(step, GuidedCalculationStepKind.MultiplicationDigit, 0),
            step => AssertStep(step, GuidedCalculationStepKind.MultiplicationDigit, 2),
            step => AssertStep(step, GuidedCalculationStepKind.MultiplicationDigit, 1),
            step => AssertStep(step, GuidedCalculationStepKind.MultiplicationFinalDigit, 0),
            step => AssertStep(step, GuidedCalculationStepKind.MultiplicationFinalDigit, 2),
            step => AssertStep(step, GuidedCalculationStepKind.MultiplicationFinalDigit, 1));
    }

    /// <summary>
    /// Vérifie que les retenues internes d'un produit sont demandées explicitement.
    /// </summary>
    [Fact]
    public void Multiplication_exposes_each_required_carry()
    {
        var plan = GuidedCalculationPlanBuilder.Create(
            WrittenCalculationKind.Multiplication,
            29,
            4);

        Assert.Equal(
            [
                GuidedCalculationStepKind.MultiplicationDigit,
                GuidedCalculationStepKind.MultiplicationCarry,
                GuidedCalculationStepKind.MultiplicationDigit,
                GuidedCalculationStepKind.MultiplicationDigit
            ],
            plan.Steps.Select(step => step.Kind));
        Assert.Equal([6, 3, 1, 1], plan.Steps.Select(step => step.ExpectedValue));
    }

    /// <summary>
    /// Vérifie que l'addition finale des lignes conserve ses propres retenues.
    /// </summary>
    [Fact]
    public void Multiplication_final_rows_are_added_column_by_column()
    {
        var plan = GuidedCalculationPlanBuilder.Create(
            WrittenCalculationKind.Multiplication,
            99,
            99);
        var finalSteps = plan.Steps
            .Where(step => step.Kind is
                GuidedCalculationStepKind.MultiplicationFinalDigit
                or GuidedCalculationStepKind.MultiplicationFinalCarry)
            .ToArray();

        Assert.Equal(
            [
                GuidedCalculationStepKind.MultiplicationFinalDigit,
                GuidedCalculationStepKind.MultiplicationFinalDigit,
                GuidedCalculationStepKind.MultiplicationFinalCarry,
                GuidedCalculationStepKind.MultiplicationFinalDigit,
                GuidedCalculationStepKind.MultiplicationFinalCarry,
                GuidedCalculationStepKind.MultiplicationFinalDigit
            ],
            finalSteps.Select(step => step.Kind));
        Assert.Equal([1, 0, 1, 8, 1, 9], finalSteps.Select(step => step.ExpectedValue));
    }

    /// <summary>
    /// Vérifie que la division conserve les zéros du quotient et son reste final.
    /// </summary>
    [Fact]
    public void Division_guides_every_visible_quotient_digit_product_and_remainder()
    {
        var plan = GuidedCalculationPlanBuilder.Create(
            WrittenCalculationKind.Division,
            100,
            10);

        Assert.Collection(
            plan.Steps,
            step => AssertStep(step, GuidedCalculationStepKind.DivisionQuotientDigit, 1),
            step => AssertStep(step, GuidedCalculationStepKind.DivisionProduct, 10),
            step => AssertStep(step, GuidedCalculationStepKind.DivisionRemainder, 0),
            step => AssertStep(step, GuidedCalculationStepKind.DivisionQuotientDigit, 0),
            step => AssertStep(step, GuidedCalculationStepKind.DivisionProduct, 0),
            step => AssertStep(step, GuidedCalculationStepKind.DivisionRemainder, 0));
        Assert.Equal(10, plan.Calculation.Result);
        Assert.Equal(0, plan.Calculation.Remainder);
    }

    /// <summary>
    /// Vérifie que le reste final n'est jamais confondu avec le quotient entier.
    /// </summary>
    [Fact]
    public void Division_keeps_the_final_remainder_in_the_last_guided_step()
    {
        var plan = GuidedCalculationPlanBuilder.Create(
            WrittenCalculationKind.Division,
            58,
            4);

        Assert.Equal(14, plan.Calculation.Result);
        Assert.Equal(2, plan.Calculation.Remainder);
        Assert.Equal(
            [1, 4, 1, 4, 16, 2],
            plan.Steps.Select(step => step.ExpectedValue));
        Assert.Equal(
            GuidedCalculationStepKind.DivisionRemainder,
            plan.Steps[^1].Kind);
    }

    /// <summary>
    /// Compare la nature et la réponse attendue d'une étape.
    /// </summary>
    private static void AssertStep(
        GuidedCalculationStep step,
        GuidedCalculationStepKind expectedKind,
        int expectedValue)
    {
        Assert.Equal(expectedKind, step.Kind);
        Assert.Equal(expectedValue, step.ExpectedValue);
    }
}
