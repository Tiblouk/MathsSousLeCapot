namespace MathsSousLeCapot.Core.Progress;

/// <summary>
/// Applique les règles de gestion des profils sans dépendre du système de fichiers.
/// </summary>
public static class LocalProfileRegistryRules
{
    /// <summary>
    /// Ajoute un profil au registre et le sélectionne immédiatement.
    /// </summary>
    public static LocalProfileRegistry Add(
        LocalProfileRegistry registry,
        LocalProfile profile)
    {
        var normalized = profile with { Name = NormalizeName(profile.Name) };
        EnsureUniqueName(registry, normalized.Name);
        if (registry.Profiles.Any(item => item.Id == normalized.Id))
        {
            throw new InvalidOperationException("L'identifiant du profil existe déjà.");
        }

        return registry with
        {
            ActiveProfileId = normalized.Id,
            Profiles = [.. registry.Profiles, normalized]
        };
    }

    /// <summary>
    /// Renomme un profil existant tout en conservant son identifiant et ses données.
    /// </summary>
    public static LocalProfileRegistry Rename(
        LocalProfileRegistry registry,
        string profileId,
        string name)
    {
        var normalizedName = NormalizeName(name);
        EnsureUniqueName(registry, normalizedName, profileId);
        if (!registry.Profiles.Any(profile => profile.Id == profileId))
        {
            throw new InvalidOperationException("Le profil demandé n'existe pas.");
        }

        return registry with
        {
            Profiles = registry.Profiles
                .Select(profile => profile.Id == profileId
                    ? profile with { Name = normalizedName }
                    : profile)
                .ToArray()
        };
    }

    /// <summary>
    /// Sélectionne un profil existant sans modifier sa progression.
    /// </summary>
    public static LocalProfileRegistry Select(
        LocalProfileRegistry registry,
        string profileId)
    {
        if (!registry.Profiles.Any(profile => profile.Id == profileId))
        {
            throw new InvalidOperationException("Le profil demandé n'existe pas.");
        }

        return registry with { ActiveProfileId = profileId };
    }

    /// <summary>
    /// Retire un profil et sélectionne le premier restant si le profil actif disparaît.
    /// </summary>
    public static LocalProfileRegistry Remove(
        LocalProfileRegistry registry,
        string profileId)
    {
        if (registry.Profiles.Count <= 1)
        {
            throw new InvalidOperationException(
                "Le dernier profil local ne peut pas être supprimé.");
        }

        if (!registry.Profiles.Any(profile => profile.Id == profileId))
        {
            return registry;
        }

        var remaining = registry.Profiles
            .Where(profile => profile.Id != profileId)
            .ToArray();
        return new LocalProfileRegistry(
            registry.ActiveProfileId == profileId
                ? remaining[0].Id
                : registry.ActiveProfileId,
            remaining);
    }

    /// <summary>
    /// Nettoie un nom et impose une longueur adaptée à l'interface mobile.
    /// </summary>
    public static string NormalizeName(string name)
    {
        var normalized = name.Trim();
        if (normalized.Length is < 1 or > 30)
        {
            throw new ArgumentException(
                "Le nom doit contenir entre 1 et 30 caractères.",
                nameof(name));
        }

        return normalized;
    }

    /// <summary>
    /// Empêche deux profils de partager le même nom indépendamment de la casse.
    /// </summary>
    private static void EnsureUniqueName(
        LocalProfileRegistry registry,
        string name,
        string? excludedProfileId = null)
    {
        if (registry.Profiles.Any(profile =>
            profile.Id != excludedProfileId
            && string.Equals(profile.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Un profil porte déjà ce nom.");
        }
    }
}
