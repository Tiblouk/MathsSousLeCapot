using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère les exercices d'addition et de soustraction sur les nombres naturels.
/// </summary>
public sealed class BasicOperationExerciseGenerator
{
    /// <summary>
    /// Source aléatoire injectable pour les tests.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Initialise le générateur avec une source aléatoire facultative.
    /// </summary>
    public BasicOperationExerciseGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>
    /// Crée cinq exercices adaptés à l'opération et à la difficulté.
    /// </summary>
    public IReadOnlyList<Exercise> CreateSession(
        BasicOperation operation,
        TrainingDifficulty difficulty)
    {
        var maximum = difficulty switch
        {
            TrainingDifficulty.Easy => 10,
            TrainingDifficulty.Moderate => 50,
            TrainingDifficulty.Hard => 500,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };

        return Enumerable.Range(0, 5)
            .Select(index => CreateExercise(operation, difficulty, maximum, index))
            .ToArray();
    }

    /// <summary>
    /// Construit un exercice localisable et son explication.
    /// </summary>
    private Exercise CreateExercise(
        BasicOperation operation,
        TrainingDifficulty difficulty,
        int maximum,
        int index)
    {
        var left = _random.Next(0, maximum + 1);
        var right = _random.Next(0, maximum + 1);

        if (operation == BasicOperation.Subtraction && right > left)
        {
            (left, right) = (right, left);
        }

        var result = BasicOperationCalculator.Calculate(operation, left, right);
        var courseId = operation == BasicOperation.Addition
            ? CourseCatalog.AdditionCourseId
            : CourseCatalog.SubtractionCourseId;
        var keyPrefix = operation == BasicOperation.Addition
            ? "exercise.addition"
            : "exercise.subtraction";
        var writtenCalculation = WrittenCalculationBuilder.Create(
            operation == BasicOperation.Addition
                ? WrittenCalculationKind.Addition
                : WrittenCalculationKind.Subtraction,
            left,
            right);

        return new Exercise(
            $"{courseId}-{difficulty}-{index}-{Guid.NewGuid():N}",
            courseId,
            string.Empty,
            result.ToString(),
            string.Empty,
            $"{keyPrefix}.question",
            [left.ToString(), right.ToString()],
            $"{keyPrefix}.explanation",
            [left.ToString(), right.ToString(), result.ToString()],
            WrittenCalculation: writtenCalculation);
    }
}
