namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Décrit une notion primaire affichée par les pages pédagogiques génériques.
/// </summary>
public sealed record PrimaryCourseDefinition(
    string Id,
    string ChapterId,
    string Title,
    string? PrerequisiteId,
    string Example,
    PrimaryExerciseKind ExerciseKind,
    string Level);
