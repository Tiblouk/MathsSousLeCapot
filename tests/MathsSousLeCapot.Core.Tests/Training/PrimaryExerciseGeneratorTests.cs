using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

/// <summary>
/// Vérifie le contrat commun de tous les générateurs du niveau primaire.
/// </summary>
public sealed class PrimaryExerciseGeneratorTests
{
    [Theory]
    [InlineData(TrainingDifficulty.Easy)]
    [InlineData(TrainingDifficulty.Moderate)]
    [InlineData(TrainingDifficulty.Hard)]
    public void Every_primary_course_generates_five_valid_numeric_exercises(
        TrainingDifficulty difficulty)
    {
        var generator = new PrimaryExerciseGenerator(new Random(42));

        foreach (var definition in PrimaryCourseCatalog.Definitions)
        {
            var exercises = generator.CreateSession(definition, difficulty);

            Assert.Equal(5, exercises.Count);
            Assert.All(exercises, exercise =>
            {
                Assert.Equal(definition.Id, exercise.CourseId);
                Assert.StartsWith("exercise.primary.", exercise.QuestionKey);
                Assert.Equal("exercise.primary.explanation", exercise.ExplanationKey);
                Assert.True(int.TryParse(exercise.CorrectAnswer, out var answer));
                Assert.InRange(answer, 0, 9999);
            });
        }
    }

    [Fact]
    public void Repeated_multiplication_uses_repeated_factors()
    {
        var definition = PrimaryCourseCatalog.Get("repeated-multiplication");
        var generator = new PrimaryExerciseGenerator(new Random(7));

        var exercises = generator.CreateSession(
            definition,
            TrainingDifficulty.Hard);

        Assert.All(
            exercises,
            exercise => Assert.Contains(
                exercise.QuestionKey,
                new[]
                {
                    "exercise.primary.repeatedMultiplication2",
                    "exercise.primary.repeatedMultiplication3",
                    "exercise.primary.repeatedMultiplication4"
                }));
    }

    [Fact]
    public void Place_value_exercises_expose_the_decimal_position()
    {
        var definition = PrimaryCourseCatalog.Get("large-numbers");
        var generator = new PrimaryExerciseGenerator(new Random(42));

        var exercise = generator.CreateExercise(
            definition,
            TrainingDifficulty.Moderate);

        Assert.Equal(ExerciseAnswerKind.PlaceValue, exercise.AnswerKind);
        Assert.Single(exercise.AnswerArguments!);
        Assert.Contains(
            exercise.AnswerArguments![0],
            new[] { "1", "10", "100", "1000" });
    }

    [Fact]
    public void Every_generated_answer_matches_the_parameters_of_its_question()
    {
        var generator = new PrimaryExerciseGenerator(new Random(1234));

        foreach (var definition in PrimaryCourseCatalog.Definitions)
        {
            var exercises = generator.CreateSession(
                definition,
                TrainingDifficulty.Hard);

            Assert.All(
                exercises,
                exercise => Assert.Equal(
                    ExpectedAnswer(exercise),
                    int.Parse(exercise.CorrectAnswer)));
        }
    }

    [Fact]
    public void Final_validation_uses_two_distinct_questions()
    {
        var generator = new PrimaryExerciseGenerator(new Random(18));

        foreach (var definition in PrimaryCourseCatalog.Definitions)
        {
            var exercises = generator.CreateValidation(
                definition,
                TrainingDifficulty.Hard);

            Assert.Equal(2, exercises.Count);
            Assert.False(
                exercises[0].QuestionKey == exercises[1].QuestionKey
                && exercises[0].QuestionArguments!.SequenceEqual(
                    exercises[1].QuestionArguments!),
                $"Validation dupliquée pour {definition.Id}");
        }
    }

    /// <summary>
    /// Recalcule une réponse à partir du contrat de question traduit.
    /// </summary>
    private static int ExpectedAnswer(Exercise exercise)
    {
        var values = (exercise.QuestionArguments ?? [])
            .Select(int.Parse)
            .ToArray();
        var key = exercise.QuestionKey!["exercise.primary.".Length..];

        return key switch
        {
            "largerNumber" or "chartMaximum" => values.Max(),
            "placeValue" => values[1] * int.Parse(exercise.AnswerArguments![0]),
            "roundToTen" => (int)(Math.Round(
                values[0] / 10d,
                MidpointRounding.AwayFromZero) * 10),
            "fractionOfSet" => values[0] * values[2] / values[1],
            "multiplication" or "unitSquareArea" or "rectangleArea"
                or "proportionality" => values[0] * values[1],
            "division" => values[0] / values[1],
            "addition" or "massReading" or "timeCalculation"
                or "dataTotal" => values.Sum(),
            "subtraction" or "mentalSubtraction" =>
                values[0] - values[1],
            "lengthReading" => values[1] - values[0],
            "repeatedMultiplication2" => values[0] * values[0],
            "repeatedMultiplication3" => values[0] * values[0] * values[0],
            "repeatedMultiplication4" =>
                values[0] * values[0] * values[0] * values[0],
            "lengthConversion" => values[0] * 100,
            "massConversion" => values[0] * 1000,
            "timeReading" => values[0] * 60,
            "circleDiameter" => values[0] * 2,
            "rectanglePerimeter" => 2 * (values[0] + values[1]),
            "cuboidVolume" => values[0] * values[1] * values[2],
            "segmentEndpoints" => 2,
            "segmentBetweenPoints" => 1,
            "parallelIntersections" => 0,
            "perpendicularAngle" or "rightAngle" => 90,
            "straightAngle" => 180,
            "angleReading" => values[0],
            "triangleSides" or "triangleVertices" => 3,
            "quadrilateralSides" or "quadrilateralVertices"
                or "squareSymmetryAxes" => 4,
            "pentagonSides" => 5,
            "hexagonSides" or "cubeFaces" or "cubeNetFaces"
                or "cubeNetSquares" => 6,
            "octagonSides" or "cubeVertices" => 8,
            "rectangleSymmetryAxes" => 2,
            "cubeEdges" => 12,
            _ => throw new InvalidOperationException(
                $"Question non vérifiée : {exercise.QuestionKey}")
        };
    }
}
