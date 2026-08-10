using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Pedagogy;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Courses;

/// <summary>
/// Vérifie l'extension progressive des contrats pédagogiques publics.
/// </summary>
public sealed class PedagogicalModelTests
{
    /// <summary>
    /// Garantit que les catalogues existants peuvent conserver leurs constructeurs historiques.
    /// </summary>
    [Fact]
    public void Legacy_course_and_step_constructors_remain_supported()
    {
        var step = new CourseStep(
            "discover",
            "course.step.title",
            "course.step.explanation",
            "course.step.example",
            CourseStepKind.Explanation);
        var course = new Course(
            "course-id",
            "chapter-id",
            "course.title",
            "chapter.level.bases",
            "course.objective",
            "course.summary",
            [],
            [step],
            IsAvailable: true);

        Assert.Null(step.ContentBlocks);
        Assert.Null(course.PedagogicalContent);
    }

    /// <summary>
    /// Vérifie qu'un cours peut porter des objectifs et des blocs riches ordonnés.
    /// </summary>
    [Fact]
    public void Rich_course_content_preserves_pedagogical_order()
    {
        var definition = new CourseContentBlock(
            "definition",
            CourseContentBlockKind.Definition,
            [new LocalizedText("course.sample.definition")]);
        var method = new CourseContentBlock(
            "method",
            CourseContentBlockKind.Method,
            [new LocalizedText("course.sample.method.intro")],
            MethodSteps:
            [
                new CourseMethodStep(
                    "identify",
                    new LocalizedText("course.sample.method.identify")),
                new CourseMethodStep(
                    "calculate",
                    new LocalizedText("course.sample.method.calculate"),
                    "a + b = c")
            ]);
        var step = new CourseStep(
            "discover",
            "course.step.title",
            "course.step.explanation",
            "course.step.example",
            CourseStepKind.Explanation,
            [definition, method]);
        var content = new CoursePedagogicalContent(
            LearningObjectives:
            [
                new LocalizedText("course.sample.objective")
            ],
            KeyTakeaways:
            [
                new LocalizedText("course.sample.remember")
            ]);
        var course = new Course(
            "course-id",
            "chapter-id",
            "course.title",
            "chapter.level.bases",
            "course.objective",
            "course.summary",
            [],
            [step],
            IsAvailable: true,
            PedagogicalContent: content);

        Assert.Same(content, course.PedagogicalContent);
        Assert.Equal(
            [CourseContentBlockKind.Definition, CourseContentBlockKind.Method],
            step.ContentBlocks!.Select(block => block.Kind));
        Assert.Equal(2, method.MethodSteps!.Count);
    }

    /// <summary>
    /// Garantit que les générateurs actuels n'ont pas à fournir immédiatement une solution riche.
    /// </summary>
    [Fact]
    public void Legacy_exercise_constructor_remains_supported()
    {
        var exercise = new Exercise(
            "exercise-id",
            "course-id",
            "2 + 2 ?",
            "4",
            "2 + 2 = 4");

        Assert.Null(exercise.DetailedSolution);
    }

    /// <summary>
    /// Vérifie qu'une correction peut décrire un raisonnement et une erreur fréquente.
    /// </summary>
    [Fact]
    public void Exercise_can_carry_a_structured_solution()
    {
        var solution = new ExerciseSolution(
            Steps:
            [
                new ExerciseSolutionStep(
                    "rule",
                    ExerciseSolutionStepKind.Rule,
                    new LocalizedText("exercise.sample.rule")),
                new ExerciseSolutionStep(
                    "calculation",
                    ExerciseSolutionStepKind.Calculation,
                    new LocalizedText("exercise.sample.calculation", ["2", "2"]),
                    "2 + 2 = 4")
            ],
            CorrectConclusion: new LocalizedText("exercise.sample.correct"),
            CommonErrors:
            [
                new ExerciseCommonError(
                    "subtraction",
                    new LocalizedText("exercise.sample.error.subtraction"),
                    ["0"])
            ]);
        var exercise = new Exercise(
            "exercise-id",
            "course-id",
            "2 + 2 ?",
            "4",
            "2 + 2 = 4",
            DetailedSolution: solution);

        var actualSolution = Assert.IsType<ExerciseSolution>(
            exercise.DetailedSolution);
        Assert.Same(solution, actualSolution);
        Assert.Equal(2, actualSolution.Steps.Count);
        Assert.Equal("0", actualSolution.CommonErrors![0].MatchingAnswers![0]);
    }
}
