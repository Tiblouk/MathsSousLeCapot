using MathsSousLeCapot.App.Features.Training;
using MathsSousLeCapot.App.Controls;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Mathematics.Operations;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.BasicOperations;

/// <summary>
/// Gère les entraînements d'addition et de soustraction.
/// </summary>
public partial class BasicOperationTrainingPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Générateur et stockage partagés par les deux opérations.
    /// </summary>
    private readonly BasicOperationExerciseGenerator _generator = new();
    private readonly LocalStorageService _storage = new();
    private readonly List<Entry> _answerEntries = [];

    /// <summary>
    /// Configuration et état de la session courante.
    /// </summary>
    private Course? _course;
    private BasicOperation _operation;
    private TrainingDifficulty _difficulty;
    private IReadOnlyList<Exercise> _exercises = [];

    /// <summary>
    /// Initialise le sélecteur de difficulté.
    /// </summary>
    public BasicOperationTrainingPage()
    {
        InitializeComponent();
        DifficultyPicker.ItemsSource = TrainingUi.CreateDifficultyLabels();
    }

    /// <summary>
    /// Configure l'opération demandée par la navigation.
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
        var text = TranslationService.Current;
        Title = text.Get(_course.Title);
        TitleLabel.Text = text.Get(
            _operation == BasicOperation.Addition
                ? "course.addition.trainingTitle"
                : "course.subtraction.trainingTitle");
        IntroLabel.Text = text.Get("course.operation.trainingIntro");
    }

    /// <summary>
    /// Démarre une session avec la difficulté choisie.
    /// </summary>
    private void OnStartClicked(object? sender, EventArgs e)
    {
        _difficulty = (TrainingDifficulty)Math.Max(0, DifficultyPicker.SelectedIndex);
        StartSession();
    }

    /// <summary>
    /// Génère et affiche les cinq exercices.
    /// </summary>
    private void StartSession()
    {
        if (_course is null)
        {
            return;
        }

        _exercises = _generator.CreateSession(_operation, _difficulty);
        _answerEntries.Clear();
        QuestionsPanel.Children.Clear();
        ResultsList.Children.Clear();

        foreach (var exercise in _exercises)
        {
            var entry = new Entry
            {
                Keyboard = Keyboard.Default,
                Placeholder = TranslationService.Current.Get("common.answerPlaceholder")
            };
            _answerEntries.Add(entry);
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
                            FontAttributes = FontAttributes.Bold,
                            Text = LocalizedExerciseText.GetQuestion(exercise)
                        },
                        exercise.WrittenCalculation is null
                            ? new Grid { IsVisible = false }
                            : new WrittenCalculationView(
                                exercise.WrittenCalculation,
                                revealSolution: false),
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

    /// <summary>
    /// Corrige et sauvegarde la session complète.
    /// </summary>
    private void OnVerifyClicked(object? sender, EventArgs e)
    {
        if (_course is null)
        {
            return;
        }

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
            _course.Id,
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
    /// Retourne à la fiche du cours.
    /// </summary>
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Retourne à la liste de tous les cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
