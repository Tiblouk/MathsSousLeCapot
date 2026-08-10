namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Associe un exercice, la réponse donnée et son résultat.
/// </summary>
public sealed record ExerciseResult(
    Exercise Exercise,
    string GivenAnswer,
    bool IsCorrect);

/// <summary>
/// Représente une session vérifiée et sauvegardable.
/// </summary>
public sealed record TrainingSession(
    string CourseId,
    TrainingDifficulty Difficulty,
    DateTimeOffset CompletedAt,
    IReadOnlyList<ExerciseResult> Results)
{
    public int CorrectCount => Results.Count(result => result.IsCorrect);

    public int IncorrectCount => Results.Count - CorrectCount;
}
