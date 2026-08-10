using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.App.Features.Training;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.PositionalCounter;

/// <summary>
/// Gère les sessions d'entraînement du compteur décimal.
/// </summary>
public partial class CounterTrainingPage : ContentPage
{
    /// <summary>
    /// Service de traduction utilisé pour les contrôles construits en C#.
    /// </summary>
    private static TranslationService Text => TranslationService.Current;

    private readonly PositionalCounterExerciseGenerator _generator = new();
    private readonly LocalStorageService _storage = new();
    private readonly List<Entry> _answerEntries = [];
    private IReadOnlyList<Exercise> _exercises = [];
    private TrainingDifficulty _difficulty;

    public CounterTrainingPage()
    {
        InitializeComponent();
        DifficultyPicker.ItemsSource = TrainingUi.CreateDifficultyLabels();
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        _difficulty = (TrainingDifficulty)Math.Max(0, DifficultyPicker.SelectedIndex);
        StartSession();
    }

    private void OnVerifyClicked(object? sender, EventArgs e)
    {
        var results = _exercises
            .Select((exercise, index) =>
            {
                var givenAnswer = _answerEntries[index].Text?.Trim() ?? string.Empty;
                return new ExerciseResult(
                    exercise,
                    givenAnswer,
                    LocalizedNumberText.Matches(
                        givenAnswer,
                        int.Parse(exercise.CorrectAnswer)));
            })
            .ToArray();

        var session = new TrainingSession(
            CourseCatalog.PositionalCounterCourseId,
            _difficulty,
            DateTimeOffset.Now,
            results);
        _storage.SaveTrainingSession(session);
        ShowSummary(session);
    }

    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }

    private void OnContinueClicked(object? sender, EventArgs e)
    {
        StartSession();
    }

    private void StartSession()
    {
        _exercises = _generator.CreateSession(_difficulty);
        _answerEntries.Clear();
        QuestionsPanel.Children.Clear();
        ResultsList.Children.Clear();

        foreach (var (exercise, index) in _exercises.Select((item, index) => (item, index)))
        {
            var entry = new Entry
            {
                BackgroundColor = ThemeService.GetColor("ControlBackground"),
                Keyboard = Keyboard.Default,
                Placeholder = Text.Get("common.answerPlaceholder"),
                PlaceholderColor = ThemeService.GetColor("TextSecondary"),
                TextColor = ThemeService.GetColor("TextPrimary")
            };
            _answerEntries.Add(entry);

            QuestionsPanel.Children.Add(new Border
            {
                Style = (Style)Application.Current!.Resources["Card"],
                Content = new VerticalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            Text = Text.Format(
                                "course.counter.trainingQuestionNumber",
                                index + 1)
                        },
                        new Label
                        {
                            FontSize = 16,
                            Text = LocalizedExerciseText.GetQuestion(exercise)
                        },
                        entry
                    }
                }
            });
        }

        SetupPanel.IsVisible = false;
        QuestionsPanel.IsVisible = true;
        VerifyButton.IsVisible = true;
        SummaryPanel.IsVisible = false;
    }

    private void ShowSummary(TrainingSession session)
    {
        QuestionsPanel.IsVisible = false;
        VerifyButton.IsVisible = false;
        SummaryPanel.IsVisible = true;
        TrainingUi.FillSummary(ResultsList, ScoreLabel, session);
    }

}
