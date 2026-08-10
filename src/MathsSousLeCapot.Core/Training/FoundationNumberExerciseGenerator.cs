using System.Globalization;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère les exercices des cours fondamentaux négatifs et décimaux.
/// </summary>
public sealed class FoundationNumberExerciseGenerator
{
    /// <summary>
    /// Source aléatoire injectable pour rendre les tests reproductibles.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Initialise le générateur.
    /// </summary>
    public FoundationNumberExerciseGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>
    /// Crée cinq exercices selon la difficulté choisie.
    /// </summary>
    public IReadOnlyList<Exercise> CreateSession(
        FoundationNumberCourseDefinition definition,
        TrainingDifficulty difficulty)
    {
        return Enumerable.Range(0, 5)
            .Select(index => CreateExercise(definition, difficulty, index))
            .ToArray();
    }

    /// <summary>
    /// Crée deux questions finales distinctes.
    /// </summary>
    public IReadOnlyList<Exercise> CreateValidation(
        FoundationNumberCourseDefinition definition,
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
    /// Crée une question adaptée au cours demandé.
    /// </summary>
    public Exercise CreateExercise(
        FoundationNumberCourseDefinition definition,
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

        return definition.Kind == FoundationNumberKind.NegativeNumbers
            ? CreateNegativeExercise(definition.Id, scale, index)
            : CreateDecimalExercise(definition.Id, scale, index);
    }

    /// <summary>
    /// Alterne déplacement, opposé et comparaison sur une droite numérique.
    /// </summary>
    private Exercise CreateNegativeExercise(
        string courseId,
        int scale,
        int index)
    {
        var variant = _random.Next(3);
        var left = _random.Next(-scale, scale + 1);
        var right = _random.Next(-scale, scale + 1);
        var key = "negativeMove";
        var answer = left + right;
        IReadOnlyList<string> arguments = [left.ToString(), right.ToString()];

        if (variant == 1)
        {
            key = "negativeOpposite";
            answer = -left;
            arguments = [left.ToString()];
        }
        else if (variant == 2)
        {
            while (right == left)
            {
                right = _random.Next(-scale, scale + 1);
            }

            key = "negativeGreater";
            answer = Math.Max(left, right);
            arguments = [left.ToString(), right.ToString()];
        }

        return Create(
            courseId,
            key,
            answer.ToString(CultureInfo.InvariantCulture),
            arguments,
            ExerciseAnswerKind.Numeric,
            index);
    }

    /// <summary>
    /// Alterne lecture, comparaison et fraction décimale avec au plus trois décimales.
    /// </summary>
    private Exercise CreateDecimalExercise(
        string courseId,
        int scale,
        int index)
    {
        var precision = scale <= 5 ? 1 : scale <= 12 ? 2 : 3;
        var factor = (int)Math.Pow(10, precision);
        var firstUnits = _random.Next(0, scale * factor + 1);
        var secondUnits = _random.Next(0, scale * factor + 1);
        var variant = _random.Next(3);
        var key = "decimalRead";
        var answerUnits = firstUnits;
        IReadOnlyList<string> arguments =
        [
            (firstUnits / factor).ToString(),
            (firstUnits % factor).ToString(),
            factor.ToString()
        ];

        if (variant == 1)
        {
            while (secondUnits == firstUnits)
            {
                secondUnits = _random.Next(0, scale * factor + 1);
            }

            key = "decimalGreater";
            answerUnits = Math.Max(firstUnits, secondUnits);
            arguments =
            [
                FormatDecimal(firstUnits, factor),
                FormatDecimal(secondUnits, factor)
            ];
        }
        else if (variant == 2)
        {
            key = "decimalFraction";
            arguments = [firstUnits.ToString(), factor.ToString()];
        }

        return Create(
            courseId,
            key,
            FormatInvariantDecimal(answerUnits, factor),
            arguments,
            ExerciseAnswerKind.Decimal,
            index);
    }

    /// <summary>
    /// Construit le modèle public commun aux deux familles de questions.
    /// </summary>
    private static Exercise Create(
        string courseId,
        string key,
        string answer,
        IReadOnlyList<string> arguments,
        ExerciseAnswerKind answerKind,
        int index)
    {
        return new Exercise(
            $"{courseId}-{index}-{Guid.NewGuid():N}",
            courseId,
            string.Empty,
            answer,
            string.Empty,
            $"exercise.foundation.{key}",
            arguments,
            "exercise.foundation.explanation",
            [answer],
            answerKind);
    }

    /// <summary>
    /// Formate un décimal avec une virgule pour l'énoncé français.
    /// </summary>
    private static string FormatDecimal(int units, int factor)
    {
        return FormatInvariantDecimal(units, factor).Replace('.', ',');
    }

    /// <summary>
    /// Produit la valeur canonique stockée dans l'exercice.
    /// </summary>
    private static string FormatInvariantDecimal(int units, int factor)
    {
        return (units / (decimal)factor)
            .ToString("0.###", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Compare la clé et les paramètres de deux questions.
    /// </summary>
    private static bool HasSameQuestion(Exercise left, Exercise right)
    {
        return left.QuestionKey == right.QuestionKey
            && (left.QuestionArguments ?? [])
                .SequenceEqual(right.QuestionArguments ?? []);
    }
}
