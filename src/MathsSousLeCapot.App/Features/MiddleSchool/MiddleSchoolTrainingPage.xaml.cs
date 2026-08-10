using MathsSousLeCapot.App.Features.Training;
using MathsSousLeCapot.App.Controls;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.MiddleSchool;

/// <summary>
/// Gère les entraînements partagés par les notions du collège.
/// </summary>
public partial class MiddleSchoolTrainingPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Générateur, stockage et champs de réponse de la session.
    /// </summary>
    private readonly MiddleSchoolExerciseGenerator _generator = new();
    private readonly LocalStorageService _storage = new();
    private readonly List<Entry> _answerEntries = [];

    /// <summary>
    /// Configuration et état de la session courante.
    /// </summary>
    private Course? _course;
    private MiddleSchoolCourseDefinition? _definition;
    private TrainingDifficulty _difficulty;
    private IReadOnlyList<Exercise> _exercises = [];

    /// <summary>
    /// Initialise le sélecteur de difficulté traduit.
    /// </summary>
    public MiddleSchoolTrainingPage()
    {
        InitializeComponent();
        DifficultyPicker.ItemsSource = TrainingUi.CreateDifficultyLabels();
    }

    /// <summary>
    /// Configure la notion demandée par la navigation.
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
        var text = TranslationService.Current;
        var title = text.Get(definition.Title);
        Title = title;
        TitleLabel.Text = text.Format("course.middle.trainingTitle", title);
        IntroLabel.Text = text.Get("course.middle.trainingIntro");
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
    /// Génère et affiche cinq exercices.
    /// </summary>
    private void StartSession()
    {
        if (_definition is null)
        {
            return;
        }

        _exercises = _generator.CreateSession(_definition, _difficulty);
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
                ExerciseAnswerValidator.Matches(
                    exercise,
                    answer,
                    TranslationService.Current.CurrentLanguageCode));
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
    /// Retourne à la liste globale des cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
