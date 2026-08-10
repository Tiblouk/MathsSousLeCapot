using MathsSousLeCapot.Core.Mathematics.Measurements;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

/// <summary>
/// Vérifie l'ordre et les facteurs du tableau des unités métriques.
/// </summary>
public sealed class MeasurementUnitCatalogTests
{
    [Fact]
    public void Metric_rows_share_seven_ordered_units()
    {
        Assert.Equal(4, MeasurementUnitCatalog.Rows.Count);
        Assert.All(
            MeasurementUnitCatalog.Rows,
            row => Assert.Equal(7, row.Units.Count));

        Assert.Equal(
            ["mm", "cm", "dm", "m", "dam", "hm", "km"],
            MeasurementUnitCatalog.Rows[0].Units);
        Assert.Equal(
            ["mL", "cL", "dL", "L", "daL", "hL", "kL"],
            MeasurementUnitCatalog.Rows[2].Units);
    }

    [Fact]
    public void Area_steps_are_squared_while_linear_steps_use_ten()
    {
        Assert.All(
            MeasurementUnitCatalog.Rows.Take(3),
            row => Assert.Equal(10, row.StepFactor));
        Assert.Equal(100, MeasurementUnitCatalog.Rows[3].StepFactor);
    }
}
