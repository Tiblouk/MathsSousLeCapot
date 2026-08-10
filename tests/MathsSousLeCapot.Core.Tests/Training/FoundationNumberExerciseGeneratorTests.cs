using System.Globalization;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

/// <summary>
/// Vérifie les exercices des nombres négatifs et décimaux.
/// </summary>
public sealed class FoundationNumberExerciseGeneratorTests
{
    /// <summary>
    /// Vérifie le contrat des cinq exercices pour chaque difficulté.
    /// </summary>
    [Theory]
    [InlineData(TrainingDifficulty.Easy)]
    [InlineData(TrainingDifficulty.Moderate)]
    [InlineData(TrainingDifficulty.Hard)]
    public void Every_foundation_course_generates_five_valid_exercises(
        TrainingDifficulty difficulty)
    {
        var generator = new FoundationNumberExerciseGenerator(new Random(42));

        foreach (var definition in FoundationNumberCourseCatalog.Definitions)
        {
            var exercises = generator.CreateSession(definition, difficulty);

            Assert.Equal(5, exercises.Count);
            Assert.All(exercises, exercise =>
            {
                Assert.Equal(definition.Id, exercise.CourseId);
                Assert.StartsWith("exercise.foundation.", exercise.QuestionKey);
                Assert.Equal(
                    "exercise.foundation.explanation",
                    exercise.ExplanationKey);
                Assert.Equal(ExpectedAnswer(exercise), ParseAnswer(exercise));
            });
        }
    }

    /// <summary>
    /// Vérifie que la validation finale ne répète pas exactement la même question.
    /// </summary>
    [Fact]
    public void Final_validation_uses_two_distinct_questions()
    {
        var generator = new FoundationNumberExerciseGenerator(new Random(18));

        foreach (var definition in FoundationNumberCourseCatalog.Definitions)
        {
            var exercises = generator.CreateValidation(
                definition,
                TrainingDifficulty.Hard);

            Assert.Equal(2, exercises.Count);
            Assert.False(
                exercises[0].QuestionKey == exercises[1].QuestionKey
                && (exercises[0].QuestionArguments ?? []).SequenceEqual(
                    exercises[1].QuestionArguments ?? []));
        }
    }

    /// <summary>
    /// Recalcule la réponse attendue depuis la famille et les paramètres.
    /// </summary>
    private static decimal ExpectedAnswer(Exercise exercise)
    {
        var values = (exercise.QuestionArguments ?? [])
            .Select(value => decimal.Parse(
                value.Replace(',', '.'),
                CultureInfo.InvariantCulture))
            .ToArray();

        return exercise.QuestionKey switch
        {
            "exercise.foundation.negativeMove" => values[0] + values[1],
            "exercise.foundation.negativeOpposite" => -values[0],
            "exercise.foundation.negativeGreater" => Math.Max(values[0], values[1]),
            "exercise.foundation.decimalRead" =>
                values[0] + values[1] / values[2],
            "exercise.foundation.decimalGreater" => Math.Max(values[0], values[1]),
            "exercise.foundation.decimalFraction" => values[0] / values[1],
            _ => throw new InvalidOperationException(exercise.QuestionKey)
        };
    }

    /// <summary>
    /// Convertit la réponse canonique en nombre décimal comparable.
    /// </summary>
    private static decimal ParseAnswer(Exercise exercise)
    {
        return decimal.Parse(exercise.CorrectAnswer, CultureInfo.InvariantCulture);
    }
}
