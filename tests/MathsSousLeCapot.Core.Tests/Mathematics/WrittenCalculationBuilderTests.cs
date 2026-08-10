using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

/// <summary>
/// Vérifie les retenues, emprunts et étapes des quatre calculs posés.
/// </summary>
public sealed class WrittenCalculationBuilderTests
{
    [Fact]
    public void Addition_exposes_its_carries()
    {
        var calculation = WrittenCalculationBuilder.Create(
            WrittenCalculationKind.Addition,
            34,
            27);

        Assert.Equal(61, calculation.Result);
        Assert.Equal([1, 0], WrittenCalculationBuilder.GetAdditionCarries(calculation));
    }

    [Fact]
    public void Subtraction_exposes_borrowed_columns()
    {
        var calculation = WrittenCalculationBuilder.Create(
            WrittenCalculationKind.Subtraction,
            324,
            167);

        Assert.Equal(157, calculation.Result);
        Assert.Equal([0, 1], WrittenCalculationBuilder.GetSubtractionBorrowColumns(calculation));
    }

    [Fact]
    public void Multiplication_exposes_shifted_partial_products()
    {
        var calculation = WrittenCalculationBuilder.Create(
            WrittenCalculationKind.Multiplication,
            529,
            25);

        Assert.Equal(13225, calculation.Result);
        Assert.Equal(
            [2645, 1058],
            WrittenCalculationBuilder.GetPartialProducts(calculation));
    }

    [Fact]
    public void Division_exposes_quotient_remainder_and_steps()
    {
        var calculation = WrittenCalculationBuilder.Create(
            WrittenCalculationKind.Division,
            5042,
            3);
        var steps = WrittenCalculationBuilder.GetDivisionSteps(calculation);

        Assert.Equal(1680, calculation.Result);
        Assert.Equal(2, calculation.Remainder);
        Assert.Equal(4, steps.Count);
        Assert.Equal(new WrittenDivisionStep(24, 8, 24, 0), steps[^2]);
        Assert.Equal(new WrittenDivisionStep(2, 0, 0, 2), steps[^1]);
    }

    [Fact]
    public void Division_by_zero_is_rejected()
    {
        Assert.Throws<DivideByZeroException>(() =>
            WrittenCalculationBuilder.Create(
                WrittenCalculationKind.Division,
                42,
                0));
    }
}
