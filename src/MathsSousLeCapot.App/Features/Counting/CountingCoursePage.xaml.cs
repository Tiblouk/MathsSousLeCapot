using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Features.Counting;

/// <summary>
/// Pilote les étapes interactives du cours consacré au comptage.
/// </summary>
public partial class CountingCoursePage : ContentPage
{
    /// <summary>
    /// Service de traduction partagé par toutes les pages.
    /// </summary>
    private static TranslationService Text => TranslationService.Current;

    /// <summary>
    /// Définition pédagogique du cours courant.
    /// </summary>
    private readonly Course _course = CourseCatalog.GetCourse(CourseCatalog.CountingCourseId);

    /// <summary>
    /// Service chargé de conserver localement la progression.
    /// </summary>
    private readonly LocalStorageService _storage = new();

    /// <summary>
    /// Index de l'étape affichée.
    /// </summary>
    private int _currentStepIndex;

    /// <summary>
    /// Nombre présenté dans l'explorateur interactif.
    /// </summary>
    private int _currentNumber;

    /// <summary>
    /// Nombre de tentatives effectuées sur la question finale.
    /// </summary>
    private int _answerAttempts;

    /// <summary>
    /// Indique si cette ouverture du cours a déjà été comptabilisée.
    /// </summary>
    private bool _readRegistered;

    /// <summary>
    /// Initialise la page et affiche sa première étape.
    /// </summary>
    public CountingCoursePage()
    {
        InitializeComponent();
        ShowCurrentStep();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_readRegistered)
        {
            _storage.RegisterCourseRead(_course.Id);
            _readRegistered = true;
        }
    }

    private void OnPreviousStepClicked(object? sender, EventArgs e)
    {
        if (_currentStepIndex > 0)
        {
            _currentStepIndex--;
            ShowCurrentStep();
        }
    }

    private async void OnNextStepClicked(object? sender, EventArgs e)
    {
        if (_currentStepIndex < _course.Steps.Count - 1)
        {
            _currentStepIndex++;
            ShowCurrentStep();
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    private void OnPreviousNumberClicked(object? sender, EventArgs e)
    {
        _currentNumber = Math.Max(0, _currentNumber - 1);
        ShowNumber();
    }

    private void OnNextNumberClicked(object? sender, EventArgs e)
    {
        _currentNumber = Math.Min(9999, _currentNumber + 1);
        ShowNumber();
    }

    private void OnValidateAnswerClicked(object? sender, EventArgs e)
    {
        _answerAttempts++;
        AnswerFeedbackPanel.IsVisible = true;
        var correct = LocalizedNumberText.Matches(FinalAnswer.Text, 20);
        var styleName = correct ? "SuccessFeedback" : "ErrorFeedback";
        var color = ThemeService.GetColor(correct ? "Success" : "Error");

        AnswerFeedbackPanel.Style = (Style)Application.Current!.Resources[styleName];
        AnswerFeedbackTitle.Text = Text.Get(
            correct ? "feedback.correctTitle" : "feedback.incorrectTitle");
        AnswerFeedbackTitle.TextColor = color;
        AnswerFeedback.TextColor = color;

        if (correct)
        {
            _storage.CompleteCourse(_course.Id);
            AnswerFeedback.Text = Text.Get("course.counting.finalCorrect");
            NextButton.Text = Text.Get("navigation.finish");
            return;
        }

        AnswerFeedback.Text = _answerAttempts switch
        {
            1 => Text.Get("course.counting.hint1"),
            2 => Text.Get("course.counting.hint2"),
            _ => Text.Get("course.counting.answer")
        };
    }

    private void ShowCurrentStep()
    {
        var step = _course.Steps[_currentStepIndex];
        StepIndicator.Text = Text.Format(
            "course.stepIndicator",
            _currentStepIndex + 1,
            _course.Steps.Count);
        StepTitle.Text = Text.Get(step.Title);
        StepExplanation.Text = Text.Get(step.Explanation);
        StepExample.Text = Text.Get(step.Example);
        NumberExplorer.IsVisible = step.Kind == CourseStepKind.InteractiveCounting;
        FinalQuestionPanel.IsVisible = step.Kind == CourseStepKind.FinalQuestion;
        PreviousButton.IsEnabled = _currentStepIndex > 0;
        NextButton.Text = Text.Get(
            _currentStepIndex == _course.Steps.Count - 1
                ? "navigation.quit"
                : "navigation.next");

        _currentNumber = step.Id switch
        {
            "counting" => 0,
            "number-names" => 21,
            "tens" => 9,
            _ => _currentNumber
        };
        ShowNumber();
    }

    private void ShowNumber()
    {
        NumberDisplay.Text = _currentNumber.ToString();
        NumberWords.Text = LocalizedNumberText.ToWords(_currentNumber);
    }

    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
