namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Construit un index de recherche sans matérialiser le contenu des cours.
/// </summary>
public static class CourseSearchCatalog
{
    /// <summary>
    /// Tags proposés comme filtres dans l'ordre d'affichage.
    /// </summary>
    public static IReadOnlyList<CourseTagDefinition> Tags { get; } =
    [
        new("numbers", "search.tag.numbers"),
        new("calculation", "search.tag.calculation"),
        new("fractions", "search.tag.fractions"),
        new("decimals", "search.tag.decimals"),
        new("signed-numbers", "search.tag.signedNumbers"),
        new("powers", "search.tag.powers"),
        new("measurements", "search.tag.measurements"),
        new("percentages", "search.tag.percentages"),
        new("proportionality", "search.tag.proportionality"),
        new("geometry", "search.tag.geometry"),
        new("statistics", "search.tag.statistics"),
        new("algebra", "search.tag.algebra"),
        new("functions", "search.tag.functions"),
        new("sequences", "search.tag.sequences"),
        new("analysis", "search.tag.analysis"),
        new("trigonometry", "search.tag.trigonometry"),
        new("probability", "search.tag.probability"),
        new("logic", "search.tag.logic"),
        new("arithmetic", "search.tag.arithmetic"),
        new("coordinates", "search.tag.coordinates"),
        new("combinatorics", "search.tag.combinatorics"),
        new("algorithms", "search.tag.algorithms")
    ];

    /// <summary>
    /// Index complet et léger, construit une seule fois à partir des définitions.
    /// </summary>
    public static IReadOnlyList<CourseSearchEntry> Entries => SearchEntries.Value;

    /// <summary>
    /// Retourne les métadonnées d'un cours à partir de son identifiant persistant.
    /// </summary>
    public static bool TryGet(string courseId, out CourseSearchEntry entry)
    {
        var match = Entries.FirstOrDefault(item => item.CourseId == courseId);
        entry = match!;
        return match is not null;
    }

    /// <summary>
    /// Crée les entrées dans le même ordre que le menu scolaire.
    /// </summary>
    private static IReadOnlyList<CourseSearchEntry> BuildEntries()
    {
        var entries = new List<CourseSearchEntry>();
        foreach (var group in SchoolGroupCatalog.Definitions.OrderBy(item => item.Order))
        {
            foreach (var level in group.Levels)
            {
                AddSpecializedEntries(entries, group.Id, level);

                foreach (var definition in PrimaryCourseCatalog.Definitions
                    .Where(item => item.Level == level))
                {
                    entries.Add(CreateEntry(
                        definition.Id,
                        definition.Title,
                        definition.Level,
                        definition.ChapterId,
                        group.Id));
                }

                foreach (var definition in FoundationNumberCourseCatalog.Definitions
                    .Where(item => item.Level == level))
                {
                    entries.Add(CreateEntry(
                        definition.Id,
                        definition.Title,
                        definition.Level,
                        "understand-numbers",
                        group.Id));
                }

                foreach (var definition in MiddleSchoolCourseCatalog.Definitions
                    .Where(item => item.Level == level))
                {
                    entries.Add(CreateEntry(
                        definition.Id,
                        definition.Title,
                        definition.Level,
                        definition.ChapterId,
                        group.Id));
                }

                foreach (var definition in HighSchoolCourseCatalog.Definitions
                    .Where(item => item.Level == level))
                {
                    entries.Add(CreateEntry(
                        definition.Id,
                        definition.Title,
                        definition.Level,
                        definition.ChapterId,
                        group.Id));
                }
            }
        }

        return entries;
    }

    /// <summary>
    /// Ajoute les cours dont les métadonnées ne proviennent pas d'un catalogue générique.
    /// </summary>
    private static void AddSpecializedEntries(
        ICollection<CourseSearchEntry> entries,
        string schoolGroupId,
        string level)
    {
        if (level == SchoolGroupCatalog.FoundationsLevel)
        {
            entries.Add(CreateEntry(
                CourseCatalog.CountingCourseId,
                "course.counting.title",
                level,
                "understand-numbers",
                schoolGroupId));
            entries.Add(CreateEntry(
                CourseCatalog.AdditionCourseId,
                "course.addition.title",
                level,
                "understand-operations",
                schoolGroupId));
            entries.Add(CreateEntry(
                CourseCatalog.SubtractionCourseId,
                "course.subtraction.title",
                level,
                "understand-operations",
                schoolGroupId));
        }

        if (level == PrimaryCourseCatalog.Ce2Level)
        {
            entries.Add(CreateEntry(
                CourseCatalog.PositionalCounterCourseId,
                "course.counter.title",
                level,
                "understand-numbers",
                schoolGroupId));
        }

        if (level == SchoolGroupCatalog.SecondLevel)
        {
            entries.Add(CreateEntry(
                CourseCatalog.BinaryCourseId,
                "course.binary.title",
                level,
                "understand-numbers",
                schoolGroupId));
        }
    }

    /// <summary>
    /// Produit une entrée et déduit ses tags depuis son chapitre et son identifiant.
    /// </summary>
    private static CourseSearchEntry CreateEntry(
        string courseId,
        string titleKey,
        string levelKey,
        string chapterId,
        string schoolGroupId)
    {
        return new CourseSearchEntry(
            courseId,
            titleKey,
            levelKey,
            chapterId,
            schoolGroupId,
            BuildTags(courseId, chapterId));
    }

    /// <summary>
    /// Déduit des tags stables sans charger les étapes pédagogiques du cours.
    /// </summary>
    private static IReadOnlyList<string> BuildTags(string courseId, string chapterId)
    {
        var tags = new HashSet<string>(StringComparer.Ordinal);
        if (ChapterTags.TryGetValue(chapterId, out var chapterTag))
        {
            tags.Add(chapterTag);
        }

        foreach (var (fragment, tagId) in IdentifierTags)
        {
            if (courseId.Contains(fragment, StringComparison.OrdinalIgnoreCase))
            {
                tags.Add(tagId);
            }
        }

        return Tags
            .Where(definition => tags.Contains(definition.Id))
            .Select(definition => definition.Id)
            .ToArray();
    }

    /// <summary>
    /// Tag principal associé à chaque catégorie du catalogue.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string> ChapterTags =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["understand-numbers"] = "numbers",
            ["understand-operations"] = "calculation",
            ["understand-powers"] = "powers",
            ["measurements"] = "measurements",
            ["proportionality"] = "proportionality",
            ["geometry-plane"] = "geometry",
            ["geometry-space"] = "geometry",
            ["statistics"] = "statistics",
            ["algebra"] = "algebra",
            ["functions"] = "functions",
            ["sequences"] = "sequences",
            ["analysis"] = "analysis",
            ["trigonometry"] = "trigonometry",
            ["probability"] = "probability",
            ["logic"] = "logic",
            ["arithmetic"] = "arithmetic",
            ["coordinate-geometry"] = "coordinates",
            ["combinatorics"] = "combinatorics",
            ["algorithms"] = "algorithms"
        };

    /// <summary>
    /// Tags complémentaires reconnaissables dans les identifiants stables.
    /// </summary>
    private static readonly IReadOnlyList<(string Fragment, string TagId)> IdentifierTags =
    [
        ("fraction", "fractions"),
        ("decimal", "decimals"),
        ("negative", "signed-numbers"),
        ("signed", "signed-numbers"),
        ("percentage", "percentages"),
        ("percent", "percentages")
    ];

    /// <summary>
    /// Retarde la construction de l'index jusqu'à la première recherche.
    /// </summary>
    private static readonly Lazy<IReadOnlyList<CourseSearchEntry>> SearchEntries =
        new(BuildEntries);
}
