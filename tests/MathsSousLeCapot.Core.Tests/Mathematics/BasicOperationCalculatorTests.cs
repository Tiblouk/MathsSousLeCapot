using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

/// <summary>
/// Vérifie les règles des opérations fondamentales sur les nombres naturels.
/// </summary>
public sealed class BasicOperationCalculatorTests
{
    [Theory]
    [InlineData(BasicOperation.Addition, 3, 2, 5)]
    [InlineData(BasicOperation.Addition, 0, 4, 4)]
    [InlineData(BasicOperation.Subtraction, 7, 3, 4)]
    [InlineData(BasicOperation.Subtraction, 5, 5, 0)]
    public void Calculates_expected_result(
        BasicOperation operation,
        int left,
        int right,
        int expected)
    {
        Assert.Equal(
            expected,
            BasicOperationCalculator.Calculate(operation, left, right));
    }

    [Fact]
    public void Subtraction_rejects_a_negative_natural_result()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => BasicOperationCalculator.Calculate(
                BasicOperation.Subtraction,
                2,
                3));
    }
}
