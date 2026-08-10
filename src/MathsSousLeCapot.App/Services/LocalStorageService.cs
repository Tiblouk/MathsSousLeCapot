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
    /// Ancienne clé MAUI Preferences de la progression, conservée pour migrer les données.
    /// </summary>
    private const string LegacyProgressKey = "course_progress";

    /// <summary>
    /// Ancienne clé MAUI Preferences de l'historique, conservée pour migrer les données.
    /// </summary>
    private const string LegacyTrainingKey = "training_history";

    /// <summary>
    /// Incrémente le nombre de lectures du cours indiqué.
    /// </summary>
    public CourseProgress RegisterCourseRead(string courseId)
    {
        var progress = LoadProgress();
        var current = progress.FirstOrDefault(item => item.CourseId == courseId)
            ?? new CourseProgress(courseId, 0, null);
        var updated = current with { ReadCount = current.ReadCount + 1 };

        Replace(progress, updated, item => item.CourseId == courseId);
        Save(ProgressFileName, progress);
        return updated;
    }

    /// <summary>
    /// Enregistre la première validation sans remplacer sa date ultérieurement.
    /// </summary>
    public CourseProgress CompleteCourse(string courseId)
    {
        var progress = LoadProgress();
        var current = progress.FirstOrDefault(item => item.CourseId == courseId)
            ?? new CourseProgress(courseId, 1, null);
        var updated = current.FirstCompletedAt is null
            ? current with { FirstCompletedAt = DateTimeOffset.Now }
            : current;

        Replace(progress, updated, item => item.CourseId == courseId);
        Save(ProgressFileName, progress);
        return updated;
    }

    /// <summary>
    /// Ajoute une session vérifiée à l'historique local.
    /// </summary>
    public void SaveTrainingSession(TrainingSession session)
    {
        var sessions = Load<List<TrainingSession>>(
            TrainingFileName,
            LegacyTrainingKey) ?? [];
        sessions.Add(session);
        Save(TrainingFileName, sessions);
    }

    /// <summary>
    /// Charge la collection de progression ou crée une liste vide.
    /// </summary>
    private static List<CourseProgress> LoadProgress()
    {
        return Load<List<CourseProgress>>(
            ProgressFileName,
            LegacyProgressKey) ?? [];
    }

    /// <summary>
    /// Désérialise une valeur depuis un fichier JSON, avec migration des anciennes préférences.
    /// </summary>
    private static T? Load<T>(string fileName, string legacyKey)
    {
        var path = AppDataPathService.GetDataFilePath(fileName);
        var json = File.Exists(path)
            ? File.ReadAllText(path)
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
    private static void Save<T>(string fileName, T value)
    {
        File.WriteAllText(
            AppDataPathService.GetDataFilePath(fileName),
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
