using Microsoft.Maui.Controls.Xaml;

namespace MathsSousLeCapot.App.Localization;

/// <summary>
/// Permet d'utiliser une clé JSON directement dans une propriété XAML.
/// </summary>
[ContentProperty(nameof(Key))]
[AcceptEmptyServiceProvider]
public sealed class TranslateExtension : IMarkupExtension<string>
{
    /// <summary>
    /// Clé recherchée dans le fichier de la langue active.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Résout la clé lorsque MAUI construit l'arbre visuel.
    /// </summary>
    /// <param name="serviceProvider">Services fournis par le moteur XAML.</param>
    public string ProvideValue(IServiceProvider serviceProvider)
    {
        return TranslationService.Current.Get(Key);
    }

    /// <summary>
    /// Implémentation non générique requise par le moteur XAML.
    /// </summary>
    /// <param name="serviceProvider">Services fournis par le moteur XAML.</param>
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
