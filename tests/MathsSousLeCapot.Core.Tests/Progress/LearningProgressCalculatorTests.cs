using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Progress;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Progress;

/// <summary>
/// Vérifie le calcul reproductible des statistiques, défis et scores locaux.
/// </summary>
public sealed class LearningProgressCalculatorTests
{
    [Fact]
    public void Calculate_aggregates_progress_sessions_and_completed_challenges()
    {
        var progress = new[]
        {
            new CourseProgress("course-a", 3, DateTimeOffset.UtcNow),
            new CourseProgress("course-b", 1, null)
        };
        var sessions = new[]
        {
            CreateSession(CourseCatalog.CountingCourseId, TrainingDifficulty.Easy, true, true),
            CreateSession(
                "course-b",
                TrainingDifficulty.Easy,
                true,
                false)
        };

        var overview = LearningProgressCalculator.Calculate(progress, sessions);

        Assert.Equal(2, overview.Statistics.ConsultedCourseCount);
        Assert.Equal(1, overview.Statistics.CompletedCourseCount);
        Assert.Equal(4, overview.Statistics.TotalReadCount);
        Assert.Equal(2, overview.Statistics.TrainingSessionCount);
        Assert.Equal(3, overview.Statistics.CorrectAnswerCount);
        Assert.Equal(4, overview.Statistics.AnswerCount);
        Assert.Equal(1, overview.Statistics.PerfectSessionCount);
        Assert.Equal(1, overview.Statistics.TrainingScore);
        Assert.True(overview.Challenges.Single(item =>
            item.Definition.Id == "first-course").IsCompleted);
        Assert.True(overview.Challenges.Single(item =>
            item.Definition.Id == "first-completion").IsCompleted);
        Assert.True(overview.Challenges.Single(item =>
            item.Definition.Id == "perfect-session").IsCompleted);
        Assert.Equal(66, overview.TotalScore);
    }

    [Fact]
    public void Calculate_returns_zero_accuracy_when_no_answer_exists()
    {
        var overview = LearningProgressCalculator.Calculate([], []);

        Assert.Equal(0, overview.Statistics.AccuracyPercent);
        Assert.Equal(0, overview.TotalScore);
        Assert.All(overview.Challenges, challenge => Assert.False(challenge.IsCompleted));
    }

    [Fact]
    public void Training_session_is_scored_once_per_course_and_difficulty()
    {
        var sessions = new[]
        {
            CreateSession(
                CourseCatalog.CountingCourseId,
                TrainingDifficulty.Easy,
                true,
                true),
            CreateSession(
                CourseCatalog.CountingCourseId,
                TrainingDifficulty.Easy,
                true,
                true),
            CreateSession(
                CourseCatalog.CountingCourseId,
                TrainingDifficulty.Moderate,
                true,
                true)
        };

        var score = LearningProgressCalculator.CalculateTrainingScore(sessions);

        Assert.Equal(3, score);
    }

    [Fact]
    public void Training_session_points_follow_school_group_and_difficulty()
    {
        var middleCourse = MiddleSchoolCourseCatalog.Definitions[0].Id;
        var highCourse = HighSchoolCourseCatalog.Definitions[0].Id;

        Assert.Equal(
            6,
            LearningProgressCalculator.GetTrainingSessionPointValue(
                middleCourse,
                TrainingDifficulty.Hard));
        Assert.Equal(
            8,
            LearningProgressCalculator.GetTrainingSessionPointValue(
                highCourse,
                TrainingDifficulty.Hard));
    }

    [Fact]
    public void Verified_imperfect_session_awards_points_once()
    {
        var session = CreateSession(
            CourseCatalog.CountingCourseId,
            TrainingDifficulty.Hard,
            true,
            false);

        Assert.Equal(
            4,
            LearningProgressCalculator.CalculateTrainingScore([session, session]));
    }

    /// <summary>
    /// Crée une session minimale avec deux résultats pour les calculs de test.
    /// </summary>
    private static TrainingSession CreateSession(
        string courseId,
        TrainingDifficulty difficulty,
        bool firstCorrect,
        bool secondCorrect)
    {
        var exercise = new Exercise(
            $"{courseId}-exercise",
            courseId,
            "Question",
            "1",
            "Explication");
        return new TrainingSession(
            courseId,
            difficulty,
            DateTimeOffset.UtcNow,
            [
                new ExerciseResult(exercise, "1", firstCorrect),
                new ExerciseResult(exercise, "1", secondCorrect)
            ]);
    }
}
