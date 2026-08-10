using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Features.FoundationNumbers;

/// <summary>
/// Affiche la fiche d'entrée d'un cours fondamental sur les nombres.
/// </summary>
public partial class FoundationNumberCourseEntryPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Cours et définition sélectionnés par la route.
    /// </summary>
    private Course? _course;
    private FoundationNumberCourseDefinition? _definition;

    /// <summary>
    /// Initialise la page et son bouton de retour.
    /// </summary>
    public FoundationNumberCourseEntryPage()
    {
        InitializeComponent();
        BackButton.Text = TranslationService.Current.Get("navigation.courseList");
    }

    /// <summary>
    /// Charge le cours demandé par la navigation.
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
        Title = text.Get(_definition.Title);
        BadgeLabel.Text = text.Get(_course.Level);
        TitleLabel.Text = Title;
        ObjectiveHeading.Text = text.Get("common.objective");
        ObjectiveLabel.Text = text.Get(_definition.Objective);
        SummaryHeading.Text = text.Get("common.summary");
        SummaryLabel.Text = text.Get(_definition.Summary);
        PrerequisiteHeading.Text = text.Get("common.prerequisites");
        PrerequisitePanel.IsVisible = _course.Prerequisites.Count > 0;
        if (_course.Prerequisites.FirstOrDefault() is { } prerequisite)
        {
            PrerequisiteButton.Text = text.Get(prerequisite.Title);
        }

        ReadButton.Text = text.Get("actions.readCourse");
        TrainButton.Text = text.Get("actions.train");
    }

    /// <summary>
    /// Ouvre le parcours pédagogique.
    /// </summary>
    private async void OnReadCourseClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(FoundationNumberCoursePage)}?courseId={_course.Id}");
        }
    }

    /// <summary>
    /// Ouvre une session de cinq exercices.
    /// </summary>
    private async void OnTrainClicked(object? sender, EventArgs e)
    {
        if (_course is not null)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(FoundationNumberTrainingPage)}?courseId={_course.Id}");
        }
    }

    /// <summary>
    /// Ouvre le cours prérequis conseillé.
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
