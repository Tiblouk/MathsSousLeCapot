using System.Numerics;

namespace MathsSousLeCapot.Core.Mathematics.PositionalNumeration;

/// <summary>
/// Représente un chiffre, sa position, sa valeur de place et son libellé.
/// </summary>
public sealed record PositionalDigit(
    int Position,
    int Value,
    BigInteger PlaceValue,
    string Label);
