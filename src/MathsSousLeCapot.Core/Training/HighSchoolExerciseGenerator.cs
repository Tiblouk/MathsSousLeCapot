using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère les exercices des cours du lycée.
/// </summary>
public sealed class HighSchoolExerciseGenerator
{
    /// <summary>
    /// Source aléatoire injectable afin de rendre les tests reproductibles.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Initialise le générateur avec une source aléatoire facultative.
    /// </summary>
    public HighSchoolExerciseGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>
    /// Crée une session de cinq exercices pour une notion.
    /// </summary>
    public IReadOnlyList<Exercise> CreateSession(
        HighSchoolCourseDefinition definition,
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
        HighSchoolCourseDefinition definition,
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
        HighSchoolCourseDefinition definition,
        TrainingDifficulty difficulty,
        int index = 0)
    {
        var scale = difficulty switch
        {
            TrainingDifficulty.Easy => 4,
            TrainingDifficulty.Moderate => 8,
            TrainingDifficulty.Hard => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
        var generated = Generate(definition.ExerciseKind, scale);

        return new Exercise(
            $"{definition.Id}-{difficulty}-{index}-{Guid.NewGuid():N}",
            definition.Id,
            string.Empty,
            generated.Answer.ToString(),
            string.Empty,
            $"exercise.high.{generated.Key}",
            generated.Arguments,
            "exercise.high.explanation",
            [generated.Answer.ToString()]);
    }

    /// <summary>
    /// Produit une question et sa réponse entière, sans dépendance à l'interface.
    /// </summary>
    private GeneratedExercise Generate(
        HighSchoolExerciseKind kind,
        int scale)
    {
        var a = _random.Next(2, scale + 3);
        var b = _random.Next(2, scale + 3);

        return kind switch
        {
            HighSchoolExerciseKind.IntervalMembership =>
                CreateIntervalMembership(scale),
            HighSchoolExerciseKind.Divisibility =>
                CreateDivisibility(scale),
            HighSchoolExerciseKind.SquareRootDistance =>
                Result("absoluteDistance", Math.Abs(a - b), a, b),
            HighSchoolExerciseKind.AlgebraExpansion =>
                Result("expandedConstant", a * b, a, b),
            HighSchoolExerciseKind.VectorCoordinates =>
                Result("vectorAbscissa", b - a, a, b),
            HighSchoolExerciseKind.LineSlope =>
                Result("lineSlope", a, 0, b, 1, b + a),
            HighSchoolExerciseKind.FunctionImage =>
                Result("functionImage", 2 * a + b, a, b),
            HighSchoolExerciseKind.FunctionVariation =>
                CreateFunctionVariation(scale),
            HighSchoolExerciseKind.StatisticsMean =>
                Result("mean", b, b - a, b, b + a),
            HighSchoolExerciseKind.SimpleProbability =>
                CreateProbabilityPercent(),
            HighSchoolExerciseKind.AlgorithmLoop =>
                Result("loop", a * b, a, b),
            HighSchoolExerciseKind.AlgorithmList =>
                Result("listSum", 2 * (a + b), a, b, a + b),
            HighSchoolExerciseKind.BaseConversion =>
                CreateBinaryConversion(),
            HighSchoolExerciseKind.ArithmeticSequenceTerm =>
                Result("arithmeticSequence", a + b * 5, a, b, 5),
            HighSchoolExerciseKind.GeometricSequenceTerm =>
                Result("geometricSequence", a * (int)Math.Pow(b, 3), a, b, 3),
            HighSchoolExerciseKind.QuadraticDiscriminant =>
                Result("discriminant", b * b - 4 * a, a, b, 1),
            HighSchoolExerciseKind.QuadraticRootSum =>
                Result("rootSum", a + b, a, b),
            HighSchoolExerciseKind.DerivativePower =>
                Result("derivativePowerCoefficient", a * b, a, b, b - 1),
            HighSchoolExerciseKind.TangentSlope =>
                Result("tangentSlopeSquare", 2 * a, a),
            HighSchoolExerciseKind.ExponentialRule =>
                Result("exponentialSum", a + b, a, b),
            HighSchoolExerciseKind.TrigonometricValue =>
                CreateTrigonometricValue(),
            HighSchoolExerciseKind.DotProduct =>
                Result("dotProduct", a * b + (a + 1) * (b + 1), a, a + 1, b, b + 1),
            HighSchoolExerciseKind.Distance =>
                Result("distanceHorizontal", Math.Abs(b - a), a, b),
            HighSchoolExerciseKind.ConditionalProbability =>
                CreateConditionalProbability(),
            HighSchoolExerciseKind.ExpectedValue =>
                Result("expectedValue", b, b - a, b + a),
            HighSchoolExerciseKind.Combination =>
                CreateCombination(scale),
            HighSchoolExerciseKind.SpaceVectorCoordinate =>
                Result("spaceVectorZ", b - a, a, b),
            HighSchoolExerciseKind.SpacePlaneNormal =>
                CreatePlaneNormal(scale),
            HighSchoolExerciseKind.SpaceDotProduct =>
                CreateSpaceDotProduct(scale),
            HighSchoolExerciseKind.GeometricSequenceLimit =>
                CreateGeometricSequenceLimit(),
            HighSchoolExerciseKind.Induction =>
                Result("inductionInitial", a, a, b),
            HighSchoolExerciseKind.FunctionLimit =>
                Result("functionLimitAtInfinity", 0, a),
            HighSchoolExerciseKind.Continuity =>
                CreateContinuity(scale),
            HighSchoolExerciseKind.LogarithmRule =>
                Result("logarithmProduct", a + b, a, b),
            HighSchoolExerciseKind.IntegralPower =>
                Result("integralTwoX", a * a, a),
            HighSchoolExerciseKind.DifferentialEquation =>
                Result("differentialExponent", a, a),
            HighSchoolExerciseKind.BinomialExpectation =>
                CreateBinomialExpectation(scale),
            HighSchoolExerciseKind.PoissonExpectation =>
                CreateDistributionExpectation(scale),
            HighSchoolExerciseKind.VarianceSum =>
                Result("varianceSum", a + b, a, b),
            HighSchoolExerciseKind.LargeNumbers =>
                CreateLargeNumbersStatement(),
            HighSchoolExerciseKind.NumericalMethod =>
                CreateIntervalMidpoint(scale),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    /// <summary>
    /// Fait varier le signe du coefficient directeur d'une fonction affine.
    /// </summary>
    private GeneratedExercise CreateFunctionVariation(int scale)
    {
        var magnitude = _random.Next(1, scale + 3);
        var coefficient = _random.Next(2) == 0 ? magnitude : -magnitude;
        return Result(
            "affineIncreasing",
            coefficient > 0 ? 1 : 0,
            coefficient);
    }

    /// <summary>
    /// Choisit une valeur remarquable entière de sinus ou de cosinus.
    /// </summary>
    private GeneratedExercise CreateTrigonometricValue()
    {
        (string Function, string Angle, int Value)[] values =
        [
            ("sin", "0", 0),
            ("sin", "π/2", 1),
            ("sin", "3π/2", -1),
            ("cos", "0", 1),
            ("cos", "π/2", 0),
            ("cos", "π", -1)
        ];
        var selected = values[_random.Next(values.Length)];
        return Result(
            "trigonometricValue",
            selected.Value,
            selected.Function,
            selected.Angle);
    }

    /// <summary>
    /// Demande une coordonnée du vecteur normal associé à un plan.
    /// </summary>
    private GeneratedExercise CreatePlaneNormal(int scale)
    {
        var x = _random.Next(1, scale + 3);
        var y = _random.Next(1, scale + 3);
        var z = _random.Next(1, scale + 3);
        var constant = _random.Next(-scale, scale + 1);
        return Result("planeNormalZ", z, x, y, z, constant);
    }

    /// <summary>
    /// Calcule un produit scalaire avec les trois coordonnées de l'espace.
    /// </summary>
    private GeneratedExercise CreateSpaceDotProduct(int scale)
    {
        var ux = _random.Next(1, scale + 2);
        var uy = _random.Next(1, scale + 2);
        var uz = _random.Next(1, scale + 2);
        var vx = _random.Next(1, scale + 2);
        var vy = _random.Next(1, scale + 2);
        var vz = _random.Next(1, scale + 2);
        return Result(
            "spaceDotProduct",
            ux * vx + uy * vy + uz * vz,
            ux,
            uy,
            uz,
            vx,
            vy,
            vz);
    }

    /// <summary>
    /// Fait varier la raison d'une suite géométrique convergeant vers zéro.
    /// </summary>
    private GeneratedExercise CreateGeometricSequenceLimit()
    {
        string[] ratios = ["1/2", "-1/2", "1/3", "-1/4"];
        var ratio = ratios[_random.Next(ratios.Length)];
        return Result("geometricLimitRatio", 0, ratio);
    }

    /// <summary>
    /// Vérifie si le changement de signe permet d'appliquer le théorème des valeurs intermédiaires.
    /// </summary>
    private GeneratedExercise CreateContinuity(int scale)
    {
        var leftValue = -_random.Next(1, scale + 2);
        var hasSignChange = _random.Next(2) == 0;
        var rightMagnitude = _random.Next(1, scale + 2);
        var rightValue = hasSignChange ? rightMagnitude : -rightMagnitude;
        return Result(
            "intermediateValueExistence",
            hasSignChange ? 1 : 0,
            leftValue,
            rightValue);
    }

    /// <summary>
    /// Produit des paramètres binomiaux valides avec une espérance entière.
    /// </summary>
    private GeneratedExercise CreateBinomialExpectation(int scale)
    {
        int[] denominators = [2, 4, 5, 10];
        var denominator = denominators[_random.Next(denominators.Length)];
        var numerator = _random.Next(1, denominator);
        var trials = denominator * _random.Next(2, scale + 3);
        return Result(
            "binomialExpectationFraction",
            trials * numerator / denominator,
            trials,
            numerator,
            denominator);
    }

    /// <summary>
    /// Alterne entre les espérances des lois de Poisson et géométrique.
    /// </summary>
    private GeneratedExercise CreateDistributionExpectation(int scale)
    {
        var parameter = _random.Next(2, scale + 3);
        return _random.Next(2) == 0
            ? Result("poissonExpectation", parameter, parameter)
            : Result("geometricExpectation", parameter, parameter);
    }

    /// <summary>
    /// Varie les affirmations utilisées pour vérifier la loi des grands nombres.
    /// </summary>
    private GeneratedExercise CreateLargeNumbersStatement()
    {
        return _random.Next(2) == 0
            ? Result("largeNumbersStabilization", 1)
            : Result("largeNumbersExact", 0);
    }

    /// <summary>
    /// Construit un intervalle dont le milieu est un entier exact.
    /// </summary>
    private GeneratedExercise CreateIntervalMidpoint(int scale)
    {
        var start = _random.Next(-scale, scale + 1);
        var halfWidth = _random.Next(1, scale + 2);
        var end = start + 2 * halfWidth;
        return Result("intervalMidpoint", start + halfWidth, start, end);
    }

    /// <summary>
    /// Demande si un nombre appartient à un intervalle fermé.
    /// </summary>
    private GeneratedExercise CreateIntervalMembership(int scale)
    {
        var start = _random.Next(-scale, 1);
        var end = _random.Next(2, scale + 5);
        var value = _random.Next(start - 2, end + 3);
        return Result(
            "intervalMembership",
            value >= start && value <= end ? 1 : 0,
            value,
            start,
            end);
    }

    /// <summary>
    /// Génère une question de divisibilité avec réponse booléenne.
    /// </summary>
    private GeneratedExercise CreateDivisibility(int scale)
    {
        var divisor = _random.Next(2, 10);
        var isDivisible = _random.Next(2) == 0;
        var number = divisor * _random.Next(2, scale + 3)
            + (isDivisible ? 0 : 1);
        return Result("divisibility", isDivisible ? 1 : 0, number, divisor);
    }

    /// <summary>
    /// Exprime une probabilité simple en pourcentage entier.
    /// </summary>
    private GeneratedExercise CreateProbabilityPercent()
    {
        int[] totals = [4, 5, 10];
        var total = totals[_random.Next(totals.Length)];
        var favorable = _random.Next(1, total + 1);
        return Result(
            "probabilityPercent",
            favorable * 100 / total,
            favorable,
            total);
    }

    /// <summary>
    /// Demande la valeur en base dix d'une écriture binaire courte.
    /// </summary>
    private GeneratedExercise CreateBinaryConversion()
    {
        (string Text, int Value)[] values =
        [
            ("101", 5),
            ("110", 6),
            ("1001", 9),
            ("1010", 10),
            ("1100", 12)
        ];
        var selected = values[_random.Next(values.Length)];
        return Result("binaryConversion", selected.Value, selected.Text);
    }

    /// <summary>
    /// Génère une probabilité conditionnelle entière en pourcentage.
    /// </summary>
    private GeneratedExercise CreateConditionalProbability()
    {
        var totalA = _random.Next(4, 11);
        var intersection = _random.Next(1, totalA + 1);
        return Result(
            "conditionalProbability",
            intersection * 100 / totalA,
            intersection,
            totalA);
    }

    /// <summary>
    /// Calcule un coefficient binomial sur de petites valeurs.
    /// </summary>
    private GeneratedExercise CreateCombination(int scale)
    {
        var n = _random.Next(4, scale + 6);
        var k = _random.Next(1, Math.Min(4, n));
        return Result("combination", Combination(n, k), n, k);
    }

    /// <summary>
    /// Calcule C(n,k) par produit entier pour éviter les grands factoriels.
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
        params object[] arguments)
    {
        return new GeneratedExercise(
            key,
            arguments.Select(value => value.ToString() ?? string.Empty).ToArray(),
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
