namespace MathsSousLeCapot.Core.Mathematics.Operations;

/// <summary>
/// Décrit une réponse intermédiaire attendue sans imposer son texte d'interface.
/// </summary>
/// <param name="Kind">Nature pédagogique de l'étape.</param>
/// <param name="ExpectedValue">Valeur que l'élève doit trouver.</param>
/// <param name="ColumnIndex">Colonne comptée depuis les unités.</param>
/// <param name="RowIndex">Ligne intermédiaire comptée depuis les unités.</param>
/// <param name="LeftValue">Première donnée utile pour expliquer l'étape.</param>
/// <param name="RightValue">Deuxième donnée utile pour expliquer l'étape.</param>
/// <param name="IncomingValue">Retenue, quotient ou produit déjà obtenu.</param>
public sealed record GuidedCalculationStep(
    GuidedCalculationStepKind Kind,
    int ExpectedValue,
    int ColumnIndex = 0,
    int RowIndex = 0,
    int LeftValue = 0,
    int RightValue = 0,
    int IncomingValue = 0);
