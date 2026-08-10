using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.App.Controls;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.Training;

/// <summary>
/// Centralise les éléments d'interface communs aux pages d'entraînement.
/// </summary>
public static class TrainingUi
{
    /// <summary>
    /// Retourne les trois difficultés traduites dans leur ordre fonctionnel.
    /// </summary>
    public static string[] CreateDifficultyLabels()
    {
        var text = TranslationService.Current;
        return
        [
            text.Get("common.easy"),
            text.Get("common.moderate"),
            text.Get("common.hard")
        ];
    }

    /// <summary>
    /// Remplit le récapitulatif avec toutes les informations de correction.
    /// </summary>
    public static void FillSummary(
        VerticalStackLayout resultsList,
        Label scoreLabel,
        TrainingSession session)
    {
        var text = TranslationService.Current;
        resultsList.Children.Clear();
        scoreLabel.Text = text.Format(
            "feedback.scoreTotal",
            session.CorrectCount,
            session.Results.Count);

        foreach (var result in session.Results)
        {
            var feedbackColor = ThemeService.GetColor(
                result.IsCorrect ? "Success" : "Error");
            var statusKey = result.IsCorrect
                ? "feedback.correctTitle"
                : "feedback.wrongTitle";

            var resultCard = new Border
            {
                Style = (Style)Application.Current!.Resources[
                    result.IsCorrect ? "SuccessFeedback" : "ErrorFeedback"],
                Content = new VerticalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 17,
                            Text = text.Get(statusKey),
                            TextColor = feedbackColor
                        },
                        new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            Text = LocalizedExerciseText.GetQuestion(result.Exercise)
                        },
                        CreateSolvedCalculation(result.Exercise),
                        new Label
                        {
                            Text = text.Format(
                                "feedback.givenAnswer",
                                result.GivenAnswer),
                            TextColor = feedbackColor
                        },
                        new Label
                        {
                            Text = text.Format(
                                "feedback.correctAnswer",
                                result.Exercise.CorrectAnswer)
                        },
                        new Label
                        {
                            Text = LocalizedExerciseText.GetExplanation(result.Exercise),
                            TextColor = ThemeService.GetColor("TextSecondary")
                        },
                        new Label
                        {
                            FontSize = 13,
                            Text = text.Get("correction.detail.openHint"),
                            TextColor = ThemeService.GetColor("Primary")
                        }
                    }
                }
            };
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (_, _) =>
                await Shell.Current.Navigation.PushModalAsync(
                    new DetailedCorrectionPage(result));
            resultCard.GestureRecognizers.Add(tapGesture);
            SemanticProperties.SetDescription(resultCard, text.Get(statusKey));
            resultsList.Children.Add(resultCard);
        }
    }

    /// <summary>
    /// Affiche la correction posée lorsqu'une opération la fournit.
    /// </summary>
    private static View CreateSolvedCalculation(Exercise exercise)
    {
        return exercise.WrittenCalculation is null
            ? new Grid { IsVisible = false }
            : new WrittenCalculationView(
                exercise.WrittenCalculation,
                revealSolution: true);
    }
}
