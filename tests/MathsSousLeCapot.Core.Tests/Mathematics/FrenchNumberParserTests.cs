using MathsSousLeCapot.Core.Mathematics.Numbers;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

/// <summary>
/// Vérifie les formes de réponse françaises acceptées par les cours décimaux.
/// </summary>
public sealed class FrenchNumberParserTests
{
    [Theory]
    [InlineData("20", 20)]
    [InlineData("020", 20)]
    [InlineData("vingt", 20)]
    [InlineData("vingt et un", 21)]
    [InlineData("vingt-et-un", 21)]
    [InlineData("VINGT ET UN", 21)]
    [InlineData("quatre vingt", 80)]
    [InlineData("quatre-vingts", 80)]
    [InlineData("cent", 100)]
    [InlineData("deux cent un", 201)]
    public void Accepts_digits_and_common_written_forms(string answer, int expected)
    {
        Assert.True(FrenchNumberParser.Matches(answer, expected));
    }

    [Theory]
    [InlineData("")]
    [InlineData("vingt-deux")]
    [InlineData("bonjour")]
    [InlineData("10000")]
    public void Rejects_empty_wrong_or_out_of_range_answers(string answer)
    {
        Assert.False(FrenchNumberParser.Matches(answer, 21));
    }
}
