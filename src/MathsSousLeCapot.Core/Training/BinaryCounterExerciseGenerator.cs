using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère localement les exercices du cours de base deux.
/// </summary>
public sealed class BinaryCounterExerciseGenerator
{
    /// <summary>
    /// Générateur aléatoire injectable pour les tests.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Initialise le générateur avec une source aléatoire facultative.
    /// </summary>
    public BinaryCounterExerciseGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>
    /// Crée cinq exercices binaires adaptés à la difficulté choisie.
    /// </summary>
    public IReadOnlyList<Exercise> CreateSession(TrainingDifficulty difficulty)
    {
        var maximum = difficulty switch
        {
            TrainingDifficulty.Easy => 15,
            TrainingDifficulty.Moderate => 63,
            TrainingDifficulty.Hard => 255,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };

        return Enumerable.Range(0, 5).Select(index =>
        {
            var value = _random.Next(1, maximum);
            var asksForNext = difficulty != TrainingDifficulty.Hard || _random.Next(2) == 0;
            var shown = Convert.ToString(value, 2)!;
            var answer = Convert.ToString(asksForNext ? value + 1 : value - 1, 2)!;

            return new Exercise(
                $"binary-{difficulty}-{index}-{Guid.NewGuid():N}",
                CourseCatalog.BinaryCourseId,
                string.Empty,
                answer,
                string.Empty,
                asksForNext ? "exercise.binary.after" : "exercise.binary.before",
                [shown],
                "exercise.binary.explanation",
                [answer]);
        }).ToArray();
    }
}
