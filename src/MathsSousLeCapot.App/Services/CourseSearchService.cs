using System.Globalization;
using System.Text;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Recherche les cours dans leurs métadonnées localisées sans charger leur contenu.
/// </summary>
public sealed class CourseSearchService
{
    /// <summary>
    /// Filtre l'index avec tous les mots saisis et, si présent, un tag précis.
    /// </summary>
    public IReadOnlyList<CourseSearchEntry> Search(string? query, string? tagId)
    {
        var tokens = Normalize(query)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var text = TranslationService.Current;
        var chapterTitles = CourseCatalog.ChapterHeaders.ToDictionary(
            chapter => chapter.Id,
            chapter => text.Get(chapter.Title),
            StringComparer.Ordinal);
        var groupTitles = SchoolGroupCatalog.Definitions.ToDictionary(
            group => group.Id,
            group => text.Get(group.Title),
            StringComparer.Ordinal);
        var tagTitles = CourseSearchCatalog.Tags.ToDictionary(
            tag => tag.Id,
            tag => text.Get(tag.LabelKey),
            StringComparer.Ordinal);

        return CourseSearchCatalog.Entries
            .Where(entry => tagId is null || entry.TagIds.Contains(tagId))
            .Where(entry => tokens.Length == 0 || MatchesAllTokens(
                entry,
                tokens,
                chapterTitles,
                groupTitles,
                tagTitles))
            .ToArray();
    }

    /// <summary>
    /// Vérifie chaque mot dans le titre, le niveau, la catégorie et les tags.
    /// </summary>
    private static bool MatchesAllTokens(
        CourseSearchEntry entry,
        IReadOnlyList<string> tokens,
        IReadOnlyDictionary<string, string> chapterTitles,
        IReadOnlyDictionary<string, string> groupTitles,
        IReadOnlyDictionary<string, string> tagTitles)
    {
        var text = TranslationService.Current;
        var searchableText = Normalize(string.Join(
            ' ',
            text.Get(entry.TitleKey),
            text.Get(entry.LevelKey),
            chapterTitles[entry.ChapterId],
            groupTitles[entry.SchoolGroupId],
            entry.CourseId.Replace('-', ' '),
            string.Join(' ', entry.TagIds.Select(tagId => tagTitles[tagId]))));
        return tokens.All(token => searchableText.Contains(token, StringComparison.Ordinal));
    }

    /// <summary>
    /// Retire la casse, les accents et la ponctuation qui ne doivent pas gêner une recherche.
    /// </summary>
    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var decomposed = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(char.IsLetterOrDigit(character) ? character : ' ');
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
