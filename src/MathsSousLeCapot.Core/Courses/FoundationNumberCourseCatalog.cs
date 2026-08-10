namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Catalogue des cours spécialisés sur les nombres négatifs et décimaux.
/// </summary>
public static class FoundationNumberCourseCatalog
{
    /// <summary>
    /// Identifiant stable du cours sur les nombres négatifs.
    /// </summary>
    public const string NegativeNumbersCourseId = "negative-numbers";

    /// <summary>
    /// Identifiant stable du cours sur les nombres décimaux.
    /// </summary>
    public const string DecimalNumbersCourseId = "decimal-numbers";

    /// <summary>
    /// Définitions des deux cours fondamentaux.
    /// </summary>
    public static IReadOnlyList<FoundationNumberCourseDefinition> Definitions { get; } =
    [
        new(
            NegativeNumbersCourseId,
            "course.negative.title",
            "course.negative.objective",
            "course.negative.summary",
            CourseCatalog.CountingCourseId,
            "course.counting.title",
            FoundationNumberKind.NegativeNumbers,
            "chapter.level.fifth"),
        new(
            DecimalNumbersCourseId,
            "course.decimal.title",
            "course.decimal.objective",
            "course.decimal.summary",
            CourseCatalog.CountingCourseId,
            "course.counting.title",
            FoundationNumberKind.DecimalNumbers,
            PrimaryCourseCatalog.Cm1Level)
    ];

    /// <summary>
    /// Recherche une définition par son identifiant.
    /// </summary>
    public static FoundationNumberCourseDefinition Get(string courseId)
    {
        return Definitions.Single(definition => definition.Id == courseId);
    }

    /// <summary>
    /// Indique si le cours appartient à ce module.
    /// </summary>
    public static bool TryGet(
        string courseId,
        out FoundationNumberCourseDefinition definition)
    {
        definition = Definitions.FirstOrDefault(item => item.Id == courseId)!;
        return definition is not null;
    }
}
