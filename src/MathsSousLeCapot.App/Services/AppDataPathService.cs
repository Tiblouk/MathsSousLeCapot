namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Résout l'emplacement lisible où l'application conserve ses données locales.
/// </summary>
public static class AppDataPathService
{
    /// <summary>
    /// Nom du dossier de données placé à côté de l'exécutable portable.
    /// </summary>
    private const string PortableDataDirectoryName = "Data";

    /// <summary>
    /// Nom du dossier applicatif utilisé hors mode portable.
    /// </summary>
    private const string ApplicationDataDirectoryName = "MathsSousLeCapot";

    /// <summary>
    /// Nom du dossier qui isole les données de chaque profil local.
    /// </summary>
    private const string ProfilesDirectoryName = "Profiles";

    /// <summary>
    /// Retourne le dossier de données en le créant si nécessaire.
    /// </summary>
    public static string GetDataDirectory()
    {
        var portableDirectory = Path.Combine(
            AppContext.BaseDirectory,
            PortableDataDirectoryName);
        if (OperatingSystem.IsWindows()
            && Directory.Exists(portableDirectory))
        {
            Directory.CreateDirectory(portableDirectory);
            return portableDirectory;
        }

        var root = OperatingSystem.IsWindows()
            ? Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                ApplicationDataDirectoryName)
            : Path.Combine(
                FileSystem.Current.AppDataDirectory,
                ApplicationDataDirectoryName);
        var dataDirectory = Path.Combine(root, PortableDataDirectoryName);
        Directory.CreateDirectory(dataDirectory);
        return dataDirectory;
    }

    /// <summary>
    /// Retourne le chemin complet d'un fichier de données local.
    /// </summary>
    public static string GetDataFilePath(string fileName)
    {
        return Path.Combine(GetDataDirectory(), fileName);
    }

    /// <summary>
    /// Retourne le dossier privé d'un profil après validation de son identifiant.
    /// </summary>
    public static string GetProfileDataDirectory(string profileId)
    {
        if (string.IsNullOrWhiteSpace(profileId)
            || profileId.Length > 64
            || profileId.Any(character =>
                !char.IsLetterOrDigit(character)
                && character is not '-' and not '_'))
        {
            throw new ArgumentException(
                "L'identifiant du profil local est invalide.",
                nameof(profileId));
        }

        var directory = Path.Combine(
            GetDataDirectory(),
            ProfilesDirectoryName,
            profileId);
        Directory.CreateDirectory(directory);
        return directory;
    }

    /// <summary>
    /// Retourne le chemin d'un fichier appartenant à un profil local.
    /// </summary>
    public static string GetProfileDataFilePath(string profileId, string fileName)
    {
        return Path.Combine(GetProfileDataDirectory(profileId), fileName);
    }
}
