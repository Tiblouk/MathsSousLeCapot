namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Décrit un grand ensemble scolaire et l'ordre de ses niveaux.
/// </summary>
public sealed record SchoolGroupDefinition(
    string Id,
    string Title,
    int Order,
    IReadOnlyList<string> Levels);
