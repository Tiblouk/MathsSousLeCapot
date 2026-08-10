namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Représente un contrôle intermédiaire généré pour une classe précise.
/// </summary>
public sealed record ClassAssessment(
    string Level,
    TrainingDifficulty Difficulty,
    IReadOnlyList<Exercise> Exercises);
