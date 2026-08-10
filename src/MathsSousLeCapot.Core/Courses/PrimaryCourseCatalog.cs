namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Catalogue des notions du niveau primaire et de leur famille d'exercices.
/// </summary>
public static class PrimaryCourseCatalog
{
    /// <summary>
    /// Clés stables des cinq classes de l'école primaire.
    /// </summary>
    public const string CpLevel = "chapter.level.cp";
    public const string Ce1Level = "chapter.level.ce1";
    public const string Ce2Level = "chapter.level.ce2";
    public const string Cm1Level = "chapter.level.cm1";
    public const string Cm2Level = "chapter.level.cm2";

    /// <summary>
    /// Définitions ordonnées selon le catalogue pédagogique.
    /// </summary>
    public static IReadOnlyList<PrimaryCourseDefinition> Definitions { get; } =
    [
        new("compare-and-order-numbers", "understand-numbers", "primary.compare.title",
            CourseCatalog.CountingCourseId, "4 < 9 < 12", PrimaryExerciseKind.CompareNumbers, CpLevel),
        new("large-numbers", "understand-numbers", "primary.largeNumbers.title",
            CourseCatalog.PositionalCounterCourseId, "24 305", PrimaryExerciseKind.PlaceValue, Cm1Level),
        new("rounding-and-estimation", "understand-numbers", "primary.rounding.title",
            "compare-and-order-numbers", "47 ≈ 50", PrimaryExerciseKind.RoundToTen, Ce2Level),
        new("fractions-as-numbers", "understand-numbers", "primary.fractions.title",
            "division-basics", "3 / 4", PrimaryExerciseKind.FractionOfSet, Cm1Level),

        new("multiplication-basics", "understand-operations", "primary.multiplication.title",
            CourseCatalog.AdditionCourseId, "3 × 4 = 12", PrimaryExerciseKind.Multiplication, Ce1Level),
        new("division-basics", "understand-operations", "primary.division.title",
            "multiplication-basics", "12 ÷ 3 = 4", PrimaryExerciseKind.ExactDivision, Ce2Level),
        new("multiplication-tables", "understand-operations", "primary.tables.title",
            "multiplication-basics", "7 × 8 = 56", PrimaryExerciseKind.MultiplicationTable, Ce2Level),
        new("written-addition", "understand-operations", "primary.writtenAddition.title",
            CourseCatalog.AdditionCourseId, "248 + 135 = 383", PrimaryExerciseKind.Addition, Ce1Level),
        new("written-subtraction", "understand-operations", "primary.writtenSubtraction.title",
            CourseCatalog.SubtractionCourseId, "482 − 157 = 325", PrimaryExerciseKind.Subtraction, Ce1Level),
        new("written-multiplication", "understand-operations", "primary.writtenMultiplication.title",
            "multiplication-tables", "24 × 13 = 312", PrimaryExerciseKind.WrittenMultiplication, Ce2Level),
        new("written-division", "understand-operations", "primary.writtenDivision.title",
            "division-basics", "84 ÷ 7 = 12", PrimaryExerciseKind.ExactDivision, Cm1Level),
        new("mental-calculation", "understand-operations", "primary.mentalCalculation.title",
            CourseCatalog.AdditionCourseId, "25 + 9 = 34", PrimaryExerciseKind.MentalCalculation, CpLevel),

        new("repeated-multiplication", "understand-powers", "primary.repeatedMultiplication.title",
            "multiplication-basics", "3 × 3 × 3 = 27", PrimaryExerciseKind.RepeatedMultiplication, Ce2Level),

        new("length-measurement", "measurements", "primary.lengthMeasurement.title",
            CourseCatalog.PositionalCounterCourseId, "8 cm", PrimaryExerciseKind.LengthReading, Ce1Level),
        new("length-unit-conversion", "measurements", "primary.lengthConversion.title",
            "length-measurement", "2 m = 200 cm", PrimaryExerciseKind.LengthConversion, Ce2Level),
        new("mass-measurement", "measurements", "primary.massMeasurement.title",
            CourseCatalog.PositionalCounterCourseId, "750 g", PrimaryExerciseKind.MassReading, Ce2Level),
        new("mass-unit-conversion", "measurements", "primary.massConversion.title",
            "mass-measurement", "3 kg = 3 000 g", PrimaryExerciseKind.MassConversion, Cm1Level),
        new("time-measurement", "measurements", "primary.timeMeasurement.title",
            CourseCatalog.CountingCourseId, "2 h = 120 min", PrimaryExerciseKind.TimeReading, Ce1Level),
        new("time-calculation", "measurements", "primary.timeCalculation.title",
            "time-measurement", "1 h 20 + 35 min = 1 h 55", PrimaryExerciseKind.TimeCalculation, Cm2Level),
        new("area-measurement", "measurements", "primary.areaMeasurement.title",
            "multiplication-basics", "4 × 3 = 12 cm²", PrimaryExerciseKind.UnitSquareArea, Cm1Level),

        new("recognize-proportionality", "proportionality", "primary.proportionality.title",
            "multiplication-basics", "2 → 6 ; 4 → 12", PrimaryExerciseKind.Proportionality, Cm1Level),

        new("points-lines-segments", "geometry-plane", "primary.pointsLines.title",
            null, "A •────• B", PrimaryExerciseKind.SegmentEndpoints, Ce2Level),
        new("parallel-and-perpendicular-lines", "geometry-plane", "primary.parallelLines.title",
            "points-lines-segments", "∥   ⟂", PrimaryExerciseKind.ParallelIntersections, Cm1Level),
        new("angles-introduction", "geometry-plane", "primary.angles.title",
            "points-lines-segments", "∠", PrimaryExerciseKind.RightAngle, Cm1Level),
        new("angle-measurement", "geometry-plane", "primary.angleMeasurement.title",
            "angles-introduction", "90°", PrimaryExerciseKind.AngleReading, Cm1Level),
        new("triangles", "geometry-plane", "primary.triangles.title",
            "points-lines-segments", "△", PrimaryExerciseKind.TriangleSides, Cm1Level),
        new("quadrilaterals", "geometry-plane", "primary.quadrilaterals.title",
            "points-lines-segments", "□", PrimaryExerciseKind.QuadrilateralSides, Cm1Level),
        new("polygons", "geometry-plane", "primary.polygons.title",
            "triangles", "⬠", PrimaryExerciseKind.PolygonSides, Ce2Level),
        new("circle-introduction", "geometry-plane", "primary.circle.title",
            "points-lines-segments", "◯", PrimaryExerciseKind.CircleDiameter, Cm2Level),
        new("perimeter", "geometry-plane", "primary.perimeter.title",
            "length-measurement", "P = 2 × (L + l)", PrimaryExerciseKind.RectanglePerimeter, Cm1Level),
        new("rectangle-area", "geometry-plane", "primary.rectangleArea.title",
            "area-measurement", "A = L × l", PrimaryExerciseKind.RectangleArea, Cm2Level),
        new("axial-symmetry", "geometry-plane", "primary.symmetry.title",
            "parallel-and-perpendicular-lines", "◁│▷", PrimaryExerciseKind.SymmetryAxes, Cm2Level),

        new("solid-shapes", "geometry-space", "primary.solids.title",
            "polygons", "□ × 6   • × 8   ─ × 12", PrimaryExerciseKind.SolidFaces, Ce2Level),
        new("solid-nets", "geometry-space", "primary.nets.title",
            "solid-shapes", "□ × 6", PrimaryExerciseKind.CubeNetFaces, Cm2Level),
        new("cuboid-volume", "geometry-space", "primary.cuboidVolume.title",
            "area-measurement", "V = L × l × h", PrimaryExerciseKind.CuboidVolume, Cm2Level),

        new("read-data-table", "statistics", "primary.dataTable.title",
            CourseCatalog.CountingCourseId, "2 + 5 + 3 = 10", PrimaryExerciseKind.DataTableTotal, CpLevel),
        new("create-chart", "statistics", "primary.chart.title",
            "read-data-table", "▂▅▇", PrimaryExerciseKind.ChartMaximum, Ce2Level)
    ];

    /// <summary>
    /// Recherche une définition par son identifiant stable.
    /// </summary>
    public static PrimaryCourseDefinition Get(string courseId)
    {
        return Definitions.Single(definition => definition.Id == courseId);
    }

    /// <summary>
    /// Indique si un identifiant appartient au catalogue primaire générique.
    /// </summary>
    public static bool TryGet(string courseId, out PrimaryCourseDefinition definition)
    {
        definition = Definitions.FirstOrDefault(item => item.Id == courseId)!;
        return definition is not null;
    }
}
