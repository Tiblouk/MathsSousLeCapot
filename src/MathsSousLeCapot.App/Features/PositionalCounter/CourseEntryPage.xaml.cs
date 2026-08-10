using MathsSousLeCapot.App.Features.Counting;

namespace MathsSousLeCapot.App.Features.PositionalCounter;

/// <summary>
/// Affiche les métadonnées et actions du compteur positionnel.
/// </summary>
public partial class CourseEntryPage : ContentPage
{
    /// <summary>
    /// Initialise la fiche d'entrée.
    /// </summary>
    public CourseEntryPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Ouvre le parcours pédagogique.
    /// </summary>
    private async void OnReadCourseClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CounterCoursePage));
    }

    /// <summary>
    /// Ouvre l'entraînement associé.
    /// </summary>
    private async void OnTrainClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CounterTrainingPage));
    }

    /// <summary>
    /// Retourne à la liste des cours.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }

    /// <summary>
    /// Ouvre le cours prérequis de comptage.
    /// </summary>
    private async void OnOpenPrerequisiteClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CountingCourseEntryPage));
    }
}
