using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère les exercices numériques des cours du collège.
/// </summary>
public sealed class MiddleSchoolExerciseGenerator
{
    /// <summary>
    /// Source aléatoire injectable afin de rendre les tests reproductibles.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Initialise le générateur avec une source aléatoire facultative.
    /// </summary>
    public MiddleSchoolExerciseGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>
    /// Crée une session de cinq exercices pour une notion.
    /// </summary>
    public IReadOnlyList<Exercise> CreateSession(
        MiddleSchoolCourseDefinition definition,
        TrainingDifficulty difficulty)
    {
        return Enumerable.Range(0, 5)
            .Select(index => CreateExercise(definition, difficulty, index))
            .ToArray();
    }

    /// <summary>
    /// Crée les deux questions distinctes de validation finale.
    /// </summary>
    public IReadOnlyList<Exercise> CreateValidation(
        MiddleSchoolCourseDefinition definition,
        TrainingDifficulty difficulty)
    {
        var first = CreateExercise(definition, difficulty, 0);
        var second = CreateExercise(definition, difficulty, 1);
        var attempts = 0;

        while (HasSameQuestion(first, second) && attempts < 30)
        {
            second = CreateExercise(definition, difficulty, 1);
            attempts++;
        }

        return [first, second];
    }

    /// <summary>
    /// Crée une question adaptée à la famille mathématique du cours.
    /// </summary>
    public Exercise CreateExercise(
        MiddleSchoolCourseDefinition definition,
        TrainingDifficulty difficulty,
        int index = 0)
    {
        var scale = difficulty switch
        {
            TrainingDifficulty.Easy => 5,
            TrainingDifficulty.Moderate => 10,
            TrainingDifficulty.Hard => 20,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
        var generated = Generate(definition, scale);

        return new Exercise(
            $"{definition.Id}-{difficulty}-{index}-{Guid.NewGuid():N}",
            definition.Id,
            string.Empty,
            generated.Answer.ToString(),
            string.Empty,
            $"exercise.middle.{generated.Key}",
            generated.Arguments,
            "exercise.middle.explanation",
            [generated.Answer.ToString()],
            WrittenCalculation: CreateWrittenCalculation(
                definition,
                generated));
    }

    /// <summary>
    /// Ajoute la division posée aux notions de sixième qui l'emploient.
    /// </summary>
    private static WrittenCalculation? CreateWrittenCalculation(
        MiddleSchoolCourseDefinition definition,
        GeneratedExercise generated)
    {
        if (definition.ExerciseKind is not
            (MiddleSchoolExerciseKind.EuclideanDivision
                or MiddleSchoolExerciseKind.Remainder))
        {
            return null;
        }

        var values = generated.Arguments.Select(int.Parse).ToArray();
        return WrittenCalculationBuilder.Create(
            WrittenCalculationKind.Division,
            values[0],
            values[1]);
    }

    /// <summary>
    /// Produit une question et sa réponse entière, sans dépendance à l'interface.
    /// </summary>
    private GeneratedExercise Generate(
        MiddleSchoolCourseDefinition definition,
        int scale)
    {
        var a = _random.Next(2, scale + 2);
        var b = _random.Next(2, scale + 2);

        return definition.ExerciseKind switch
        {
            MiddleSchoolExerciseKind.FractionComparison =>
                Result("fractionComparison", Math.Max(a, b), a, b, scale + 3),
            MiddleSchoolExerciseKind.RationalOperation =>
                Result("signedDifference", a - b, a, b),
            MiddleSchoolExerciseKind.ScientificNotation =>
                CreateScientificNotation(scale),
            MiddleSchoolExerciseKind.EuclideanDivision =>
                CreateEuclideanDivision(scale, askRemainder: false),
            MiddleSchoolExerciseKind.Remainder =>
                CreateEuclideanDivision(scale, askRemainder: true),
            MiddleSchoolExerciseKind.Divisibility =>
                CreateDivisibility(scale),
            MiddleSchoolExerciseKind.PrimeNumber =>
                CreatePrimeQuestion(),
            MiddleSchoolExerciseKind.PrimeFactorization =>
                CreatePrimeFactorization(),
            MiddleSchoolExerciseKind.LeastCommonMultiple =>
                CreateLeastCommonMultiple(scale),
            MiddleSchoolExerciseKind.OperationPriority =>
                CreatePriorityCalculation(definition.Id, a, b),
            MiddleSchoolExerciseKind.SignedOperation =>
                Result("signedDifference", a - b * 2, a, b * 2),
            MiddleSchoolExerciseKind.DecimalOperation =>
                Result("decimalTenths", a + b, a, b),
            MiddleSchoolExerciseKind.FractionOperation =>
                CreateFractionOperation(definition.Id, a, b, scale + 5),
            MiddleSchoolExerciseKind.SimplifyFraction =>
                CreateGreatestCommonDivisor(scale),
            MiddleSchoolExerciseKind.Percentage =>
                CreatePercentage(scale),
            MiddleSchoolExerciseKind.PercentageChange =>
                CreatePercentageChange(scale),
            MiddleSchoolExerciseKind.Approximation =>
                CreateApproximation(scale),
            MiddleSchoolExerciseKind.Power =>
                CreatePower(definition.Id, scale),
            MiddleSchoolExerciseKind.SquareRoot =>
                CreateSquareRoot(scale),
            MiddleSchoolExerciseKind.AreaConversion =>
                Result("areaConversion", a * 10_000, a),
            MiddleSchoolExerciseKind.Volume =>
                CreateVolume(definition.Id, a, b),
            MiddleSchoolExerciseKind.VolumeConversion =>
                Result("volumeConversion", a * 1000, a),
            MiddleSchoolExerciseKind.Speed =>
                Result("speed", a * b, a * b * 3, 3),
            MiddleSchoolExerciseKind.Scale =>
                Result("scale", a * b, a, b),
            MiddleSchoolExerciseKind.Proportion =>
                Result("proportion", a * b, a, b),
            MiddleSchoolExerciseKind.TriangleArea =>
                Result("triangleArea", a * b, a * 2, b),
            MiddleSchoolExerciseKind.CircleArea =>
                CreateCircleMeasure(definition.Id, a, b),
            MiddleSchoolExerciseKind.Transformation =>
                CreateTransformation(definition.Id, a),
            MiddleSchoolExerciseKind.Pythagoras =>
                CreatePythagoras(scale),
            MiddleSchoolExerciseKind.Thales =>
                Result("thales", a * b, a, b),
            MiddleSchoolExerciseKind.Algebra =>
                Result("algebra", 3 * a + b, a, b),
            MiddleSchoolExerciseKind.Equation =>
                Result("equation", a, a + b, b),
            MiddleSchoolExerciseKind.Function =>
                CreateFunction(definition.Id, a, b),
            MiddleSchoolExerciseKind.Trigonometry =>
                Result("trigonometry", 4 * a, 5 * a, 3 * a),
            MiddleSchoolExerciseKind.Statistics =>
                CreateStatistics(definition.Id, scale),
            MiddleSchoolExerciseKind.Probability =>
                CreateProbability(scale),
            MiddleSchoolExerciseKind.Logic =>
                CreateLogic(definition.Id, a),
            MiddleSchoolExerciseKind.Coordinate =>
                Result("coordinate", a + b, a, b),
            MiddleSchoolExerciseKind.Combinatorics =>
                Result("combinatorics", a * b, a, b),
            MiddleSchoolExerciseKind.Algorithm =>
                CreateAlgorithm(definition.Id, a, b),
            _ => throw new ArgumentOutOfRangeException(nameof(definition))
        };
    }

    /// <summary>
    /// Génère un entier écrit comme puissance de dix et demande son exposant.
    /// </summary>
    private GeneratedExercise CreateScientificNotation(int scale)
    {
        var exponent = _random.Next(2, scale <= 5 ? 5 : 7);
        return Result(
            "scientificExponent",
            exponent,
            (int)Math.Pow(10, exponent));
    }

    /// <summary>
    /// Construit une division euclidienne cohérente avec quotient et reste.
    /// </summary>
    private GeneratedExercise CreateEuclideanDivision(
        int scale,
        bool askRemainder)
    {
        var divisor = _random.Next(2, Math.Min(12, scale + 2));
        var quotient = _random.Next(2, scale + 2);
        var remainder = _random.Next(0, divisor);
        var dividend = divisor * quotient + remainder;
        return askRemainder
            ? Result("remainder", remainder, dividend, divisor)
            : Result("euclideanQuotient", quotient, dividend, divisor);
    }

    /// <summary>
    /// Demande si un nombre est divisible par un diviseur donné.
    /// </summary>
    private GeneratedExercise CreateDivisibility(int scale)
    {
        var divisor = _random.Next(2, 10);
        var isDivisible = _random.Next(2) == 0;
        var number = divisor * _random.Next(2, scale + 2)
            + (isDivisible ? 0 : 1);
        return Result("divisibility", isDivisible ? 1 : 0, number, divisor);
    }

    /// <summary>
    /// Interroge le plus petit diviseur d'un nombre composé connu.
    /// </summary>
    private GeneratedExercise CreatePrimeQuestion()
    {
        int[] primes = [2, 3, 5, 7];
        var divisor = primes[_random.Next(primes.Length)];
        var number = divisor * primes[_random.Next(primes.Length)];
        return Result("smallestDivisor", SmallestDivisor(number), number);
    }

    /// <summary>
    /// Demande l'exposant de deux dans une décomposition en facteurs premiers.
    /// </summary>
    private GeneratedExercise CreatePrimeFactorization()
    {
        var exponent = _random.Next(1, 5);
        var oddFactor = _random.Next(1, 6) * 2 + 1;
        var number = (int)Math.Pow(2, exponent) * oddFactor;
        return Result("primeFactorExponent", exponent, number);
    }

    /// <summary>
    /// Génère deux entiers puis calcule leur plus petit commun multiple.
    /// </summary>
    private GeneratedExercise CreateLeastCommonMultiple(int scale)
    {
        var left = _random.Next(2, scale + 2);
        var right = _random.Next(2, scale + 2);
        var gcd = GreatestCommonDivisor(left, right);
        return Result("leastCommonMultiple", left / gcd * right, left, right);
    }

    /// <summary>
    /// Distingue une priorité implicite d'un regroupement par parenthèses.
    /// </summary>
    private static GeneratedExercise CreatePriorityCalculation(
        string courseId,
        int left,
        int right)
    {
        return courseId == "parentheses"
            ? Result("parentheses", (left + right) * 3, left, right, 3)
            : Result("operationPriority", left + right * 3, left, right, 3);
    }

    /// <summary>
    /// Adapte le calcul aux opérations réellement étudiées sur les fractions.
    /// </summary>
    private static GeneratedExercise CreateFractionOperation(
        string courseId,
        int left,
        int right,
        int denominator)
    {
        return courseId == "fraction-multiplication-division"
            ? Result(
                "fractionProductNumerator",
                left * right,
                left,
                right,
                denominator)
            : Result("fractionNumerator", left + right, left, right, denominator);
    }

    /// <summary>
    /// Retourne le plus petit diviseur supérieur ou égal à deux.
    /// </summary>
    private static int SmallestDivisor(int number)
    {
        for (var divisor = 2; divisor <= number; divisor++)
        {
            if (number % divisor == 0)
            {
                return divisor;
            }
        }

        return number;
    }

    /// <summary>
    /// Génère un PGCD non trivial pour les simplifications de fractions.
    /// </summary>
    private GeneratedExercise CreateGreatestCommonDivisor(int scale)
    {
        var factor = _random.Next(2, Math.Min(8, scale + 1));
        var left = factor * _random.Next(2, scale + 2);
        var right = factor * _random.Next(2, scale + 2);
        return Result("greatestCommonDivisor", GreatestCommonDivisor(left, right), left, right);
    }

    /// <summary>
    /// Calcule le plus grand commun diviseur par l'algorithme d'Euclide.
    /// </summary>
    private static int GreatestCommonDivisor(int left, int right)
    {
        while (right != 0)
        {
            (left, right) = (right, left % right);
        }

        return Math.Abs(left);
    }

    /// <summary>
    /// Utilise des pourcentages simples afin de conserver une réponse entière.
    /// </summary>
    private GeneratedExercise CreatePercentage(int scale)
    {
        int[] percentages = [10, 20, 25, 50];
        var percentage = percentages[_random.Next(percentages.Length)];
        var amount = _random.Next(1, scale + 2) * 20;
        return Result("percentage", amount * percentage / 100, percentage, amount);
    }

    /// <summary>
    /// Génère une augmentation en pourcentage dont le résultat reste entier.
    /// </summary>
    private GeneratedExercise CreatePercentageChange(int scale)
    {
        int[] percentages = [10, 20, 25, 50];
        var percentage = percentages[_random.Next(percentages.Length)];
        var amount = _random.Next(1, scale + 2) * 20;
        return Result(
            "percentageIncrease",
            amount + amount * percentage / 100,
            amount,
            percentage);
    }

    /// <summary>
    /// Demande l'arrondi d'un entier à la dizaine la plus proche.
    /// </summary>
    private GeneratedExercise CreateApproximation(int scale)
    {
        var value = _random.Next(11, scale * 20 + 11);
        var rounded = (int)(Math.Round(
            value / 10d,
            MidpointRounding.AwayFromZero) * 10);
        return Result("roundToTen", rounded, value);
    }

    /// <summary>
    /// Limite les exposants afin d'éviter les dépassements d'entier.
    /// </summary>
    private GeneratedExercise CreatePower(string courseId, int scale)
    {
        if (courseId == "power-rules")
        {
            var leftExponent = _random.Next(2, 6);
            var rightExponent = _random.Next(2, 6);
            return Result(
                "powerProductExponent",
                leftExponent + rightExponent,
                leftExponent,
                rightExponent);
        }

        var exponent = _random.Next(2, scale <= 5 ? 4 : 5);
        exponent = courseId switch
        {
            "squares" => 2,
            "cubes" => 3,
            _ => exponent
        };
        var basis = _random.Next(2, scale <= 5 ? 6 : 9);
        return Result("power", (int)Math.Pow(basis, exponent), basis, exponent);
    }

    /// <summary>
    /// Construit un carré parfait puis demande sa racine positive.
    /// </summary>
    private GeneratedExercise CreateSquareRoot(int scale)
    {
        var root = _random.Next(2, scale + 2);
        return Result("squareRoot", root, root * root);
    }

    /// <summary>
    /// Adapte la propriété numérique à la transformation étudiée.
    /// </summary>
    private static GeneratedExercise CreateTransformation(
        string courseId,
        int value)
    {
        return courseId switch
        {
            "central-symmetry" => value % 2 == 0
                ? Result("centralSymmetry", 2)
                : Result("centralSymmetryAngle", 180),
            "rotations" => value % 2 == 0
                ? Result("fullTurn", 360)
                : Result("halfTurn", 180),
            "homothety" or "enlargement-and-reduction" =>
                Result("enlargement", value * 2, value),
            _ => Result("translation", value + 3, value, 3)
        };
    }

    /// <summary>
    /// Adapte la formule du volume au solide étudié.
    /// </summary>
    private static GeneratedExercise CreateVolume(
        string courseId,
        int first,
        int second)
    {
        return courseId == "pyramid-volume"
            ? Result("pyramidVolume", first * second, first * 3, second)
            : Result("volume", first * second * 3, first, second, 3);
    }

    /// <summary>
    /// Demande le coefficient de pi dans une aire ou un volume de révolution.
    /// </summary>
    private static GeneratedExercise CreateCircleMeasure(
        string courseId,
        int radius,
        int height)
    {
        return courseId switch
        {
            "cylinder-volume" =>
                Result("cylinderVolume", radius * radius * height, radius, height),
            "cone-volume" =>
                Result("coneVolume", radius * radius * height, radius, height * 3),
            "sphere-volume" => CreateSphereVolume(radius),
            _ => Result("circleArea", radius * radius, radius)
        };
    }

    /// <summary>
    /// Choisit un rayon multiple de trois afin d'obtenir un coefficient entier.
    /// </summary>
    private static GeneratedExercise CreateSphereVolume(int seed)
    {
        var radius = 3 * (seed % 3 + 1);
        return Result(
            "sphereVolume",
            4 * radius * radius * radius / 3,
            radius);
    }

    /// <summary>
    /// Utilise un triplet de Pythagore agrandi pour demander l'hypoténuse.
    /// </summary>
    private GeneratedExercise CreatePythagoras(int scale)
    {
        var factor = _random.Next(1, Math.Max(2, scale / 3));
        return Result("pythagoras", 5 * factor, 3 * factor, 4 * factor);
    }

    /// <summary>
    /// Adapte la lecture ou le calcul à la représentation de fonction étudiée.
    /// </summary>
    private static GeneratedExercise CreateFunction(
        string courseId,
        int input,
        int constant)
    {
        return courseId switch
        {
            "read-a-graph" =>
                Result("graphOrdinate", constant, input, constant),
            "image-and-preimage" =>
                Result("functionPreimage", input, 2 * input + constant, constant),
            "linear-function" =>
                Result("linearFunction", 3 * input, input),
            _ => Result("function", 2 * input + constant, input, constant)
        };
    }

    /// <summary>
    /// Varie le calcul statistique selon la notion sélectionnée.
    /// </summary>
    private GeneratedExercise CreateStatistics(string courseId, int scale)
    {
        var first = _random.Next(1, scale + 2);
        var second = _random.Next(first, first + scale + 2);
        var third = _random.Next(second, second + scale + 2);

        return courseId switch
        {
            "mean" => Result("mean", second, first, second, third, 3 * second - first - third),
            "median" => Result("median", second, first, second, third),
            "range-and-dispersion" => Result("range", third - first, first, second, third),
            "quartiles" => Result("firstQuartile", first, first, second, third, third + 2),
            _ => Result("totalCount", first + second + third, first, second, third)
        };
    }

    /// <summary>
    /// Exprime la probabilité demandée en pourcentage entier.
    /// </summary>
    private GeneratedExercise CreateProbability(int scale)
    {
        int[] totals = [2, 4, 5, 10];
        var total = totals[_random.Next(totals.Length)];
        var favorable = _random.Next(1, total + 1);
        return Result(
            "probabilityPercent",
            favorable * 100 / total,
            favorable,
            total);
    }

    /// <summary>
    /// Utilise 1 pour vrai et 0 pour faux dans les exercices logiques.
    /// </summary>
    private static GeneratedExercise CreateLogic(string courseId, int value)
    {
        return courseId == "truth-and-counterexamples"
            ? Result("isEven", value % 2 == 0 ? 1 : 0, value)
            : Result("trueStatement", 1, value, value + 1);
    }

    /// <summary>
    /// Simule une variable, une condition ou une boucle courte.
    /// </summary>
    private static GeneratedExercise CreateAlgorithm(
        string courseId,
        int first,
        int second)
    {
        return courseId switch
        {
            "algorithm-conditions" =>
                Result("condition", first > second ? first : second, first, second),
            "algorithm-loops" =>
                Result("loop", first * second, first, second),
            _ => Result("variable", first + second, first, second)
        };
    }

    /// <summary>
    /// Compare deux questions paramétrées.
    /// </summary>
    private static bool HasSameQuestion(Exercise left, Exercise right)
    {
        return left.QuestionKey == right.QuestionKey
            && (left.QuestionArguments ?? [])
                .SequenceEqual(right.QuestionArguments ?? []);
    }

    /// <summary>
    /// Construit le résultat intermédiaire d'une question.
    /// </summary>
    private static GeneratedExercise Result(
        string key,
        int answer,
        params int[] arguments)
    {
        return new GeneratedExercise(
            key,
            arguments.Select(value => value.ToString()).ToArray(),
            answer);
    }

    /// <summary>
    /// Porte la clé, les paramètres et la réponse avant création du modèle public.
    /// </summary>
    private sealed record GeneratedExercise(
        string Key,
        IReadOnlyList<string> Arguments,
        int Answer);
}
