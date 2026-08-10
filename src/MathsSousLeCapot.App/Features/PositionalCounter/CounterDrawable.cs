using MathsSousLeCapot.Core.Mathematics.PositionalNumeration;
using MathsSousLeCapot.App.Services;

namespace MathsSousLeCapot.App.Features.PositionalCounter;

/// <summary>
/// Dessine les colonnes du compteur dans un GraphicsView MAUI.
/// </summary>
public sealed class CounterDrawable : IDrawable
{
    /// <summary>
    /// Chiffres positionnels à représenter de gauche à droite.
    /// </summary>
    public IReadOnlyList<PositionalDigit> Digits { get; set; } = [];

    /// <summary>
    /// Positions mises en évidence après la dernière transition.
    /// </summary>
    public IReadOnlySet<int> HighlightedPositions { get; set; } = new HashSet<int>();

    /// <summary>
    /// Dessine les cartes, chiffres et libellés dans la zone disponible.
    /// </summary>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Digits.Count == 0)
        {
            return;
        }

        // Les dimensions sont calculées à chaque rendu pour rester responsives.
        const float gap = 8;
        var availableWidth = Math.Max(0, dirtyRect.Width - 16);
        var columnWidth = Math.Min(96, (availableWidth - gap * (Digits.Count - 1)) / Digits.Count);
        var totalWidth = columnWidth * Digits.Count + gap * (Digits.Count - 1);
        var startX = dirtyRect.Center.X - totalWidth / 2;
        var top = Math.Max(8, dirtyRect.Center.Y - 70);

        canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
        canvas.FontColor = ThemeService.GetColor("TextPrimary");
        canvas.FontSize = Math.Clamp(columnWidth * 0.48f, 10, 42);
        canvas.StrokeSize = 2;

        for (var index = 0; index < Digits.Count; index++)
        {
            var digit = Digits[index];
            var left = startX + index * (columnWidth + gap);
            var rectangle = new RectF(left, top, columnWidth, 92);
            var highlighted = HighlightedPositions.Contains(digit.Position);

            canvas.FillColor = highlighted
                ? ThemeService.GetColor("PrimaryLight")
                : ThemeService.GetColor("ControlBackground");
            canvas.StrokeColor = highlighted
                ? ThemeService.GetColor("Primary")
                : ThemeService.GetColor("Border");
            canvas.FillRoundedRectangle(rectangle, 14);
            canvas.DrawRoundedRectangle(rectangle, 14);
            canvas.DrawString(
                digit.Value.ToString(),
                rectangle,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);

            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.FontSize = Math.Clamp(columnWidth * 0.16f, 8, 14);
            canvas.FontColor = ThemeService.GetColor("TextSecondary");
            canvas.DrawString(
                digit.Label,
                left - 4,
                top + 104,
                columnWidth + 8,
                36,
                HorizontalAlignment.Center,
                VerticalAlignment.Top,
                TextFlow.OverflowBounds);
            canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.FontSize = Math.Clamp(columnWidth * 0.48f, 10, 42);
            canvas.FontColor = ThemeService.GetColor("TextPrimary");
        }
    }
}
