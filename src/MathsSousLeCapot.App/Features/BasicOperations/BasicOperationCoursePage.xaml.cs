using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;
using Microsoft.Maui.Controls.Shapes;

namespace MathsSousLeCapot.App.Features.BasicOperations;

/// <summary>
/// Pilote le parcours interactif d'une addition ou d'une soustraction.
/// </summary>
public partial class BasicOperationCoursePage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Service de traduction de l'interface.
    /// </summary>
    private static TranslationService Text => TranslationService.Current;

    /// <summary>
    /// Service de progression locale.
    /// </summary>
    private readonly LocalStorageService _storage = new();

    /// <summary>
    /// Cours et opération configurés par la route.
    /// </summary>
    private Course? _course;
    private BasicOperation _operation;

    /// <summary>
    /// État courant du parcours et de la manipulation.
    /// </summary>
    private int _currentStepIndex;
    private int _leftValue;
    private int _rightValue;
    private int _answerAttempts;
    private bool _readRegistered;

    /// <summary>
    /// Initialise les composants communs aux deux cours.
    /// </summary>
    public BasicOperationCoursePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Configure le cours demandé par la navigation.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("courseId", out var value)
            || value is not string courseId)
        {
            return;
        }

        _course = CourseCatalog.GetCourse(courseId);
        _operation = courseId == CourseCatalog.AdditionCourseId
            ? BasicOperation.Addition
            : BasicOperation.Subtraction;
        (_leftValue, _rightValue) = _operation == BasicOperation.Addition
            ? (3, 2)
            : (7, 3);
        Title = Text.Get(_course.Title);
        ShowCurrentStep();
    }

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

    private void OnDecreaseLeftClicked(object? sender, EventArgs e)
    {
        _leftValue = Math.Max(0, _leftValue - 1);
        _rightValue = Math.Min(_rightValue, _leftValue);
        RefreshOperation();
    }

    private void OnIncreaseLeftClicked(object? sender, EventArgs e)
    {
        _leftValue = Math.Min(10, _leftValue + 1);
        RefreshOperation();
    }

    private void OnDecreaseRightClicked(object? sender, EventArgs e)
    {
        _rightValue = Math.Max(0, _rightValue - 1);
        RefreshOperation();
    }

    private void OnIncreaseRightClicked(object? sender, EventArgs e)
    {
        var maximum = _operation == BasicOperation.Addition ? 10 : _leftValue;
        _rightValue = Math.Min(maximum, _rightValue + 1);
        RefreshOperation();
    }

    /// <summary>
    /// Valide la question finale et sauvegarde la première réussite.
    /// </summary>
    private void OnValidateAnswerClicked(object? sender, EventArgs e)
    {
        if (_course is null)
        {
            return;
        }

        _answerAttempts++;
        var expected = _operation == BasicOperation.Addition ? 5 : 4;
        var correct = LocalizedNumberText.Matches(FinalAnswer.Text, expected);
        var color = ThemeService.GetColor(correct ? "Success" : "Error");

        AnswerFeedbackPanel.IsVisible = true;
        AnswerFeedbackPanel.Style = (Style)Application.Current!.Resources[
            correct ? "SuccessFeedback" : "ErrorFeedback"];
        AnswerFeedbackTitle.Text = Text.Get(
            correct ? "feedback.correctTitle" : "feedback.incorrectTitle");
        AnswerFeedbackTitle.TextColor = color;
        AnswerFeedback.TextColor = color;

        if (correct)
        {
            _storage.CompleteCourse(_course.Id);
            AnswerFeedback.Text = Text.Get(GetKey("finalCorrect"));
            NextButton.Text = Text.Get("navigation.finish");
            return;
        }

        AnswerFeedback.Text = Text.Get(
            _answerAttempts switch
            {
                1 => GetKey("hint1"),
                2 => GetKey("hint2"),
                _ => GetKey("answer")
            });
    }

    /// <summary>
    /// Synchronise les textes et panneaux avec l'étape courante.
    /// </summary>
    private void ShowCurrentStep()
    {
        if (_course is null)
        {
            return;
        }

        var step = _course.Steps[_currentStepIndex];
        StepIndicator.Text = Text.Format(
            "course.stepIndicator",
            _currentStepIndex + 1,
            _course.Steps.Count);
        StepTitle.Text = Text.Get(step.Title);
        StepExplanation.Text = Text.Get(step.Explanation);
        StepExample.Text = Text.Get(step.Example);
        OperationExplorer.IsVisible = step.Kind == CourseStepKind.InteractiveOperation;
        FinalQuestionPanel.IsVisible = step.Kind == CourseStepKind.FinalQuestion;
        PreviousButton.IsEnabled = _currentStepIndex > 0;
        NextButton.Text = Text.Get(
            _currentStepIndex == _course.Steps.Count - 1
                ? "navigation.quit"
                : "navigation.next");
        FinalQuestionLabel.Text = Text.Get(GetKey("finalQuestion"));
        FinalWrittenCalculation.SetCalculation(
            WrittenCalculationBuilder.Create(
                _operation == BasicOperation.Addition
                    ? WrittenCalculationKind.Addition
                    : WrittenCalculationKind.Subtraction,
                _operation == BasicOperation.Addition ? 3 : 7,
                _operation == BasicOperation.Addition ? 2 : 3),
            revealSolution: false);
        LeftGroupTitle.Text = Text.Get(
            _operation == BasicOperation.Addition
                ? "course.operation.firstGroup"
                : "course.operation.startingGroup");
        RightGroupTitle.Text = Text.Get(
            _operation == BasicOperation.Addition
                ? "course.operation.secondGroup"
                : "course.operation.removedGroup");
        OperationSymbolLabel.Text = _operation == BasicOperation.Addition ? "+" : "−";
        RefreshOperation();
    }

    /// <summary>
    /// Recalcule l'équation et les objets représentant les quantités.
    /// </summary>
    private void RefreshOperation()
    {
        var result = BasicOperationCalculator.Calculate(
            _operation,
            _leftValue,
            _rightValue);
        LeftValueLabel.Text = _leftValue.ToString();
        RightValueLabel.Text = _rightValue.ToString();
        EquationLabel.Text = $"{_leftValue} {OperationSymbolLabel.Text} {_rightValue} = {result}";
        InteractiveCalculation.SetCalculation(
            WrittenCalculationBuilder.Create(
                _operation == BasicOperation.Addition
                    ? WrittenCalculationKind.Addition
                    : WrittenCalculationKind.Subtraction,
                _leftValue,
                _rightValue),
            revealSolution: true);
        SemanticProperties.SetDescription(
            EquationLabel,
            Text.Format(
                _operation == BasicOperation.Addition
                    ? "course.addition.equationDescription"
                    : "course.subtraction.equationDescription",
                _leftValue,
                _rightValue,
                result));

        QuantityDots.Children.Clear();
        if (_operation == BasicOperation.Addition)
        {
            AddDots(_leftValue, ThemeService.GetColor("Primary"));
            AddDots(_rightValue, ThemeService.GetColor("Success"));
        }
        else
        {
            AddDots(result, ThemeService.GetColor("Primary"));
            AddDots(_rightValue, ThemeService.GetColor("Error"));
        }
    }

    /// <summary>
    /// Ajoute des pastilles visuelles pour représenter une quantité.
    /// </summary>
    private void AddDots(int count, Color color)
    {
        for (var index = 0; index < count; index++)
        {
            QuantityDots.Children.Add(new Border
            {
                BackgroundColor = color,
                HeightRequest = 24,
                WidthRequest = 24,
                Margin = 4,
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = 12 }
            });
        }
    }

    /// <summary>
    /// Construit une clé propre à l'opération active.
    /// </summary>
    private string GetKey(string suffix)
    {
        var operation = _operation == BasicOperation.Addition
            ? "addition"
            : "subtraction";
        return $"course.{operation}.{suffix}";
    }

    /// <summary>
    /// Retourne à la liste de tous les cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
