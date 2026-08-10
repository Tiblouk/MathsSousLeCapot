using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Mathematics.Operations;
using Microsoft.Maui.Controls.Shapes;

namespace MathsSousLeCapot.App.Controls;

/// <summary>
/// Affiche un calcul posé responsive, incomplet pendant la question et détaillé après correction.
/// </summary>
public sealed class WrittenCalculationView : ContentView
{
    /// <summary>
    /// Largeur commune d'une colonne de chiffre.
    /// </summary>
    private const double DigitWidth = 30;

    /// <summary>
    /// Calcul actuellement représenté.
    /// </summary>
    private WrittenCalculation? _calculation;

    /// <summary>
    /// Indique si le résultat et les étapes intermédiaires sont visibles.
    /// </summary>
    private bool _revealSolution;

    /// <summary>
    /// Initialise un contrôle vide utilisable depuis XAML.
    /// </summary>
    public WrittenCalculationView()
    {
        HorizontalOptions = LayoutOptions.Center;
    }

    /// <summary>
    /// Initialise directement le contrôle avec un calcul.
    /// </summary>
    public WrittenCalculationView(
        WrittenCalculation calculation,
        bool revealSolution)
        : this()
    {
        SetCalculation(calculation, revealSolution);
    }

    /// <summary>
    /// Remplace le calcul affiché et reconstruit sa représentation.
    /// </summary>
    public void SetCalculation(
        WrittenCalculation calculation,
        bool revealSolution)
    {
        _calculation = calculation;
        _revealSolution = revealSolution;
        Content = BuildContent();
        IsVisible = true;
    }

    /// <summary>
    /// Masque le contrôle lorsqu'aucun calcul posé n'est pertinent.
    /// </summary>
    public void Clear()
    {
        _calculation = null;
        Content = null;
        IsVisible = false;
    }

    /// <summary>
    /// Construit la carte correspondant au type d'opération.
    /// </summary>
    private View BuildContent()
    {
        if (_calculation is null)
        {
            return new Grid();
        }

        var body = _calculation.Kind == WrittenCalculationKind.Division
            ? BuildDivision(_calculation)
            : BuildVerticalOperation(_calculation);
        var panel = new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 13,
                    Text = TranslationService.Current.Get(
                        _revealSolution
                            ? "writtenCalculation.solutionTitle"
                            : "writtenCalculation.questionTitle"),
                    TextColor = ThemeService.GetColor("Primary")
                },
                body
            }
        };

        return new Border
        {
            BackgroundColor = ThemeService.GetColor("ControlBackground"),
            Stroke = ThemeService.GetColor("Border"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 14,
            Content = panel
        };
    }

    /// <summary>
    /// Construit une addition, une soustraction ou une multiplication en colonnes.
    /// </summary>
    private View BuildVerticalOperation(WrittenCalculation calculation)
    {
        var symbol = calculation.Kind switch
        {
            WrittenCalculationKind.Addition => "+",
            WrittenCalculationKind.Subtraction => "−",
            WrittenCalculationKind.Multiplication => "×",
            _ => throw new ArgumentOutOfRangeException(nameof(calculation))
        };
        var partialProducts = calculation.Kind == WrittenCalculationKind.Multiplication
            && _revealSolution
            ? WrittenCalculationBuilder.GetPartialProducts(calculation)
            : [];
        var values = new List<int>
        {
            calculation.LeftOperand,
            calculation.RightOperand,
            calculation.Result
        };
        values.AddRange(partialProducts);
        var digitCount = Math.Max(1, values.Max().ToString().Length);
        var panel = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 2
        };

        if (_revealSolution)
        {
            var note = CreateOperationNote(calculation);
            if (note is not null)
            {
                panel.Children.Add(note);
            }

            if (calculation.Kind == WrittenCalculationKind.Addition)
            {
                panel.Children.Add(CreateCarryRow(calculation, digitCount));
            }
        }

        panel.Children.Add(CreateDigitRow(
            calculation.LeftOperand.ToString(),
            digitCount,
            string.Empty,
            ThemeService.GetColor("TextPrimary")));
        panel.Children.Add(CreateDigitRow(
            calculation.RightOperand.ToString(),
            digitCount,
            symbol,
            ThemeService.GetColor("TextPrimary")));
        panel.Children.Add(CreateSeparator(digitCount));

        if (partialProducts.Count > 1)
        {
            for (var index = 0; index < partialProducts.Count; index++)
            {
                panel.Children.Add(CreateDigitRow(
                    partialProducts[index].ToString(),
                    digitCount,
                    string.Empty,
                    ThemeService.GetColor("Primary"),
                    trailingPlaces: index));
            }

            panel.Children.Add(CreateSeparator(digitCount));
        }

        panel.Children.Add(CreateDigitRow(
            _revealSolution
                ? calculation.Result.ToString()
                : new string('?', digitCount),
            digitCount,
            string.Empty,
            ThemeService.GetColor(_revealSolution ? "Success" : "TextSecondary")));
        return panel;
    }

    /// <summary>
    /// Construit la potence et les étapes successives d'une division.
    /// </summary>
    private View BuildDivision(WrittenCalculation calculation)
    {
        var text = TranslationService.Current;
        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            ColumnSpacing = 0,
            RowSpacing = 0,
            HorizontalOptions = LayoutOptions.Center
        };
        var dividend = CreateLargeNumberLabel(calculation.LeftOperand.ToString());
        dividend.Padding = new Thickness(0, 4, 14, 4);
        header.Children.Add(dividend);
        var divisor = CreateLargeNumberLabel(calculation.RightOperand.ToString());
        divisor.Padding = new Thickness(14, 4, 14, 4);
        divisor.BackgroundColor = ThemeService.GetColor("PrimaryLight");
        header.Children.Add(divisor);
        Grid.SetColumn(divisor, 1);

        var quotientBorder = new Border
        {
            Stroke = ThemeService.GetColor("TextPrimary"),
            StrokeThickness = 0,
            Padding = new Thickness(14, 5),
            Content = CreateLargeNumberLabel(
                _revealSolution
                    ? calculation.Result.ToString()
                    : "?")
        };
        quotientBorder.StrokeShape = new RoundRectangle { CornerRadius = 0 };
        header.Children.Add(quotientBorder);
        Grid.SetColumn(quotientBorder, 1);
        Grid.SetRow(quotientBorder, 1);

        if (_revealSolution)
        {
            var workings = BuildDivisionWorkings(calculation);
            header.Children.Add(workings);
            Grid.SetRow(workings, 1);
        }

        var divider = new BoxView
        {
            BackgroundColor = ThemeService.GetColor("TextPrimary"),
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 2
        };
        header.Children.Add(divider);
        Grid.SetColumn(divider, 1);
        Grid.SetRowSpan(divider, 2);
        var quotientLine = new BoxView
        {
            BackgroundColor = ThemeService.GetColor("TextPrimary"),
            HeightRequest = 2,
            VerticalOptions = LayoutOptions.Start
        };
        header.Children.Add(quotientLine);
        Grid.SetColumn(quotientLine, 1);
        Grid.SetRow(quotientLine, 1);

        var panel = new VerticalStackLayout
        {
            Spacing = 10,
            Children = { header }
        };

        if (!_revealSolution)
        {
            return panel;
        }

        var stepsPanel = new VerticalStackLayout { Spacing = 4 };
        var visibleStep = 0;
        foreach (var step in WrittenCalculationBuilder.GetDivisionSteps(calculation))
        {
            if (step.QuotientDigit == 0 && visibleStep == 0)
            {
                continue;
            }

            visibleStep++;
            stepsPanel.Children.Add(new Label
            {
                Text = text.Format(
                    "writtenCalculation.divisionStep",
                    visibleStep,
                    step.PartialDividend,
                    step.SubtractedValue,
                    step.Remainder),
                TextColor = ThemeService.GetColor("TextSecondary")
            });
        }

        stepsPanel.Children.Add(new Label
        {
            FontAttributes = FontAttributes.Bold,
            Text = text.Format(
                "writtenCalculation.divisionResult",
                calculation.Result,
                calculation.Remainder),
            TextColor = ThemeService.GetColor("Success")
        });
        panel.Children.Add(stepsPanel);
        return panel;
    }

    /// <summary>
    /// Affiche les soustractions successives et le reste sous le dividende.
    /// </summary>
    private static View BuildDivisionWorkings(WrittenCalculation calculation)
    {
        var steps = WrittenCalculationBuilder.GetDivisionSteps(calculation)
            .SkipWhile(step => step.QuotientDigit == 0)
            .ToArray();
        var digitCount = Math.Max(
            calculation.LeftOperand.ToString().Length,
            steps.Select(step => step.PartialDividend.ToString().Length)
                .DefaultIfEmpty(1)
                .Max());
        var panel = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            Spacing = 1
        };

        for (var index = 0; index < steps.Length; index++)
        {
            var step = steps[index];
            panel.Children.Add(CreateDigitRow(
                step.SubtractedValue.ToString(),
                digitCount,
                "−",
                ThemeService.GetColor("TextPrimary")));
            panel.Children.Add(CreateSeparator(digitCount));

            var isLastStep = index == steps.Length - 1;
            var value = isLastStep
                ? step.Remainder
                : steps[index + 1].PartialDividend;
            panel.Children.Add(CreateDigitRow(
                value.ToString(),
                digitCount,
                string.Empty,
                ThemeService.GetColor(isLastStep ? "Success" : "Primary")));
        }

        return panel;
    }

    /// <summary>
    /// Produit une note sur les retenues, emprunts ou produits intermédiaires.
    /// </summary>
    private static Label? CreateOperationNote(WrittenCalculation calculation)
    {
        var text = TranslationService.Current;
        string? note = calculation.Kind switch
        {
            WrittenCalculationKind.Addition => CreateAdditionNote(calculation, text),
            WrittenCalculationKind.Subtraction => CreateSubtractionNote(calculation, text),
            WrittenCalculationKind.Multiplication
                when calculation.RightOperand >= 10 =>
                text.Get("writtenCalculation.partialProducts"),
            _ => null
        };

        return note is null
            ? null
            : new Label
            {
                FontSize = 13,
                Text = note,
                TextColor = ThemeService.GetColor("Primary")
            };
    }

    /// <summary>
    /// Décrit le nombre de retenues réellement utilisées.
    /// </summary>
    private static string? CreateAdditionNote(
        WrittenCalculation calculation,
        TranslationService text)
    {
        var carryCount = WrittenCalculationBuilder.GetAdditionCarries(calculation)
            .Count(carry => carry > 0);
        return carryCount == 0
            ? null
            : text.Format("writtenCalculation.carries", carryCount);
    }

    /// <summary>
    /// Décrit le nombre d'emprunts réellement utilisés.
    /// </summary>
    private static string? CreateSubtractionNote(
        WrittenCalculation calculation,
        TranslationService text)
    {
        var borrowCount = WrittenCalculationBuilder
            .GetSubtractionBorrowColumns(calculation)
            .Count;
        return borrowCount == 0
            ? null
            : text.Format("writtenCalculation.borrows", borrowCount);
    }

    /// <summary>
    /// Crée une ligne dont chaque caractère occupe une colonne fixe.
    /// </summary>
    private static Grid CreateDigitRow(
        string value,
        int digitCount,
        string symbol,
        Color color,
        int trailingPlaces = 0)
    {
        var grid = new Grid
        {
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Center
        };
        for (var index = 0; index <= digitCount; index++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(DigitWidth));
        }

        grid.Children.Add(CreateDigitLabel(symbol, color, 24));
        var offset = digitCount - value.Length - trailingPlaces + 1;
        for (var index = 0; index < value.Length; index++)
        {
            var label = CreateDigitLabel(value[index].ToString(), color, 28);
            grid.Children.Add(label);
            Grid.SetColumn(label, offset + index);
        }

        return grid;
    }

    /// <summary>
    /// Place chaque retenue au-dessus de la colonne qui la reçoit.
    /// </summary>
    private static Grid CreateCarryRow(
        WrittenCalculation calculation,
        int digitCount)
    {
        var grid = new Grid
        {
            ColumnSpacing = 0,
            HeightRequest = 24,
            HorizontalOptions = LayoutOptions.Center
        };
        for (var index = 0; index <= digitCount; index++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(DigitWidth));
        }

        var carries = WrittenCalculationBuilder.GetAdditionCarries(calculation);
        for (var index = 0; index < carries.Count; index++)
        {
            if (carries[index] == 0)
            {
                continue;
            }

            var label = CreateDigitLabel(
                carries[index].ToString(),
                ThemeService.GetColor("Primary"),
                16);
            label.HeightRequest = 24;
            grid.Children.Add(label);
            Grid.SetColumn(label, Math.Max(0, digitCount - index - 1));
        }

        return grid;
    }

    /// <summary>
    /// Crée la barre horizontale d'une opération posée.
    /// </summary>
    private static BoxView CreateSeparator(int digitCount)
    {
        return new BoxView
        {
            BackgroundColor = ThemeService.GetColor("TextPrimary"),
            HeightRequest = 2,
            WidthRequest = DigitWidth * (digitCount + 1),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 2)
        };
    }

    /// <summary>
    /// Crée une cellule de chiffre centrée.
    /// </summary>
    private static Label CreateDigitLabel(
        string value,
        Color color,
        double fontSize)
    {
        return new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = fontSize,
            HeightRequest = 36,
            HorizontalTextAlignment = TextAlignment.Center,
            Text = value,
            TextColor = color,
            VerticalTextAlignment = TextAlignment.Center,
            WidthRequest = DigitWidth
        };
    }

    /// <summary>
    /// Crée un nombre de grande taille pour la potence de division.
    /// </summary>
    private static Label CreateLargeNumberLabel(string value)
    {
        return new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 28,
            HorizontalTextAlignment = TextAlignment.Center,
            Text = value,
            VerticalTextAlignment = TextAlignment.Center
        };
    }
}
