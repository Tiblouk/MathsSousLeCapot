using MathsSousLeCapot.Core.Courses;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.Core.Tests.Training;

/// <summary>
/// Vérifie les formes textuelles légitimes des réponses spécialisées.
/// </summary>
public sealed class ExerciseAnswerValidatorTests
{
    [Theory]
    [InlineData("centaine", "fr_FR")]
    [InlineData("centaines", "fr_FR")]
    [InlineData("hundreds place", "en_US")]
    [InlineData("centena", "es_ES")]
    [InlineData("centinaia", "it_IT")]
    [InlineData("百の位", "ja_JP")]
    public void Accepts_the_localized_name_of_a_digit_position(
        string answer,
        string languageCode)
    {
        var exercise = PlaceValueExercise(
            correctAnswer: "900",
            place: "100");

        Assert.True(ExerciseAnswerValidator.Matches(
            exercise,
            answer,
            languageCode));
    }

    [Theory]
    [InlineData("unité", "1", "1")]
    [InlineData("dizaine", "20", "10")]
    [InlineData("centaine", "900", "100")]
    public void Accepts_unit_ten_and_hundred_positions(
        string answer,
        string correctAnswer,
        string place)
    {
        Assert.True(ExerciseAnswerValidator.Matches(
            PlaceValueExercise(correctAnswer, place),
            answer,
            "fr_FR"));
    }

    [Fact]
    public void Still_accepts_the_numeric_value_represented_by_the_digit()
    {
        Assert.True(ExerciseAnswerValidator.Matches(
            PlaceValueExercise("900", "100"),
            "neuf cents",
            "fr_FR"));
    }

    [Fact]
    public void Rejects_a_wrong_position_name()
    {
        Assert.False(ExerciseAnswerValidator.Matches(
            PlaceValueExercise("900", "100"),
            "dizaine",
            "fr_FR"));
    }

    [Theory]
    [InlineData("-2")]
    [InlineData("−2")]
    [InlineData("moins deux")]
    public void Accepts_negative_integer_forms(string answer)
    {
        var exercise = new Exercise(
            "negative-test",
            "negative-number-operations",
            string.Empty,
            "-2",
            string.Empty);

        Assert.True(ExerciseAnswerValidator.Matches(
            exercise,
            answer,
            "fr_FR"));
    }

    [Theory]
    [InlineData("2,75")]
    [InlineData("2.75")]
    [InlineData("02,750")]
    public void Accepts_comma_or_point_for_decimal_answers(string answer)
    {
        var exercise = new Exercise(
            "decimal-test",
            FoundationNumberCourseCatalog.DecimalNumbersCourseId,
            string.Empty,
            "2.75",
            string.Empty,
            AnswerKind: ExerciseAnswerKind.Decimal);

        Assert.True(ExerciseAnswerValidator.Matches(
            exercise,
            answer,
            "fr_FR"));
    }

    /// <summary>
    /// Construit le contrat minimal d'une question de valeur positionnelle.
    /// </summary>
    private static Exercise PlaceValueExercise(
        string correctAnswer,
        string place)
    {
        return new Exercise(
            "place-value-test",
            "large-numbers",
            string.Empty,
            correctAnswer,
            string.Empty,
            AnswerKind: ExerciseAnswerKind.PlaceValue,
            AnswerArguments: [place]);
    }
}
