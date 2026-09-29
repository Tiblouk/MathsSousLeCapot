using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Progress;

/// <summary>
/// Résume l'activité pédagogique enregistrée pour un profil local.
/// </summary>
public sealed record LearningStatistics(
    int ConsultedCourseCount,
    int CompletedCourseCount,
    int TotalReadCount,
    int TrainingSessionCount,
    int CorrectAnswerCount,
    int AnswerCount,
    int PerfectSessionCount,
    int TrainingScore)
{
    /// <summary>
    /// Pourcentage de réponses correctes, nul en l'absence d'exercice vérifié.
    /// </summary>
    public int AccuracyPercent => AnswerCount == 0
        ? 0
        : (int)Math.Round(100d * CorrectAnswerCount / AnswerCount);
}

/// <summary>
/// Indique la donnée locale suivie par un défi.
/// </summary>
public enum LocalChallengeMetric
{
    ConsultedCourses,
    CompletedCourses,
    TrainingSessions,
    PerfectSessions
}

/// <summary>
/// Décrit un défi local, son objectif et les points accordés.
/// </summary>
public sealed record LocalChallengeDefinition(
    string Id,
    string TitleKey,
    string DescriptionKey,
    LocalChallengeMetric Metric,
    int Target,
    int Reward);

/// <summary>
/// Associe un défi à sa progression calculée pour un profil.
/// </summary>
public sealed record LocalChallengeProgress(
    LocalChallengeDefinition Definition,
    int CurrentValue)
{
    /// <summary>
    /// Indique si l'objectif du défi est atteint.
    /// </summary>
    public bool IsCompleted => CurrentValue >= Definition.Target;

    /// <summary>
    /// Valeur plafonnée utilisée par l'affichage de la progression.
    /// </summary>
    public int DisplayValue => Math.Min(CurrentValue, Definition.Target);
}

/// <summary>
/// Regroupe les statistiques, les défis et le score total d'un profil.
/// </summary>
public sealed record LearningOverview(
    LearningStatistics Statistics,
    IReadOnlyList<LocalChallengeProgress> Challenges,
    int ChallengeScore,
    int TotalScore);

/// <summary>
/// Calcule les statistiques, défis et scores à partir des données locales existantes.
/// </summary>
public static class LearningProgressCalculator
{
    /// <summary>
    /// Défis communs à tous les profils et toutes les classes.
    /// </summary>
    public static IReadOnlyList<LocalChallengeDefinition> Challenges { get; } =
    [
        new(
            "first-course",
            "challenge.firstCourse.title",
            "challenge.firstCourse.description",
            LocalChallengeMetric.ConsultedCourses,
            1,
            10),
        new(
            "first-completion",
            "challenge.firstCompletion.title",
            "challenge.firstCompletion.description",
            LocalChallengeMetric.CompletedCourses,
            1,
            25),
        new(
            "curious-learner",
            "challenge.curiousLearner.title",
            "challenge.curiousLearner.description",
            LocalChallengeMetric.ConsultedCourses,
            10,
            50),
        new(
            "regular-practice",
            "challenge.regularPractice.title",
            "challenge.regularPractice.description",
            LocalChallengeMetric.TrainingSessions,
            5,
            50),
        new(
            "perfect-session",
            "challenge.perfectSession.title",
            "challenge.perfectSession.description",
            LocalChallengeMetric.PerfectSessions,
            1,
            30),
        new(
            "course-explorer",
            "challenge.courseExplorer.title",
            "challenge.courseExplorer.description",
            LocalChallengeMetric.CompletedCourses,
            25,
            100)
    ];

    /// <summary>
    /// Calcule un aperçu complet sans modifier les données reçues.
    /// </summary>
    public static LearningOverview Calculate(
        IEnumerable<CourseProgress> progress,
        IEnumerable<TrainingSession> sessions)
    {
        var progressItems = progress.ToArray();
        var sessionItems = sessions.ToArray();
        var results = sessionItems.SelectMany(session => session.Results).ToArray();
        var consultedCount = progressItems.Count(item => item.ReadCount > 0);
        var completedCount = progressItems.Count(item => item.FirstCompletedAt is not null);
        var correctCount = results.Count(result => result.IsCorrect);
        var perfectCount = sessionItems.Count(CourseAchievementCalculator.IsPerfect);
        var trainingScore = CalculateTrainingScore(sessionItems);
        var statistics = new LearningStatistics(
            consultedCount,
            completedCount,
            progressItems.Sum(item => item.ReadCount),
            sessionItems.Length,
            correctCount,
            results.Length,
            perfectCount,
            trainingScore);
        var challenges = Challenges
            .Select(definition => new LocalChallengeProgress(
                definition,
                GetMetricValue(statistics, definition.Metric)))
            .ToArray();
        var challengeScore = challenges
            .Where(challenge => challenge.IsCompleted)
            .Sum(challenge => challenge.Definition.Reward);

        return new LearningOverview(
            statistics,
            challenges,
            challengeScore,
            trainingScore + challengeScore);
    }

    /// <summary>
    /// Additionne une seule session vérifiée par cours et par difficulté.
    /// </summary>
    public static int CalculateTrainingScore(
        IEnumerable<TrainingSession> sessions)
    {
        return sessions
            .Where(session => session.Results.Count > 0)
            .DistinctBy(session => (session.CourseId, session.Difficulty))
            .Sum(session => GetTrainingSessionPointValue(
                session.CourseId,
                session.Difficulty));
    }

    /// <summary>
    /// Retourne les points d'une session vérifiée selon le niveau et la difficulté.
    /// </summary>
    public static int GetTrainingSessionPointValue(
        string courseId,
        TrainingDifficulty difficulty)
    {
        if (!Courses.CourseSearchCatalog.TryGet(courseId, out var course))
        {
            return 0;
        }

        var schoolPoints = course.SchoolGroupId switch
        {
            Courses.SchoolGroupCatalog.FoundationsGroupId => 1,
            Courses.SchoolGroupCatalog.PrimarySchoolGroupId => 1,
            Courses.SchoolGroupCatalog.MiddleSchoolGroupId => 3,
            Courses.SchoolGroupCatalog.HighSchoolGroupId => 5,
            _ => 0
        };
        var difficultyPoints = difficulty switch
        {
            TrainingDifficulty.Easy => 0,
            TrainingDifficulty.Moderate => 1,
            TrainingDifficulty.Hard => 3,
            _ => 0
        };
        return schoolPoints + difficultyPoints;
    }

    /// <summary>
    /// Lit la statistique correspondant à la métrique d'un défi.
    /// </summary>
    private static int GetMetricValue(
        LearningStatistics statistics,
        LocalChallengeMetric metric)
    {
        return metric switch
        {
            LocalChallengeMetric.ConsultedCourses => statistics.ConsultedCourseCount,
            LocalChallengeMetric.CompletedCourses => statistics.CompletedCourseCount,
            LocalChallengeMetric.TrainingSessions => statistics.TrainingSessionCount,
            LocalChallengeMetric.PerfectSessions => statistics.PerfectSessionCount,
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, null)
        };
    }
}
