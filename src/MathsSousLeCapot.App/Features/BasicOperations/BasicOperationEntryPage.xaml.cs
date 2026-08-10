using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Features.BasicOperations;

/// <summary>
/// Affiche la fiche d'entrée d'un cours d'opération fondamentale.
/// </summary>
public partial class BasicOperationEntryPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Cours sélectionné par la route.
    /// </summary>
    private Course? _course;

    /// <summary>
    /// Initialise les composants visuels communs aux deux opérations.
    /// </summary>
    public BasicOperationEntryPage()
    {
        InitializeComponent();
        BackButton.Text = TranslationService.Current.Get("navigation.courseList");
    }

    /// <summary>
    /// Charge le cours demandé dans les paramètres de navigation.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("courseId", out var value)
            || value is not string courseId)
        {
            return;
        }

        _course = CourseCatalog.GetCourse(courseId);
        RefreshContent();
    }

    /// <summary>
    /// Traduit et affiche les métadonnées du cours sélectionné.
    /// </summary>
    private void RefreshContent()
    {
        if (_course is null)
        {
            return;
        }

        var text = TranslationService.Current;
        Title = text.Get(_course.Title);
        BadgeLabel.Text = text.Format("course.operation.badge", text.Get(_course.Title));
        TitleLabel.Text = text.Get(_course.Title);
        ObjectiveHeading.Text = text.Get("common.objective");
        ObjectiveLabel.Text = text.Get(_course.Objective);
        SummaryHeading.Text = text.Get("common.summary");
        SummaryLabel.Text = text.Get(_course.Summary);
        PrerequisiteHeading.Text = text.Get("common.prerequisites");
        PrerequisiteButton.Text = text.Get(_course.Prerequisites.Single().Title);
        ReadButton.Text = text.Get("actions.readCourse");
        TrainButton.Text = text.Get("actions.train");
    }

    /// <summary>
    /// Ouvre le parcours pédagogique du cours sélectionné.
    /// </summary>
    private async void OnReadCourseClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(BasicOperationCoursePage)}?courseId={_course.Id}");
        }
    }

    /// <summary>
    /// Ouvre l'entraînement du cours sélectionné.
    /// </summary>
    private async void OnTrainClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(BasicOperationTrainingPage)}?courseId={_course.Id}");
        }
    }

    /// <summary>
    /// Ouvre la fiche du prérequis conseillé.
    /// </summary>
    private async void OnPrerequisiteClicked(object? sender, EventArgs e)
    {
        if (_course is null)
        {
            return;
        }

        var prerequisite = _course.Prerequisites.Single();
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
