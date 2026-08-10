using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

/// <summary>
/// Vérifie la génération des contrôles intermédiaires par classe.
/// </summary>
public sealed class ClassAssessmentGeneratorTests
{
    /// <summary>
    /// Niveaux qui possèdent actuellement au moins un cours disponible.
    /// </summary>
    public static TheoryData<string> LevelsWithCourses => new()
    {
        SchoolGroupCatalog.FoundationsLevel,
        PrimaryCourseCatalog.CpLevel,
        PrimaryCourseCatalog.Ce1Level,
        PrimaryCourseCatalog.Ce2Level,
        PrimaryCourseCatalog.Cm1Level,
        PrimaryCourseCatalog.Cm2Level,
        SchoolGroupCatalog.SixthLevel,
        SchoolGroupCatalog.FifthLevel,
        SchoolGroupCatalog.FourthLevel,
        SchoolGroupCatalog.ThirdLevel,
        SchoolGroupCatalog.SecondLevel,
        SchoolGroupCatalog.FirstLevel,
        SchoolGroupCatalog.TerminalLevel
    };

    [Theory]
    [MemberData(nameof(LevelsWithCourses))]
    public void Assessment_uses_only_courses_from_the_requested_level(string level)
    {
        var generator = new ClassAssessmentGenerator(new Random(42));

        var assessment = generator.Create(level);

        Assert.NotEmpty(assessment.Exercises);
        Assert.All(
            assessment.Exercises,
            exercise => Assert.Equal(
                level,
                CourseCatalog.GetCourse(exercise.CourseId).Level));
    }

    [Theory]
    [MemberData(nameof(LevelsWithCourses))]
    public void Assessment_mixes_several_courses_when_the_level_allows_it(
        string level)
    {
        var availableCourseCount = CourseCatalog.GetCourseCount(level);
        var generator = new ClassAssessmentGenerator(new Random(7));

        var assessment = generator.Create(level);
        var courseCount = assessment.Exercises
            .Select(exercise => exercise.CourseId)
            .Distinct()
            .Count();

        Assert.True(
            courseCount >= Math.Min(2, availableCourseCount),
            $"Le contrôle de {level} ne mélange pas assez de cours.");
    }

    [Fact]
    public void Assessment_varies_between_attempts_when_possible()
    {
        var firstGenerator = new ClassAssessmentGenerator(new Random(1));
        var secondGenerator = new ClassAssessmentGenerator(new Random(2));

        var first = firstGenerator.Create(PrimaryCourseCatalog.Ce2Level);
        var second = secondGenerator.Create(PrimaryCourseCatalog.Ce2Level);

        Assert.NotEqual(
            first.Exercises.Select(CreateSignature),
            second.Exercises.Select(CreateSignature));
    }

    [Theory]
    [MemberData(nameof(LevelsWithCourses))]
    public void Assessment_avoids_exact_duplicate_questions(string level)
    {
        var generator = new ClassAssessmentGenerator(new Random(123));

        var assessment = generator.Create(level);
        var signatures = assessment.Exercises.Select(CreateSignature).ToArray();

        Assert.Equal(signatures.Length, signatures.Distinct().Count());
    }

    [Fact]
    public void Empty_level_returns_an_empty_assessment_without_error()
    {
        var generator = new ClassAssessmentGenerator(new Random(5));

        var assessment = generator.Create("chapter.level.unknown");

        Assert.Empty(assessment.Exercises);
    }

    /// <summary>
    /// Résume le contrat d'une question pour comparer deux générations.
    /// </summary>
    private static string CreateSignature(Exercise exercise)
    {
        return string.Join(
            "|",
            exercise.CourseId,
            exercise.QuestionKey,
            string.Join(";", exercise.QuestionArguments ?? []),
            exercise.CorrectAnswer);
    }
}
