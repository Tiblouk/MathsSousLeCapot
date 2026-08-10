using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

public sealed class PositionalCounterExerciseGeneratorTests
{
    [Theory]
    [InlineData(TrainingDifficulty.Easy)]
    [InlineData(TrainingDifficulty.Moderate)]
    [InlineData(TrainingDifficulty.Hard)]
    public void A_session_contains_five_exercises(TrainingDifficulty difficulty)
    {
        var generator = new PositionalCounterExerciseGenerator(new Random(42));

        var exercises = generator.CreateSession(difficulty);

        Assert.Equal(5, exercises.Count);
        Assert.All(exercises, exercise =>
        {
            Assert.NotEmpty(exercise.QuestionKey!);
            Assert.NotEmpty(exercise.QuestionArguments!);
            Assert.NotEmpty(exercise.CorrectAnswer);
            Assert.NotEmpty(exercise.ExplanationKey!);
            Assert.NotEmpty(exercise.ExplanationArguments!);
        });
    }

    [Fact]
    public void Decimal_training_does_not_introduce_base_two()
    {
        var generator = new PositionalCounterExerciseGenerator(new Random(42));

        var exercises = generator.CreateSession(TrainingDifficulty.Hard);

        Assert.All(exercises, exercise =>
        {
            Assert.StartsWith("exercise.counter.", exercise.QuestionKey);
            Assert.Equal("10", exercise.QuestionArguments![1]);
        });
    }

    [Fact]
    public void Binary_training_is_associated_with_the_lycee_course()
    {
        var generator = new BinaryCounterExerciseGenerator(new Random(42));

        var exercises = generator.CreateSession(TrainingDifficulty.Easy);

        Assert.Equal(5, exercises.Count);
        Assert.All(exercises, exercise =>
        {
            Assert.Equal("binary-numbers", exercise.CourseId);
            Assert.StartsWith("exercise.binary.", exercise.QuestionKey);
        });
    }
}
