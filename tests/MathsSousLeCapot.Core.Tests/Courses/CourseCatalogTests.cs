using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.Core.Tests.Courses;

public sealed class CourseCatalogTests
{
    [Fact]
    public void Number_courses_follow_the_expected_levels()
    {
        Assert.Equal(
            "chapter.level.bases",
            CourseCatalog.GetCourse(CourseCatalog.CountingCourseId).Level);
        Assert.Equal(
            PrimaryCourseCatalog.Ce2Level,
            CourseCatalog.GetCourse(CourseCatalog.PositionalCounterCourseId).Level);
        Assert.Equal(
            "chapter.level.second",
            CourseCatalog.GetCourse(CourseCatalog.BinaryCourseId).Level);
    }

    [Fact]
    public void Decimal_counter_no_longer_contains_a_binary_step()
    {
        var course = CourseCatalog.GetCourse(CourseCatalog.PositionalCounterCourseId);

        Assert.DoesNotContain(
            course.Steps,
            step => step.Id.Contains("binary", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Foundation_operations_are_available_in_the_operations_chapter()
    {
        var addition = CourseCatalog.GetCourse(CourseCatalog.AdditionCourseId);
        var subtraction = CourseCatalog.GetCourse(CourseCatalog.SubtractionCourseId);

        Assert.True(addition.IsAvailable);
        Assert.True(subtraction.IsAvailable);
        Assert.Equal("understand-operations", addition.ChapterId);
        Assert.Equal("understand-operations", subtraction.ChapterId);
        Assert.Equal("chapter.level.bases", addition.Level);
        Assert.Equal("chapter.level.bases", subtraction.Level);
        Assert.Contains(
            subtraction.Prerequisites,
            prerequisite => prerequisite.CourseId == CourseCatalog.AdditionCourseId);
    }

    [Fact]
    public void Specialized_number_courses_use_their_documented_levels()
    {
        foreach (var definition in FoundationNumberCourseCatalog.Definitions)
        {
            var course = CourseCatalog.GetCourse(definition.Id);

            Assert.True(course.IsAvailable);
            Assert.Equal("understand-numbers", course.ChapterId);
            Assert.Equal(definition.Level, course.Level);
            Assert.Equal(5, course.Steps.Count);
            Assert.Equal(
                CourseStepKind.InteractiveFoundationNumber,
                course.Steps[2].Kind);
            Assert.Equal(CourseStepKind.FinalQuestion, course.Steps[4].Kind);
        }
    }

    [Fact]
    public void Every_primary_definition_is_available_in_its_expected_chapter()
    {
        Assert.Equal(37, PrimaryCourseCatalog.Definitions.Count);
        Assert.Equal(
            PrimaryCourseCatalog.Definitions.Count,
            PrimaryCourseCatalog.Definitions.Select(item => item.Id).Distinct().Count());

        foreach (var definition in PrimaryCourseCatalog.Definitions)
        {
            var course = CourseCatalog.GetCourse(definition.Id);

            Assert.True(course.IsAvailable);
            Assert.Equal(definition.Level, course.Level);
            Assert.Contains(
                definition.Level,
                new[]
                {
                    PrimaryCourseCatalog.CpLevel,
                    PrimaryCourseCatalog.Ce1Level,
                    PrimaryCourseCatalog.Ce2Level,
                    PrimaryCourseCatalog.Cm1Level,
                    PrimaryCourseCatalog.Cm2Level
                });
            Assert.Equal(definition.ChapterId, course.ChapterId);
            Assert.Contains(
                CourseCatalog.Chapters,
                chapter => chapter.Id == definition.ChapterId);
        }
    }

    [Fact]
    public void Primary_classes_are_split_into_distinct_subject_chapters()
    {
        string[] expectedChapters =
        [
            "understand-numbers",
            "understand-operations",
            "understand-powers",
            "measurements",
            "proportionality",
            "geometry-plane",
            "geometry-space",
            "statistics"
        ];

        foreach (var chapterId in expectedChapters)
        {
            Assert.Contains(
                CourseCatalog.Chapters,
                chapter => chapter.Id == chapterId
                    && chapter.Courses.Any(course =>
                        PrimaryCourseCatalog.Definitions.Any(definition =>
                            definition.Id == course.Id)));
        }
    }

    [Fact]
    public void Primary_courses_follow_the_expected_class_distribution()
    {
        var counts = PrimaryCourseCatalog.Definitions
            .GroupBy(definition => definition.Level)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(3, counts[PrimaryCourseCatalog.CpLevel]);
        Assert.Equal(5, counts[PrimaryCourseCatalog.Ce1Level]);
        Assert.Equal(11, counts[PrimaryCourseCatalog.Ce2Level]);
        Assert.Equal(12, counts[PrimaryCourseCatalog.Cm1Level]);
        Assert.Equal(6, counts[PrimaryCourseCatalog.Cm2Level]);
    }

    [Theory]
    [InlineData("compare-and-order-numbers", PrimaryCourseCatalog.CpLevel)]
    [InlineData("mental-calculation", PrimaryCourseCatalog.CpLevel)]
    [InlineData("read-data-table", PrimaryCourseCatalog.CpLevel)]
    [InlineData("multiplication-basics", PrimaryCourseCatalog.Ce1Level)]
    [InlineData("written-addition", PrimaryCourseCatalog.Ce1Level)]
    [InlineData("written-subtraction", PrimaryCourseCatalog.Ce1Level)]
    [InlineData("length-measurement", PrimaryCourseCatalog.Ce1Level)]
    [InlineData("time-measurement", PrimaryCourseCatalog.Ce1Level)]
    [InlineData("rounding-and-estimation", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("division-basics", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("multiplication-tables", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("written-multiplication", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("repeated-multiplication", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("length-unit-conversion", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("mass-measurement", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("points-lines-segments", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("polygons", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("solid-shapes", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("create-chart", PrimaryCourseCatalog.Ce2Level)]
    [InlineData("large-numbers", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("fractions-as-numbers", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("written-division", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("mass-unit-conversion", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("area-measurement", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("recognize-proportionality", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("parallel-and-perpendicular-lines", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("angles-introduction", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("angle-measurement", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("triangles", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("quadrilaterals", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("perimeter", PrimaryCourseCatalog.Cm1Level)]
    [InlineData("time-calculation", PrimaryCourseCatalog.Cm2Level)]
    [InlineData("circle-introduction", PrimaryCourseCatalog.Cm2Level)]
    [InlineData("rectangle-area", PrimaryCourseCatalog.Cm2Level)]
    [InlineData("axial-symmetry", PrimaryCourseCatalog.Cm2Level)]
    [InlineData("solid-nets", PrimaryCourseCatalog.Cm2Level)]
    [InlineData("cuboid-volume", PrimaryCourseCatalog.Cm2Level)]
    public void Primary_course_uses_its_documented_class(
        string courseId,
        string expectedLevel)
    {
        Assert.Equal(expectedLevel, PrimaryCourseCatalog.Get(courseId).Level);
        Assert.Equal(expectedLevel, CourseCatalog.GetCourse(courseId).Level);
    }

    [Fact]
    public void No_available_course_uses_the_generic_primary_level()
    {
        Assert.DoesNotContain(
            CourseCatalog.Chapters.SelectMany(chapter => chapter.Courses),
            course => course.Level == "chapter.level.primary");
    }

    [Fact]
    public void School_groups_follow_the_documented_progression()
    {
        Assert.Collection(
            SchoolGroupCatalog.Definitions,
            group => Assert.Equal(
                new[] { SchoolGroupCatalog.FoundationsLevel },
                group.Levels),
            group => Assert.Equal(
                new[]
                {
                    PrimaryCourseCatalog.CpLevel,
                    PrimaryCourseCatalog.Ce1Level,
                    PrimaryCourseCatalog.Ce2Level,
                    PrimaryCourseCatalog.Cm1Level,
                    PrimaryCourseCatalog.Cm2Level
                },
                group.Levels),
            group => Assert.Equal(
                new[]
                {
                    SchoolGroupCatalog.SixthLevel,
                    SchoolGroupCatalog.FifthLevel,
                    SchoolGroupCatalog.FourthLevel,
                    SchoolGroupCatalog.ThirdLevel
                },
                group.Levels),
            group => Assert.Equal(
                new[]
                {
                    SchoolGroupCatalog.SecondLevel,
                    SchoolGroupCatalog.FirstLevel,
                    SchoolGroupCatalog.TerminalLevel
                },
                group.Levels));
    }

    [Fact]
    public void Every_available_course_belongs_to_a_school_group()
    {
        foreach (var course in CourseCatalog.Chapters.SelectMany(
            chapter => chapter.Courses))
        {
            var group = SchoolGroupCatalog.GetForLevel(course.Level);

            Assert.Contains(course.Level, group.Levels);
        }
    }

    [Fact]
    public void Lightweight_chapter_headers_do_not_materialize_courses()
    {
        Assert.NotEmpty(CourseCatalog.ChapterHeaders);
        Assert.All(
            CourseCatalog.ChapterHeaders,
            chapter => Assert.Empty(chapter.Courses));
    }

    [Theory]
    [InlineData(PrimaryCourseCatalog.CpLevel)]
    [InlineData(PrimaryCourseCatalog.Ce2Level)]
    [InlineData(PrimaryCourseCatalog.Cm1Level)]
    [InlineData(SchoolGroupCatalog.SixthLevel)]
    [InlineData(SchoolGroupCatalog.FifthLevel)]
    [InlineData(SchoolGroupCatalog.SecondLevel)]
    [InlineData(SchoolGroupCatalog.FirstLevel)]
    [InlineData(SchoolGroupCatalog.TerminalLevel)]
    public void Level_loading_materializes_only_the_requested_level(string level)
    {
        var courses = CourseCatalog.GetCoursesForLevel(level);

        Assert.NotEmpty(courses);
        Assert.All(courses, course => Assert.Equal(level, course.Level));
        Assert.Equal(CourseCatalog.GetCourseCount(level), courses.Count);
    }

    [Fact]
    public void Primary_examples_are_not_duplicated_between_courses()
    {
        var duplicates = PrimaryCourseCatalog.Definitions
            .GroupBy(definition => definition.Example)
            .Where(group => group.Count() > 1)
            .ToArray();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void Repeated_multiplication_uses_its_specialized_exercise_family()
    {
        Assert.Equal(
            PrimaryExerciseKind.RepeatedMultiplication,
            PrimaryCourseCatalog.Get("repeated-multiplication").ExerciseKind);
    }

    [Fact]
    public void Every_middle_school_definition_is_available_in_one_grade()
    {
        string[] expectedLevels =
        [
            "chapter.level.sixth",
            "chapter.level.fifth",
            "chapter.level.fourth",
            "chapter.level.third"
        ];

        Assert.Equal(93, MiddleSchoolCourseCatalog.Definitions.Count);
        Assert.Equal(
            93,
            MiddleSchoolCourseCatalog.Definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());

        foreach (var definition in MiddleSchoolCourseCatalog.Definitions)
        {
            var course = CourseCatalog.GetCourse(definition.Id);

            Assert.True(course.IsAvailable);
            Assert.Equal(definition.Level, course.Level);
            Assert.Contains(definition.Level, expectedLevels);
            Assert.Equal(3, course.Steps.Count);
            Assert.Equal(
                CourseStepKind.InteractiveMiddleSchool,
                course.Steps[1].Kind);
            Assert.Equal(CourseStepKind.FinalQuestion, course.Steps[2].Kind);
        }
    }

    [Fact]
    public void Middle_school_examples_are_unique()
    {
        var duplicates = MiddleSchoolCourseCatalog.Definitions
            .GroupBy(definition => definition.Example)
            .Where(group => group.Count() > 1)
            .ToArray();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void Every_high_school_definition_is_available_in_one_grade()
    {
        string[] expectedLevels =
        [
            SchoolGroupCatalog.SecondLevel,
            SchoolGroupCatalog.FirstLevel,
            SchoolGroupCatalog.TerminalLevel
        ];

        Assert.Equal(46, HighSchoolCourseCatalog.Definitions.Count);
        Assert.Equal(
            46,
            HighSchoolCourseCatalog.Definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());

        foreach (var definition in HighSchoolCourseCatalog.Definitions)
        {
            var course = CourseCatalog.GetCourse(definition.Id);

            Assert.True(course.IsAvailable);
            Assert.Equal(definition.Level, course.Level);
            Assert.Contains(definition.Level, expectedLevels);
            Assert.Equal(3, course.Steps.Count);
            Assert.Equal(
                CourseStepKind.InteractiveHighSchool,
                course.Steps[1].Kind);
            Assert.Equal(CourseStepKind.FinalQuestion, course.Steps[2].Kind);
        }
    }

    [Fact]
    public void High_school_courses_follow_the_expected_distribution()
    {
        var counts = HighSchoolCourseCatalog.Definitions
            .GroupBy(definition => definition.Level)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(12, counts[SchoolGroupCatalog.SecondLevel]);
        Assert.Equal(14, counts[SchoolGroupCatalog.FirstLevel]);
        Assert.Equal(20, counts[SchoolGroupCatalog.TerminalLevel]);
    }

    [Fact]
    public void High_school_examples_are_unique()
    {
        var duplicates = HighSchoolCourseCatalog.Definitions
            .GroupBy(definition => definition.Example)
            .Where(group => group.Count() > 1)
            .ToArray();

        Assert.Empty(duplicates);
    }

    /// <summary>
    /// Vérifie que les notions avancées utilisent leur propre famille d'exercices.
    /// </summary>
    [Theory]
    [InlineData("list-algorithms-simulations", HighSchoolExerciseKind.AlgorithmList)]
    [InlineData("lines-and-planes-in-space", HighSchoolExerciseKind.SpacePlaneNormal)]
    [InlineData("dot-product-in-space", HighSchoolExerciseKind.SpaceDotProduct)]
    [InlineData("mathematical-induction", HighSchoolExerciseKind.Induction)]
    [InlineData("function-limits", HighSchoolExerciseKind.FunctionLimit)]
    [InlineData("continuity-intermediate-value", HighSchoolExerciseKind.Continuity)]
    public void Advanced_high_school_courses_use_matching_exercise_families(
        string courseId,
        HighSchoolExerciseKind expectedKind)
    {
        Assert.Equal(
            expectedKind,
            HighSchoolCourseCatalog.Get(courseId).ExerciseKind);
    }
}
