namespace MathsSousLeCapot.Core.Training;

using MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Représente une question d'entraînement et ses informations de correction.
/// </summary>
public sealed record Exercise(
    string Id,
    string CourseId,
    string Question,
    string CorrectAnswer,
    string Explanation,
    string? QuestionKey = null,
    IReadOnlyList<string>? QuestionArguments = null,
    string? ExplanationKey = null,
    IReadOnlyList<string>? ExplanationArguments = null,
    ExerciseAnswerKind AnswerKind = ExerciseAnswerKind.Numeric,
    IReadOnlyList<string>? AnswerArguments = null,
    WrittenCalculation? WrittenCalculation = null,
    ExerciseSolution? DetailedSolution = null);
