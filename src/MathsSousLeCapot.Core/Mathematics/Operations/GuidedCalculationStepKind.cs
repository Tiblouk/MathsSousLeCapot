namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Identifie la décision élémentaire demandée pendant un calcul posé guidé.
/// </summary>
public enum GuidedCalculationStepKind
{
    AdditionDigit,
    AdditionCarry,
    SubtractionBorrow,
    SubtractionDigit,
    MultiplicationDigit,
    MultiplicationCarry,
    MultiplicationFinalDigit,
    MultiplicationFinalCarry,
    DivisionQuotientDigit,
    DivisionProduct,
    DivisionRemainder
}
