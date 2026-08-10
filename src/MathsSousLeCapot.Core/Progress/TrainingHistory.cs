using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Progress;

/// <summary>
/// Regroupe les sessions d'entraînement enregistrées pour un cours.
/// </summary>
public sealed record TrainingHistory(
    string CourseId,
    IReadOnlyList<TrainingSession> Sessions);
