using System.Diagnostics;
using MathsSousLeCapot.App.Localization;
using MathsSousLeCapot.Core.Progress;

namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Crée, sélectionne et supprime les profils conservés uniquement sur l'appareil.
/// </summary>
public sealed class LocalProfileService
{
    /// <summary>
    /// Identifiant stable du profil qui reçoit les données des anciennes versions.
    /// </summary>
    public const string DefaultProfileId = "default";

    /// <summary>
    /// Instance partagée afin que toutes les pages utilisent le même profil actif.
    /// </summary>
    public static LocalProfileService Current { get; } = new();

    /// <summary>
    /// Nom du registre JSON conservé à la racine des données locales.
    /// </summary>
    private const string RegistryFileName = "profiles.json";

    /// <summary>
    /// Fichiers historiques copiés vers le profil principal lors de la migration.
    /// </summary>
    private static readonly string[] LegacyDataFileNames =
    [
        "course_progress.json",
        "training_history.json"
    ];

    /// <summary>
    /// Verrou protégeant le registre contre deux modifications simultanées.
    /// </summary>
    private readonly object _sync = new();

    /// <summary>
    /// Copie en mémoire du registre après sa première lecture.
    /// </summary>
    private LocalProfileRegistry? _registry;

    /// <summary>
    /// Retourne le profil actuellement sélectionné.
    /// </summary>
    public LocalProfile ActiveProfile
    {
        get
        {
            lock (_sync)
            {
                var registry = EnsureRegistry();
                return registry.Profiles.Single(
                    profile => profile.Id == registry.ActiveProfileId);
            }
        }
    }

    /// <summary>
    /// Retourne une copie ordonnée des profils présents sur l'appareil.
    /// </summary>
    public IReadOnlyList<LocalProfile> GetProfiles()
    {
        lock (_sync)
        {
            return EnsureRegistry().Profiles.ToArray();
        }
    }

    /// <summary>
    /// Crée un nouveau profil vide et le rend immédiatement actif.
    /// </summary>
    public LocalProfile CreateProfile(string name)
    {
        lock (_sync)
        {
            var registry = EnsureRegistry();
            var profile = new LocalProfile(
                Guid.NewGuid().ToString("N"),
                name,
                DateTimeOffset.Now);
            _registry = LocalProfileRegistryRules.Add(registry, profile);
            SaveRegistry(_registry);
            return _registry.Profiles.Single(item => item.Id == profile.Id);
        }
    }

    /// <summary>
    /// Modifie le nom visible d'un profil sans toucher à sa progression.
    /// </summary>
    public LocalProfile RenameProfile(string profileId, string name)
    {
        lock (_sync)
        {
            var registry = EnsureRegistry();
            _registry = LocalProfileRegistryRules.Rename(registry, profileId, name);
            SaveRegistry(_registry);
            return _registry.Profiles.Single(item => item.Id == profileId);
        }
    }

    /// <summary>
    /// Sélectionne un profil existant pour les prochaines lectures et écritures.
    /// </summary>
    public void SetActiveProfile(string profileId)
    {
        lock (_sync)
        {
            var registry = EnsureRegistry();
            _registry = LocalProfileRegistryRules.Select(registry, profileId);
            SaveRegistry(_registry);
        }
    }

    /// <summary>
    /// Supprime un profil et ses données, à condition qu'un autre profil subsiste.
    /// </summary>
    public void DeleteProfile(string profileId)
    {
        lock (_sync)
        {
            var registry = EnsureRegistry();
            if (!registry.Profiles.Any(profile => profile.Id == profileId))
            {
                return;
            }

            _registry = LocalProfileRegistryRules.Remove(registry, profileId);
            SaveRegistry(_registry);

            var profileDirectory = AppDataPathService.GetProfileDataDirectory(profileId);
            if (Directory.Exists(profileDirectory))
            {
                Directory.Delete(profileDirectory, recursive: true);
            }
        }
    }

    /// <summary>
    /// Charge, répare ou crée le registre local selon les données disponibles.
    /// </summary>
    private LocalProfileRegistry EnsureRegistry()
    {
        if (_registry is not null)
        {
            return _registry;
        }

        var path = AppDataPathService.GetDataFilePath(RegistryFileName);
        if (File.Exists(path)
            && LocalDataSerializer.TryDeserialize<LocalProfileRegistry>(
                File.ReadAllText(path),
                out var stored)
            && stored is not null)
        {
            _registry = RepairRegistry(stored);
            SaveRegistry(_registry);
            return _registry;
        }

        if (File.Exists(path))
        {
            BackupInvalidRegistry(path);
        }

        var defaultProfile = new LocalProfile(
            DefaultProfileId,
            TranslationService.Current.Get("profile.defaultName"),
            DateTimeOffset.Now);
        _registry = new LocalProfileRegistry(DefaultProfileId, [defaultProfile]);
        MigrateLegacyData(DefaultProfileId);
        SaveRegistry(_registry);
        return _registry;
    }

    /// <summary>
    /// Écarte les profils invalides et garantit un identifiant actif existant.
    /// </summary>
    private static LocalProfileRegistry RepairRegistry(LocalProfileRegistry registry)
    {
        var profiles = (registry.Profiles ?? [])
            .Where(profile =>
                IsValidProfileId(profile.Id)
                && !string.IsNullOrWhiteSpace(profile.Name))
            .Select(profile => profile with { Name = profile.Name.Trim() })
            .DistinctBy(profile => profile.Id, StringComparer.Ordinal)
            .ToArray();
        if (profiles.Length == 0)
        {
            var fallback = new LocalProfile(
                DefaultProfileId,
                "Principal",
                DateTimeOffset.Now);
            return new LocalProfileRegistry(fallback.Id, [fallback]);
        }

        var activeProfileId = profiles.Any(
            profile => profile.Id == registry.ActiveProfileId)
            ? registry.ActiveProfileId
            : profiles[0].Id;
        return new LocalProfileRegistry(activeProfileId, profiles);
    }

    /// <summary>
    /// Copie les fichiers des versions sans profils sans supprimer les originaux.
    /// </summary>
    private static void MigrateLegacyData(string profileId)
    {
        foreach (var fileName in LegacyDataFileNames)
        {
            var source = AppDataPathService.GetDataFilePath(fileName);
            var destination = AppDataPathService.GetProfileDataFilePath(
                profileId,
                fileName);
            if (File.Exists(source) && !File.Exists(destination))
            {
                File.Copy(source, destination);
            }
        }
    }

    /// <summary>
    /// Préserve un registre illisible avant de recréer un fichier sain.
    /// </summary>
    private static void BackupInvalidRegistry(string path)
    {
        var directory = Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException(
                "Le dossier du registre de profils est introuvable.");
        var backupPath = Path.Combine(
            directory,
            $"profiles.invalid-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
        File.Copy(path, backupPath, overwrite: false);
    }

    /// <summary>
    /// Enregistre le registre avec le sérialiseur partagé du cœur.
    /// </summary>
    private static void SaveRegistry(LocalProfileRegistry registry)
    {
        try
        {
            File.WriteAllText(
                AppDataPathService.GetDataFilePath(RegistryFileName),
                LocalDataSerializer.Serialize(registry));
        }
        catch (IOException exception)
        {
            Debug.WriteLine($"Impossible d'enregistrer les profils : {exception}");
            throw;
        }
    }

    /// <summary>
    /// Accepte l'identifiant historique et les GUID générés localement.
    /// </summary>
    private static bool IsValidProfileId(string profileId)
    {
        return profileId == DefaultProfileId || Guid.TryParse(profileId, out _);
    }
}
