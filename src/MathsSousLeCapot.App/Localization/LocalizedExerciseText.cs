using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Localization;

/// <summary>
/// Résout les clés optionnelles d'un exercice tout en conservant un texte de secours.
/// </summary>
public static class LocalizedExerciseText
{
    /// <summary>
    /// Retourne la question traduite d'un exercice.
    /// </summary>
    /// <param name="exercise">Exercice dont la question doit être affichée.</param>
    public static string GetQuestion(Exercise exercise)
    {
        return Resolve(
            exercise.QuestionKey,
            exercise.QuestionArguments,
            exercise.Question);
    }

    /// <summary>
    /// Retourne l'explication traduite d'un exercice.
    /// </summary>
    /// <param name="exercise">Exercice dont l'explication doit être affichée.</param>
    public static string GetExplanation(Exercise exercise)
    {
        return Resolve(
            exercise.ExplanationKey,
            exercise.ExplanationArguments,
            exercise.Explanation);
    }

    /// <summary>
    /// Choisit entre une clé paramétrée et le texte de secours fourni par le moteur.
    /// </summary>
    private static string Resolve(
        string? key,
        IReadOnlyList<string>? arguments,
        string fallback)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return fallback;
        }

        return TranslationService.Current.Format(
            key,
            arguments?.Cast<object?>().ToArray() ?? []);
    }
}
