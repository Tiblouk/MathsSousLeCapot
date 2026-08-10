namespace MathsSousLeCapot.Core.Mathematics.Measurements;

/// <summary>
/// Décrit une ligne du tableau métrique et le facteur entre deux colonnes.
/// </summary>
public sealed record MeasurementUnitRow(
    string NameKey,
    IReadOnlyList<string> Units,
    int StepFactor);
