using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Features.MiddleSchool;

/// <summary>
/// Affiche la fiche d'entrée d'une notion du collège.
/// </summary>
public partial class MiddleSchoolCourseEntryPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Cours et définition sélectionnés par la route.
    /// </summary>
    private Course? _course;
    private MiddleSchoolCourseDefinition? _definition;

    /// <summary>
    /// Initialise les composants et le bouton de retour.
    /// </summary>
    public MiddleSchoolCourseEntryPage()
    {
        InitializeComponent();
        BackButton.Text = TranslationService.Current.Get("navigation.courseList");
    }

    /// <summary>
    /// Charge la notion demandée par la navigation.
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
        RefreshContent();
    }

    /// <summary>
    /// Traduit et affiche les métadonnées du cours.
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
        ObjectiveLabel.Text = text.Format("course.middle.objectiveFormat", title);
        ExampleLabel.Text = _definition.Example;
        SummaryHeading.Text = text.Get("common.summary");
        SummaryLabel.Text = text.Format("course.middle.summaryFormat", title);
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
    /// Ouvre le parcours pédagogique du cours.
    /// </summary>
    private async void OnReadCourseClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(MiddleSchoolCoursePage)}?courseId={_course.Id}");
        }
    }

    /// <summary>
    /// Ouvre une session d'entraînement.
    /// </summary>
    private async void OnTrainClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(MiddleSchoolTrainingPage)}?courseId={_course.Id}");
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
    /// Retourne à la liste globale des cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
