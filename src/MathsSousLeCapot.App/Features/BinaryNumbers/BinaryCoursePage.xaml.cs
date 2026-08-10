using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.PositionalNumeration;

namespace MathsSousLeCapot.App.Features.BinaryNumbers;

/// <summary>
/// Pilote les étapes du cours de numération binaire.
/// </summary>
public partial class BinaryCoursePage : ContentPage
{
    /// <summary>
    /// Service de traduction des contenus calculés pendant l'interaction.
    /// </summary>
    private static TranslationService Text => TranslationService.Current;

    private readonly Course _course = CourseCatalog.GetCourse(CourseCatalog.BinaryCourseId);
    private readonly LocalStorageService _storage = new();
    private int _currentStepIndex;
    private int _attempts;
    private bool _readRegistered;

    public BinaryCoursePage()
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
        if (_currentStepIndex > 0)
        {
            _currentStepIndex--;
            ShowCurrentStep();
        }
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
        _attempts++;
        var answer = FinalAnswer.Text?.Trim().TrimStart('0');
        answer = string.IsNullOrEmpty(answer) ? "0" : answer;
        var correct = answer == "100";
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
            AnswerFeedback.Text = Text.Get("course.binary.finalCorrect");
            NextButton.Text = Text.Get("navigation.finish");
            return;
        }

        AnswerFeedback.Text = _attempts switch
        {
            1 => Text.Get("course.binary.hint1"),
            2 => Text.Get("course.binary.hint2"),
            _ => Text.Get("course.binary.answer")
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

        if (step.Id == "binary-counter")
        {
            InteractiveCounter.UseExample(NumeralBase.Binary, 3);
        }
    }

    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
