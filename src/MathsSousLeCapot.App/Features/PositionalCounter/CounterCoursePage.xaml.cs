using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.PositionalNumeration;

namespace MathsSousLeCapot.App.Features.PositionalCounter;

/// <summary>
/// Pilote le cours interactif du compteur positionnel décimal.
/// </summary>
public partial class CounterCoursePage : ContentPage
{
    /// <summary>
    /// Service de traduction utilisé pour les contenus dynamiques.
    /// </summary>
    private static TranslationService Text => TranslationService.Current;

    private readonly Course _course =
        CourseCatalog.GetCourse(CourseCatalog.PositionalCounterCourseId);
    private readonly LocalStorageService _storage = new();
    private int _currentStepIndex;
    private int _answerAttempts;
    private bool _readRegistered;

    public CounterCoursePage()
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

    private void OnPreviousClicked(object? sender, EventArgs e)
    {
        if (_currentStepIndex == 0)
        {
            return;
        }

        _currentStepIndex--;
        ShowCurrentStep();
    }

    private async void OnNextClicked(object? sender, EventArgs e)
    {
        if (_currentStepIndex < _course.Steps.Count - 1)
        {
            _currentStepIndex++;
            ShowCurrentStep();
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    private void OnValidateAnswerClicked(object? sender, EventArgs e)
    {
        _answerAttempts++;

        AnswerFeedbackPanel.IsVisible = true;

        if (LocalizedNumberText.Matches(FinalAnswer.Text, 100))
        {
            _storage.CompleteCourse(_course.Id);
            AnswerFeedbackPanel.Style =
                (Style)Application.Current!.Resources["SuccessFeedback"];
            AnswerFeedbackTitle.Text = Text.Get("feedback.correctTitle");
            AnswerFeedbackTitle.TextColor = ThemeService.GetColor("Success");
            AnswerFeedback.TextColor = ThemeService.GetColor("Success");
            AnswerFeedback.Text = Text.Get("course.counter.finalCorrect");
            NextButton.Text = Text.Get("navigation.finish");
            return;
        }

        AnswerFeedbackPanel.Style =
            (Style)Application.Current!.Resources["ErrorFeedback"];
        AnswerFeedbackTitle.Text = Text.Get("feedback.incorrectTitle");
        AnswerFeedbackTitle.TextColor = ThemeService.GetColor("Error");
        AnswerFeedback.TextColor = ThemeService.GetColor("Error");
        AnswerFeedback.Text = _answerAttempts switch
        {
            1 => Text.Get("course.counter.hint1"),
            2 => Text.Get("course.counter.hint2"),
            _ => Text.Get("course.counter.answer")
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

        InteractiveCounter.IsVisible = step.Kind == CourseStepKind.InteractiveCounter;
        FinalQuestionPanel.IsVisible = step.Kind == CourseStepKind.FinalQuestion;
        PreviousButton.IsEnabled = _currentStepIndex > 0;
        NextButton.Text = Text.Get(
            _currentStepIndex == _course.Steps.Count - 1
                ? "navigation.quit"
                : "navigation.next");

        if (step.Id == "counter")
        {
            InteractiveCounter.UseExample(NumeralBase.Decimal, 9);
        }
        else if (step.Id == "jumps")
        {
            InteractiveCounter.UseExample(NumeralBase.Decimal, 125);
        }
    }

    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
