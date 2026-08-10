namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Décrit une notion du collège affichée par le parcours pédagogique commun.
/// </summary>
public sealed record MiddleSchoolCourseDefinition(
    string Id,
    string ChapterId,
    string Level,
    string Title,
    string? PrerequisiteId,
    string Example,
    string DiscoveryKey,
    MiddleSchoolExerciseKind ExerciseKind);
