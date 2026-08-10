namespace MathsSousLeCapot.Core.Mathematics.Measurements;

/// <summary>
/// Fournit les familles d'unités métriques dans un ordre commun.
/// </summary>
public static class MeasurementUnitCatalog
{
    /// <summary>
    /// Lignes affichées dans le tableau des unités de mesure.
    /// </summary>
    public static IReadOnlyList<MeasurementUnitRow> Rows { get; } =
    [
        new(
            "measure.table.length",
            ["mm", "cm", "dm", "m", "dam", "hm", "km"],
            10),
        new(
            "measure.table.mass",
            ["mg", "cg", "dg", "g", "dag", "hg", "kg"],
            10),
        new(
            "measure.table.capacity",
            ["mL", "cL", "dL", "L", "daL", "hL", "kL"],
            10),
        new(
            "measure.table.area",
            ["mm²", "cm²", "dm²", "m²", "dam²", "hm²", "km²"],
            100)
    ];
}
