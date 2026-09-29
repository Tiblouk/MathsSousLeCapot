using MathsSousLeCapot.Core.Progress;
using MathsSousLeCapot.Core.Pedagogy;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Progress;

/// <summary>
/// Vérifie la robustesse du format utilisé pour les données locales.
/// </summary>
public sealed class LocalDataSerializerTests
{
    [Fact]
    public void Round_trip_preserves_progress_data()
    {
        var expected = new CourseProgress(
            "course-id",
            3,
            new DateTimeOffset(2026, 6, 9, 12, 30, 0, TimeSpan.Zero));

        var json = LocalDataSerializer.Serialize(expected);
        var success = LocalDataSerializer.TryDeserialize<CourseProgress>(
            json,
            out var actual);

        Assert.True(success);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Vérifie que le registre de profils reste portable entre les plateformes.
    /// </summary>
    [Fact]
    public void Round_trip_preserves_local_profile_registry()
    {
        var profile = new LocalProfile(
            "default",
            "Principal",
            new DateTimeOffset(2026, 8, 16, 12, 0, 0, TimeSpan.Zero));
        var expected = new LocalProfileRegistry(profile.Id, [profile]);

        var json = LocalDataSerializer.Serialize(expected);
        var success = LocalDataSerializer.TryDeserialize<LocalProfileRegistry>(
            json,
            out var actual);

        Assert.True(success);
        Assert.NotNull(actual);
        Assert.Equal(expected.ActiveProfileId, actual.ActiveProfileId);
        Assert.Equal(expected.Profiles, actual.Profiles);
    }

    /// <summary>
    /// Vérifie que le résumé léger des badges peut être sauvegardé puis relu.
    /// </summary>
    [Fact]
    public void Round_trip_preserves_perfect_training_achievement()
    {
        var expected = new PerfectTrainingAchievement(
            "course-id",
            TrainingDifficulty.Hard,
            new DateTimeOffset(2026, 9, 28, 12, 0, 0, TimeSpan.Zero));

        var json = LocalDataSerializer.Serialize(expected);
        var success = LocalDataSerializer.TryDeserialize<PerfectTrainingAchievement>(
            json,
            out var actual);

        Assert.True(success);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-json")]
    [InlineData("{\"courseId\":")]
    public void Invalid_content_is_rejected_without_throwing(string json)
    {
        var success = LocalDataSerializer.TryDeserialize<CourseProgress>(
            json,
            out var value);

        Assert.False(success);
        Assert.Null(value);
    }

    /// <summary>
    /// Vérifie qu'une session enregistrée avant les corrections structurées reste lisible.
    /// </summary>
    [Fact]
    public void Legacy_training_session_without_detailed_solution_is_supported()
    {
        const string Json = """
            {
              "courseId": "legacy-course",
              "difficulty": 0,
              "completedAt": "2026-06-09T12:30:00+00:00",
              "results": [
                {
                  "exercise": {
                    "id": "legacy-exercise",
                    "courseId": "legacy-course",
                    "question": "2 + 2 ?",
                    "correctAnswer": "4",
                    "explanation": "2 + 2 = 4"
                  },
                  "givenAnswer": "4",
                  "isCorrect": true
                }
              ]
            }
            """;

        var success = LocalDataSerializer.TryDeserialize<TrainingSession>(
            Json,
            out var session);

        Assert.True(success);
        Assert.NotNull(session);
        Assert.Single(session.Results);
        Assert.Null(session.Results[0].Exercise.DetailedSolution);
    }

    /// <summary>
    /// Vérifie que les nouvelles étapes de correction survivent à la sérialisation locale.
    /// </summary>
    [Fact]
    public void Round_trip_preserves_structured_exercise_solution()
    {
        var exercise = new Exercise(
            "exercise-id",
            "course-id",
            "2 + 3 ?",
            "5",
            "2 + 3 = 5",
            DetailedSolution: new ExerciseSolution(
                Steps:
                [
                    new ExerciseSolutionStep(
                        "calculation",
                        ExerciseSolutionStepKind.Calculation,
                        new LocalizedText(
                            "exercise.sample.calculation",
                            ["2", "3", "5"]),
                        "2 + 3 = 5")
                ]));
        var expected = new TrainingSession(
            "course-id",
            TrainingDifficulty.Easy,
            new DateTimeOffset(2026, 6, 9, 12, 30, 0, TimeSpan.Zero),
            [new ExerciseResult(exercise, "5", IsCorrect: true)]);

        var json = LocalDataSerializer.Serialize(expected);
        var success = LocalDataSerializer.TryDeserialize<TrainingSession>(
            json,
            out var actual);

        Assert.True(success);
        Assert.NotNull(actual);
        var solution = Assert.IsType<ExerciseSolution>(
            actual.Results[0].Exercise.DetailedSolution);
        Assert.Single(solution.Steps);
        Assert.Equal("2 + 3 = 5", solution.Steps[0].Expression);
        Assert.Equal(
            ["2", "3", "5"],
            solution.Steps[0].Explanation.Arguments);
    }
}
