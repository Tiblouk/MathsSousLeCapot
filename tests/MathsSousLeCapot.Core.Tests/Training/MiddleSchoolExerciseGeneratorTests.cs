using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

/// <summary>
/// Vérifie le contrat commun des générateurs du collège.
/// </summary>
public sealed class MiddleSchoolExerciseGeneratorTests
{
    [Theory]
    [InlineData(TrainingDifficulty.Easy)]
    [InlineData(TrainingDifficulty.Moderate)]
    [InlineData(TrainingDifficulty.Hard)]
    public void Every_middle_school_course_generates_five_numeric_exercises(
        TrainingDifficulty difficulty)
    {
        var generator = new MiddleSchoolExerciseGenerator(new Random(42));

        foreach (var definition in MiddleSchoolCourseCatalog.Definitions)
        {
            var exercises = generator.CreateSession(definition, difficulty);

            Assert.Equal(5, exercises.Count);
            Assert.All(exercises, exercise =>
            {
                Assert.Equal(definition.Id, exercise.CourseId);
                Assert.StartsWith("exercise.middle.", exercise.QuestionKey);
                Assert.Equal("exercise.middle.explanation", exercise.ExplanationKey);
                Assert.True(
                    int.TryParse(exercise.CorrectAnswer, out _),
                    $"Réponse non entière pour {definition.Id}");
            });
        }
    }

    [Fact]
    public void Every_final_validation_uses_two_distinct_questions()
    {
        var generator = new MiddleSchoolExerciseGenerator(new Random(18));

        foreach (var definition in MiddleSchoolCourseCatalog.Definitions)
        {
            var exercises = generator.CreateValidation(
                definition,
                TrainingDifficulty.Hard);

            Assert.Equal(2, exercises.Count);
            Assert.False(
                exercises[0].QuestionKey == exercises[1].QuestionKey
                && (exercises[0].QuestionArguments ?? []).SequenceEqual(
                    exercises[1].QuestionArguments ?? []),
                $"Validation dupliquée pour {definition.Id}");
        }
    }
}
