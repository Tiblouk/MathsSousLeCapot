using System.Text.Json;

namespace MathsSousLeCapot.Core.Progress;

/// <summary>
/// Sérialise les données locales et détecte les contenus incompatibles sans lever d'exception.
/// </summary>
public static class LocalDataSerializer
{
    /// <summary>
    /// Options partagées par toutes les données persistées localement.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Sérialise une valeur dans le format JSON local.
    /// </summary>
    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    /// <summary>
    /// Tente de lire une valeur et renvoie faux lorsque le contenu est vide ou invalide.
    /// </summary>
    public static bool TryDeserialize<T>(string? json, out T? value)
    {
        value = default;
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            value = JsonSerializer.Deserialize<T>(json, JsonOptions);
            return value is not null;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}
