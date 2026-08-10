using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère localement les exercices du compteur décimal.
/// </summary>
public sealed class PositionalCounterExerciseGenerator
{
    /// <summary>
    /// Générateur aléatoire injectable pour rendre les tests déterministes.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Initialise le générateur avec une source aléatoire facultative.
    /// </summary>
    public PositionalCounterExerciseGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>
    /// Crée une session de cinq exercices pour la difficulté choisie.
    /// </summary>
    public IReadOnlyList<Exercise> CreateSession(TrainingDifficulty difficulty)
    {
        return Enumerable
            .Range(0, 5)
            .Select(index => CreateExercise(index, difficulty))
            .ToArray();
    }

    /// <summary>
    /// Construit une question localisable de nombre précédent ou suivant.
    /// </summary>
    private Exercise CreateExercise(int index, TrainingDifficulty difficulty)
    {
        const int numeralBase = 10;
        var asksForNext = difficulty != TrainingDifficulty.Hard || _random.Next(2) == 0;
        var value = CreateValue(numeralBase, difficulty, asksForNext);
        var shown = Convert.ToString(value, numeralBase)!;
        var answerValue = asksForNext ? value + 1 : value - 1;
        var answer = Convert.ToString(answerValue, numeralBase)!;
        return new Exercise(
            $"positional-{difficulty}-{index}-{Guid.NewGuid():N}",
            CourseCatalog.PositionalCounterCourseId,
            string.Empty,
            answer,
            string.Empty,
            asksForNext ? "exercise.counter.after" : "exercise.counter.before",
            [shown, numeralBase.ToString()],
            asksForNext
                ? "exercise.counter.addExplanation"
                : "exercise.counter.removeExplanation",
            [numeralBase.ToString(), answer]);
    }

    /// <summary>
    /// Sélectionne une valeur adaptée à la difficulté.
    /// </summary>
    private int CreateValue(
        int numeralBase,
        TrainingDifficulty difficulty,
        bool asksForNext)
    {
        var range = difficulty switch
        {
            TrainingDifficulty.Easy => (Minimum: 1, Maximum: 89),
            TrainingDifficulty.Moderate => (Minimum: 8, Maximum: 255),
            TrainingDifficulty.Hard => (Minimum: 15, Maximum: 999),
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };

        var value = _random.Next(range.Minimum, range.Maximum + 1);
        return asksForNext ? value : Math.Max(1, value);
    }
}
