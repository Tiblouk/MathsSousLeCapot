namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Décrit les opérandes et le résultat exact d'un calcul posé.
/// </summary>
public sealed record WrittenCalculation(
    WrittenCalculationKind Kind,
    int LeftOperand,
    int RightOperand,
    int Result,
    int Remainder = 0);
