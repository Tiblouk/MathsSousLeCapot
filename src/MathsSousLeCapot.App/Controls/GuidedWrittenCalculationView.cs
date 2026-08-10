using System.Globalization;
using MathsSousLeCapot.App.Services;
using MathsSousLeCapot.Core.Mathematics.Operations;
using Microsoft.Maui.Controls.Shapes;

namespace MathsSousLeCapot.App.Controls;

/// <summary>
/// Rend un calcul posé dont les cases se remplissent directement dans l'ordre pédagogique.
/// </summary>
public sealed class GuidedWrittenCalculationView : ContentView
{
    /// <summary>
    /// Dimensions communes assurant l'alignement vertical des chiffres.
    /// </summary>
    private const double CellWidth = 38;
    private const double CellHeight = 42;

    /// <summary>
    /// Cellules associées aux étapes, conservées dans leur ordre de validation.
    /// </summary>
    private readonly List<GuidedStepCell> _cells = [];

    /// <summary>
    /// Position courante et verrou empêchant les événements de saisie imbriqués.
    /// </summary>
    private int _currentStepIndex;
    private bool _isAdvancing;

    /// <summary>
    /// Signale un changement de case active ou une saisie incorrecte.
    /// </summary>
    public event EventHandler<GuidedCalculationProgressEventArgs>? ProgressChanged;

    /// <summary>
    /// Signale que toutes les cases de la pose ont été correctement remplies.
    /// </summary>
    public event EventHandler? CalculationCompleted;

    /// <summary>
    /// Initialise le contrôle centré, sans calcul chargé.
    /// </summary>
    public GuidedWrittenCalculationView()
    {
        HorizontalOptions = LayoutOptions.Center;
    }

    /// <summary>
    /// Construit la disposition correspondant au plan et active sa première case.
    /// </summary>
    public void SetPlan(GuidedCalculationPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var cellSlots = new GuidedStepCell?[plan.Steps.Count];
        var layout = plan.Calculation.Kind switch
        {
            WrittenCalculationKind.Addition
                or WrittenCalculationKind.Subtraction =>
                BuildAdditionOrSubtraction(plan, cellSlots),
            WrittenCalculationKind.Multiplication =>
                BuildMultiplication(plan, cellSlots),
            WrittenCalculationKind.Division =>
                BuildDivision(plan, cellSlots),
            _ => throw new ArgumentOutOfRangeException(nameof(plan))
        };

        _cells.Clear();
        foreach (var cell in cellSlots)
        {
            _cells.Add(cell ?? throw new InvalidOperationException(
                "Une étape du calcul guidé ne possède aucune cellule visuelle."));
        }

        _currentStepIndex = 0;
        Content = CreateCard(layout);
        IsVisible = true;
        ActivateCurrentCell(isIncorrect: false);
    }

    /// <summary>
    /// Construit les colonnes partagées d'une addition ou d'une soustraction.
    /// </summary>
    private View BuildAdditionOrSubtraction(
        GuidedCalculationPlan plan,
        GuidedStepCell?[] cellSlots)
    {
        var calculation = plan.Calculation;
        var digitCount = new[]
        {
            calculation.LeftOperand,
            calculation.RightOperand,
            calculation.Result
        }.Max(GetDigitCount);
        var grid = CreateDigitGrid(digitCount, rowCount: 5);
        AddStaticNumberRow(
            grid,
            calculation.LeftOperand,
            digitCount,
            row: 1,
            symbol: string.Empty);
        AddStaticNumberRow(
            grid,
            calculation.RightOperand,
            digitCount,
            row: 2,
            symbol: calculation.Kind == WrittenCalculationKind.Addition
                ? "+"
                : "−");
        AddSeparator(grid, digitCount, row: 3);

        for (var index = 0; index < plan.Steps.Count; index++)
        {
            var step = plan.Steps[index];
            var isAuxiliary = step.Kind is
                GuidedCalculationStepKind.AdditionCarry
                or GuidedCalculationStepKind.SubtractionBorrow;
            AddDigitStepCell(
                grid,
                step,
                index,
                cellSlots,
                row: isAuxiliary ? 0 : 4,
                column: ToGridColumn(digitCount, step.ColumnIndex),
                compact: isAuxiliary);
        }

        return grid;
    }

    /// <summary>
    /// Construit les lignes partielles décalées et leur addition finale.
    /// </summary>
    private View BuildMultiplication(
        GuidedCalculationPlan plan,
        GuidedStepCell?[] cellSlots)
    {
        var calculation = plan.Calculation;
        var multiplierDigits = GetDigitCount(calculation.RightOperand);
        var digitCount = Math.Max(
            GetDigitCount(calculation.Result),
            Math.Max(
                GetDigitCount(calculation.LeftOperand),
                GetDigitCount(calculation.RightOperand)));
        var hasFinalSum = multiplierDigits > 1;
        var rowCount = 3 + multiplierDigits * 2 + (hasFinalSum ? 3 : 0);
        var grid = CreateDigitGrid(digitCount, rowCount);
        AddStaticNumberRow(
            grid,
            calculation.LeftOperand,
            digitCount,
            row: 0,
            symbol: string.Empty);
        AddStaticNumberRow(
            grid,
            calculation.RightOperand,
            digitCount,
            row: 1,
            symbol: "×");
        AddSeparator(grid, digitCount, row: 2);

        var finalSeparatorRow = 3 + multiplierDigits * 2;
        var finalCarryRow = finalSeparatorRow + 1;
        var finalResultRow = finalSeparatorRow + 2;
        if (hasFinalSum)
        {
            AddSeparator(grid, digitCount, finalSeparatorRow);
        }

        for (var index = 0; index < plan.Steps.Count; index++)
        {
            var step = plan.Steps[index];
            var shiftedColumn = step.ColumnIndex + step.RowIndex;
            var row = step.Kind switch
            {
                GuidedCalculationStepKind.MultiplicationCarry =>
                    3 + step.RowIndex * 2,
                GuidedCalculationStepKind.MultiplicationDigit =>
                    4 + step.RowIndex * 2,
                GuidedCalculationStepKind.MultiplicationFinalCarry =>
                    finalCarryRow,
                GuidedCalculationStepKind.MultiplicationFinalDigit =>
                    finalResultRow,
                _ => throw new InvalidOperationException(
                    $"Étape de multiplication non affichable : {step.Kind}.")
            };
            var decimalColumn = step.Kind is
                GuidedCalculationStepKind.MultiplicationFinalCarry
                or GuidedCalculationStepKind.MultiplicationFinalDigit
                ? step.ColumnIndex
                : shiftedColumn;
            AddDigitStepCell(
                grid,
                step,
                index,
                cellSlots,
                row,
                ToGridColumn(digitCount, decimalColumn),
                compact: step.Kind is
                    GuidedCalculationStepKind.MultiplicationCarry
                    or GuidedCalculationStepKind.MultiplicationFinalCarry);
        }

        return grid;
    }

    /// <summary>
    /// Construit la potence et les soustractions successives d'une division.
    /// </summary>
    private View BuildDivision(
        GuidedCalculationPlan plan,
        GuidedStepCell?[] cellSlots)
    {
        var calculation = plan.Calculation;
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
        var dividend = CreateLargeNumberLabel(
            calculation.LeftOperand.ToString(CultureInfo.InvariantCulture));
        dividend.Padding = new Thickness(0, 6, 16, 6);
        header.Children.Add(dividend);

        var divisor = CreateLargeNumberLabel(
            calculation.RightOperand.ToString(CultureInfo.InvariantCulture));
        divisor.Padding = new Thickness(16, 6);
        divisor.BackgroundColor = ThemeService.GetColor("PrimaryLight");
        header.Children.Add(divisor);
        Grid.SetColumn(divisor, 1);

        var quotientSteps = plan.Steps
            .Select((step, index) => (Step: step, Index: index))
            .Where(item => item.Step.Kind ==
                GuidedCalculationStepKind.DivisionQuotientDigit)
            .ToArray();
        var quotientGrid = new Grid
        {
            ColumnSpacing = 2,
            Padding = new Thickness(10, 5),
            HorizontalOptions = LayoutOptions.Center
        };
        for (var column = 0; column < quotientSteps.Length; column++)
        {
            quotientGrid.ColumnDefinitions.Add(new ColumnDefinition(CellWidth));
            var item = quotientSteps[column];
            AddDigitStepCell(
                quotientGrid,
                item.Step,
                item.Index,
                cellSlots,
                row: 0,
                column,
                compact: false);
        }

        header.Children.Add(quotientGrid);
        Grid.SetColumn(quotientGrid, 1);
        Grid.SetRow(quotientGrid, 1);
        AddDivisionBars(header);

        var workings = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            Spacing = 8
        };
        var groupedSteps = plan.Steps
            .Select((step, index) => (Step: step, Index: index))
            .Where(item => item.Step.Kind is
                GuidedCalculationStepKind.DivisionProduct
                or GuidedCalculationStepKind.DivisionRemainder)
            .GroupBy(item => item.Step.RowIndex)
            .OrderBy(group => group.Key);

        foreach (var group in groupedSteps)
        {
            var product = group.Single(item => item.Step.Kind ==
                GuidedCalculationStepKind.DivisionProduct);
            var remainder = group.Single(item => item.Step.Kind ==
                GuidedCalculationStepKind.DivisionRemainder);
            var divisionGroup = BuildDivisionGroup(
                product,
                remainder,
                cellSlots);
            divisionGroup.IsVisible = group.Key == 0;
            cellSlots[product.Index]!.Container = divisionGroup;
            cellSlots[remainder.Index]!.Container = divisionGroup;
            var quotient = quotientSteps.Single(
                item => item.Step.RowIndex == group.Key);
            cellSlots[quotient.Index]!.Container = divisionGroup;
            workings.Children.Add(divisionGroup);
        }

        return new VerticalStackLayout
        {
            Spacing = 8,
            HorizontalOptions = LayoutOptions.Center,
            Children = { header, workings }
        };
    }

    /// <summary>
    /// Construit un bloc vertical composé du dividende partiel, du produit et du reste.
    /// </summary>
    private View BuildDivisionGroup(
        (GuidedCalculationStep Step, int Index) product,
        (GuidedCalculationStep Step, int Index) remainder,
        GuidedStepCell?[] cellSlots)
    {
        var width = new[]
        {
            2,
            GetDigitCount(product.Step.RightValue),
            GetDigitCount(product.Step.ExpectedValue),
            GetDigitCount(remainder.Step.LeftValue),
            GetDigitCount(remainder.Step.ExpectedValue)
        }.Max();
        var grid = CreateDigitGrid(width, rowCount: 4);
        AddStaticNumberRow(
            grid,
            remainder.Step.LeftValue,
            width,
            row: 0,
            symbol: string.Empty,
            colorKey: "Primary");
        AddWholeNumberStepCell(
            grid,
            product.Step,
            product.Index,
            cellSlots,
            row: 1,
            digitCount: width,
            symbol: "−");
        AddSeparator(grid, width, row: 2);
        AddWholeNumberStepCell(
            grid,
            remainder.Step,
            remainder.Index,
            cellSlots,
            row: 3,
            digitCount: width,
            symbol: string.Empty);
        return grid;
    }

    /// <summary>
    /// Ajoute les deux traits caractéristiques de la potence française.
    /// </summary>
    private static void AddDivisionBars(Grid header)
    {
        var verticalBar = new BoxView
        {
            BackgroundColor = ThemeService.GetColor("TextPrimary"),
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 2
        };
        header.Children.Add(verticalBar);
        Grid.SetColumn(verticalBar, 1);
        Grid.SetRowSpan(verticalBar, 2);

        var horizontalBar = new BoxView
        {
            BackgroundColor = ThemeService.GetColor("TextPrimary"),
            HeightRequest = 2,
            VerticalOptions = LayoutOptions.Start
        };
        header.Children.Add(horizontalBar);
        Grid.SetColumn(horizontalBar, 1);
        Grid.SetRow(horizontalBar, 1);
    }

    /// <summary>
    /// Crée une grille de chiffres avec une colonne réservée au symbole.
    /// </summary>
    private static Grid CreateDigitGrid(int digitCount, int rowCount)
    {
        var grid = new Grid
        {
            ColumnSpacing = 1,
            RowSpacing = 2,
            HorizontalOptions = LayoutOptions.Center
        };
        for (var column = 0; column <= digitCount; column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(CellWidth));
        }

        for (var row = 0; row < rowCount; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        return grid;
    }

    /// <summary>
    /// Place un opérande immuable chiffre par chiffre dans une ligne.
    /// </summary>
    private static void AddStaticNumberRow(
        Grid grid,
        int value,
        int digitCount,
        int row,
        string symbol,
        string colorKey = "TextPrimary")
    {
        var symbolLabel = CreateDigitLabel(
            symbol,
            ThemeService.GetColor(colorKey),
            fontSize: 25);
        grid.Children.Add(symbolLabel);
        Grid.SetRow(symbolLabel, row);

        var text = value.ToString(CultureInfo.InvariantCulture);
        var firstColumn = digitCount - text.Length + 1;
        for (var index = 0; index < text.Length; index++)
        {
            var label = CreateDigitLabel(
                text[index].ToString(),
                ThemeService.GetColor(colorKey),
                fontSize: 29);
            grid.Children.Add(label);
            Grid.SetColumn(label, firstColumn + index);
            Grid.SetRow(label, row);
        }
    }

    /// <summary>
    /// Ajoute une cellule d'un seul chiffre à sa position décimale.
    /// </summary>
    private void AddDigitStepCell(
        Grid grid,
        GuidedCalculationStep step,
        int stepIndex,
        GuidedStepCell?[] cellSlots,
        int row,
        int column,
        bool compact)
    {
        var cell = CreateStepCell(step, stepIndex, compact);
        cellSlots[stepIndex] = cell;
        grid.Children.Add(cell.Border);
        Grid.SetColumn(cell.Border, column);
        Grid.SetRow(cell.Border, row);
    }

    /// <summary>
    /// Ajoute une valeur intermédiaire de division alignée sur plusieurs chiffres.
    /// </summary>
    private void AddWholeNumberStepCell(
        Grid grid,
        GuidedCalculationStep step,
        int stepIndex,
        GuidedStepCell?[] cellSlots,
        int row,
        int digitCount,
        string symbol)
    {
        var symbolLabel = CreateDigitLabel(
            symbol,
            ThemeService.GetColor("TextPrimary"),
            fontSize: 24);
        grid.Children.Add(symbolLabel);
        Grid.SetRow(symbolLabel, row);

        var cell = CreateStepCell(step, stepIndex, compact: false);
        cell.Border.WidthRequest = CellWidth * digitCount;
        cell.Display.HorizontalTextAlignment = TextAlignment.End;
        cellSlots[stepIndex] = cell;
        grid.Children.Add(cell.Border);
        Grid.SetColumn(cell.Border, 1);
        Grid.SetColumnSpan(cell.Border, digitCount);
        Grid.SetRow(cell.Border, row);
    }

    /// <summary>
    /// Crée une case inactive qui sera déverrouillée lorsque son tour arrivera.
    /// </summary>
    private GuidedStepCell CreateStepCell(
        GuidedCalculationStep step,
        int stepIndex,
        bool compact)
    {
        var expectedLength = step.ExpectedValue
            .ToString(CultureInfo.InvariantCulture)
            .Length;
        var entry = new Entry
        {
            BackgroundColor = Colors.Transparent,
            HeightRequest = compact ? 30 : CellHeight,
            HorizontalOptions = LayoutOptions.Fill,
            IsReadOnly = true,
            Keyboard = Keyboard.Numeric,
            MaxLength = expectedLength,
            Opacity = 0.01,
            VerticalOptions = LayoutOptions.Fill
        };
        var display = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = compact ? 17 : 26,
            HeightRequest = compact ? 30 : CellHeight,
            HorizontalTextAlignment = TextAlignment.Center,
            InputTransparent = true,
            TextColor = ThemeService.GetColor("TextSecondary"),
            VerticalTextAlignment = TextAlignment.Center
        };
        entry.TextChanged += (_, args) =>
        {
            display.Text = args.NewTextValue;
            OnCellTextChanged(stepIndex, args.NewTextValue);
        };
        var cellContent = new Grid
        {
            Children = { display, entry }
        };
        var border = new Border
        {
            BackgroundColor = Colors.Transparent,
            HeightRequest = compact ? 30 : CellHeight,
            Stroke = ThemeService.GetColor("Border"),
            StrokeShape = new RoundRectangle { CornerRadius = compact ? 6 : 8 },
            StrokeThickness = 1,
            WidthRequest = CellWidth,
            Content = cellContent
        };
        return new GuidedStepCell(step, border, entry, display, border);
    }

    /// <summary>
    /// Valide automatiquement la case lorsque sa valeur complète est saisie.
    /// </summary>
    private void OnCellTextChanged(int stepIndex, string? value)
    {
        if (_isAdvancing
            || stepIndex != _currentStepIndex
            || stepIndex >= _cells.Count)
        {
            return;
        }

        var cell = _cells[stepIndex];
        var expected = cell.Step.ExpectedValue.ToString(CultureInfo.InvariantCulture);
        if (string.Equals(value, expected, StringComparison.Ordinal))
        {
            _isAdvancing = true;
            MarkCellAsCorrect(cell);
            _currentStepIndex++;
            _isAdvancing = false;
            ActivateCurrentCell(isIncorrect: false);
            return;
        }

        var isCompleteWrongValue = !string.IsNullOrEmpty(value)
            && value.Length >= expected.Length;
        SetActiveCellAppearance(cell, isIncorrect: isCompleteWrongValue);
        if (isCompleteWrongValue)
        {
            ProgressChanged?.Invoke(
                this,
                new GuidedCalculationProgressEventArgs(
                    _currentStepIndex + 1,
                    _cells.Count,
                    isIncorrect: true));
        }
    }

    /// <summary>
    /// Déverrouille la case courante, révèle son bloc et lui donne le focus.
    /// </summary>
    private void ActivateCurrentCell(bool isIncorrect)
    {
        if (_currentStepIndex >= _cells.Count)
        {
            ProgressChanged?.Invoke(
                this,
                new GuidedCalculationProgressEventArgs(
                    _cells.Count,
                    _cells.Count,
                    isIncorrect: false));
            CalculationCompleted?.Invoke(this, EventArgs.Empty);
            return;
        }

        var cell = _cells[_currentStepIndex];
        cell.Container.IsVisible = true;
        cell.Entry.IsReadOnly = false;
        SetActiveCellAppearance(cell, isIncorrect);
        ProgressChanged?.Invoke(
            this,
            new GuidedCalculationProgressEventArgs(
                _currentStepIndex + 1,
                _cells.Count,
                isIncorrect: isIncorrect));
        Dispatcher.Dispatch(() => cell.Entry.Focus());
    }

    /// <summary>
    /// Applique les couleurs de sélection ou d'erreur à la case active.
    /// </summary>
    private static void SetActiveCellAppearance(
        GuidedStepCell cell,
        bool isIncorrect)
    {
        cell.Border.BackgroundColor = ThemeService.GetColor(
            isIncorrect ? "ErrorBackground" : "PrimaryLight");
        cell.Border.Stroke = ThemeService.GetColor(
            isIncorrect ? "Error" : "Primary");
        cell.Border.StrokeThickness = 2;
        cell.Display.TextColor = ThemeService.GetColor(
            isIncorrect ? "Error" : "TextPrimary");
    }

    /// <summary>
    /// Verrouille une case juste et la conserve en vert dans la pose.
    /// </summary>
    private static void MarkCellAsCorrect(GuidedStepCell cell)
    {
        cell.Entry.IsReadOnly = true;
        cell.Display.TextColor = ThemeService.GetColor("Success");
        cell.Border.BackgroundColor = ThemeService.GetColor("SuccessBackground");
        cell.Border.Stroke = ThemeService.GetColor("Success");
        cell.Border.StrokeThickness = 1;
    }

    /// <summary>
    /// Entoure toute la pose d'une carte neutre compatible avec les thèmes.
    /// </summary>
    private static Border CreateCard(View content)
    {
        return new Border
        {
            BackgroundColor = ThemeService.GetColor("ControlBackground"),
            Stroke = ThemeService.GetColor("Border"),
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Padding = 16,
            Content = content
        };
    }

    /// <summary>
    /// Ajoute une barre horizontale sous les opérandes ou produits intermédiaires.
    /// </summary>
    private static void AddSeparator(Grid grid, int digitCount, int row)
    {
        var separator = new BoxView
        {
            BackgroundColor = ThemeService.GetColor("TextPrimary"),
            HeightRequest = 2,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 2)
        };
        grid.Children.Add(separator);
        Grid.SetColumnSpan(separator, digitCount + 1);
        Grid.SetRow(separator, row);
    }

    /// <summary>
    /// Convertit une position décimale en colonne visuelle comptée depuis la gauche.
    /// </summary>
    private static int ToGridColumn(int digitCount, int decimalColumn)
    {
        return digitCount - decimalColumn;
    }

    /// <summary>
    /// Compte les chiffres d'un entier naturel, zéro occupant une case.
    /// </summary>
    private static int GetDigitCount(int value)
    {
        return value.ToString(CultureInfo.InvariantCulture).Length;
    }

    /// <summary>
    /// Crée un chiffre statique aligné avec les cellules modifiables.
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
            HeightRequest = CellHeight,
            HorizontalTextAlignment = TextAlignment.Center,
            Text = value,
            TextColor = color,
            VerticalTextAlignment = TextAlignment.Center,
            WidthRequest = CellWidth
        };
    }

    /// <summary>
    /// Crée un nombre statique de grande taille pour la potence.
    /// </summary>
    private static Label CreateLargeNumberLabel(string value)
    {
        return new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 29,
            HorizontalTextAlignment = TextAlignment.Center,
            Text = value,
            VerticalTextAlignment = TextAlignment.Center
        };
    }

    /// <summary>
    /// Associe une étape à sa bordure, son champ et son bloc éventuellement différé.
    /// </summary>
    private sealed record GuidedStepCell(
        GuidedCalculationStep Step,
        Border Border,
        Entry Entry,
        Label Display,
        VisualElement InitialContainer)
    {
        /// <summary>
        /// Bloc à révéler lorsque cette étape devient active.
        /// </summary>
        public VisualElement Container { get; set; } = InitialContainer;
    }
}

/// <summary>
/// Décrit la position active de la pose et indique une éventuelle saisie incorrecte.
/// </summary>
public sealed class GuidedCalculationProgressEventArgs : EventArgs
{
    /// <summary>
    /// Initialise l'état transmis par la pose interactive.
    /// </summary>
    /// <param name="stepNumber">Numéro humain de la case active.</param>
    /// <param name="totalSteps">Nombre total de cases à compléter.</param>
    /// <param name="isIncorrect">Indique que la valeur complète saisie est fausse.</param>
    public GuidedCalculationProgressEventArgs(
        int stepNumber,
        int totalSteps,
        bool isIncorrect)
    {
        StepNumber = stepNumber;
        TotalSteps = totalSteps;
        IsIncorrect = isIncorrect;
    }

    /// <summary>
    /// Numéro humain de la case active.
    /// </summary>
    public int StepNumber { get; }

    /// <summary>
    /// Nombre total de cases à compléter.
    /// </summary>
    public int TotalSteps { get; }

    /// <summary>
    /// Indique que la valeur complète saisie dans la case est fausse.
    /// </summary>
    public bool IsIncorrect { get; }
}
