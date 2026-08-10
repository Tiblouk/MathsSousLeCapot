using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

/// <summary>
/// Vérifie les sessions d'addition et de soustraction.
/// </summary>
public sealed class BasicOperationExerciseGeneratorTests
{
    [Theory]
    [InlineData(BasicOperation.Addition, TrainingDifficulty.Easy)]
    [InlineData(BasicOperation.Addition, TrainingDifficulty.Hard)]
    [InlineData(BasicOperation.Subtraction, TrainingDifficulty.Easy)]
    [InlineData(BasicOperation.Subtraction, TrainingDifficulty.Hard)]
    public void Creates_five_localizable_exercises(
        BasicOperation operation,
        TrainingDifficulty difficulty)
    {
        var generator = new BasicOperationExerciseGenerator(new Random(42));

        var exercises = generator.CreateSession(operation, difficulty);

        Assert.Equal(5, exercises.Count);
        Assert.All(exercises, exercise =>
        {
            Assert.NotNull(exercise.QuestionKey);
            Assert.NotNull(exercise.ExplanationKey);
            Assert.Equal(2, exercise.QuestionArguments!.Count);
            Assert.Equal(3, exercise.ExplanationArguments!.Count);
        });
    }

    [Fact]
    public void Subtraction_never_generates_a_negative_result()
    {
        var generator = new BasicOperationExerciseGenerator(new Random(42));

        var exercises = generator.CreateSession(
            BasicOperation.Subtraction,
            TrainingDifficulty.Hard);

        Assert.All(exercises, exercise =>
        {
            var left = int.Parse(exercise.QuestionArguments![0]);
            var right = int.Parse(exercise.QuestionArguments[1]);
            Assert.True(left >= right);
            Assert.Equal(left - right, int.Parse(exercise.CorrectAnswer));
            Assert.Equal(CourseCatalog.SubtractionCourseId, exercise.CourseId);
        });
    }
}
