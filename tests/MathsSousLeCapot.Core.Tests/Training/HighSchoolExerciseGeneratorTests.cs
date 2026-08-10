using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

/// <summary>
/// Vérifie la génération des exercices des cours du lycée.
/// </summary>
public sealed class HighSchoolExerciseGeneratorTests
{
    [Fact]
    public void Session_contains_five_exercises_for_each_high_school_kind()
    {
        var generator = new HighSchoolExerciseGenerator(new Random(42));

        foreach (var definition in HighSchoolCourseCatalog.Definitions)
        {
            var exercises = generator.CreateSession(
                definition,
                TrainingDifficulty.Moderate);

            Assert.Equal(5, exercises.Count);
            Assert.All(exercises, exercise =>
            {
                Assert.Equal(definition.Id, exercise.CourseId);
                Assert.StartsWith("exercise.high.", exercise.QuestionKey);
                Assert.True(
                    int.TryParse(exercise.CorrectAnswer, out _),
                    $"Réponse non entière pour {definition.Id}");
                Assert.Equal(
                    CalculateExpectedAnswer(exercise),
                    int.Parse(exercise.CorrectAnswer));
            });
        }
    }

    [Fact]
    public void Validation_uses_two_distinct_questions_for_every_course()
    {
        var generator = new HighSchoolExerciseGenerator(new Random(7));

        foreach (var definition in HighSchoolCourseCatalog.Definitions)
        {
            var exercises = generator.CreateValidation(
                definition,
                TrainingDifficulty.Hard);

            Assert.Equal(2, exercises.Count);
            Assert.NotEqual(
                CreateSignature(exercises[0]),
                CreateSignature(exercises[1]));
        }
    }

    /// <summary>
    /// Vérifie que la probabilité d'une loi binomiale reste comprise entre zéro et un.
    /// </summary>
    [Fact]
    public void Binomial_expectation_uses_a_valid_probability()
    {
        var definition = HighSchoolCourseCatalog.Get("binomial-distribution");
        var generator = new HighSchoolExerciseGenerator(new Random(19));

        foreach (var index in Enumerable.Range(0, 50))
        {
            var exercise = generator.CreateExercise(
                definition,
                TrainingDifficulty.Hard,
                index);
            var arguments = exercise.QuestionArguments!.Select(int.Parse).ToArray();
            var trials = arguments[0];
            var numerator = arguments[1];
            var denominator = arguments[2];

            Assert.Equal("exercise.high.binomialExpectationFraction", exercise.QuestionKey);
            Assert.InRange(numerator, 1, denominator - 1);
            Assert.Equal(0, trials % denominator);
            Assert.Equal(
                trials * numerator / denominator,
                int.Parse(exercise.CorrectAnswer));
        }
    }

    /// <summary>
    /// Vérifie que les fonctions affines proposées peuvent être croissantes ou décroissantes.
    /// </summary>
    [Fact]
    public void Function_variation_exercises_cover_both_directions()
    {
        var definition = HighSchoolCourseCatalog.Get("function-variations");
        var generator = new HighSchoolExerciseGenerator(new Random(23));
        var answers = Enumerable.Range(0, 50)
            .Select(index => generator.CreateExercise(
                definition,
                TrainingDifficulty.Moderate,
                index).CorrectAnswer)
            .ToArray();

        Assert.Contains("0", answers);
        Assert.Contains("1", answers);
    }

    /// <summary>
    /// Résume une question paramétrée pour comparer deux générations.
    /// </summary>
    private static string CreateSignature(Exercise exercise)
    {
        return string.Join(
            "|",
            exercise.QuestionKey,
            string.Join(";", exercise.QuestionArguments ?? []),
            exercise.CorrectAnswer);
    }

    /// <summary>
    /// Recalcule la réponse depuis les seules données visibles dans l'énoncé.
    /// </summary>
    private static int CalculateExpectedAnswer(Exercise exercise)
    {
        var values = exercise.QuestionArguments?.ToArray() ?? [];
        var a = Int(values, 0);
        var b = Int(values, 1);
        var c = Int(values, 2);
        var d = Int(values, 3);
        var e = Int(values, 4);
        var f = Int(values, 5);

        return exercise.QuestionKey switch
        {
            "exercise.high.intervalMembership" => a >= b && a <= c ? 1 : 0,
            "exercise.high.divisibility" => a % b == 0 ? 1 : 0,
            "exercise.high.absoluteDistance" => Math.Abs(a - b),
            "exercise.high.expandedConstant" => a * b,
            "exercise.high.vectorAbscissa" => b - a,
            "exercise.high.lineSlope" => (d - b) / (c - a),
            "exercise.high.functionImage" => 2 * a + b,
            "exercise.high.affineIncreasing" => a > 0 ? 1 : 0,
            "exercise.high.mean" => (a + b + c) / 3,
            "exercise.high.probabilityPercent" => a * 100 / b,
            "exercise.high.loop" => a * b,
            "exercise.high.listSum" => a + b + c,
            "exercise.high.binaryConversion" => Convert.ToInt32(values[0], 2),
            "exercise.high.arithmeticSequence" => a + b * c,
            "exercise.high.geometricSequence" => a * (int)Math.Pow(b, c),
            "exercise.high.discriminant" => b * b - 4 * a * c,
            "exercise.high.rootSum" => a + b,
            "exercise.high.derivativePowerCoefficient" => a * b,
            "exercise.high.tangentSlopeSquare" => 2 * a,
            "exercise.high.exponentialSum" => a + b,
            "exercise.high.trigonometricValue" => TrigonometricValue(values),
            "exercise.high.dotProduct" => a * c + b * d,
            "exercise.high.distanceHorizontal" => Math.Abs(b - a),
            "exercise.high.conditionalProbability" => a * 100 / b,
            "exercise.high.expectedValue" => (a + b) / 2,
            "exercise.high.combination" => Combination(a, b),
            "exercise.high.spaceVectorZ" => b - a,
            "exercise.high.planeNormalZ" => c,
            "exercise.high.spaceDotProduct" => a * d + b * e + c * f,
            "exercise.high.geometricLimitRatio" => 0,
            "exercise.high.inductionInitial" => a,
            "exercise.high.functionLimitAtInfinity" => 0,
            "exercise.high.intermediateValueExistence" => a * b < 0 ? 1 : 0,
            "exercise.high.logarithmProduct" => a + b,
            "exercise.high.integralTwoX" => a * a,
            "exercise.high.differentialExponent" => a,
            "exercise.high.binomialExpectationFraction" => a * b / c,
            "exercise.high.poissonExpectation" => a,
            "exercise.high.geometricExpectation" => a,
            "exercise.high.varianceSum" => a + b,
            "exercise.high.largeNumbersStabilization" => 1,
            "exercise.high.largeNumbersExact" => 0,
            "exercise.high.intervalMidpoint" => (a + b) / 2,
            _ => throw new InvalidOperationException(
                $"Question lycée non vérifiée : {exercise.QuestionKey}")
        };
    }

    /// <summary>
    /// Retourne une valeur remarquable entière de sinus ou de cosinus.
    /// </summary>
    private static int TrigonometricValue(string[] values)
    {
        return (values[0], values[1]) switch
        {
            ("sin", "0") => 0,
            ("sin", "π/2") => 1,
            ("sin", "3π/2") => -1,
            ("cos", "0") => 1,
            ("cos", "π/2") => 0,
            ("cos", "π") => -1,
            _ => throw new InvalidOperationException(
                $"Valeur trigonométrique non vérifiée : {values[0]}({values[1]})")
        };
    }

    /// <summary>
    /// Calcule indépendamment un coefficient binomial pour les tests.
    /// </summary>
    private static int Combination(int n, int k)
    {
        k = Math.Min(k, n - k);
        var result = 1;
        for (var index = 1; index <= k; index++)
        {
            result = result * (n - k + index) / index;
        }

        return result;
    }

    /// <summary>
    /// Lit un argument entier lorsqu'il existe.
    /// </summary>
    private static int Int(string[] values, int index)
    {
        return index < values.Length && int.TryParse(values[index], out var value)
            ? value
            : 0;
    }
}
