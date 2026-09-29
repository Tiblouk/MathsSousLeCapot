using MathsSousLeCapot.Core.Progress;

namespace MathsSousLeCapot.Core.Tests.Progress;

/// <summary>
/// Vérifie la gestion des profils indépendamment de l'interface et du stockage.
/// </summary>
public sealed class LocalProfileRegistryRulesTests
{
    [Fact]
    public void Add_creates_an_active_profile_with_a_normalized_name()
    {
        var registry = CreateRegistry();
        var profile = new LocalProfile("second", "  Élève  ", DateTimeOffset.UtcNow);

        var updated = LocalProfileRegistryRules.Add(registry, profile);

        Assert.Equal("second", updated.ActiveProfileId);
        Assert.Equal("Élève", updated.Profiles.Single(item => item.Id == "second").Name);
    }

    [Fact]
    public void Duplicate_names_are_rejected_ignoring_case()
    {
        var registry = CreateRegistry();
        var profile = new LocalProfile("second", "principal", DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            LocalProfileRegistryRules.Add(registry, profile));
    }

    [Fact]
    public void Removing_the_active_profile_selects_a_remaining_profile()
    {
        var first = new LocalProfile("first", "Premier", DateTimeOffset.UtcNow);
        var second = new LocalProfile("second", "Second", DateTimeOffset.UtcNow);
        var registry = new LocalProfileRegistry(second.Id, [first, second]);

        var updated = LocalProfileRegistryRules.Remove(registry, second.Id);

        Assert.Equal(first.Id, updated.ActiveProfileId);
        Assert.Equal([first], updated.Profiles);
    }

    [Fact]
    public void Last_profile_cannot_be_removed()
    {
        var registry = CreateRegistry();

        Assert.Throws<InvalidOperationException>(() =>
            LocalProfileRegistryRules.Remove(registry, "default"));
    }

    /// <summary>
    /// Crée un registre minimal utilisé par les scénarios de test.
    /// </summary>
    private static LocalProfileRegistry CreateRegistry()
    {
        var profile = new LocalProfile(
            "default",
            "Principal",
            DateTimeOffset.UtcNow);
        return new LocalProfileRegistry(profile.Id, [profile]);
    }
}
