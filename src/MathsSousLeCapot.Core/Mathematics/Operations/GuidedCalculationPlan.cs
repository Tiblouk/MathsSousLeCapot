namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Regroupe l'opération posée et les réponses intermédiaires à vérifier dans l'ordre.
/// </summary>
/// <param name="Calculation">Calcul naturel construit et vérifié.</param>
/// <param name="Steps">Étapes que l'élève doit valider successivement.</param>
public sealed record GuidedCalculationPlan(
    WrittenCalculation Calculation,
    IReadOnlyList<GuidedCalculationStep> Steps);
