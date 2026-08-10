namespace MathsSousLeCapot.Core.Progress;

/// <summary>
/// Contient la progression locale associée à un cours.
/// </summary>
public sealed record CourseProgress(
    string CourseId,
    int ReadCount,
    DateTimeOffset? FirstCompletedAt);
