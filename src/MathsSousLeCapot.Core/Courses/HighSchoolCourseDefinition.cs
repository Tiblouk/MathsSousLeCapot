namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Décrit une notion du lycée affichée par le parcours pédagogique commun.
/// </summary>
public sealed record HighSchoolCourseDefinition(
    string Id,
    string ChapterId,
    string Level,
    string Title,
    string? PrerequisiteId,
    string Example,
    string DiscoveryKey,
    HighSchoolExerciseKind ExerciseKind);
