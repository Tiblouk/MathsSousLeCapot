using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.App.Controls;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.MiddleSchool;

/// <summary>
/// Pilote le parcours pédagogique partagé par les notions du collège.
/// </summary>
public partial class MiddleSchoolCoursePage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Services, définition et état de la lecture courante.
    /// </summary>
    private readonly LocalStorageService _storage = new();
    private readonly MiddleSchoolExerciseGenerator _generator = new();
    private readonly List<ExerciseControls> _exerciseControls = [];
    private Course? _course;
    private MiddleSchoolCourseDefinition? _definition;
    private IReadOnlyList<Exercise> _exercises = [];
    private int _currentStepIndex;
    private int _answerAttempts;
    private bool _readRegistered;

    /// <summary>
    /// Initialise les composants de la page.
    /// </summary>
    public MiddleSchoolCoursePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Configure le cours demandé par la navigation.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("courseId", out var value)
            || value is not string courseId
            || !MiddleSchoolCourseCatalog.TryGet(courseId, out var definition))
        {
            return;
        }

        _definition = definition;
        _course = CourseCatalog.GetCourse(courseId);
        Title = TranslationService.Current.Get(definition.Title);
        ShowCurrentStep();
    }

    /// <summary>
    /// Enregistre une lecture une seule fois lors de l'affichage.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_course is not null && !_readRegistered)
        {
            _storage.RegisterCourseRead(_course.Id);
            _readRegistered = true;
        }
    }

    /// <summary>
    /// Affiche l'étape précédente.
    /// </summary>
    private void OnPreviousClicked(object? sender, EventArgs e)
    {
        if (_currentStepIndex > 0)
        {
            _currentStepIndex--;
            ShowCurrentStep();
        }
    }

    /// <summary>
    /// Affiche l'étape suivante ou ferme le cours.
    /// </summary>
    private async void OnNextClicked(object? sender, EventArgs e)
    {
        if (_course is null)
        {
            return;
        }

        if (_currentStepIndex < _course.Steps.Count - 1)
        {
            _currentStepIndex++;
            ShowCurrentStep();
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Génère un nouvel exercice intermédiaire.
    /// </summary>
    private void OnNewExampleClicked(object? sender, EventArgs e)
    {
        CreateExercises(1, TrainingDifficulty.Moderate);
    }

    /// <summary>
    /// Corrige les réponses et termine le cours lorsque la validation est réussie.
    /// </summary>
    private void OnCheckAnswersClicked(object? sender, EventArgs e)
    {
        if (_course is null || _exercises.Count == 0)
        {
            return;
        }

        _answerAttempts++;
        var text = TranslationService.Current;
        var allCorrect = true;

        for (var index = 0; index < _exercises.Count; index++)
        {
            var exercise = _exercises[index];
            var controls = _exerciseControls[index];
            var correct = ExerciseAnswerValidator.Matches(
                exercise,
                controls.Answer.Text,
                text.CurrentLanguageCode);
            var color = ThemeService.GetColor(correct ? "Success" : "Error");

            controls.Feedback.IsVisible = true;
            controls.Feedback.Style = (Style)Application.Current!.Resources[
                correct ? "SuccessFeedback" : "ErrorFeedback"];
            controls.FeedbackTitle.Text = text.Get(
                correct ? "feedback.correctTitle" : "feedback.incorrectTitle");
            controls.FeedbackTitle.TextColor = color;
            controls.FeedbackText.TextColor = color;
            controls.FeedbackText.Text = correct
                ? LocalizedExerciseText.GetExplanation(exercise)
                : _answerAttempts < 3
                    ? text.Get("course.middle.hint")
                    : text.Format("course.middle.answer", exercise.CorrectAnswer);
            controls.Solution.IsVisible =
                exercise.WrittenCalculation is not null
                && (correct || _answerAttempts >= 3);
            allCorrect &= correct;
        }

        if (allCorrect
            && _course.Steps[_currentStepIndex].Kind == CourseStepKind.FinalQuestion)
        {
            _storage.CompleteCourse(_course.Id);
            NextButton.Text = text.Get("navigation.finish");
        }
    }

    /// <summary>
    /// Synchronise les textes et interactions avec l'étape courante.
    /// </summary>
    private void ShowCurrentStep()
    {
        if (_course is null || _definition is null)
        {
            return;
        }

        var text = TranslationService.Current;
        var step = _course.Steps[_currentStepIndex];
        var title = text.Get(_definition.Title);
        StepIndicator.Text = text.Format(
            "course.stepIndicator",
            _currentStepIndex + 1,
            _course.Steps.Count);
        StepTitle.Text = text.Get(step.Title);
        StepExplanation.SetText(
            step.Kind == CourseStepKind.Explanation
                ? text.Format(_definition.DiscoveryKey, title, _definition.Example)
                : text.Format(step.Explanation, title));
        StepExample.Text = _definition.Example;
        ExamplePanel.IsVisible = _currentStepIndex == 0;
        if (_currentStepIndex == 0
            && WrittenCalculationExamples.TryCreate(
                _definition.Id,
                out var discoveryCalculation))
        {
            DiscoveryCalculation.SetCalculation(
                discoveryCalculation,
                revealSolution: true);
        }
        else
        {
            DiscoveryCalculation.Clear();
        }
        ExercisePanel.IsVisible = step.Kind is
            CourseStepKind.InteractiveMiddleSchool or CourseStepKind.FinalQuestion;
        PreviousButton.IsEnabled = _currentStepIndex > 0;
        NextButton.Text = text.Get(
            _currentStepIndex == _course.Steps.Count - 1
                ? "navigation.quit"
                : "navigation.next");

        if (ExercisePanel.IsVisible)
        {
            var isValidation = step.Kind == CourseStepKind.FinalQuestion;
            CreateExercises(
                isValidation ? 2 : 1,
                isValidation
                    ? TrainingDifficulty.Hard
                    : TrainingDifficulty.Moderate);
            NewExampleButton.IsVisible = !isValidation;
            Grid.SetColumnSpan(CheckButton, isValidation ? 2 : 1);
        }
    }

    /// <summary>
    /// Prépare une ou deux questions selon l'étape.
    /// </summary>
    private void CreateExercises(int count, TrainingDifficulty difficulty)
    {
        if (_definition is null)
        {
            return;
        }

        _exercises = count == 2
            ? _generator.CreateValidation(_definition, difficulty)
            : [_generator.CreateExercise(_definition, difficulty)];
        _exerciseControls.Clear();
        ExerciseQuestionsPanel.Children.Clear();
        for (var index = 0; index < _exercises.Count; index++)
        {
            ExerciseQuestionsPanel.Children.Add(
                CreateExerciseCard(_exercises[index], index + 1));
        }

        _answerAttempts = 0;
    }

    /// <summary>
    /// Construit une carte de question et conserve ses contrôles de correction.
    /// </summary>
    private Border CreateExerciseCard(Exercise exercise, int number)
    {
        var text = TranslationService.Current;
        var answer = new Entry
        {
            Keyboard = Keyboard.Default,
            Placeholder = text.Get("common.answerPlaceholder")
        };
        var feedbackTitle = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 17
        };
        var feedbackText = new Label();
        var solution = exercise.WrittenCalculation is null
            ? new WrittenCalculationView { IsVisible = false }
            : new WrittenCalculationView(
                exercise.WrittenCalculation,
                revealSolution: true)
            {
                IsVisible = false
            };
        var feedback = new Border
        {
            IsVisible = false,
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children = { feedbackTitle, feedbackText, solution }
            }
        };
        _exerciseControls.Add(new ExerciseControls(
            answer,
            feedback,
            feedbackTitle,
            feedbackText,
            solution));

        return new Border
        {
            BackgroundColor = ThemeService.GetColor("ControlBackground"),
            Stroke = ThemeService.GetColor("Border"),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 12
            },
            Padding = 14,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 13,
                        Text = text.Format("course.middle.questionNumber", number),
                        TextColor = ThemeService.GetColor("Primary")
                    },
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        Text = LocalizedExerciseText.GetQuestion(exercise)
                    },
                    exercise.WrittenCalculation is null
                        ? new Grid { IsVisible = false }
                        : new WrittenCalculationView(
                            exercise.WrittenCalculation,
                            revealSolution: false),
                    answer,
                    feedback
                }
            }
        };
    }

    /// <summary>
    /// Retourne à la liste globale des cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }

    /// <summary>
    /// Regroupe les contrôles associés à une question.
    /// </summary>
    private sealed record ExerciseControls(
        Entry Answer,
        Border Feedback,
        Label FeedbackTitle,
        Label FeedbackText,
        WrittenCalculationView Solution);
}
