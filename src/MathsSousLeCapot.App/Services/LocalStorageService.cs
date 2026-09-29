using System.Diagnostics;
using MathsSousLeCapot.Core.Progress;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Persiste la progression et les entraînements dans des fichiers JSON locaux.
/// </summary>
public sealed class LocalStorageService
{
    /// <summary>
    /// Nom du fichier de progression des cours.
    /// </summary>
    private const string ProgressFileName = "course_progress.json";

    /// <summary>
    /// Nom du fichier d'historique des entraînements.
    /// </summary>
    private const string TrainingFileName = "training_history.json";

    /// <summary>
    /// Nom du résumé léger utilisé par les badges sans relire tout l'historique.
    /// </summary>
    private const string AchievementFileName = "perfect_achievements.json";

    /// <summary>
    /// Ancienne clé MAUI Preferences de la progression, conservée pour migrer les données.
    /// </summary>
    private const string LegacyProgressKey = "course_progress";

    /// <summary>
    /// Ancienne clé MAUI Preferences de l'historique, conservée pour migrer les données.
    /// </summary>
    private const string LegacyTrainingKey = "training_history";

    /// <summary>
    /// Verrou commun aux instances créées par les différentes pages.
    /// </summary>
    private static readonly object StorageSync = new();

    /// <summary>
    /// Incrémente le nombre de lectures du cours indiqué.
    /// </summary>
    public CourseProgress RegisterCourseRead(string courseId)
    {
        lock (StorageSync)
        {
            var profileId = LocalProfileService.Current.ActiveProfile.Id;
            var progress = LoadProgress(profileId);
            var current = progress.FirstOrDefault(item => item.CourseId == courseId)
                ?? new CourseProgress(courseId, 0, null);
            var updated = current with { ReadCount = current.ReadCount + 1 };

            Replace(progress, updated, item => item.CourseId == courseId);
            Save(profileId, ProgressFileName, progress);
            return updated;
        }
    }

    /// <summary>
    /// Enregistre la première validation sans remplacer sa date ultérieurement.
    /// </summary>
    public CourseProgress CompleteCourse(string courseId)
    {
        lock (StorageSync)
        {
            var profileId = LocalProfileService.Current.ActiveProfile.Id;
            var progress = LoadProgress(profileId);
            var current = progress.FirstOrDefault(item => item.CourseId == courseId)
                ?? new CourseProgress(courseId, 1, null);
            var updated = current.FirstCompletedAt is null
                ? current with { FirstCompletedAt = DateTimeOffset.Now }
                : current;

            Replace(progress, updated, item => item.CourseId == courseId);
            Save(profileId, ProgressFileName, progress);
            return updated;
        }
    }

    /// <summary>
    /// Ajoute une session vérifiée à l'historique local.
    /// </summary>
    public void SaveTrainingSession(TrainingSession session)
    {
        lock (StorageSync)
        {
            var profileId = LocalProfileService.Current.ActiveProfile.Id;
            var sessions = Load<List<TrainingSession>>(
                profileId,
                TrainingFileName,
                profileId == LocalProfileService.DefaultProfileId
                    ? LegacyTrainingKey
                    : null) ?? [];
            sessions.Add(session);
            Save(profileId, TrainingFileName, sessions);

            var achievements = LoadAchievements(profileId, sessions);
            if (CourseAchievementCalculator.IsPerfect(session)
                && !achievements.Any(achievement =>
                    achievement.CourseId == session.CourseId
                    && achievement.Difficulty == session.Difficulty))
            {
                achievements.Add(new PerfectTrainingAchievement(
                    session.CourseId,
                    session.Difficulty,
                    session.CompletedAt));
                Save(profileId, AchievementFileName, achievements);
            }
        }
    }

    /// <summary>
    /// Retourne toute la progression du profil actif.
    /// </summary>
    public IReadOnlyList<CourseProgress> GetCourseProgress()
    {
        return GetCourseProgress(LocalProfileService.Current.ActiveProfile.Id);
    }

    /// <summary>
    /// Retourne toute la progression d'un profil précis pour les comparaisons locales.
    /// </summary>
    public IReadOnlyList<CourseProgress> GetCourseProgress(string profileId)
    {
        lock (StorageSync)
        {
            return LoadProgress(profileId).ToArray();
        }
    }

    /// <summary>
    /// Retourne les sessions du profil actif de la plus récente à la plus ancienne.
    /// </summary>
    public IReadOnlyList<TrainingSession> GetTrainingSessions()
    {
        return GetTrainingSessions(LocalProfileService.Current.ActiveProfile.Id);
    }

    /// <summary>
    /// Retourne les sessions d'un profil précis pour les statistiques locales.
    /// </summary>
    public IReadOnlyList<TrainingSession> GetTrainingSessions(string profileId)
    {
        lock (StorageSync)
        {
            return (Load<List<TrainingSession>>(
                    profileId,
                    TrainingFileName,
                    profileId == LocalProfileService.DefaultProfileId
                        ? LegacyTrainingKey
                        : null) ?? [])
                .OrderByDescending(session => session.CompletedAt)
                .ToArray();
        }
    }

    /// <summary>
    /// Retourne le résumé des réussites parfaites du profil actif.
    /// </summary>
    public IReadOnlyList<PerfectTrainingAchievement> GetPerfectTrainingAchievements()
    {
        lock (StorageSync)
        {
            return LoadAchievements(LocalProfileService.Current.ActiveProfile.Id)
                .ToArray();
        }
    }

    /// <summary>
    /// Charge la collection de progression ou crée une liste vide.
    /// </summary>
    private static List<CourseProgress> LoadProgress(string profileId)
    {
        return Load<List<CourseProgress>>(
            profileId,
            ProgressFileName,
            profileId == LocalProfileService.DefaultProfileId
                ? LegacyProgressKey
                : null) ?? [];
    }

    /// <summary>
    /// Charge le résumé ou le crée une seule fois depuis un ancien historique détaillé.
    /// </summary>
    private static List<PerfectTrainingAchievement> LoadAchievements(
        string profileId,
        IReadOnlyList<TrainingSession>? knownSessions = null)
    {
        var stored = Load<List<PerfectTrainingAchievement>>(
            profileId,
            AchievementFileName,
            null);
        if (stored is not null)
        {
            return stored;
        }

        var sessions = knownSessions
            ?? Load<List<TrainingSession>>(
                profileId,
                TrainingFileName,
                profileId == LocalProfileService.DefaultProfileId
                    ? LegacyTrainingKey
                    : null)
            ?? [];
        var achievements = CourseAchievementCalculator.CreateAchievements(sessions)
            .ToList();
        Save(profileId, AchievementFileName, achievements);
        return achievements;
    }

    /// <summary>
    /// Désérialise une valeur depuis un fichier JSON, avec migration des anciennes préférences.
    /// </summary>
    private static T? Load<T>(
        string profileId,
        string fileName,
        string? legacyKey)
    {
        var path = AppDataPathService.GetProfileDataFilePath(profileId, fileName);
        var json = File.Exists(path)
            ? File.ReadAllText(path)
            : legacyKey is null
                ? string.Empty
                : Preferences.Default.Get(legacyKey, string.Empty);
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        if (LocalDataSerializer.TryDeserialize<T>(json, out var value))
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, json);
            }

            return value;
        }

        // La valeur invalide est conservée dans son emplacement d'origine pour diagnostic.
        Debug.WriteLine(
            $"Impossible de désérialiser les données locales associées à '{fileName}'.");
        return default;
    }

    /// <summary>
    /// Sérialise une valeur dans un fichier JSON sauvegardable.
    /// </summary>
    private static void Save<T>(string profileId, string fileName, T value)
    {
        File.WriteAllText(
            AppDataPathService.GetProfileDataFilePath(profileId, fileName),
            LocalDataSerializer.Serialize(value));
    }

    /// <summary>
    /// Remplace un élément existant ou l'ajoute lorsqu'il est absent.
    /// </summary>
    private static void Replace<T>(
        List<T> items,
        T value,
        Func<T, bool> predicate)
    {
        var index = items.FindIndex(item => predicate(item));
        if (index >= 0)
        {
            items[index] = value;
        }
        else
        {
            items.Add(value);
        }
    }
}
