using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère des exercices numériques adaptés aux notions du niveau primaire.
/// </summary>
public sealed class PrimaryExerciseGenerator
{
    /// <summary>
    /// Source aléatoire injectable pour rendre les tests reproductibles.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Initialise le générateur avec une source aléatoire facultative.
    /// </summary>
    public PrimaryExerciseGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>
    /// Crée une session de cinq exercices pour la notion demandée.
    /// </summary>
    public IReadOnlyList<Exercise> CreateSession(
        PrimaryCourseDefinition definition,
        TrainingDifficulty difficulty)
    {
        return Enumerable.Range(0, 5)
            .Select(index => CreateExercise(definition, difficulty, index))
            .ToArray();
    }

    /// <summary>
    /// Crée deux questions finales distinctes autant que la notion le permet.
    /// </summary>
    public IReadOnlyList<Exercise> CreateValidation(
        PrimaryCourseDefinition definition,
        TrainingDifficulty difficulty)
    {
        var first = CreateExercise(definition, difficulty, 0);
        var second = CreateExercise(definition, difficulty, 1);
        var attempts = 0;

        while (HasSameQuestion(first, second) && attempts < 20)
        {
            second = CreateExercise(definition, difficulty, 1);
            attempts++;
        }

        return [first, second];
    }

    /// <summary>
    /// Compare le texte paramétré de deux questions sans tenir compte de leur identifiant.
    /// </summary>
    private static bool HasSameQuestion(Exercise left, Exercise right)
    {
        return left.QuestionKey == right.QuestionKey
            && (left.QuestionArguments ?? [])
                .SequenceEqual(right.QuestionArguments ?? []);
    }

    /// <summary>
    /// Crée un exercice déterministe utilisable comme démonstration ou question finale.
    /// </summary>
    public Exercise CreateExercise(
        PrimaryCourseDefinition definition,
        TrainingDifficulty difficulty,
        int index = 0)
    {
        var scale = difficulty switch
        {
            TrainingDifficulty.Easy => 5,
            TrainingDifficulty.Moderate => 12,
            TrainingDifficulty.Hard => 30,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
        var generated = Generate(definition.ExerciseKind, scale);

        return new Exercise(
            $"{definition.Id}-{difficulty}-{index}-{Guid.NewGuid():N}",
            definition.Id,
            string.Empty,
            generated.Answer.ToString(),
            string.Empty,
            $"exercise.primary.{generated.Key}",
            generated.Arguments,
            "exercise.primary.explanation",
            [generated.Answer.ToString()],
            generated.AnswerKind,
            generated.AnswerArguments,
            CreateWrittenCalculation(definition.ExerciseKind, generated));
    }

    /// <summary>
    /// Associe une disposition posée aux exercices des quatre opérations.
    /// </summary>
    private static WrittenCalculation? CreateWrittenCalculation(
        PrimaryExerciseKind kind,
        GeneratedExercise generated)
    {
        var values = generated.Arguments.Select(int.Parse).ToArray();

        return kind switch
        {
            PrimaryExerciseKind.Multiplication
                or PrimaryExerciseKind.MultiplicationTable
                or PrimaryExerciseKind.WrittenMultiplication =>
                WrittenCalculationBuilder.Create(
                    WrittenCalculationKind.Multiplication,
                    values[0],
                    values[1]),
            PrimaryExerciseKind.ExactDivision =>
                WrittenCalculationBuilder.Create(
                    WrittenCalculationKind.Division,
                    values[0],
                    values[1]),
            PrimaryExerciseKind.Addition =>
                WrittenCalculationBuilder.Create(
                    WrittenCalculationKind.Addition,
                    values[0],
                    values[1]),
            PrimaryExerciseKind.Subtraction =>
                WrittenCalculationBuilder.Create(
                    WrittenCalculationKind.Subtraction,
                    values[0],
                    values[1]),
            _ => null
        };
    }

    /// <summary>
    /// Produit les paramètres et le résultat associés à une famille d'exercices.
    /// </summary>
    private GeneratedExercise Generate(PrimaryExerciseKind kind, int scale)
    {
        var a = _random.Next(1, scale + 1);
        var b = _random.Next(1, scale + 1);

        return kind switch
        {
            PrimaryExerciseKind.CompareNumbers => CreateComparison(scale),
            PrimaryExerciseKind.PlaceValue => CreatePlaceValue(scale),
            PrimaryExerciseKind.RoundToTen => CreateRounding(scale),
            PrimaryExerciseKind.FractionOfSet => CreateFraction(scale),
            PrimaryExerciseKind.Multiplication =>
                Result("multiplication", a * b, a, b),
            PrimaryExerciseKind.ExactDivision => CreateDivision(scale),
            PrimaryExerciseKind.MultiplicationTable =>
                CreateMultiplicationTable(scale),
            PrimaryExerciseKind.WrittenMultiplication =>
                CreateWrittenMultiplication(scale),
            PrimaryExerciseKind.Addition =>
                Result("addition", a * scale + b, a * scale, b),
            PrimaryExerciseKind.Subtraction => CreateSubtraction(scale),
            PrimaryExerciseKind.MentalCalculation =>
                CreateMentalCalculation(scale),
            PrimaryExerciseKind.RepeatedMultiplication =>
                CreateRepeatedMultiplication(scale),
            PrimaryExerciseKind.LengthReading =>
                CreateLengthReading(scale),
            PrimaryExerciseKind.LengthConversion =>
                Result("lengthConversion", a * 100, a),
            PrimaryExerciseKind.MassReading =>
                Result("massReading", a * 100 + b * 10, a * 100, b * 10),
            PrimaryExerciseKind.MassConversion =>
                Result("massConversion", Math.Min(a, 9) * 1000, Math.Min(a, 9)),
            PrimaryExerciseKind.TimeReading =>
                Result("timeReading", a * 60, a),
            PrimaryExerciseKind.TimeCalculation =>
                Result("timeCalculation", a * 10 + b * 5, a * 10, b * 5),
            PrimaryExerciseKind.UnitSquareArea =>
                Result("unitSquareArea", a * b, a, b),
            PrimaryExerciseKind.RectangleArea =>
                Result("rectangleArea", a * b, a, b),
            PrimaryExerciseKind.Proportionality =>
                Result("proportionality", a * b, a, b),
            PrimaryExerciseKind.SegmentEndpoints =>
                CreateSegmentProperty(),
            PrimaryExerciseKind.ParallelIntersections =>
                CreateLineRelationship(),
            PrimaryExerciseKind.RightAngle =>
                CreateBasicAngle(),
            PrimaryExerciseKind.AngleReading => CreateAngle(),
            PrimaryExerciseKind.TriangleSides =>
                CreateTriangleProperty(),
            PrimaryExerciseKind.QuadrilateralSides =>
                CreateQuadrilateralProperty(),
            PrimaryExerciseKind.PolygonSides => CreatePolygon(),
            PrimaryExerciseKind.CircleDiameter =>
                Result("circleDiameter", a * 2, a),
            PrimaryExerciseKind.RectanglePerimeter =>
                Result("rectanglePerimeter", 2 * (a + b), a, b),
            PrimaryExerciseKind.SymmetryAxes =>
                CreateSymmetry(),
            PrimaryExerciseKind.SolidFaces =>
                CreateSolidProperty(),
            PrimaryExerciseKind.CubeNetFaces =>
                CreateCubeNetProperty(),
            PrimaryExerciseKind.CuboidVolume =>
                Result("cuboidVolume", a * b * Math.Max(1, scale / 3), a, b, Math.Max(1, scale / 3)),
            PrimaryExerciseKind.DataTableTotal =>
                Result("dataTotal", a + b + scale, a, b, scale),
            PrimaryExerciseKind.ChartMaximum => CreateChartMaximum(scale),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    private GeneratedExercise CreatePlaceValue(int scale)
    {
        var highestPlace = scale <= 12 ? 100 : 1000;
        var places = highestPlace == 100
            ? new[] { 1, 10, 100 }
            : new[] { 1, 10, 100, 1000 };
        var place = places[_random.Next(places.Length)];
        var digit = _random.Next(1, 10);
        var number = 0;
        foreach (var currentPlace in places)
        {
            var currentDigit = currentPlace == place
                ? digit
                : NextDifferentDigit(digit, currentPlace == highestPlace);
            number += currentDigit * currentPlace;
        }

        return Result(
            "placeValue",
            digit * place,
            ExerciseAnswerKind.PlaceValue,
            [place.ToString()],
            number,
            digit);
    }

    /// <summary>
    /// Évite qu'un même chiffre apparaisse à plusieurs positions dans la question.
    /// </summary>
    private int NextDifferentDigit(int excludedDigit, bool nonZero)
    {
        int value;
        do
        {
            value = _random.Next(nonZero ? 1 : 0, 10);
        }
        while (value == excludedDigit);

        return value;
    }

    /// <summary>
    /// Produit deux nombres distincts pour que la comparaison soit sans ambiguïté.
    /// </summary>
    private GeneratedExercise CreateComparison(int scale)
    {
        var left = _random.Next(1, scale * 10 + 1);
        var right = _random.Next(1, scale * 10 + 1);
        while (right == left)
        {
            right = _random.Next(1, scale * 10 + 1);
        }

        return Result("largerNumber", Math.Max(left, right), left, right);
    }

    /// <summary>
    /// Garde les tables dans leur domaine usuel tout en augmentant progressivement la borne.
    /// </summary>
    private GeneratedExercise CreateMultiplicationTable(int scale)
    {
        var maximum = scale <= 5 ? 5 : scale <= 12 ? 10 : 12;
        var left = _random.Next(1, maximum + 1);
        var right = _random.Next(1, maximum + 1);
        return Result("multiplication", left * right, left, right);
    }

    /// <summary>
    /// Génère de vrais facteurs à plusieurs chiffres pour la multiplication posée.
    /// </summary>
    private GeneratedExercise CreateWrittenMultiplication(int scale)
    {
        var left = _random.Next(12, scale * 10 + 13);
        var right = _random.Next(2, Math.Min(20, scale + 2));
        return Result("multiplication", left * right, left, right);
    }

    /// <summary>
    /// Alterne addition et soustraction autour de dizaines faciles à transformer mentalement.
    /// </summary>
    private GeneratedExercise CreateMentalCalculation(int scale)
    {
        var tens = _random.Next(1, scale + 1) * 10;
        var adjustment = _random.Next(1, 10);
        return _random.Next(2) == 0
            ? Result("addition", tens + adjustment, tens, adjustment)
            : Result("mentalSubtraction", tens - adjustment, tens, adjustment);
    }

    private GeneratedExercise CreateRounding(int scale)
    {
        var value = _random.Next(1, scale * 10 + 1);
        var rounded = (int)(Math.Round(value / 10d, MidpointRounding.AwayFromZero) * 10);
        return Result("roundToTen", rounded, value);
    }

    private GeneratedExercise CreateFraction(int scale)
    {
        var denominator = _random.Next(2, Math.Min(6, scale) + 1);
        var numerator = _random.Next(1, denominator);
        var unit = _random.Next(1, scale + 1);
        var total = denominator * unit;
        return Result("fractionOfSet", numerator * unit, numerator, denominator, total);
    }

    private GeneratedExercise CreateDivision(int scale)
    {
        var divisor = _random.Next(1, scale + 1);
        var quotient = _random.Next(1, scale + 1);
        return Result("division", quotient, divisor * quotient, divisor);
    }

    private GeneratedExercise CreateSubtraction(int scale)
    {
        var left = _random.Next(scale, scale * 10 + 1);
        var right = _random.Next(0, left + 1);
        return Result("subtraction", left - right, left, right);
    }

    private GeneratedExercise CreateAngle()
    {
        int[] angles = [30, 45, 60, 90, 120];
        var angle = angles[_random.Next(angles.Length)];
        return Result("angleReading", angle, angle);
    }

    /// <summary>
    /// Varie le nombre de facteurs tout en conservant une multiplication répétée.
    /// </summary>
    private GeneratedExercise CreateRepeatedMultiplication(int scale)
    {
        var repetitions = _random.Next(2, 5);
        var maximumFactor = repetitions == 4 ? 9 : Math.Min(10, scale);
        var factor = _random.Next(2, maximumFactor + 1);
        var answer = (int)Math.Pow(factor, repetitions);

        return repetitions switch
        {
            2 => Result("repeatedMultiplication2", answer, factor),
            3 => Result("repeatedMultiplication3", answer, factor),
            _ => Result("repeatedMultiplication4", answer, factor)
        };
    }

    /// <summary>
    /// Mesure l'écart entre deux graduations plutôt que de répéter une valeur donnée.
    /// </summary>
    private GeneratedExercise CreateLengthReading(int scale)
    {
        var start = _random.Next(0, scale);
        var length = _random.Next(1, scale + 1);
        return Result("lengthReading", length, start, start + length);
    }

    /// <summary>
    /// Alterne entre les propriétés des droites parallèles et perpendiculaires.
    /// </summary>
    private GeneratedExercise CreateLineRelationship()
    {
        return _random.Next(2) == 0
            ? Result("parallelIntersections", 0)
            : Result("perpendicularAngle", 90);
    }

    /// <summary>
    /// Fait reconnaître un angle droit ou un angle plat.
    /// </summary>
    private GeneratedExercise CreateBasicAngle()
    {
        return _random.Next(2) == 0
            ? Result("rightAngle", 90)
            : Result("straightAngle", 180);
    }

    /// <summary>
    /// Compare les axes de symétrie d'un carré et d'un rectangle non carré.
    /// </summary>
    private GeneratedExercise CreateSymmetry()
    {
        return _random.Next(2) == 0
            ? Result("squareSymmetryAxes", 4)
            : Result("rectangleSymmetryAxes", 2);
    }

    /// <summary>
    /// Interroge successivement les faces, sommets ou arêtes d'un cube.
    /// </summary>
    private GeneratedExercise CreateSolidProperty()
    {
        return _random.Next(3) switch
        {
            0 => Result("cubeFaces", 6),
            1 => Result("cubeVertices", 8),
            _ => Result("cubeEdges", 12)
        };
    }

    /// <summary>
    /// Varie les propriétés fondamentales d'un segment.
    /// </summary>
    private GeneratedExercise CreateSegmentProperty()
    {
        return _random.Next(2) == 0
            ? Result("segmentEndpoints", 2)
            : Result("segmentBetweenPoints", 1);
    }

    /// <summary>
    /// Alterne côtés et sommets du triangle.
    /// </summary>
    private GeneratedExercise CreateTriangleProperty()
    {
        return _random.Next(2) == 0
            ? Result("triangleSides", 3)
            : Result("triangleVertices", 3);
    }

    /// <summary>
    /// Alterne côtés et sommets du quadrilatère.
    /// </summary>
    private GeneratedExercise CreateQuadrilateralProperty()
    {
        return _random.Next(2) == 0
            ? Result("quadrilateralSides", 4)
            : Result("quadrilateralVertices", 4);
    }

    /// <summary>
    /// Demande le nombre de côtés d'un polygone nommé sans donner la réponse dans l'énoncé.
    /// </summary>
    private GeneratedExercise CreatePolygon()
    {
        return _random.Next(3) switch
        {
            0 => Result("pentagonSides", 5),
            1 => Result("hexagonSides", 6),
            _ => Result("octagonSides", 8)
        };
    }

    /// <summary>
    /// Varie la lecture d'un patron de cube sans changer la propriété étudiée.
    /// </summary>
    private GeneratedExercise CreateCubeNetProperty()
    {
        return _random.Next(2) == 0
            ? Result("cubeNetFaces", 6)
            : Result("cubeNetSquares", 6);
    }

    /// <summary>
    /// Place la valeur maximale à une position aléatoire dans la série.
    /// </summary>
    private GeneratedExercise CreateChartMaximum(int scale)
    {
        var first = _random.Next(1, scale + 1);
        var second = _random.Next(1, scale + 1);
        var third = _random.Next(1, scale + 1);
        return Result(
            "chartMaximum",
            Math.Max(first, Math.Max(second, third)),
            first,
            second,
            third);
    }

    private static GeneratedExercise Result(string key, int answer, params int[] arguments)
    {
        return Result(
            key,
            answer,
            ExerciseAnswerKind.Numeric,
            null,
            arguments);
    }

    /// <summary>
    /// Construit un résultat qui porte un contrat de validation spécialisé.
    /// </summary>
    private static GeneratedExercise Result(
        string key,
        int answer,
        ExerciseAnswerKind answerKind,
        IReadOnlyList<string>? answerArguments,
        params int[] arguments)
    {
        return new GeneratedExercise(
            key,
            arguments.Select(value => value.ToString()).ToArray(),
            answer,
            answerKind,
            answerArguments);
    }

    /// <summary>
    /// Résultat intermédiaire avant la création du modèle d'exercice.
    /// </summary>
    private sealed record GeneratedExercise(
        string Key,
        IReadOnlyList<string> Arguments,
        int Answer,
        ExerciseAnswerKind AnswerKind,
        IReadOnlyList<string>? AnswerArguments);
}
