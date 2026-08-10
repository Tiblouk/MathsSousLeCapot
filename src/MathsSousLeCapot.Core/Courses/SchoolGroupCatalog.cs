namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Centralise les grands ensembles scolaires et leur progression.
/// </summary>
public static class SchoolGroupCatalog
{
    /// <summary>
    /// Clés des niveaux scolaires qui ne sont pas portés par un autre catalogue.
    /// </summary>
    public const string FoundationsLevel = "chapter.level.bases";
    public const string SixthLevel = "chapter.level.sixth";
    public const string FifthLevel = "chapter.level.fifth";
    public const string FourthLevel = "chapter.level.fourth";
    public const string ThirdLevel = "chapter.level.third";
    public const string SecondLevel = "chapter.level.second";
    public const string FirstLevel = "chapter.level.first";
    public const string TerminalLevel = "chapter.level.terminal";

    /// <summary>
    /// Ensembles ordonnés affichés dans la liste des cours.
    /// </summary>
    public static IReadOnlyList<SchoolGroupDefinition> Definitions { get; } =
    [
        new(
            "foundations",
            "chapter.group.foundations",
            0,
            [FoundationsLevel]),
        new(
            "primary-school",
            "chapter.group.primarySchool",
            1,
            [
                PrimaryCourseCatalog.CpLevel,
                PrimaryCourseCatalog.Ce1Level,
                PrimaryCourseCatalog.Ce2Level,
                PrimaryCourseCatalog.Cm1Level,
                PrimaryCourseCatalog.Cm2Level
            ]),
        new(
            "middle-school",
            "chapter.group.middleSchool",
            2,
            [SixthLevel, FifthLevel, FourthLevel, ThirdLevel]),
        new(
            "high-school",
            "chapter.group.highSchool",
            3,
            [SecondLevel, FirstLevel, TerminalLevel])
    ];

    /// <summary>
    /// Retourne l'ensemble scolaire auquel appartient un niveau.
    /// </summary>
    public static SchoolGroupDefinition GetForLevel(string level)
    {
        return Definitions.Single(definition => definition.Levels.Contains(level));
    }

    /// <summary>
    /// Retourne la position d'un niveau dans son ensemble scolaire.
    /// </summary>
    public static int GetLevelOrder(string level)
    {
        var levels = GetForLevel(level).Levels;
        for (var index = 0; index < levels.Count; index++)
        {
            if (levels[index] == level)
            {
                return index;
            }
        }

        throw new InvalidOperationException($"Niveau scolaire inconnu : {level}");
    }
}
