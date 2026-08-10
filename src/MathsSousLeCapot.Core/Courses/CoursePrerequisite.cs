namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Référence un cours conseillé avant le cours courant.
/// </summary>
public sealed record CoursePrerequisite(string CourseId, string Title);
