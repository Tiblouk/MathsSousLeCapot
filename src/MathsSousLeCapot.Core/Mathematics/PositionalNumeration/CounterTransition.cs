using System.Numerics;

namespace MathsSousLeCapot.Core.Mathematics.PositionalNumeration;

/// <summary>
/// Décrit la modification d'une seule colonne.
/// </summary>
public sealed record DigitTransition(int Position, int Before, int After);

/// <summary>
/// Décrit le passage complet d'une valeur à une autre.
/// </summary>
public sealed record CounterTransition(
    BigInteger Before,
    BigInteger After,
    IReadOnlyList<DigitTransition> Digits,
    bool HasCarry,
    bool HasBorrow,
    string Explanation);
