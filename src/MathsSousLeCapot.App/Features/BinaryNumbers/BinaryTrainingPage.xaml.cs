using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.App.Features.Training;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.BinaryNumbers;

/// <summary>
/// Gère les séries d'exercices consacrées à la base deux.
/// </summary>
public partial class BinaryTrainingPage : ContentPage
{
    /// <summary>
    /// Service de traduction utilisé pour les éléments créés en C#.
    /// </summary>
    private static TranslationService Text => TranslationService.Current;

    private readonly BinaryCounterExerciseGenerator _generator = new();
    private readonly LocalStorageService _storage = new();
    private readonly List<Entry> _entries = [];
    private IReadOnlyList<Exercise> _exercises = [];
    private TrainingDifficulty _difficulty;

    public BinaryTrainingPage()
    {
        InitializeComponent();
        DifficultyPicker.ItemsSource = TrainingUi.CreateDifficultyLabels();
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        _difficulty = (TrainingDifficulty)Math.Max(0, DifficultyPicker.SelectedIndex);
        StartSession();
    }

    /// <summary>
    /// Génère et affiche cinq nouveaux exercices binaires.
    /// </summary>
    private void StartSession()
    {
        _exercises = _generator.CreateSession(_difficulty);
        _entries.Clear();
        QuestionsPanel.Children.Clear();
        ResultsList.Children.Clear();

        foreach (var exercise in _exercises)
        {
            var entry = new Entry
            {
                Keyboard = Keyboard.Numeric,
                Placeholder = Text.Get("common.answerPlaceholder")
            };
            _entries.Add(entry);
            QuestionsPanel.Children.Add(new Border
            {
                Style = (Style)Application.Current!.Resources["Card"],
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        new Label
                        {
                            Text = LocalizedExerciseText.GetQuestion(exercise),
                            FontAttributes = FontAttributes.Bold
                        },
                        entry
                    }
                }
            });
        }

        StartButton.IsVisible = false;
        QuestionsPanel.IsVisible = true;
        VerifyButton.IsVisible = true;
        SummaryPanel.IsVisible = false;
    }

    private void OnVerifyClicked(object? sender, EventArgs e)
    {
        var results = _exercises.Select((exercise, index) =>
        {
            var answer = Normalize(_entries[index].Text ?? string.Empty);
            return new ExerciseResult(exercise, answer, answer == Normalize(exercise.CorrectAnswer));
        }).ToArray();
        var session = new TrainingSession(
            CourseCatalog.BinaryCourseId,
            _difficulty,
            DateTimeOffset.Now,
            results);
        _storage.SaveTrainingSession(session);

        QuestionsPanel.IsVisible = false;
        VerifyButton.IsVisible = false;
        SummaryPanel.IsVisible = true;
        TrainingUi.FillSummary(ResultsList, ScoreLabel, session);
    }

    /// <summary>
    /// Lance cinq nouveaux exercices avec la même difficulté.
    /// </summary>
    private void OnContinueClicked(object? sender, EventArgs e)
    {
        StartSession();
    }

    /// <summary>
    /// Quitte l'entraînement et revient à sa fiche de cours.
    /// </summary>
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private static string Normalize(string value)
    {
        var normalized = value.Trim().TrimStart('0');
        return string.IsNullOrEmpty(normalized) ? "0" : normalized;
    }

    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
