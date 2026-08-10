using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.Core.Mathematics.PositionalNumeration;
using CounterEngine = MathsSousLeCapot.Core.Mathematics.PositionalNumeration.PositionalCounter;

namespace MathsSousLeCapot.App.Features.PositionalCounter;

/// <summary>
/// Adapte le moteur du compteur aux propriétés observables utilisées par l'interface.
/// </summary>
public sealed class CounterViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Moteur mathématique indépendant de MAUI.
    /// </summary>
    private readonly CounterEngine _counter = new();

    /// <summary>
    /// Dernière transition utilisée pour mettre en évidence les colonnes modifiées.
    /// </summary>
    private CounterTransition? _lastTransition;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<PositionalDigit> Digits => _counter.Digits
        .Select(digit => digit with
        {
            Label = digit.Label.StartsWith(
                "counter.",
                StringComparison.Ordinal)
                ? TranslationService.Current.Get(digit.Label)
                : digit.Label
        })
        .ToArray();

    public string DisplayValue => _counter.FormatValue();

    public string BaseLabel => TranslationService.Current.Get(
        _counter.Base == NumeralBase.Decimal
            ? "counter.baseTen"
            : "counter.baseTwo");

    public bool CanIncrement => _counter.CanIncrement;

    public bool CanDecrement => _counter.CanDecrement;

    public bool CanAddTen => _counter.CanAdd(10);

    public bool CanRemoveTen => _counter.CanAdd(-10);

    public bool CanAddHundred => _counter.CanAdd(100);

    public bool CanRemoveHundred => _counter.CanAdd(-100);

    public NumeralBase Base => _counter.Base;

    public string Explanation { get; private set; } =
        TranslationService.Current.Get("counter.initialExplanation");

    public IReadOnlySet<int> ChangedPositions =>
        _lastTransition?.Digits.Select(digit => digit.Position).ToHashSet()
        ?? new HashSet<int>();

    public CounterTransition Increment()
    {
        return Apply(_counter.Increment());
    }

    public CounterTransition Decrement()
    {
        return Apply(_counter.Decrement());
    }

    public CounterTransition SetValue(int value)
    {
        return Apply(_counter.SetValue(value));
    }

    public CounterTransition Add(int amount)
    {
        return Apply(_counter.Add(amount));
    }

    public void ChangeBase(NumeralBase numeralBase)
    {
        _counter.ChangeBase(numeralBase);
        _lastTransition = null;
        Explanation = TranslationService.Current.Get(
            numeralBase == NumeralBase.Decimal
                ? "counter.decimalExplanation"
                : "counter.binaryExplanation");
        NotifyStateChanged();
    }

    private CounterTransition Apply(CounterTransition transition)
    {
        _lastTransition = transition;
        Explanation = string.Concat(
            transition.Explanation
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(TranslationService.Current.Get));
        NotifyStateChanged();
        return transition;
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(Digits));
        OnPropertyChanged(nameof(DisplayValue));
        OnPropertyChanged(nameof(BaseLabel));
        OnPropertyChanged(nameof(CanIncrement));
        OnPropertyChanged(nameof(CanDecrement));
        OnPropertyChanged(nameof(CanAddTen));
        OnPropertyChanged(nameof(CanRemoveTen));
        OnPropertyChanged(nameof(CanAddHundred));
        OnPropertyChanged(nameof(CanRemoveHundred));
        OnPropertyChanged(nameof(Explanation));
        OnPropertyChanged(nameof(ChangedPositions));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
