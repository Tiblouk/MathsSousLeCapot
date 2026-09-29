using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Progress;

/// <summary>
/// Classe les accomplissements visibles d'un cours du moins avancé au plus avancé.
/// </summary>
public enum CourseAchievementStatus
{
    Unseen,
    Read,
    PerfectSession,
    PerfectHardSession
}

/// <summary>
/// Mémorise la première réussite sans faute d'un cours pour une difficulté donnée.
/// </summary>
public sealed record PerfectTrainingAchievement(
    string CourseId,
    TrainingDifficulty Difficulty,
    DateTimeOffset FirstAchievedAt);

/// <summary>
/// Déduit le meilleur badge de chaque cours depuis les données du profil.
/// </summary>
public static class CourseAchievementCalculator
{
    /// <summary>
    /// Calcule les statuts sans charger le détail des exercices déjà enregistrés.
    /// </summary>
    public static IReadOnlyDictionary<string, CourseAchievementStatus> Calculate(
        IEnumerable<CourseProgress> progress,
        IEnumerable<PerfectTrainingAchievement> achievements)
    {
        var statuses = progress
            .Where(item => item.ReadCount > 0)
            .ToDictionary(
                item => item.CourseId,
                _ => CourseAchievementStatus.Read,
                StringComparer.Ordinal);

        foreach (var achievement in achievements)
        {
            var achievedStatus = achievement.Difficulty == TrainingDifficulty.Hard
                ? CourseAchievementStatus.PerfectHardSession
                : CourseAchievementStatus.PerfectSession;
            if (!statuses.TryGetValue(achievement.CourseId, out var currentStatus)
                || achievedStatus > currentStatus)
            {
                statuses[achievement.CourseId] = achievedStatus;
            }
        }

        return statuses;
    }

    /// <summary>
    /// Extrait un résumé léger en ne conservant que la première réussite de chaque difficulté.
    /// </summary>
    public static IReadOnlyList<PerfectTrainingAchievement> CreateAchievements(
        IEnumerable<TrainingSession> sessions)
    {
        return sessions
            .Where(IsPerfect)
            .GroupBy(session => (session.CourseId, session.Difficulty))
            .Select(group => new PerfectTrainingAchievement(
                group.Key.CourseId,
                group.Key.Difficulty,
                group.Min(session => session.CompletedAt)))
            .OrderBy(achievement => achievement.FirstAchievedAt)
            .ToArray();
    }

    /// <summary>
    /// Reconnaît une session vérifiée dont toutes les réponses sont correctes.
    /// </summary>
    public static bool IsPerfect(TrainingSession session)
    {
        return session.Results.Count > 0 && session.IncorrectCount == 0;
    }
}
