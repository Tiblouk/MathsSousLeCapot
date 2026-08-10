using System.Globalization;

namespace MathsSousLeCapot.App.Converters;

/// <summary>
/// Convertit un état booléen en couleur de réussite ou d'erreur.
/// </summary>
public sealed class BooleanToColorConverter : IValueConverter
{
    /// <summary>
    /// Retourne le vert pour vrai et le rouge pour faux.
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? Colors.Green : Colors.Red;
    }

    /// <summary>
    /// La conversion inverse n'est pas utilisée par l'interface.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
