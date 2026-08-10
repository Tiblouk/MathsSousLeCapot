using MathsSousLeCapot.Core.Pedagogy;

namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Identifie le rôle pédagogique d'un bloc dans une étape de cours.
/// </summary>
public enum CourseContentBlockKind
{
    Introduction,
    Definition,
    Intuition,
    Vocabulary,
    Property,
    Theorem,
    Formula,
    Method,
    WorkedExample,
    CommonMistake,
    Visualization,
    Summary
}

/// <summary>
/// Décrit une instruction ordonnée d'une méthode ou d'un exemple résolu.
/// </summary>
public sealed record CourseMethodStep(
    string Id,
    LocalizedText Instruction,
    string? Expression = null);

/// <summary>
/// Porte un bloc de contenu affichable dans l'ordre défini par le cours.
/// </summary>
public sealed record CourseContentBlock(
    string Id,
    CourseContentBlockKind Kind,
    IReadOnlyList<LocalizedText> Paragraphs,
    LocalizedText? Title = null,
    string? Formula = null,
    IReadOnlyList<CourseMethodStep>? MethodSteps = null,
    string? VisualizationId = null);

/// <summary>
/// Décrit une erreur fréquente et la manière de corriger le raisonnement.
/// </summary>
public sealed record CourseCommonMistake(
    string Id,
    LocalizedText Description,
    LocalizedText Correction);

/// <summary>
/// Regroupe les informations pédagogiques transversales d'un cours.
/// </summary>
public sealed record CoursePedagogicalContent(
    IReadOnlyList<LocalizedText>? LearningObjectives = null,
    IReadOnlyList<LocalizedText>? Vocabulary = null,
    IReadOnlyList<LocalizedText>? KeyTakeaways = null,
    IReadOnlyList<CourseCommonMistake>? CommonMistakes = null);
