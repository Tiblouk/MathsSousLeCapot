using System.Globalization;
using MathsSousLeCapot.App.Controls;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Mathematics.Operations;

namespace MathsSousLeCapot.App.Features.PrimaryCourses;

/// <summary>
/// Affiche une ardoise où l'élève complète directement les cases d'un calcul posé.
/// </summary>
public partial class PrimaryCalculationAssistantPage : ContentPage
{
    /// <summary>
    /// Plus grand opérande accepté afin de conserver une pose lisible sur téléphone.
    /// </summary>
    private const int MaximumOperand = 9999;

    /// <summary>
    /// Plan courant conservé jusqu'au changement d'opération.
    /// </summary>
    private GuidedCalculationPlan? _plan;

    /// <summary>
    /// Initialise une ardoise vide depuis un bouton sans opération sélectionnée.
    /// </summary>
    public PrimaryCalculationAssistantPage()
    {
        InitializeComponent();
        InitializeOperationPicker();
    }

    /// <summary>
    /// Ouvre immédiatement la pose correspondant à l'exercice sélectionné.
    /// </summary>
    public PrimaryCalculationAssistantPage(WrittenCalculation calculation)
        : this()
    {
        OperationPicker.SelectedIndex = (int)calculation.Kind;
        LeftOperandEntry.Text = calculation.LeftOperand.ToString(
            CultureInfo.InvariantCulture);
        RightOperandEntry.Text = calculation.RightOperand.ToString(
            CultureInfo.InvariantCulture);
        StartPlan(
            calculation.Kind,
            calculation.LeftOperand,
            calculation.RightOperand);
    }

    /// <summary>
    /// Traduit les quatre opérations dans l'ordre stable de leur énumération.
    /// </summary>
    private void InitializeOperationPicker()
    {
        var text = TranslationService.Current;
        OperationPicker.ItemsSource = new[]
        {
            text.Get("primaryAssistant.operation.addition"),
            text.Get("primaryAssistant.operation.subtraction"),
            text.Get("primaryAssistant.operation.multiplication"),
            text.Get("primaryAssistant.operation.division")
        };
    }

    /// <summary>
    /// Valide les opérandes puis remplace le formulaire par la pose interactive.
    /// </summary>
    private void OnStartClicked(object? sender, EventArgs e)
    {
        var text = TranslationService.Current;
        if (!TryReadOperand(LeftOperandEntry.Text, out var leftOperand)
            || !TryReadOperand(RightOperandEntry.Text, out var rightOperand))
        {
            ShowSetupError(text.Get("primaryAssistant.error.invalidNumbers"));
            return;
        }

        var kind = (WrittenCalculationKind)Math.Max(
            0,
            OperationPicker.SelectedIndex);
        if (kind == WrittenCalculationKind.Subtraction
            && rightOperand > leftOperand)
        {
            ShowSetupError(text.Get("primaryAssistant.error.subtractionOrder"));
            return;
        }

        if (kind == WrittenCalculationKind.Division && rightOperand == 0)
        {
            ShowSetupError(text.Get("primaryAssistant.error.divisionByZero"));
            return;
        }

        StartPlan(kind, leftOperand, rightOperand);
    }

    /// <summary>
    /// Construit la pose et active sa première case sans montrer le résultat.
    /// </summary>
    private void StartPlan(
        WrittenCalculationKind kind,
        int leftOperand,
        int rightOperand)
    {
        _plan = GuidedCalculationPlanBuilder.Create(
            kind,
            leftOperand,
            rightOperand);
        SetupFeedback.IsVisible = false;
        SetupPanel.IsVisible = false;
        IntroLabel.IsVisible = false;
        GuidancePanel.IsVisible = true;
        CompletionActions.IsVisible = false;
        GuidanceMessage.Text = TranslationService.Current.Get(
            "primaryAssistant.activeCell");
        GuidanceMessage.TextColor = ThemeService.GetColor("Primary");
        GuidedCalculation.SetPlan(_plan);
    }

    /// <summary>
    /// Met à jour le compteur et signale une erreur directement sous la pose.
    /// </summary>
    private void OnCalculationProgressChanged(
        object? sender,
        GuidedCalculationProgressEventArgs e)
    {
        var text = TranslationService.Current;
        StepIndicator.Text = text.Format(
            "primaryAssistant.stepIndicator",
            e.StepNumber,
            e.TotalSteps);
        GuidanceMessage.Text = text.Get(
            e.IsIncorrect
                ? "primaryAssistant.incorrect"
                : "primaryAssistant.activeCell");
        GuidanceMessage.TextColor = ThemeService.GetColor(
            e.IsIncorrect ? "Error" : "Primary");
    }

    /// <summary>
    /// Conserve la pose remplie et affiche seulement les actions de fin.
    /// </summary>
    private void OnCalculationCompleted(object? sender, EventArgs e)
    {
        var text = TranslationService.Current;
        StepIndicator.Text = text.Get("primaryAssistant.completedIndicator");
        GuidanceMessage.Text = text.Get("primaryAssistant.completedText");
        GuidanceMessage.TextColor = ThemeService.GetColor("Success");
        CompletionActions.IsVisible = true;
    }

    /// <summary>
    /// Accepte uniquement un entier naturel assez court pour l'affichage mobile.
    /// </summary>
    private static bool TryReadOperand(string? value, out int operand)
    {
        return int.TryParse(
                value,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out operand)
            && operand is >= 0 and <= MaximumOperand;
    }

    /// <summary>
    /// Affiche une erreur de préparation sous les deux nombres.
    /// </summary>
    private void ShowSetupError(string message)
    {
        SetupFeedback.Text = message;
        SetupFeedback.IsVisible = true;
    }

    /// <summary>
    /// Abandonne la pose et réaffiche le choix de l'opération.
    /// </summary>
    private void OnChangeOperationClicked(object? sender, EventArgs e)
    {
        _plan = null;
        GuidancePanel.IsVisible = false;
        SetupPanel.IsVisible = true;
        IntroLabel.IsVisible = true;
        SetupFeedback.IsVisible = false;
    }

    /// <summary>
    /// Ferme la page modale et retourne au cours ou à l'entraînement.
    /// </summary>
    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
