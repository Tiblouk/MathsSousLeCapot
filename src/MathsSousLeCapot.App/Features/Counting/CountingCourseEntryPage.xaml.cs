namespace MathsSousLeCapot.App.Features.Counting;

/// <summary>
/// Affiche les métadonnées et actions du cours de comptage.
/// </summary>
public partial class CountingCourseEntryPage : ContentPage
{
    /// <summary>
    /// Initialise la fiche d'entrée.
    /// </summary>
    public CountingCourseEntryPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Ouvre le parcours pédagogique.
    /// </summary>
    private async void OnReadCourseClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CountingCoursePage));
    }

    /// <summary>
    /// Ouvre l'entraînement associé.
    /// </summary>
    private async void OnTrainClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CountingTrainingPage));
    }

    /// <summary>
    /// Retourne à la liste du chapitre.
    /// </summary>
    private async void OnBackToCoursesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//understand-numbers");
    }
}
