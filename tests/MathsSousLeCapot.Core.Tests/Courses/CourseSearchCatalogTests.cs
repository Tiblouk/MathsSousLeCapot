using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.Core.Tests.Courses;

/// <summary>
/// Vérifie que l'index de recherche reste cohérent avec le catalogue visible.
/// </summary>
public sealed class CourseSearchCatalogTests
{
    [Fact]
    public void Search_index_contains_every_available_course_once()
    {
        var expectedIds = SchoolGroupCatalog.Definitions
            .SelectMany(group => group.Levels)
            .SelectMany(CourseCatalog.GetCoursesForLevel)
            .Where(course => course.IsAvailable)
            .Select(course => course.Id)
            .OrderBy(id => id)
            .ToArray();
        var indexedIds = CourseSearchCatalog.Entries
            .Select(entry => entry.CourseId)
            .OrderBy(id => id)
            .ToArray();

        Assert.Equal(expectedIds, indexedIds);
        Assert.Equal(indexedIds.Length, indexedIds.Distinct().Count());
    }

    [Fact]
    public void Every_search_entry_has_at_least_one_known_tag()
    {
        var knownTags = CourseSearchCatalog.Tags
            .Select(tag => tag.Id)
            .ToHashSet(StringComparer.Ordinal);

        Assert.All(
            CourseSearchCatalog.Entries,
            entry =>
            {
                Assert.NotEmpty(entry.TagIds);
                Assert.All(entry.TagIds, tag => Assert.Contains(tag, knownTags));
            });
    }

    [Fact]
    public void Specialized_topics_receive_specific_tags()
    {
        Assert.Contains(
            "fractions",
            CourseSearchCatalog.Entries
                .First(entry => entry.CourseId.Contains("fraction"))
                .TagIds);
        Assert.Contains(
            "decimals",
            CourseSearchCatalog.Entries
                .First(entry => entry.CourseId.Contains("decimal"))
                .TagIds);
    }
}
