using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Features.PrimaryCourses;

/// <summary>
/// Affiche la fiche d'entrée d'une notion générique du niveau primaire.
/// </summary>
public partial class PrimaryCourseEntryPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Cours et définition sélectionnés par la route.
    /// </summary>
    private Course? _course;
    private PrimaryCourseDefinition? _definition;

    /// <summary>
    /// Initialise les composants de la fiche.
    /// </summary>
    public PrimaryCourseEntryPage()
    {
        InitializeComponent();
        BackButton.Text = TranslationService.Current.Get("navigation.courseList");
    }

    /// <summary>
    /// Charge la notion primaire demandée par la navigation.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("courseId", out var value)
            || value is not string courseId
            || !PrimaryCourseCatalog.TryGet(courseId, out var definition))
        {
            return;
        }

        _definition = definition;
        _course = CourseCatalog.GetCourse(courseId);
        RefreshContent();
    }

    /// <summary>
    /// Traduit et affiche les métadonnées de la notion.
    /// </summary>
    private void RefreshContent()
    {
        if (_course is null || _definition is null)
        {
            return;
        }

        var text = TranslationService.Current;
        var title = text.Get(_definition.Title);
        Title = title;
        BadgeLabel.Text = text.Get(_definition.Level);
        TitleLabel.Text = title;
        ObjectiveHeading.Text = text.Get("common.objective");
        ObjectiveLabel.Text = text.Format("course.primary.objectiveFormat", title);
        ExampleLabel.Text = _definition.Example;
        SummaryHeading.Text = text.Get("common.summary");
        SummaryLabel.Text = text.Format("course.primary.summaryFormat", title);
        PrerequisiteHeading.Text = text.Get("common.prerequisites");
        PrerequisitePanel.IsVisible = _course.Prerequisites.Count > 0;
        if (_course.Prerequisites.Count > 0)
        {
            PrerequisiteButton.Text = text.Get(_course.Prerequisites[0].Title);
        }

        ReadButton.Text = text.Get("actions.readCourse");
        TrainButton.Text = text.Get("actions.train");
    }

    /// <summary>
    /// Ouvre le parcours pédagogique de la notion.
    /// </summary>
    private async void OnReadCourseClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(PrimaryCoursePage)}?courseId={_course.Id}");
        }
    }

    /// <summary>
    /// Ouvre l'entraînement de la notion.
    /// </summary>
    private async void OnTrainClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(PrimaryTrainingPage)}?courseId={_course.Id}");
        }
    }

    /// <summary>
    /// Ouvre la fiche du prérequis conseillé.
    /// </summary>
    private async void OnPrerequisiteClicked(object? sender, EventArgs e)
    {
        if (_course?.Prerequisites.FirstOrDefault() is not { } prerequisite)
        {
            return;
        }

        if (NavigationService.TryGetCourseEntryRoute(prerequisite.CourseId, out var route))
        {
            await Shell.Current.GoToAsync(route);
        }
    }

    /// <summary>
    /// Retourne à la liste de tous les cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
