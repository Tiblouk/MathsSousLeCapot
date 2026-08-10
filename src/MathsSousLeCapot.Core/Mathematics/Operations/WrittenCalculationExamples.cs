namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Fournit les calculs posés fixes utilisés dans les découvertes de cours.
/// </summary>
public static class WrittenCalculationExamples
{
    /// <summary>
    /// Recherche un exemple posé pour un cours qui enseigne directement une opération.
    /// </summary>
    public static bool TryCreate(
        string courseId,
        out WrittenCalculation calculation)
    {
        calculation = courseId switch
        {
            "written-addition" => WrittenCalculationBuilder.Create(
                WrittenCalculationKind.Addition,
                248,
                135),
            "written-subtraction" => WrittenCalculationBuilder.Create(
                WrittenCalculationKind.Subtraction,
                482,
                157),
            "written-multiplication" => WrittenCalculationBuilder.Create(
                WrittenCalculationKind.Multiplication,
                24,
                13),
            "written-division" => WrittenCalculationBuilder.Create(
                WrittenCalculationKind.Division,
                84,
                7),
            "euclidean-division" or "quotient-and-remainder" =>
                WrittenCalculationBuilder.Create(
                    WrittenCalculationKind.Division,
                    47,
                    6),
            _ => null!
        };

        return calculation is not null;
    }
}
