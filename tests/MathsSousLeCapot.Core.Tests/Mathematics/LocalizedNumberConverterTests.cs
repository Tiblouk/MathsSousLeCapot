using MathsSousLeCapot.Core.Mathematics.Numbers;

namespace MathsSousLeCapot.Core.Tests.Mathematics;

/// <summary>
/// Vérifie l'écriture et la reconnaissance des nombres dans chaque langue fournie.
/// </summary>
public sealed class LocalizedNumberConverterTests
{
    [Theory]
    [InlineData(21, "fr_FR", "vingt et un")]
    [InlineData(21, "en_US", "twenty-one")]
    [InlineData(21, "es_ES", "veintiuno")]
    [InlineData(21, "it_IT", "ventuno")]
    [InlineData(21, "ja_JP", "二十一")]
    [InlineData(108, "it_IT", "centotto")]
    [InlineData(9999, "en_US", "nine thousand nine hundred ninety-nine")]
    [InlineData(-2, "fr_FR", "moins deux")]
    public void Converts_numbers_using_the_requested_language(
        int value,
        string languageCode,
        string expected)
    {
        Assert.Equal(expected, LocalizedNumberConverter.ToWords(value, languageCode));
    }

    [Theory]
    [InlineData("twenty one", 21, "en_US")]
    [InlineData("TWENTY-ONE", 21, "en_US")]
    [InlineData("veintiúnó", 21, "es_ES")]
    [InlineData("ventuno", 21, "it_IT")]
    [InlineData("二十一", 21, "ja_JP")]
    [InlineData("vingt-et-un", 21, "fr_FR")]
    [InlineData("021", 21, "fr_FR")]
    [InlineData("-2", -2, "fr_FR")]
    [InlineData("−2", -2, "fr_FR")]
    [InlineData("moins deux", -2, "fr_FR")]
    public void Accepts_reasonable_written_and_numeric_forms(
        string answer,
        int expected,
        string languageCode)
    {
        Assert.True(LocalizedNumberConverter.Matches(answer, expected, languageCode));
    }

    [Theory]
    [InlineData("", 21, "fr_FR")]
    [InlineData("twenty-two", 21, "en_US")]
    [InlineData("10000", 9999, "en_US")]
    public void Rejects_wrong_or_out_of_range_answers(
        string answer,
        int expected,
        string languageCode)
    {
        Assert.False(LocalizedNumberConverter.Matches(answer, expected, languageCode));
    }
}
