using MathsSousLeCapot.App.Controls;
using MathsSousLeCapot.App.Features.Training;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.Assessments;

/// <summary>
/// Affiche et corrige un contrôle intermédiaire généré pour une classe.
/// </summary>
public partial class ClassAssessmentPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Générateur responsable de construire les questions au moment utile.
    /// </summary>
    private readonly ClassAssessmentGenerator _generator = new();

    /// <summary>
    /// Champs de saisie associés aux questions affichées.
    /// </summary>
    private readonly List<Entry> _answerEntries = [];

    /// <summary>
    /// Niveau scolaire ciblé par le contrôle.
    /// </summary>
    private string _level = string.Empty;

    /// <summary>
    /// Contrôle actuellement affiché à l'écran.
    /// </summary>
    private ClassAssessment? _assessment;

    /// <summary>
    /// Initialise la page de contrôle.
    /// </summary>
    public ClassAssessmentPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Configure la page à partir du niveau reçu par la navigation.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("level", out var value) || value is not string level)
        {
            return;
        }

        _level = level;
        var text = TranslationService.Current;
        var levelTitle = text.Get(level);
        Title = text.Format("assessment.class.pageTitle", levelTitle);
        TitleLabel.Text = Title;
        IntroLabel.Text = text.Format("assessment.class.intro", levelTitle);
    }

    /// <summary>
    /// Génère une nouvelle tentative et affiche ses questions.
    /// </summary>
    private void OnStartClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_level))
        {
            return;
        }

        _assessment = _generator.Create(_level);
        _answerEntries.Clear();
        QuestionsPanel.Children.Clear();
        ResultsList.Children.Clear();
        MasteredList.Children.Clear();
        ReviewList.Children.Clear();

        if (_assessment.Exercises.Count == 0)
        {
            QuestionsPanel.Children.Add(new Border
            {
                Style = (Style)Application.Current!.Resources["Card"],
                Content = new Label
                {
                    Text = TranslationService.Current.Get("assessment.class.empty"),
                    TextColor = ThemeService.GetColor("TextSecondary")
                }
            });
            SetupPanel.IsVisible = false;
            QuestionsPanel.IsVisible = true;
            VerifyButton.IsVisible = false;
            SummaryPanel.IsVisible = false;
            return;
        }

        foreach (var (exercise, index) in _assessment.Exercises.Select(
            (exercise, index) => (exercise, index)))
        {
            AddQuestionCard(exercise, index);
        }

        SetupPanel.IsVisible = false;
        QuestionsPanel.IsVisible = true;
        VerifyButton.IsVisible = true;
        SummaryPanel.IsVisible = false;
    }

    /// <summary>
    /// Ajoute une carte de question avec sa saisie et son éventuel calcul posé.
    /// </summary>
    private void AddQuestionCard(Exercise exercise, int index)
    {
        var text = TranslationService.Current;
        var entry = new Entry
        {
            Keyboard = Keyboard.Default,
            Placeholder = text.Get("common.answerPlaceholder")
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
                        Text = text.Format(
                            "assessment.class.questionNumber",
                            index + 1)
                    },
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

    /// <summary>
    /// Corrige toutes les réponses et affiche le bilan du contrôle.
    /// </summary>
    private void OnVerifyClicked(object? sender, EventArgs e)
    {
        if (_assessment is null)
        {
            return;
        }

        var results = _assessment.Exercises.Select((exercise, index) =>
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
            $"assessment:{_assessment.Level}",
            _assessment.Difficulty,
            DateTimeOffset.Now,
            results);

        QuestionsPanel.IsVisible = false;
        VerifyButton.IsVisible = false;
        SummaryPanel.IsVisible = true;
        FillMasterySummary(results);
        TrainingUi.FillSummary(ResultsList, ScoreLabel, session);
    }

    /// <summary>
    /// Affiche les notions maîtrisées et celles à revoir selon les réponses.
    /// </summary>
    private void FillMasterySummary(IReadOnlyList<ExerciseResult> results)
    {
        var text = TranslationService.Current;
        var groups = results
            .GroupBy(result => result.Exercise.CourseId)
            .Select(group => new
            {
                Course = CourseCatalog.GetCourse(group.Key),
                Correct = group.Count(result => result.IsCorrect),
                Total = group.Count()
            })
            .OrderBy(group => text.Get(group.Course.Title))
            .ToArray();

        foreach (var group in groups)
        {
            var target = group.Correct >= Math.Ceiling(group.Total * 0.7d)
                ? MasteredList
                : ReviewList;
            target.Children.Add(new Label
            {
                Text = text.Format(
                    "assessment.class.masteryItem",
                    text.Get(group.Course.Title),
                    group.Correct,
                    group.Total),
                TextColor = target == MasteredList
                    ? ThemeService.GetColor("Success")
                    : ThemeService.GetColor("Error")
            });
        }

        AddEmptyMasteryLabelIfNeeded(
            MasteredList,
            "assessment.class.masteredEmpty");
        AddEmptyMasteryLabelIfNeeded(
            ReviewList,
            "assessment.class.reviewEmpty");
    }

    /// <summary>
    /// Ajoute un message discret lorsqu'une liste de bilan est vide.
    /// </summary>
    private static void AddEmptyMasteryLabelIfNeeded(
        VerticalStackLayout target,
        string translationKey)
    {
        if (target.Children.Count > 0)
        {
            return;
        }

        target.Children.Add(new Label
        {
            Text = TranslationService.Current.Get(translationKey),
            TextColor = ThemeService.GetColor("TextSecondary")
        });
    }

    /// <summary>
    /// Retourne à la page précédente.
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
