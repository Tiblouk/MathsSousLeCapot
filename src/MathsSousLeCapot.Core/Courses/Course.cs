namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Décrit un cours, ses métadonnées, ses prérequis et ses étapes.
/// </summary>
public sealed record Course(
    string Id,
    string ChapterId,
    string Title,
    string Level,
    string Objective,
    string Summary,
    IReadOnlyList<CoursePrerequisite> Prerequisites,
    IReadOnlyList<CourseStep> Steps,
    bool IsAvailable,
    CoursePedagogicalContent? PedagogicalContent = null);
