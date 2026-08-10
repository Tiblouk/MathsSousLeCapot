namespace MathsSousLeCapot.App.Features.PositionalCounter;

/// <summary>
/// Regroupe les animations qui rendent visibles les retenues et emprunts.
/// </summary>
public static class CounterAnimations
{
    /// <summary>
    /// Joue une impulsion simple ou une animation renforcée lorsque plusieurs colonnes changent.
    /// </summary>
    public static async Task PlayTransitionAsync(
        VisualElement element,
        bool crossesColumn)
    {
        if (!crossesColumn)
        {
            await element.ScaleTo(1.03, 90, Easing.CubicOut);
            await element.ScaleTo(1, 90, Easing.CubicIn);
            return;
        }

        await Task.WhenAll(
            element.ScaleTo(1.05, 140, Easing.CubicOut),
            element.FadeTo(0.72, 140, Easing.CubicOut));
        await Task.WhenAll(
            element.ScaleTo(1, 220, Easing.BounceOut),
            element.FadeTo(1, 180, Easing.CubicIn));
    }
}
