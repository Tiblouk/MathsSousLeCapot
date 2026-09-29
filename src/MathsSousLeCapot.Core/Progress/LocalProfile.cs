namespace MathsSousLeCapot.Core.Progress;

/// <summary>
/// Représente un profil utilisateur conservé uniquement sur l'appareil.
/// </summary>
public sealed record LocalProfile(
    string Id,
    string Name,
    DateTimeOffset CreatedAt);

/// <summary>
/// Regroupe les profils locaux et l'identifiant du profil actif.
/// </summary>
public sealed record LocalProfileRegistry(
    string ActiveProfileId,
    IReadOnlyList<LocalProfile> Profiles);
