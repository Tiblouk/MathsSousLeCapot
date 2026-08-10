using MathsSousLeCapot.App.Services;

namespace MathsSousLeCapot.App.Controls;

/// <summary>
/// Affiche une explication courte ou la découpe en blocs pédagogiques lisibles.
/// </summary>
public sealed class PedagogicalExplanationView : ContentView
{
    /// <summary>
    /// Conteneur vertical reconstruit lorsque l'étape du cours change.
    /// </summary>
    private readonly VerticalStackLayout _sections = new()
    {
        Spacing = 12
    };

    /// <summary>
    /// Initialise le composant avec son conteneur interne.
    /// </summary>
    public PedagogicalExplanationView()
    {
        Content = _sections;
    }

    /// <summary>
    /// Affiche un texte simple ou plusieurs paragraphes dans des cartes distinctes.
    /// </summary>
    public void SetText(string text)
    {
        _sections.Children.Clear();
        var paragraphs = text.Split(
            ["\r\n\r\n", "\n\n"],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (paragraphs.Length <= 1)
        {
            _sections.Children.Add(CreateBodyLabel(text));
            return;
        }

        foreach (var paragraph in paragraphs)
        {
            _sections.Children.Add(CreateSection(paragraph));
        }
    }

    /// <summary>
    /// Crée une carte en distinguant le titre placé avant le premier deux-points.
    /// </summary>
    private static Border CreateSection(string paragraph)
    {
        var separator = paragraph.IndexOf(':');
        var hasShortHeading = separator is > 0 and < 40;
        var content = new VerticalStackLayout
        {
            Spacing = 6
        };

        if (hasShortHeading)
        {
            content.Children.Add(new Label
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                Text = paragraph[..separator].Trim(),
                TextColor = ThemeService.GetColor("Primary")
            });
            content.Children.Add(CreateBodyLabel(
                paragraph[(separator + 1)..].Trim()));
        }
        else
        {
            content.Children.Add(CreateBodyLabel(paragraph));
        }

        return new Border
        {
            BackgroundColor = ThemeService.GetColor("CardBackground"),
            Stroke = ThemeService.GetColor("Border"),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 14
            },
            Padding = 16,
            Content = content
        };
    }

    /// <summary>
    /// Crée le texte principal commun aux explications simples et structurées.
    /// </summary>
    private static Label CreateBodyLabel(string text)
    {
        return new Label
        {
            FontSize = 17,
            LineBreakMode = LineBreakMode.WordWrap,
            Text = text
        };
    }
}
