using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Progress;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Progress;

/// <summary>
/// Vérifie la priorité des badges calculés pour les cartes de cours.
/// </summary>
public sealed class CourseAchievementCalculatorTests
{
    [Fact]
    public void Read_course_receives_read_status()
    {
        var progress = new CourseProgress(
            CourseCatalog.CountingCourseId,
            1,
            null);

        var statuses = CourseAchievementCalculator.Calculate([progress], []);

        Assert.Equal(
            CourseAchievementStatus.Read,
            statuses[CourseCatalog.CountingCourseId]);
    }

    [Fact]
    public void Perfect_session_overrides_read_status()
    {
        var courseId = CourseCatalog.CountingCourseId;
        var statuses = CourseAchievementCalculator.Calculate(
            [new CourseProgress(courseId, 2, null)],
            CourseAchievementCalculator.CreateAchievements(
                [CreateSession(courseId, TrainingDifficulty.Moderate, isPerfect: true)]));

        Assert.Equal(CourseAchievementStatus.PerfectSession, statuses[courseId]);
    }

    [Fact]
    public void Perfect_hard_session_has_highest_priority()
    {
        var courseId = CourseCatalog.CountingCourseId;
        var statuses = CourseAchievementCalculator.Calculate(
            [],
            CourseAchievementCalculator.CreateAchievements(
            [
                CreateSession(courseId, TrainingDifficulty.Hard, isPerfect: true),
                CreateSession(courseId, TrainingDifficulty.Easy, isPerfect: true)
            ]));

        Assert.Equal(CourseAchievementStatus.PerfectHardSession, statuses[courseId]);
    }

    [Fact]
    public void Imperfect_session_does_not_create_a_badge()
    {
        var courseId = CourseCatalog.CountingCourseId;
        var statuses = CourseAchievementCalculator.Calculate(
            [],
            CourseAchievementCalculator.CreateAchievements(
                [CreateSession(courseId, TrainingDifficulty.Hard, isPerfect: false)]));

        Assert.DoesNotContain(courseId, statuses);
    }

    /// <summary>
    /// Crée une session minimale parfaite ou imparfaite pour le statut demandé.
    /// </summary>
    private static TrainingSession CreateSession(
        string courseId,
        TrainingDifficulty difficulty,
        bool isPerfect)
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
            [new ExerciseResult(exercise, isPerfect ? "1" : "0", isPerfect)]);
    }
}
