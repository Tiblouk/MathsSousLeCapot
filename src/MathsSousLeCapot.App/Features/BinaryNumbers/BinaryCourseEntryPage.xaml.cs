using MathsSousLeCapot.App.Features.PositionalCounter;

namespace MathsSousLeCapot.App.Features.BinaryNumbers;

/// <summary>
/// Affiche les métadonnées et actions du cours de base deux.
/// </summary>
public partial class BinaryCourseEntryPage : ContentPage
{
    /// <summary>
    /// Initialise la fiche d'entrée.
    /// </summary>
    public BinaryCourseEntryPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Ouvre le parcours pédagogique.
    /// </summary>
    private async void OnReadCourseClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BinaryCoursePage));
    }

    /// <summary>
    /// Ouvre l'entraînement binaire.
    /// </summary>
    private async void OnTrainClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BinaryTrainingPage));
    }

    /// <summary>
    /// Ouvre le compteur décimal prérequis.
    /// </summary>
    private async void OnOpenPrerequisiteClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CourseEntryPage));
    }

    /// <summary>
    /// Retourne à la liste du chapitre.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
