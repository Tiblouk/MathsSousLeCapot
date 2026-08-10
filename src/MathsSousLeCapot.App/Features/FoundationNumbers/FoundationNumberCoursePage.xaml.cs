using System.Globalization;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.FoundationNumbers;

/// <summary>
/// Pilote les cours interactifs sur les nombres négatifs et décimaux.
/// </summary>
public partial class FoundationNumberCoursePage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Services, générateur et contrôles de correction de la page.
    /// </summary>
    private readonly LocalStorageService _storage = new();
    private readonly FoundationNumberExerciseGenerator _generator = new();
    private readonly List<ExerciseControls> _exerciseControls = [];

    /// <summary>
    /// Cours, exercices et état de navigation courants.
    /// </summary>
    private Course? _course;
    private FoundationNumberCourseDefinition? _definition;
    private IReadOnlyList<Exercise> _exercises = [];
    private int _currentStepIndex;
    private int _answerAttempts;
    private int _numberLineValue = -2;
    private int _decimalThousandths = 2375;
    private bool _readRegistered;

    /// <summary>
    /// Initialise les composants de la page.
    /// </summary>
    public FoundationNumberCoursePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Configure le cours demandé par la navigation.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("courseId", out var value)
            || value is not string courseId
            || !FoundationNumberCourseCatalog.TryGet(courseId, out var definition))
        {
            return;
        }

        _definition = definition;
        _course = CourseCatalog.GetCourse(courseId);
        Title = TranslationService.Current.Get(definition.Title);
        ShowCurrentStep();
    }

    /// <summary>
    /// Enregistre une lecture une seule fois lors de l'affichage.
    /// </summary>
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

    /// <summary>
    /// Déplace la valeur de la visualisation vers la gauche.
    /// </summary>
    private void OnDecreaseVisualClicked(object? sender, EventArgs e)
    {
        if (_definition?.Kind == FoundationNumberKind.NegativeNumbers)
        {
            _numberLineValue = Math.Max(-20, _numberLineValue - 1);
        }
        else
        {
            _decimalThousandths = Math.Max(0, _decimalThousandths - 100);
        }

        RefreshVisualization();
    }

    /// <summary>
    /// Déplace la valeur de la visualisation vers la droite.
    /// </summary>
    private void OnIncreaseVisualClicked(object? sender, EventArgs e)
    {
        if (_definition?.Kind == FoundationNumberKind.NegativeNumbers)
        {
            _numberLineValue = Math.Min(20, _numberLineValue + 1);
        }
        else
        {
            _decimalThousandths = Math.Min(9999, _decimalThousandths + 100);
        }

        RefreshVisualization();
    }

    /// <summary>
    /// Génère un nouvel exercice intermédiaire.
    /// </summary>
    private void OnNewExampleClicked(object? sender, EventArgs e)
    {
        CreateExercises(1, TrainingDifficulty.Moderate);
    }

    /// <summary>
    /// Corrige toutes les réponses visibles.
    /// </summary>
    private void OnCheckAnswersClicked(object? sender, EventArgs e)
    {
        if (_course is null || _exercises.Count == 0)
        {
            return;
        }

        _answerAttempts++;
        var text = TranslationService.Current;
        var allCorrect = true;
        for (var index = 0; index < _exercises.Count; index++)
        {
            var exercise = _exercises[index];
            var controls = _exerciseControls[index];
            var correct = ExerciseAnswerValidator.Matches(
                exercise,
                controls.Answer.Text,
                text.CurrentLanguageCode);
            var color = ThemeService.GetColor(correct ? "Success" : "Error");

            controls.Feedback.IsVisible = true;
            controls.Feedback.Style = (Style)Application.Current!.Resources[
                correct ? "SuccessFeedback" : "ErrorFeedback"];
            controls.FeedbackTitle.Text = text.Get(
                correct ? "feedback.correctTitle" : "feedback.incorrectTitle");
            controls.FeedbackTitle.TextColor = color;
            controls.FeedbackText.TextColor = color;
            controls.FeedbackText.Text = correct
                ? LocalizedExerciseText.GetExplanation(exercise)
                : _answerAttempts < 3
                    ? text.Get("course.foundation.hint")
                    : text.Format("course.foundation.answer", exercise.CorrectAnswer);
            allCorrect &= correct;
        }

        if (allCorrect
            && _course.Steps[_currentStepIndex].Kind == CourseStepKind.FinalQuestion)
        {
            _storage.CompleteCourse(_course.Id);
            NextButton.Text = text.Get("navigation.finish");
        }
    }

    /// <summary>
    /// Synchronise la page avec l'étape courante.
    /// </summary>
    private void ShowCurrentStep()
    {
        if (_course is null || _definition is null)
        {
            return;
        }

        var text = TranslationService.Current;
        var step = _course.Steps[_currentStepIndex];
        StepIndicator.Text = text.Format(
            "course.stepIndicator",
            _currentStepIndex + 1,
            _course.Steps.Count);
        StepTitle.Text = text.Get(step.Title);
        StepExplanation.Text = text.Get(step.Explanation);
        StepExample.Text = text.Get(step.Example);
        ExamplePanel.IsVisible = step.Id is "meaning" or "positions";
        VisualPanel.IsVisible = step.Id == "visualize";
        ExercisePanel.IsVisible = step.Id == "try"
            || step.Kind == CourseStepKind.FinalQuestion;
        PreviousButton.IsEnabled = _currentStepIndex > 0;
        NextButton.Text = text.Get(
            _currentStepIndex == _course.Steps.Count - 1
                ? "navigation.quit"
                : "navigation.next");

        if (VisualPanel.IsVisible)
        {
            RefreshVisualization();
        }

        if (ExercisePanel.IsVisible)
        {
            var isValidation = step.Kind == CourseStepKind.FinalQuestion;
            CreateExercises(
                isValidation ? 2 : 1,
                isValidation ? TrainingDifficulty.Hard : TrainingDifficulty.Moderate);
            NewExampleButton.IsVisible = !isValidation;
            Grid.SetColumnSpan(CheckButton, isValidation ? 2 : 1);
        }
    }

    /// <summary>
    /// Reconstruit la visualisation correspondant à la notion courante.
    /// </summary>
    private void RefreshVisualization()
    {
        if (_definition is null)
        {
            return;
        }

        VisualContent.Children.Clear();
        if (_definition.Kind == FoundationNumberKind.NegativeNumbers)
        {
            BuildNumberLine();
        }
        else
        {
            BuildDecimalPlaceTable();
        }
    }

    /// <summary>
    /// Construit une droite graduée centrée sur la valeur sélectionnée.
    /// </summary>
    private void BuildNumberLine()
    {
        var text = TranslationService.Current;
        VisualTitle.Text = text.Get("course.negative.visual.title");
        VisualValue.Text = _numberLineValue.ToString(CultureInfo.CurrentCulture);
        DecreaseButton.Text = "− 1";
        IncreaseButton.Text = "+ 1";

        var grid = new Grid
        {
            ColumnSpacing = 3,
            MinimumWidthRequest = 560
        };
        for (var index = 0; index < 11; index++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            var value = _numberLineValue - 5 + index;
            var label = new Label
            {
                Text = value.ToString(CultureInfo.CurrentCulture),
                FontAttributes = value == _numberLineValue
                    ? FontAttributes.Bold
                    : FontAttributes.None,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = new Thickness(8, 12),
                BackgroundColor = value == _numberLineValue
                    ? ThemeService.GetColor("PrimaryLight")
                    : Colors.Transparent,
                TextColor = value == 0
                    ? ThemeService.GetColor("Success")
                    : ThemeService.GetColor("TextPrimary")
            };
            grid.Add(label, index);
        }

        VisualContent.Children.Add(grid);
        VisualContent.Children.Add(new BoxView
        {
            HeightRequest = 2,
            MinimumWidthRequest = 560,
            Color = ThemeService.GetColor("TextPrimary")
        });
    }

    /// <summary>
    /// Construit le tableau unités, dixièmes, centièmes et millièmes.
    /// </summary>
    private void BuildDecimalPlaceTable()
    {
        var text = TranslationService.Current;
        var digits = _decimalThousandths.ToString("0000", CultureInfo.InvariantCulture);
        VisualTitle.Text = text.Get("course.decimal.visual.title");
        VisualValue.Text = (_decimalThousandths / 1000m)
            .ToString("0.000", CultureInfo.CurrentCulture);
        DecreaseButton.Text = "− 0,1";
        IncreaseButton.Text = "+ 0,1";

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 4,
            RowSpacing = 4,
            MinimumWidthRequest = 600
        };
        var headings = new[]
        {
            "course.decimal.column.units",
            "course.decimal.column.tenths",
            "course.decimal.column.hundredths",
            "course.decimal.column.thousandths"
        };
        for (var index = 0; index < headings.Length; index++)
        {
            var heading = new Label
            {
                Text = text.Get(headings[index]),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = 8,
                TextColor = ThemeService.GetColor("Primary")
            };
            var digit = new Label
            {
                Text = digits[index].ToString(),
                FontAttributes = FontAttributes.Bold,
                FontSize = 28,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = 12,
                BackgroundColor = ThemeService.GetColor("ControlBackground")
            };
            grid.Add(heading, index, 0);
            grid.Add(digit, index, 1);
        }

        VisualContent.Children.Add(grid);
    }

    /// <summary>
    /// Prépare une ou deux questions selon l'étape.
    /// </summary>
    private void CreateExercises(int count, TrainingDifficulty difficulty)
    {
        if (_definition is null)
        {
            return;
        }

        _exercises = count == 2
            ? _generator.CreateValidation(_definition, difficulty)
            : [_generator.CreateExercise(_definition, difficulty)];
        _exerciseControls.Clear();
        ExerciseQuestionsPanel.Children.Clear();
        for (var index = 0; index < _exercises.Count; index++)
        {
            ExerciseQuestionsPanel.Children.Add(
                CreateExerciseCard(_exercises[index], index + 1));
        }

        _answerAttempts = 0;
    }

    /// <summary>
    /// Construit une carte de question et conserve ses contrôles de correction.
    /// </summary>
    private Border CreateExerciseCard(Exercise exercise, int number)
    {
        var text = TranslationService.Current;
        var answer = new Entry
        {
            Keyboard = _definition?.Kind == FoundationNumberKind.NegativeNumbers
                ? Keyboard.Default
                : Keyboard.Numeric,
            Placeholder = text.Get("common.answerPlaceholder")
        };
        var feedbackTitle = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 17
        };
        var feedbackText = new Label();
        var feedback = new Border
        {
            IsVisible = false,
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children = { feedbackTitle, feedbackText }
            }
        };
        _exerciseControls.Add(new ExerciseControls(
            answer,
            feedback,
            feedbackTitle,
            feedbackText));

        return new Border
        {
            BackgroundColor = ThemeService.GetColor("ControlBackground"),
            Stroke = ThemeService.GetColor("Border"),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 12
            },
            Padding = 14,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 13,
                        Text = text.Format("course.foundation.questionNumber", number),
                        TextColor = ThemeService.GetColor("Primary")
                    },
                    new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        Text = LocalizedExerciseText.GetQuestion(exercise)
                    },
                    answer,
                    feedback
                }
            }
        };
    }

    /// <summary>
    /// Retourne à la liste globale des cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }

    /// <summary>
    /// Regroupe les contrôles associés à une question.
    /// </summary>
    private sealed record ExerciseControls(
        Entry Answer,
        Border Feedback,
        Label FeedbackTitle,
        Label FeedbackText);
}
