using MathsSousLeCapot.App.Controls;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.Training;

/// <summary>
/// Affiche une correction détaillée dans une page modale fermable.
/// </summary>
public partial class DetailedCorrectionPage : ContentPage
{
    /// <summary>
    /// Résultat dont la correction est détaillée.
    /// </summary>
    private readonly ExerciseResult _result;

    /// <summary>
    /// Initialise la modale avec le résultat sélectionné.
    /// </summary>
    public DetailedCorrectionPage(ExerciseResult result)
    {
        _result = result;
        InitializeComponent();
        FillContent();
    }

    /// <summary>
    /// Remplit les zones de texte, le statut et le calcul posé éventuel.
    /// </summary>
    private void FillContent()
    {
        var text = TranslationService.Current;
        var feedbackColor = ThemeService.GetColor(
            _result.IsCorrect ? "Success" : "Error");
        var statusKey = _result.IsCorrect
            ? "feedback.correctTitle"
            : "feedback.wrongTitle";

        Title = text.Get("correction.detail.title");
        TitleLabel.Text = Title;
        StatusCard.Style = (Style)Application.Current!.Resources[
            _result.IsCorrect ? "SuccessFeedback" : "ErrorFeedback"];
        StatusLabel.Text = text.Get(statusKey);
        StatusLabel.TextColor = feedbackColor;
        StatusExplanationLabel.Text = DetailedExerciseExplanation.CreateStatus(
            _result);
        QuestionLabel.Text = LocalizedExerciseText.GetQuestion(_result.Exercise);
        GivenAnswerLabel.Text = string.IsNullOrWhiteSpace(_result.GivenAnswer)
            ? text.Get("correction.detail.emptyAnswer")
            : _result.GivenAnswer;
        GivenAnswerLabel.TextColor = feedbackColor;
        CorrectAnswerLabel.Text = _result.Exercise.CorrectAnswer;
        DetailedExplanationLabel.Text =
            DetailedExerciseExplanation.CreateExplanation(_result);

        CalculationPanel.Children.Clear();
        if (_result.Exercise.WrittenCalculation is not null)
        {
            CalculationPanel.Children.Add(new WrittenCalculationView(
                _result.Exercise.WrittenCalculation,
                revealSolution: true));
        }
    }

    /// <summary>
    /// Ferme la page avec le bouton ou la commande retour clavier/système.
    /// </summary>
    private async Task CloseAsync()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }

    /// <summary>
    /// Ferme la modale depuis les boutons visibles.
    /// </summary>
    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await CloseAsync();
    }

    /// <summary>
    /// Intercepte le retour système, qui correspond aussi à Échap sur Windows.
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        _ = CloseAsync();
        return true;
    }
}
