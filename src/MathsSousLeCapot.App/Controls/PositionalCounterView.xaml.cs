using MathsSousLeCapot.App.Features.PositionalCounter;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.Core.Mathematics.PositionalNumeration;

namespace MathsSousLeCapot.App.Controls;

/// <summary>
/// Composant visuel réutilisable qui affiche et manipule un compteur positionnel.
/// </summary>
public partial class PositionalCounterView : ContentView
{
    /// <summary>
    /// Dessinateur associé au GraphicsView.
    /// </summary>
    private readonly CounterDrawable _drawable = new();

    /// <summary>
    /// État observable du compteur présenté à l'utilisateur.
    /// </summary>
    private readonly CounterViewModel _viewModel = new();

    /// <summary>
    /// Empêche deux transitions de modifier simultanément le compteur.
    /// </summary>
    private bool _isAnimating;

    /// <summary>
    /// Initialise les liaisons et le moteur de dessin.
    /// </summary>
    public PositionalCounterView()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        CounterGraphics.Drawable = _drawable;
        BasePicker.ItemsSource = new[]
        {
            TranslationService.Current.Get("counter.baseTen"),
            TranslationService.Current.Get("counter.baseTwo")
        };
        _viewModel.PropertyChanged += (_, _) => RefreshDrawing();
        RefreshDrawing();
    }

    /// <summary>
    /// Configure le composant avec une base et une valeur d'exemple.
    /// </summary>
    public void UseExample(
        NumeralBase numeralBase,
        int value,
        bool allowBaseSelection = false)
    {
        BasePicker.IsVisible = allowBaseSelection;
        BasePicker.SelectedIndex = numeralBase == NumeralBase.Decimal ? 0 : 1;
        _viewModel.ChangeBase(numeralBase);
        _viewModel.SetValue(value);
        RefreshDrawing();
    }

    /// <summary>
    /// Ajoute une unité puis anime les colonnes concernées.
    /// </summary>
    private async void OnIncrementClicked(object? sender, EventArgs e)
    {
        await ApplyChangeAsync(1);
    }

    /// <summary>
    /// Retire une unité.
    /// </summary>
    private async void OnDecrementClicked(object? sender, EventArgs e)
    {
        await ApplyChangeAsync(-1);
    }

    /// <summary>
    /// Ajoute une dizaine.
    /// </summary>
    private async void OnAddTenClicked(object? sender, EventArgs e)
    {
        await ApplyChangeAsync(10);
    }

    /// <summary>
    /// Retire une dizaine.
    /// </summary>
    private async void OnRemoveTenClicked(object? sender, EventArgs e)
    {
        await ApplyChangeAsync(-10);
    }

    /// <summary>
    /// Ajoute une centaine.
    /// </summary>
    private async void OnAddHundredClicked(object? sender, EventArgs e)
    {
        await ApplyChangeAsync(100);
    }

    /// <summary>
    /// Retire une centaine.
    /// </summary>
    private async void OnRemoveHundredClicked(object? sender, EventArgs e)
    {
        await ApplyChangeAsync(-100);
    }

    /// <summary>
    /// Change la représentation lorsque l'utilisateur choisit une autre base.
    /// </summary>
    private void OnBaseChanged(object? sender, EventArgs e)
    {
        _viewModel.ChangeBase(
            BasePicker.SelectedIndex == 1
                ? NumeralBase.Binary
                : NumeralBase.Decimal);
        RefreshDrawing();
    }

    /// <summary>
    /// Synchronise le dessin et l'état activé des boutons.
    /// </summary>
    private void RefreshDrawing()
    {
        _drawable.Digits = _viewModel.Digits;
        _drawable.HighlightedPositions = _viewModel.ChangedPositions;
        DecrementButton.IsEnabled = !_isAnimating && _viewModel.CanDecrement;
        IncrementButton.IsEnabled = !_isAnimating && _viewModel.CanIncrement;
        AddTenButton.IsEnabled = !_isAnimating && _viewModel.CanAddTen;
        RemoveTenButton.IsEnabled = !_isAnimating && _viewModel.CanRemoveTen;
        AddHundredButton.IsEnabled = !_isAnimating && _viewModel.CanAddHundred;
        RemoveHundredButton.IsEnabled = !_isAnimating && _viewModel.CanRemoveHundred;
        BasePicker.IsEnabled = !_isAnimating;
        CounterGraphics.Invalidate();
    }

    /// <summary>
    /// Applique un déplacement arbitraire puis joue l'animation pédagogique.
    /// </summary>
    private async Task ApplyChangeAsync(int amount)
    {
        if (_isAnimating)
        {
            return;
        }

        _isAnimating = true;
        try
        {
            var transition = _viewModel.Add(amount);
            RefreshDrawing();
            await CounterAnimations.PlayTransitionAsync(
                CounterGraphics,
                transition.HasCarry || transition.HasBorrow);
        }
        finally
        {
            _isAnimating = false;
            RefreshDrawing();
        }
    }
}
