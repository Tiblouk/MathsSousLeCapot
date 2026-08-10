using System.Numerics;

namespace MathsSousLeCapot.Core.Mathematics.PositionalNumeration;

/// <summary>
/// Représente une valeur entière sous forme de colonnes dans une base numérique.
/// </summary>
public sealed class PositionalCounter
{
    /// <summary>
    /// Nombre minimal de colonnes affichées par le premier cours.
    /// </summary>
    public const int MinimumColumnCount = 4;

    /// <summary>
    /// Valeur maximale autorisée dans le périmètre pédagogique actuel.
    /// </summary>
    public static readonly BigInteger MaximumValue = 9999;

    /// <summary>
    /// Initialise un compteur à zéro dans la base demandée.
    /// </summary>
    public PositionalCounter(NumeralBase numeralBase = NumeralBase.Decimal)
    {
        Base = numeralBase;
    }

    public NumeralBase Base { get; private set; }

    public BigInteger Value { get; private set; }

    public bool CanIncrement => Value < MaximumValue;

    public bool CanDecrement => Value > BigInteger.Zero;

    public IReadOnlyList<PositionalDigit> Digits => CreateDigits(Value, Base);

    /// <summary>
    /// Change uniquement la représentation, sans modifier la valeur exacte.
    /// </summary>
    public void ChangeBase(NumeralBase numeralBase)
    {
        Base = numeralBase;
    }

    /// <summary>
    /// Ajoute une unité au compteur.
    /// </summary>
    public CounterTransition Increment()
    {
        return Add(1);
    }

    /// <summary>
    /// Retire une unité au compteur.
    /// </summary>
    public CounterTransition Decrement()
    {
        return Add(-1);
    }

    /// <summary>
    /// Indique si un déplacement respecte les limites du cours.
    /// </summary>
    public bool CanAdd(int amount)
    {
        var target = Value + amount;
        return target >= BigInteger.Zero && target <= MaximumValue;
    }

    /// <summary>
    /// Applique un déplacement positif ou négatif et décrit les colonnes modifiées.
    /// </summary>
    public CounterTransition Add(int amount)
    {
        if (amount == 0)
        {
            return CreateUnchangedTransition("counter.explanation.noChange");
        }

        if (!CanAdd(amount))
        {
            return CreateUnchangedTransition(
                amount > 0
                    ? "counter.explanation.maximum"
                    : "counter.explanation.minimum");
        }

        return MoveTo(
            Value + amount,
            isIncrement: amount > 0,
            amount: Math.Abs(amount));
    }

    /// <summary>
    /// Remplace la valeur courante par une valeur exacte autorisée.
    /// </summary>
    public CounterTransition SetValue(BigInteger value)
    {
        if (value < BigInteger.Zero || value > MaximumValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"La valeur doit être comprise entre 0 et {MaximumValue}.");
        }

        var difference = BigInteger.Abs(value - Value);
        var amount = difference > int.MaxValue ? int.MaxValue : (int)difference;
        return MoveTo(value, isIncrement: value >= Value, amount);
    }

    /// <summary>
    /// Formate les chiffres avec les zéros nécessaires à l'affichage.
    /// </summary>
    public string FormatValue()
    {
        return string.Concat(Digits.Select(digit => digit.Value));
    }

    /// <summary>
    /// Calcule une transition et compare les chiffres avant et après le déplacement.
    /// </summary>
    private CounterTransition MoveTo(
        BigInteger target,
        bool isIncrement,
        int amount)
    {
        // Ces deux instantanés permettent de déterminer précisément les colonnes touchées.
        var before = Value;
        var beforeDigits = CreateDigits(before, Base);
        Value = target;
        var afterDigits = CreateDigits(Value, Base);
        var width = Math.Max(beforeDigits.Count, afterDigits.Count);

        var paddedBefore = PadDigits(beforeDigits, width);
        var paddedAfter = PadDigits(afterDigits, width);
        var changes = paddedBefore
            .Zip(paddedAfter)
            .Where(pair => pair.First.Value != pair.Second.Value)
            .Select(pair => new DigitTransition(
                pair.Second.Position,
                pair.First.Value,
                pair.Second.Value))
            .ToArray();

        var crossesColumn = changes.Length > 1;
        var explanation = CreateExplanation(
            amount,
            isIncrement,
            crossesColumn);

        return new CounterTransition(
            before,
            Value,
            changes,
            HasCarry: crossesColumn && isIncrement,
            HasBorrow: crossesColumn && !isIncrement,
            explanation);
    }

    /// <summary>
    /// Choisit les clés de traduction qui expliquent le déplacement.
    /// </summary>
    private static string CreateExplanation(
        int amount,
        bool isIncrement,
        bool crossesColumn)
    {
        if (amount == 1 && crossesColumn)
        {
            return isIncrement
                ? "counter.explanation.carry"
                : "counter.explanation.borrow";
        }

        var explanationKey = (isIncrement, amount) switch
        {
            (true, 1) => "counter.explanation.addOne",
            (false, 1) => "counter.explanation.removeOne",
            (true, 10) => "counter.explanation.addTen",
            (false, 10) => "counter.explanation.removeTen",
            (true, 100) => "counter.explanation.addHundred",
            (false, 100) => "counter.explanation.removeHundred",
            _ => "counter.explanation.noChange"
        };

        return crossesColumn
            ? $"{explanationKey}|counter.explanation.multipleColumns"
            : explanationKey;
    }

    /// <summary>
    /// Crée une transition sans modification lorsque la limite est atteinte.
    /// </summary>
    private CounterTransition CreateUnchangedTransition(string explanation)
    {
        return new CounterTransition(
            Value,
            Value,
            Array.Empty<DigitTransition>(),
            HasCarry: false,
            HasBorrow: false,
            explanation);
    }

    /// <summary>
    /// Décompose une valeur exacte en chiffres positionnels.
    /// </summary>
    private static IReadOnlyList<PositionalDigit> CreateDigits(
        BigInteger value,
        NumeralBase numeralBase)
    {
        var radix = (int)numeralBase;
        var requiredColumns = value.IsZero
            ? MinimumColumnCount
            : Math.Max(
                MinimumColumnCount,
                (int)Math.Floor(BigInteger.Log(value, radix)) + 1);

        var digits = new List<PositionalDigit>(requiredColumns);

        for (var position = requiredColumns - 1; position >= 0; position--)
        {
            var placeValue = BigInteger.Pow(radix, position);
            var digit = (int)((value / placeValue) % radix);
            digits.Add(new PositionalDigit(
                position,
                digit,
                placeValue,
                CreateLabel(position, numeralBase, placeValue)));
        }

        return digits;
    }

    /// <summary>
    /// Aligne deux représentations en ajoutant des colonnes nulles à gauche.
    /// </summary>
    private static IReadOnlyList<PositionalDigit> PadDigits(
        IReadOnlyList<PositionalDigit> digits,
        int width)
    {
        if (digits.Count == width)
        {
            return digits;
        }

        var result = new List<PositionalDigit>(width);

        for (var index = width - 1; index >= digits.Count; index--)
        {
            result.Add(new PositionalDigit(index, 0, BigInteger.Zero, string.Empty));
        }

        result.AddRange(digits);
        return result;
    }

    /// <summary>
    /// Retourne une clé de traduction pour les colonnes décimales.
    /// </summary>
    private static string CreateLabel(
        int position,
        NumeralBase numeralBase,
        BigInteger placeValue)
    {
        if (numeralBase == NumeralBase.Binary)
        {
            return placeValue.ToString();
        }

        return position switch
        {
            0 => "counter.column.units",
            1 => "counter.column.tens",
            2 => "counter.column.hundreds",
            3 => "counter.column.thousands",
            _ => $"10^{position}"
        };
    }
}
