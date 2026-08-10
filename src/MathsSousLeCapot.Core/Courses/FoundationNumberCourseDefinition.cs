namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Décrit un cours fondamental sur les nombres négatifs ou décimaux.
/// </summary>
public sealed record FoundationNumberCourseDefinition(
    string Id,
    string Title,
    string Objective,
    string Summary,
    string PrerequisiteId,
    string PrerequisiteTitle,
    FoundationNumberKind Kind,
    string Level);
