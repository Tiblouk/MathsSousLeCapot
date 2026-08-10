namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Regroupe les métadonnées et les cours d'un chapitre pédagogique.
/// </summary>
public sealed record Chapter(
    string Id,
    string Title,
    string Description,
    string Level,
    IReadOnlyList<Course> Courses);
