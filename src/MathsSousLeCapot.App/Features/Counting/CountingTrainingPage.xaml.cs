using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.App.Features.Training;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.Counting;

/// <summary>
/// Génère et corrige les exercices de lecture des nombres.
/// </summary>
public partial class CountingTrainingPage : ContentPage
{
    /// <summary>
    /// Service de traduction utilisé pour les textes générés en C#.
    /// </summary>
    private static TranslationService Text => TranslationService.Current;

    /// <summary>
    /// Service de persistance des sessions terminées.
    /// </summary>
    private readonly LocalStorageService _storage = new();

    /// <summary>
    /// Champs de réponse alignés avec les exercices de la session.
    /// </summary>
    private readonly List<Entry> _answerEntries = [];

    /// <summary>
    /// Exercices de la session courante.
    /// </summary>
    private IReadOnlyList<Exercise> _exercises = [];

    /// <summary>
    /// Difficulté choisie pour la session courante.
    /// </summary>
    private TrainingDifficulty _difficulty;

    /// <summary>
    /// Initialise la page avec les difficultés traduites.
    /// </summary>
    public CountingTrainingPage()
    {
        InitializeComponent();
        DifficultyPicker.ItemsSource = TrainingUi.CreateDifficultyLabels();
    }

    /// <summary>
    /// Démarre une session avec la difficulté sélectionnée.
    /// </summary>
    private void OnStartClicked(object? sender, EventArgs e)
    {
        _difficulty = (TrainingDifficulty)Math.Max(0, DifficultyPicker.SelectedIndex);
        StartSession();
    }

    /// <summary>
    /// Génère cinq nombres adaptés à la difficulté choisie.
    /// </summary>
    private void StartSession()
    {
        var maximum = _difficulty switch
        {
            TrainingDifficulty.Easy => 20,
            TrainingDifficulty.Moderate => 100,
            TrainingDifficulty.Hard => 999,
            _ => throw new ArgumentOutOfRangeException(nameof(_difficulty))
        };

        _exercises = Enumerable.Range(0, 5)
            .Select(index =>
            {
                var value = Random.Shared.Next(0, maximum + 1);
                return new Exercise(
                    $"counting-{_difficulty}-{index}-{Guid.NewGuid():N}",
                    CourseCatalog.CountingCourseId,
                    Text.Format(
                        "course.counting.trainingQuestion",
                        LocalizedNumberText.ToWords(value)),
                    value.ToString(),
                    Text.Format(
                        "course.counting.trainingExplanation",
                        value,
                        LocalizedNumberText.ToWords(value)));
            })
            .ToArray();

        _answerEntries.Clear();
        QuestionsPanel.Children.Clear();
        ResultsList.Children.Clear();
        SummaryPanel.IsVisible = false;
        VerifyButton.IsVisible = true;

        foreach (var exercise in _exercises)
        {
            var entry = new Entry
            {
                Keyboard = Keyboard.Default,
                Placeholder = Text.Get("common.answerPlaceholder")
            };
            _answerEntries.Add(entry);

            QuestionsPanel.Children.Add(new Border
            {
                Style = (Style)Application.Current!.Resources["Card"],
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children = { new Label { Text = exercise.Question, FontAttributes = FontAttributes.Bold }, entry }
                }
            });
        }

        SetupPanel.IsVisible = false;
        QuestionsPanel.IsVisible = true;
    }

    /// <summary>
    /// Corrige et sauvegarde les cinq réponses en une seule opération.
    /// </summary>
    private void OnVerifyClicked(object? sender, EventArgs e)
    {
        var results = _exercises.Select((exercise, index) =>
        {
            var answer = _answerEntries[index].Text?.Trim() ?? string.Empty;
            return new ExerciseResult(
                exercise,
                answer,
                LocalizedNumberText.Matches(
                    answer,
                    int.Parse(exercise.CorrectAnswer)));
        }).ToArray();
        var session = new TrainingSession(
            CourseCatalog.CountingCourseId,
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
    private void OnNewSessionClicked(object? sender, EventArgs e)
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

    /// <summary>
    /// Retourne directement à la liste des cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
