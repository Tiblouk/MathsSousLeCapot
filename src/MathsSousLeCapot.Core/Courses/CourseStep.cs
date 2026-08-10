namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Définit le type d'interaction attendu pour une étape de cours.
/// </summary>
public enum CourseStepKind
{
    Explanation,
    InteractiveCounting,
    InteractiveCounter,
    InteractiveOperation,
    InteractivePrimary,
    InteractiveMiddleSchool,
    InteractiveHighSchool,
    InteractiveFoundationNumber,
    FinalQuestion
}

/// <summary>
/// Décrit une idée pédagogique unique affichée dans le parcours du cours.
/// </summary>
public sealed record CourseStep(
    string Id,
    string Title,
    string Explanation,
    string Example,
    CourseStepKind Kind,
    IReadOnlyList<CourseContentBlock>? ContentBlocks = null);
