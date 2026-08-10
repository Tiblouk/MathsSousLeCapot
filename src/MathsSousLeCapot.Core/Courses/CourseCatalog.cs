namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Catalogue local et ordonné des chapitres et cours disponibles.
/// </summary>
public static class CourseCatalog
{
    /// <summary>
    /// Identifiant stable du cours fondamental de comptage.
    /// </summary>
    public const string CountingCourseId = "counting-and-number-names";

    /// <summary>
    /// Identifiant stable du compteur positionnel décimal.
    /// </summary>
    public const string PositionalCounterCourseId = "positional-counter";

    /// <summary>
    /// Identifiant stable du cours de base deux.
    /// </summary>
    public const string BinaryCourseId = "binary-numbers";

    /// <summary>
    /// Identifiant stable du cours d'addition de quantités.
    /// </summary>
    public const string AdditionCourseId = "addition-basics";

    /// <summary>
    /// Identifiant stable du cours de soustraction de quantités.
    /// </summary>
    public const string SubtractionCourseId = "subtraction-basics";

    /// <summary>
    /// Métadonnées légères des catégories, sans construire leurs cours.
    /// </summary>
    public static IReadOnlyList<Chapter> ChapterHeaders { get; } =
    [
        new Chapter(
            "understand-numbers",
            "chapter.numbers.title",
            "chapter.numbers.intro",
            "chapter.level.bases",
            []),
        new Chapter(
            "understand-operations",
            "chapter.operations.title",
            "chapter.operations.intro",
            "chapter.level.bases",
            []),
        CreateChapterHeader(
            "understand-powers",
            "chapter.powers.title",
            "chapter.powers.intro"),
        CreateChapterHeader(
            "measurements",
            "chapter.measurements.title",
            "chapter.measurements.intro"),
        CreateChapterHeader(
            "proportionality",
            "chapter.proportionality.title",
            "chapter.proportionality.intro"),
        CreateChapterHeader(
            "geometry-plane",
            "chapter.geometryPlane.title",
            "chapter.geometryPlane.intro"),
        CreateChapterHeader(
            "geometry-space",
            "chapter.geometrySpace.title",
            "chapter.geometrySpace.intro"),
        CreateChapterHeader(
            "statistics",
            "chapter.statistics.title",
            "chapter.statistics.intro"),
        CreateChapterHeader(
            "algebra",
            "middle.chapter.algebra.title",
            "middle.chapter.algebra.intro"),
        CreateChapterHeader(
            "functions",
            "middle.chapter.functions.title",
            "middle.chapter.functions.intro"),
        CreateChapterHeader(
            "sequences",
            "high.chapter.sequences.title",
            "high.chapter.sequences.intro"),
        CreateChapterHeader(
            "analysis",
            "high.chapter.analysis.title",
            "high.chapter.analysis.intro"),
        CreateChapterHeader(
            "trigonometry",
            "middle.chapter.trigonometry.title",
            "middle.chapter.trigonometry.intro"),
        CreateChapterHeader(
            "probability",
            "middle.chapter.probability.title",
            "middle.chapter.probability.intro"),
        CreateChapterHeader(
            "logic",
            "middle.chapter.logic.title",
            "middle.chapter.logic.intro"),
        CreateChapterHeader(
            "arithmetic",
            "middle.chapter.arithmetic.title",
            "middle.chapter.arithmetic.intro"),
        CreateChapterHeader(
            "coordinate-geometry",
            "middle.chapter.coordinates.title",
            "middle.chapter.coordinates.intro"),
        CreateChapterHeader(
            "combinatorics",
            "middle.chapter.combinatorics.title",
            "middle.chapter.combinatorics.intro"),
        CreateChapterHeader(
            "algorithms",
            "middle.chapter.algorithms.title",
            "middle.chapter.algorithms.intro")
    ];

    /// <summary>
    /// Catalogue complet conservé pour les traitements globaux hors démarrage.
    /// </summary>
    public static IReadOnlyList<Chapter> Chapters => CompleteChapters.Value;

    /// <summary>
    /// Retarde la construction de tous les parcours jusqu'à un besoin explicite.
    /// </summary>
    private static readonly Lazy<IReadOnlyList<Chapter>> CompleteChapters =
        new(BuildCompleteChapters);

    /// <summary>
    /// Recherche et construit uniquement le cours demandé.
    /// </summary>
    public static Course GetCourse(string courseId)
    {
        var specializedCourse = courseId switch
        {
            CountingCourseId => CreateCountingCourse(),
            PositionalCounterCourseId => CreatePositionalCounterCourse(),
            BinaryCourseId => CreateBinaryCourse(),
            AdditionCourseId => CreateAdditionCourse(),
            SubtractionCourseId => CreateSubtractionCourse(),
            _ => null
        };
        if (specializedCourse is not null)
        {
            return specializedCourse;
        }

        if (FoundationNumberCourseCatalog.TryGet(
            courseId,
            out var foundationDefinition))
        {
            return CreateFoundationNumberCourse(foundationDefinition);
        }

        if (PrimaryCourseCatalog.TryGet(courseId, out var primaryDefinition))
        {
            return CreatePrimaryCourse(primaryDefinition);
        }

        if (MiddleSchoolCourseCatalog.TryGet(courseId, out var middleDefinition))
        {
            return CreateMiddleSchoolCourse(middleDefinition);
        }

        if (HighSchoolCourseCatalog.TryGet(courseId, out var highDefinition))
        {
            return CreateHighSchoolCourse(highDefinition);
        }

        throw new InvalidOperationException($"Cours inconnu : {courseId}");
    }

    /// <summary>
    /// Construit seulement les cours du niveau que l'utilisateur vient d'ouvrir.
    /// </summary>
    public static IReadOnlyList<Course> GetCoursesForLevel(string level)
    {
        var courses = new List<Course>();

        if (level == SchoolGroupCatalog.FoundationsLevel)
        {
            courses.Add(CreateCountingCourse());
            courses.Add(CreateAdditionCourse());
            courses.Add(CreateSubtractionCourse());
        }

        if (level == PrimaryCourseCatalog.Ce2Level)
        {
            courses.Add(CreatePositionalCounterCourse());
        }

        if (level == SchoolGroupCatalog.SecondLevel)
        {
            courses.Add(CreateBinaryCourse());
        }

        if (IsPrimaryLevel(level))
        {
            courses.AddRange(PrimaryCourseCatalog.Definitions
                .Where(definition => definition.Level == level)
                .Select(CreatePrimaryCourse));
            courses.AddRange(FoundationNumberCourseCatalog.Definitions
                .Where(definition => definition.Level == level)
                .Select(CreateFoundationNumberCourse));
        }
        else if (IsMiddleSchoolLevel(level))
        {
            courses.AddRange(MiddleSchoolCourseCatalog.Definitions
                .Where(definition => definition.Level == level)
                .Select(CreateMiddleSchoolCourse));
            courses.AddRange(FoundationNumberCourseCatalog.Definitions
                .Where(definition => definition.Level == level)
                .Select(CreateFoundationNumberCourse));
        }
        else if (IsHighSchoolLevel(level))
        {
            courses.AddRange(HighSchoolCourseCatalog.Definitions
                .Where(definition => definition.Level == level)
                .Select(CreateHighSchoolCourse));
        }

        var chapterOrder = ChapterHeaders
            .Select((chapter, index) => (chapter.Id, index))
            .ToDictionary(item => item.Id, item => item.index);
        return courses
            .OrderBy(course => chapterOrder[course.ChapterId])
            .ToArray();
    }

    /// <summary>
    /// Compte les définitions d'un niveau sans construire leurs étapes.
    /// </summary>
    public static int GetCourseCount(string level)
    {
        if (level == SchoolGroupCatalog.FoundationsLevel)
        {
            return 3;
        }

        if (IsPrimaryLevel(level))
        {
            var specializedCount = level == PrimaryCourseCatalog.Ce2Level ? 1 : 0;
            return specializedCount
                + PrimaryCourseCatalog.Definitions.Count(
                    definition => definition.Level == level)
                + FoundationNumberCourseCatalog.Definitions.Count(
                    definition => definition.Level == level);
        }

        if (IsMiddleSchoolLevel(level))
        {
            return MiddleSchoolCourseCatalog.Definitions.Count(
                    definition => definition.Level == level)
                + FoundationNumberCourseCatalog.Definitions.Count(
                    definition => definition.Level == level);
        }

        if (IsHighSchoolLevel(level))
        {
            var specializedCount = level == SchoolGroupCatalog.SecondLevel ? 1 : 0;
            return specializedCount
                + HighSchoolCourseCatalog.Definitions.Count(
                    definition => definition.Level == level);
        }

        return 0;
    }

    /// <summary>
    /// Indique si une clé correspond à une classe de l'école primaire.
    /// </summary>
    private static bool IsPrimaryLevel(string level)
    {
        return level is
            PrimaryCourseCatalog.CpLevel
            or PrimaryCourseCatalog.Ce1Level
            or PrimaryCourseCatalog.Ce2Level
            or PrimaryCourseCatalog.Cm1Level
            or PrimaryCourseCatalog.Cm2Level;
    }

    /// <summary>
    /// Indique si une clé correspond à une classe du collège.
    /// </summary>
    private static bool IsMiddleSchoolLevel(string level)
    {
        return level is
            SchoolGroupCatalog.SixthLevel
            or SchoolGroupCatalog.FifthLevel
            or SchoolGroupCatalog.FourthLevel
            or SchoolGroupCatalog.ThirdLevel;
    }

    /// <summary>
    /// Indique si une clé correspond à une classe du lycée.
    /// </summary>
    private static bool IsHighSchoolLevel(string level)
    {
        return level is
            SchoolGroupCatalog.SecondLevel
            or SchoolGroupCatalog.FirstLevel
            or SchoolGroupCatalog.TerminalLevel;
    }

    /// <summary>
    /// Construit le cours consacré aux premiers nombres.
    /// </summary>
    private static Course CreateCountingCourse()
    {
        return new Course(
            CountingCourseId,
            "understand-numbers",
            "course.counting.title",
            "chapter.level.bases",
            "course.counting.objective",
            "course.counting.summary",
            [],
            [
                new CourseStep(
                    "digits",
                    "course.counting.step.digits.title",
                    "course.counting.step.digits.explanation",
                    "course.counting.step.digits.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "counting",
                    "course.counting.step.order.title",
                    "course.counting.step.order.explanation",
                    "course.counting.step.order.example",
                    CourseStepKind.InteractiveCounting),
                new CourseStep(
                    "number-names",
                    "course.counting.step.names.title",
                    "course.counting.step.names.explanation",
                    "course.counting.step.names.example",
                    CourseStepKind.InteractiveCounting),
                new CourseStep(
                    "tens",
                    "course.counting.step.tens.title",
                    "course.counting.step.tens.explanation",
                    "course.counting.step.tens.example",
                    CourseStepKind.InteractiveCounting),
                new CourseStep(
                    "validation",
                    "course.counting.step.validation.title",
                    "course.counting.step.validation.explanation",
                    "course.counting.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Construit le cours de numération positionnelle décimale.
    /// </summary>
    private static Course CreatePositionalCounterCourse()
    {
        return new Course(
            PositionalCounterCourseId,
            "understand-numbers",
            "course.counter.title",
            PrimaryCourseCatalog.Ce2Level,
            "course.counter.objective",
            "course.counter.summary",
            [new CoursePrerequisite(CountingCourseId, "course.counting.title")],
            [
                new CourseStep(
                    "positions",
                    "course.counter.step.positions.title",
                    "course.counter.step.positions.explanation",
                    "course.counter.step.positions.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "counter",
                    "course.counter.step.count.title",
                    "course.counter.step.count.explanation",
                    "course.counter.step.count.example",
                    CourseStepKind.InteractiveCounter),
                new CourseStep(
                    "jumps",
                    "course.counter.step.jumps.title",
                    "course.counter.step.jumps.explanation",
                    "course.counter.step.jumps.example",
                    CourseStepKind.InteractiveCounter),
                new CourseStep(
                    "place-values",
                    "course.counter.step.values.title",
                    "course.counter.step.values.explanation",
                    "course.counter.step.values.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "validation",
                    "course.counter.step.validation.title",
                    "course.counter.step.validation.explanation",
                    "course.counter.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Construit le cours de numération binaire classé au niveau lycée.
    /// </summary>
    private static Course CreateBinaryCourse()
    {
        return new Course(
            BinaryCourseId,
            "understand-numbers",
            "course.binary.title",
            "chapter.level.second",
            "course.binary.objective",
            "course.binary.summary",
            [new CoursePrerequisite(PositionalCounterCourseId, "course.counter.title")],
            [
                new CourseStep(
                    "binary-digits",
                    "course.binary.step.digits.title",
                    "course.binary.step.digits.explanation",
                    "course.binary.step.digits.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "binary-counter",
                    "course.binary.step.counter.title",
                    "course.binary.step.counter.explanation",
                    "course.binary.step.counter.example",
                    CourseStepKind.InteractiveCounter),
                new CourseStep(
                    "binary-values",
                    "course.binary.step.values.title",
                    "course.binary.step.values.explanation",
                    "course.binary.step.values.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "binary-validation",
                    "course.binary.step.validation.title",
                    "course.binary.step.validation.explanation",
                    "course.binary.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Construit le cours qui présente l'addition comme réunion de quantités.
    /// </summary>
    private static Course CreateAdditionCourse()
    {
        return new Course(
            AdditionCourseId,
            "understand-operations",
            "course.addition.title",
            "chapter.level.bases",
            "course.addition.objective",
            "course.addition.summary",
            [new CoursePrerequisite(CountingCourseId, "course.counting.title")],
            [
                new CourseStep(
                    "meaning",
                    "course.addition.step.meaning.title",
                    "course.addition.step.meaning.explanation",
                    "course.addition.step.meaning.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "combine",
                    "course.addition.step.combine.title",
                    "course.addition.step.combine.explanation",
                    "course.addition.step.combine.example",
                    CourseStepKind.InteractiveOperation),
                new CourseStep(
                    "zero",
                    "course.addition.step.zero.title",
                    "course.addition.step.zero.explanation",
                    "course.addition.step.zero.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "validation",
                    "course.addition.step.validation.title",
                    "course.addition.step.validation.explanation",
                    "course.addition.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Construit le cours qui présente la soustraction comme retrait d'une quantité.
    /// </summary>
    private static Course CreateSubtractionCourse()
    {
        return new Course(
            SubtractionCourseId,
            "understand-operations",
            "course.subtraction.title",
            "chapter.level.bases",
            "course.subtraction.objective",
            "course.subtraction.summary",
            [new CoursePrerequisite(AdditionCourseId, "course.addition.title")],
            [
                new CourseStep(
                    "meaning",
                    "course.subtraction.step.meaning.title",
                    "course.subtraction.step.meaning.explanation",
                    "course.subtraction.step.meaning.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "remove",
                    "course.subtraction.step.remove.title",
                    "course.subtraction.step.remove.explanation",
                    "course.subtraction.step.remove.example",
                    CourseStepKind.InteractiveOperation),
                new CourseStep(
                    "difference",
                    "course.subtraction.step.difference.title",
                    "course.subtraction.step.difference.explanation",
                    "course.subtraction.step.difference.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "validation",
                    "course.subtraction.step.validation.title",
                    "course.subtraction.step.validation.explanation",
                    "course.subtraction.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Construit un chapitre composé des notions génériques du niveau primaire.
    /// </summary>
    private static Chapter CreateChapterHeader(
        string id,
        string title,
        string description)
    {
        return new Chapter(
            id,
            title,
            description,
            "chapter.level.primary",
            []);
    }

    /// <summary>
    /// Crée le parcours commun d'une notion primaire.
    /// </summary>
    private static Course CreatePrimaryCourse(PrimaryCourseDefinition definition)
    {
        var prerequisites = definition.PrerequisiteId is null
            ? Array.Empty<CoursePrerequisite>()
            :
            [
                new CoursePrerequisite(
                    definition.PrerequisiteId,
                    GetCourseTitle(definition.PrerequisiteId))
            ];

        return new Course(
            definition.Id,
            definition.ChapterId,
            definition.Title,
            definition.Level,
            "course.primary.objective",
            "course.primary.summary",
            prerequisites,
            [
                new CourseStep(
                    "discover",
                    "course.primary.step.discover.title",
                    "course.primary.step.discover.explanation",
                    "course.primary.step.discover.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "try",
                    "course.primary.step.try.title",
                    "course.primary.step.try.explanation",
                    "course.primary.step.try.example",
                    CourseStepKind.InteractivePrimary),
                new CourseStep(
                    "validation",
                    "course.primary.step.validation.title",
                    "course.primary.step.validation.explanation",
                    "course.primary.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Crée le parcours détaillé d'un cours fondamental de numération.
    /// </summary>
    private static Course CreateFoundationNumberCourse(
        FoundationNumberCourseDefinition definition)
    {
        var prefix = definition.Kind == FoundationNumberKind.NegativeNumbers
            ? "course.negative"
            : "course.decimal";

        return new Course(
            definition.Id,
            "understand-numbers",
            definition.Title,
            definition.Level,
            definition.Objective,
            definition.Summary,
            [
                new CoursePrerequisite(
                    definition.PrerequisiteId,
                    definition.PrerequisiteTitle)
            ],
            [
                new CourseStep(
                    "meaning",
                    $"{prefix}.step.meaning.title",
                    $"{prefix}.step.meaning.explanation",
                    $"{prefix}.step.meaning.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "positions",
                    $"{prefix}.step.positions.title",
                    $"{prefix}.step.positions.explanation",
                    $"{prefix}.step.positions.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "visualize",
                    $"{prefix}.step.visualize.title",
                    $"{prefix}.step.visualize.explanation",
                    $"{prefix}.step.visualize.example",
                    CourseStepKind.InteractiveFoundationNumber),
                new CourseStep(
                    "try",
                    $"{prefix}.step.try.title",
                    $"{prefix}.step.try.explanation",
                    $"{prefix}.step.try.example",
                    CourseStepKind.InteractiveFoundationNumber),
                new CourseStep(
                    "validation",
                    $"{prefix}.step.validation.title",
                    $"{prefix}.step.validation.explanation",
                    $"{prefix}.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Crée le parcours découverte, essai et validation d'une notion du collège.
    /// </summary>
    private static Course CreateMiddleSchoolCourse(
        MiddleSchoolCourseDefinition definition)
    {
        var prerequisites = definition.PrerequisiteId is null
            ? Array.Empty<CoursePrerequisite>()
            :
            [
                new CoursePrerequisite(
                    definition.PrerequisiteId,
                    GetCourseTitle(definition.PrerequisiteId))
            ];

        return new Course(
            definition.Id,
            definition.ChapterId,
            definition.Title,
            definition.Level,
            "course.middle.objective",
            "course.middle.summary",
            prerequisites,
            [
                new CourseStep(
                    "discover",
                    "course.middle.step.discover.title",
                    definition.DiscoveryKey,
                    "course.middle.step.discover.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "try",
                    "course.middle.step.try.title",
                    "course.middle.step.try.explanation",
                    "course.middle.step.try.example",
                    CourseStepKind.InteractiveMiddleSchool),
                new CourseStep(
                    "validation",
                    "course.middle.step.validation.title",
                    "course.middle.step.validation.explanation",
                    "course.middle.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Crée le parcours découverte, essai et validation d'une notion du lycée.
    /// </summary>
    private static Course CreateHighSchoolCourse(
        HighSchoolCourseDefinition definition)
    {
        var prerequisites = definition.PrerequisiteId is null
            ? Array.Empty<CoursePrerequisite>()
            :
            [
                new CoursePrerequisite(
                    definition.PrerequisiteId,
                    GetCourseTitle(definition.PrerequisiteId))
            ];

        return new Course(
            definition.Id,
            definition.ChapterId,
            definition.Title,
            definition.Level,
            "course.high.objective",
            "course.high.summary",
            prerequisites,
            [
                new CourseStep(
                    "discover",
                    "course.high.step.discover.title",
                    definition.DiscoveryKey,
                    "course.high.step.discover.example",
                    CourseStepKind.Explanation),
                new CourseStep(
                    "try",
                    "course.high.step.try.title",
                    "course.high.step.try.explanation",
                    "course.high.step.try.example",
                    CourseStepKind.InteractiveHighSchool),
                new CourseStep(
                    "validation",
                    "course.high.step.validation.title",
                    "course.high.step.validation.explanation",
                    "course.high.step.validation.example",
                    CourseStepKind.FinalQuestion)
            ],
            IsAvailable: true);
    }

    /// <summary>
    /// Résout la clé de titre d'un prérequis, spécialisé ou générique.
    /// </summary>
    private static string GetCourseTitle(string courseId)
    {
        if (PrimaryCourseCatalog.TryGet(courseId, out var definition))
        {
            return definition.Title;
        }

        if (MiddleSchoolCourseCatalog.TryGet(courseId, out var middleDefinition))
        {
            return middleDefinition.Title;
        }

        if (HighSchoolCourseCatalog.TryGet(courseId, out var highDefinition))
        {
            return highDefinition.Title;
        }

        if (FoundationNumberCourseCatalog.TryGet(
            courseId,
            out var foundationDefinition))
        {
            return foundationDefinition.Title;
        }

        return courseId switch
        {
            CountingCourseId => "course.counting.title",
            PositionalCounterCourseId => "course.counter.title",
            BinaryCourseId => "course.binary.title",
            AdditionCourseId => "course.addition.title",
            SubtractionCourseId => "course.subtraction.title",
            _ => throw new InvalidOperationException(
                $"Le prérequis '{courseId}' ne possède pas de titre.")
        };
    }

    /// <summary>
    /// Matérialise le catalogue complet uniquement pour les consommateurs globaux.
    /// </summary>
    private static IReadOnlyList<Chapter> BuildCompleteChapters()
    {
        var courses = SchoolGroupCatalog.Definitions
            .SelectMany(group => group.Levels)
            .SelectMany(GetCoursesForLevel)
            .ToArray();

        return ChapterHeaders
            .Select(header => header with
            {
                Courses = courses
                    .Where(course => course.ChapterId == header.Id)
                    .ToArray()
            })
            .ToArray();
    }
}
