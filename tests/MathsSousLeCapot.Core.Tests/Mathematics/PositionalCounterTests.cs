using MathsSousLeCapot.Core.Mathematics.PositionalNumeration;
using CounterEngine = MathsSousLeCapot.Core.Mathematics.PositionalNumeration.PositionalCounter;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

public sealed class PositionalCounterTests
{
    [Fact]
    public void Decimal_increment_turns_009_into_010()
    {
        var counter = new CounterEngine();
        counter.SetValue(9);

        var transition = counter.Increment();

        Assert.Equal("0010", counter.FormatValue());
        Assert.True(transition.HasCarry);
        Assert.Equal(2, transition.Digits.Count);
    }

    [Fact]
    public void Decimal_increment_turns_099_into_100()
    {
        var counter = new CounterEngine();
        counter.SetValue(99);

        var transition = counter.Increment();

        Assert.Equal("0100", counter.FormatValue());
        Assert.True(transition.HasCarry);
        Assert.Equal(3, transition.Digits.Count);
    }

    [Fact]
    public void Binary_increment_turns_011_into_100()
    {
        var counter = new CounterEngine(NumeralBase.Binary);
        counter.SetValue(3);

        var transition = counter.Increment();

        Assert.Equal("0100", counter.FormatValue());
        Assert.True(transition.HasCarry);
        Assert.Equal(3, transition.Digits.Count);
    }

    [Fact]
    public void Decimal_decrement_turns_100_into_099()
    {
        var counter = new CounterEngine();
        counter.SetValue(100);

        var transition = counter.Decrement();

        Assert.Equal("0099", counter.FormatValue());
        Assert.True(transition.HasBorrow);
        Assert.Equal(3, transition.Digits.Count);
    }

    [Fact]
    public void Changing_base_preserves_the_exact_value()
    {
        var counter = new CounterEngine();
        counter.SetValue(10);

        counter.ChangeBase(NumeralBase.Binary);

        Assert.Equal(10, counter.Value);
        Assert.Equal("1010", counter.FormatValue());
    }

    [Fact]
    public void Counter_does_not_decrement_below_zero()
    {
        var counter = new CounterEngine();

        var transition = counter.Decrement();

        Assert.Equal(0, counter.Value);
        Assert.Empty(transition.Digits);
        Assert.False(counter.CanDecrement);
    }

    [Fact]
    public void Counter_does_not_increment_above_course_limit()
    {
        var counter = new CounterEngine();
        counter.SetValue(CounterEngine.MaximumValue);

        var transition = counter.Increment();

        Assert.Equal(CounterEngine.MaximumValue, counter.Value);
        Assert.Empty(transition.Digits);
        Assert.False(counter.CanIncrement);
    }

    [Theory]
    [InlineData(125, 10, 135)]
    [InlineData(125, -10, 115)]
    [InlineData(125, 100, 225)]
    [InlineData(125, -100, 25)]
    public void Counter_moves_by_tens_and_hundreds(
        int start,
        int amount,
        int expected)
    {
        var counter = new CounterEngine();
        counter.SetValue(start);

        counter.Add(amount);

        Assert.Equal(expected, counter.Value);
    }

    [Fact]
    public void Counter_prevents_removing_a_hundred_below_zero()
    {
        var counter = new CounterEngine();
        counter.SetValue(99);

        var transition = counter.Add(-100);

        Assert.Equal(99, counter.Value);
        Assert.Empty(transition.Digits);
    }
}
