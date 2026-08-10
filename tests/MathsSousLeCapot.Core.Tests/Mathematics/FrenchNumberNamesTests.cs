using MathsSousLeCapot.Core.Mathematics.Numbers;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

public sealed class FrenchNumberNamesTests
{
    [Theory]
    [InlineData(0, "zéro")]
    [InlineData(9, "neuf")]
    [InlineData(17, "dix-sept")]
    [InlineData(21, "vingt et un")]
    [InlineData(71, "soixante et onze")]
    [InlineData(80, "quatre-vingts")]
    [InlineData(81, "quatre-vingt-un")]
    [InlineData(200, "deux cents")]
    [InlineData(201, "deux cent un")]
    [InlineData(1000, "mille")]
    [InlineData(9999, "neuf mille neuf cent quatre-vingt-dix-neuf")]
    public void Converts_numbers_to_french_words(int value, string expected)
    {
        Assert.Equal(expected, FrenchNumberNames.ToWords(value));
    }
}
