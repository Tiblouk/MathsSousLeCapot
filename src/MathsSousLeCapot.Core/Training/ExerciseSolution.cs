using MathsSousLeCapot.Core.Pedagogy;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Identifie le rôle d'une étape dans le raisonnement d'une correction.
/// </summary>
public enum ExerciseSolutionStepKind
{
    Rule,
    Formula,
    Substitution,
    Calculation,
    Reasoning,
    Verification,
    Conclusion
}

/// <summary>
/// Décrit une étape localisable du raisonnement et son écriture mathématique.
/// </summary>
public sealed record ExerciseSolutionStep(
    string Id,
    ExerciseSolutionStepKind Kind,
    LocalizedText Explanation,
    string? Expression = null);

/// <summary>
/// Associe une réponse erronée fréquente à une explication corrective.
/// </summary>
public sealed record ExerciseCommonError(
    string Id,
    LocalizedText Explanation,
    IReadOnlyList<string>? MatchingAnswers = null);

/// <summary>
/// Porte la correction détaillée structurée d'un exercice.
/// </summary>
public sealed record ExerciseSolution(
    IReadOnlyList<ExerciseSolutionStep> Steps,
    LocalizedText? CorrectConclusion = null,
    LocalizedText? IncorrectConclusion = null,
    IReadOnlyList<ExerciseCommonError>? CommonErrors = null);
