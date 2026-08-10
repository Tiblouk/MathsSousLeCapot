namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Calcule les opérations fondamentales dans le périmètre des nombres naturels.
/// </summary>
public static class BasicOperationCalculator
{
    /// <summary>
    /// Calcule le résultat de l'opération demandée.
    /// </summary>
    public static int Calculate(BasicOperation operation, int left, int right)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(left);
        ArgumentOutOfRangeException.ThrowIfNegative(right);

        return operation switch
        {
            BasicOperation.Addition => checked(left + right),
            BasicOperation.Subtraction when right <= left => left - right,
            BasicOperation.Subtraction => throw new ArgumentOutOfRangeException(
                nameof(right),
                "Le nombre retiré ne peut pas dépasser la quantité de départ."),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
    }
}
