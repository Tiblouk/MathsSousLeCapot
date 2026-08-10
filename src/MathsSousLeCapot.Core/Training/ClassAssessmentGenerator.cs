using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.Core.Training;

/// <summary>
/// Génère des contrôles intermédiaires à partir des cours d'une classe.
/// </summary>
public sealed class ClassAssessmentGenerator
{
    /// <summary>
    /// Nombre de questions proposées dans un contrôle général.
    /// </summary>
    public const int DefaultQuestionCount = 10;

    /// <summary>
    /// Source aléatoire partagée par les générateurs spécialisés.
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// Générateur réutilisé pour les notions du primaire.
    /// </summary>
    private readonly PrimaryExerciseGenerator _primaryGenerator;

    /// <summary>
    /// Générateur réutilisé pour les notions du collège.
    /// </summary>
    private readonly MiddleSchoolExerciseGenerator _middleSchoolGenerator;

    /// <summary>
    /// Générateur réutilisé pour les notions du lycée.
    /// </summary>
    private readonly HighSchoolExerciseGenerator _highSchoolGenerator;

    /// <summary>
    /// Générateur réutilisé pour les additions et soustractions de base.
    /// </summary>
    private readonly BasicOperationExerciseGenerator _basicOperationGenerator;

    /// <summary>
    /// Générateur réutilisé pour les nombres négatifs et décimaux.
    /// </summary>
    private readonly FoundationNumberExerciseGenerator _foundationNumberGenerator;

    /// <summary>
    /// Générateur réutilisé pour le compteur décimal.
    /// </summary>
    private readonly PositionalCounterExerciseGenerator _positionalCounterGenerator;

    /// <summary>
    /// Générateur réutilisé pour le compteur binaire.
    /// </summary>
    private readonly BinaryCounterExerciseGenerator _binaryCounterGenerator;

    /// <summary>
    /// Initialise le générateur avec une source aléatoire facultative.
    /// </summary>
    public ClassAssessmentGenerator(Random? random = null)
    {
        _random = random ?? Random.Shared;
        _primaryGenerator = new PrimaryExerciseGenerator(_random);
        _middleSchoolGenerator = new MiddleSchoolExerciseGenerator(_random);
        _highSchoolGenerator = new HighSchoolExerciseGenerator(_random);
        _basicOperationGenerator = new BasicOperationExerciseGenerator(_random);
        _foundationNumberGenerator = new FoundationNumberExerciseGenerator(_random);
        _positionalCounterGenerator = new PositionalCounterExerciseGenerator(_random);
        _binaryCounterGenerator = new BinaryCounterExerciseGenerator(_random);
    }

    /// <summary>
    /// Crée un contrôle général pour le niveau demandé.
    /// </summary>
    public ClassAssessment Create(
        string level,
        TrainingDifficulty difficulty = TrainingDifficulty.Moderate,
        int questionCount = DefaultQuestionCount)
    {
        var courses = CourseCatalog.GetCoursesForLevel(level)
            .Where(course => course.IsAvailable)
            .ToArray();
        if (courses.Length == 0 || questionCount <= 0)
        {
            return new ClassAssessment(level, difficulty, []);
        }

        var exercises = new List<Exercise>(questionCount);
        var signatures = new HashSet<string>(StringComparer.Ordinal);
        var attempts = 0;
        var maximumAttempts = Math.Max(questionCount * courses.Length * 8, 40);

        while (exercises.Count < questionCount && attempts < maximumAttempts)
        {
            var course = courses[attempts % courses.Length];
            var exercise = CreateExercise(course.Id, difficulty, attempts);
            attempts++;

            if (exercise is null)
            {
                continue;
            }

            var signature = CreateSignature(exercise);
            if (signatures.Add(signature))
            {
                exercises.Add(exercise);
            }
        }

        return new ClassAssessment(level, difficulty, exercises);
    }

    /// <summary>
    /// Délègue la création d'une question au générateur associé au cours.
    /// </summary>
    private Exercise? CreateExercise(
        string courseId,
        TrainingDifficulty difficulty,
        int index)
    {
        if (courseId == CourseCatalog.CountingCourseId)
        {
            return CreateCountingExercise(difficulty, index);
        }

        if (courseId == CourseCatalog.AdditionCourseId
            || courseId == CourseCatalog.SubtractionCourseId)
        {
            var operation = courseId == CourseCatalog.AdditionCourseId
                ? BasicOperation.Addition
                : BasicOperation.Subtraction;
            return PickFromSession(
                _basicOperationGenerator.CreateSession(operation, difficulty),
                index);
        }

        if (courseId == CourseCatalog.PositionalCounterCourseId)
        {
            return PickFromSession(
                _positionalCounterGenerator.CreateSession(difficulty),
                index);
        }

        if (courseId == CourseCatalog.BinaryCourseId)
        {
            return PickFromSession(
                _binaryCounterGenerator.CreateSession(difficulty),
                index);
        }

        if (FoundationNumberCourseCatalog.TryGet(courseId, out var foundation))
        {
            return _foundationNumberGenerator.CreateExercise(
                foundation,
                difficulty,
                index);
        }

        if (PrimaryCourseCatalog.TryGet(courseId, out var primary))
        {
            return _primaryGenerator.CreateExercise(primary, difficulty, index);
        }

        if (MiddleSchoolCourseCatalog.TryGet(courseId, out var middleSchool))
        {
            return _middleSchoolGenerator.CreateExercise(
                middleSchool,
                difficulty,
                index);
        }

        if (HighSchoolCourseCatalog.TryGet(courseId, out var highSchool))
        {
            return _highSchoolGenerator.CreateExercise(
                highSchool,
                difficulty,
                index);
        }

        return null;
    }

    /// <summary>
    /// Crée une question de comptage simple et variable.
    /// </summary>
    private Exercise CreateCountingExercise(
        TrainingDifficulty difficulty,
        int index)
    {
        var maximum = difficulty switch
        {
            TrainingDifficulty.Easy => 20,
            TrainingDifficulty.Moderate => 100,
            TrainingDifficulty.Hard => 999,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
        var variant = _random.Next(3);
        var shown = _random.Next(0, maximum);

        if (variant == 0)
        {
            return CreateCountingResult(
                index,
                "nextNumber",
                shown + 1,
                shown);
        }

        if (variant == 1)
        {
            shown = _random.Next(1, maximum + 1);
            return CreateCountingResult(
                index,
                "previousNumber",
                shown - 1,
                shown);
        }

        var tens = _random.Next(1, Math.Max(2, maximum / 10 + 1));
        var units = _random.Next(0, 10);
        var answer = tens * 10 + units;
        return CreateCountingResult(
            index,
            "tensAndUnits",
            answer,
            tens,
            units);
    }

    /// <summary>
    /// Construit le modèle d'exercice commun aux questions de comptage.
    /// </summary>
    private static Exercise CreateCountingResult(
        int index,
        string key,
        int answer,
        params int[] arguments)
    {
        return new Exercise(
            $"assessment-counting-{index}-{Guid.NewGuid():N}",
            CourseCatalog.CountingCourseId,
            string.Empty,
            answer.ToString(),
            string.Empty,
            $"exercise.assessment.{key}",
            arguments.Select(value => value.ToString()).ToArray(),
            "exercise.assessment.explanation",
            [answer.ToString()]);
    }

    /// <summary>
    /// Prend une question dans une session courte en gardant l'index stable.
    /// </summary>
    private static Exercise PickFromSession(
        IReadOnlyList<Exercise> exercises,
        int index)
    {
        return exercises[index % exercises.Count];
    }

    /// <summary>
    /// Résume une question pour éviter les doublons exacts dans un contrôle.
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
