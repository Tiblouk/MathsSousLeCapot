namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Décrit les métadonnées légères nécessaires à la recherche d'un cours.
/// </summary>
public sealed record CourseSearchEntry(
    string CourseId,
    string TitleKey,
    string LevelKey,
    string ChapterId,
    string SchoolGroupId,
    IReadOnlyList<string> TagIds);

/// <summary>
/// Associe un identifiant de tag stable à sa clé de traduction.
/// </summary>
public sealed record CourseTagDefinition(
    string Id,
    string LabelKey);
