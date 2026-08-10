using System.Globalization;
using MathsSousLeCapot.Core.Training;

namespace MathsSousLeCapot.App.Features.Training;

/// <summary>
/// Produit un raisonnement détaillé à partir du contrat d'un exercice.
/// </summary>
public static class DetailedExerciseExplanation
{
    /// <summary>
    /// Phrase courte expliquant si la réponse donnée est correcte.
    /// </summary>
    public static string CreateStatus(ExerciseResult result)
    {
        if (result.IsCorrect)
        {
            return "Ta réponse correspond bien à la réponse attendue. Le détail ci-dessous montre pourquoi.";
        }

        if (string.IsNullOrWhiteSpace(result.GivenAnswer))
        {
            return "Tu n’as pas donné de réponse. Le détail ci-dessous montre comment trouver la réponse attendue.";
        }

        return $"Ta réponse ({result.GivenAnswer}) ne correspond pas à la réponse attendue ({result.Exercise.CorrectAnswer}). Le détail ci-dessous montre où mène le raisonnement.";
    }

    /// <summary>
    /// Construit l'explication détaillée adaptée à la clé de question.
    /// </summary>
    public static string CreateExplanation(ExerciseResult result)
    {
        var exercise = result.Exercise;
        var key = exercise.QuestionKey ?? string.Empty;
        var values = exercise.QuestionArguments?.ToArray() ?? [];
        var answer = exercise.CorrectAnswer;
        var explanation = key switch
        {
            "exercise.addition.question" => ExplainAddition(values, answer),
            "exercise.subtraction.question" => ExplainSubtraction(values, answer),
            "exercise.counter.after" => ExplainCounter(values, answer, next: true),
            "exercise.counter.before" => ExplainCounter(values, answer, next: false),
            "exercise.binary.after" => ExplainBinary(values, answer, next: true),
            "exercise.binary.before" => ExplainBinary(values, answer, next: false),
            _ when key.StartsWith("exercise.assessment.", StringComparison.Ordinal) =>
                ExplainAssessment(key["exercise.assessment.".Length..], values, answer),
            _ when key.StartsWith("exercise.foundation.", StringComparison.Ordinal) =>
                ExplainFoundation(key["exercise.foundation.".Length..], values, answer),
            _ when key.StartsWith("exercise.primary.", StringComparison.Ordinal) =>
                ExplainPrimary(
                    key["exercise.primary.".Length..],
                    values,
                    answer,
                    exercise.AnswerArguments?.ToArray() ?? []),
            _ when key.StartsWith("exercise.middle.", StringComparison.Ordinal) =>
                ExplainMiddle(key["exercise.middle.".Length..], values, answer),
            _ when key.StartsWith("exercise.high.", StringComparison.Ordinal) =>
                ExplainHigh(key["exercise.high.".Length..], values, answer),
            _ => ExplainFallback(values, answer)
        };

        return $"{explanation}\n\n{CreateMisconception(result)}";
    }

    /// <summary>
    /// Explique une addition de deux quantités.
    /// </summary>
    private static string ExplainAddition(string[] values, string answer)
    {
        var left = I(values, 0);
        var right = I(values, 1);
        return Join(
            "Pour une addition, on réunit les deux quantités.",
            $"Ici, on réunit {left} et {right}.",
            $"Calcul : {left} + {right} = {answer}.",
            $"Le total est donc {answer}.");
    }

    /// <summary>
    /// Explique une soustraction de deux quantités.
    /// </summary>
    private static string ExplainSubtraction(string[] values, string answer)
    {
        var left = I(values, 0);
        var right = I(values, 1);
        return Join(
            "Pour une soustraction, on retire la deuxième quantité à la première.",
            $"Ici, on part de {left} et on retire {right}.",
            $"Calcul : {left} − {right} = {answer}.",
            $"Il reste donc {answer}.");
    }

    /// <summary>
    /// Explique le nombre suivant ou précédent dans une base.
    /// </summary>
    private static string ExplainCounter(
        string[] values,
        string answer,
        bool next)
    {
        var shown = values.ElementAtOrDefault(0) ?? string.Empty;
        var numberBase = values.ElementAtOrDefault(1) ?? "10";
        var direction = next ? "suivant" : "précédent";
        var operation = next ? "ajoute une unité" : "retire une unité";
        return Join(
            $"On cherche le nombre {direction} en base {numberBase}.",
            $"On part de {shown}, puis on {operation}.",
            $"Quand une colonne dépasse la base, on reporte vers la colonne suivante.",
            $"Le résultat est {answer}.");
    }

    /// <summary>
    /// Explique le nombre binaire suivant ou précédent.
    /// </summary>
    private static string ExplainBinary(
        string[] values,
        string answer,
        bool next)
    {
        var shown = values.ElementAtOrDefault(0) ?? string.Empty;
        return Join(
            "En base deux, on utilise seulement les chiffres 0 et 1.",
            next
                ? $"Pour trouver le suivant de {shown}, on ajoute 1 en base deux."
                : $"Pour trouver le précédent de {shown}, on retire 1 en base deux.",
            "Dès qu'une colonne dépasse 1, on fait une retenue vers la colonne de gauche.",
            $"Le résultat écrit en base deux est {answer}.");
    }

    /// <summary>
    /// Explique les questions de comptage ajoutées aux contrôles.
    /// </summary>
    private static string ExplainAssessment(
        string key,
        string[] values,
        string answer)
    {
        return key switch
        {
            "nextNumber" => Join(
                $"On part de {I(values, 0)}.",
                "Le nombre juste après s'obtient en ajoutant 1.",
                $"{I(values, 0)} + 1 = {answer}."),
            "previousNumber" => Join(
                $"On part de {I(values, 0)}.",
                "Le nombre juste avant s'obtient en retirant 1.",
                $"{I(values, 0)} − 1 = {answer}."),
            "tensAndUnits" => Join(
                "Une dizaine vaut 10 unités.",
                $"{I(values, 0)} dizaine(s) valent {I(values, 0) * 10}.",
                $"On ajoute {I(values, 1)} unité(s).",
                $"{I(values, 0) * 10} + {I(values, 1)} = {answer}."),
            _ => ExplainFallback(values, answer)
        };
    }

    /// <summary>
    /// Explique les exercices sur les nombres négatifs et décimaux.
    /// </summary>
    private static string ExplainFoundation(
        string key,
        string[] values,
        string answer)
    {
        return key switch
        {
            "negativeMove" => Join(
                "Sur une droite graduée, un déplacement positif va vers la droite et un déplacement négatif va vers la gauche.",
                $"On part de {I(values, 0)} et on effectue le déplacement {I(values, 1)}.",
                $"Calcul : {I(values, 0)} + ({I(values, 1)}) = {answer}."),
            "negativeOpposite" => Join(
                "Deux nombres opposés sont à la même distance de 0, mais de côtés différents.",
                $"L'opposé de {I(values, 0)} est donc {answer}."),
            "negativeGreater" => Join(
                "Sur une droite graduée, le plus grand nombre est celui qui est le plus à droite.",
                $"On compare {values[0]} et {values[1]}.",
                $"Le plus grand est {answer}."),
            "decimalRead" => Join(
                $"La fraction {values[1]}/{values[2]} représente une partie décimale.",
                $"On ajoute cette partie à {values[0]}.",
                $"Le nombre décimal obtenu est {answer}."),
            "decimalGreater" => Join(
                "Pour comparer deux décimaux, on compare d'abord les unités, puis les dixièmes, centièmes, etc.",
                $"On compare {values[0]} et {values[1]}.",
                $"Le plus grand est {answer}."),
            "decimalFraction" => Join(
                $"La fraction {values[0]}/{values[1]} se lit comme une division.",
                $"On calcule {values[0]} ÷ {values[1]}.",
                $"La forme décimale est {answer}."),
            _ => ExplainFallback(values, answer)
        };
    }

    /// <summary>
    /// Explique les exercices du primaire.
    /// </summary>
    private static string ExplainPrimary(
        string key,
        string[] values,
        string answer,
        string[] answerArguments)
    {
        var a = I(values, 0);
        var b = I(values, 1);
        var c = I(values, 2);

        return key switch
        {
            "largerNumber" => Join(
                $"On compare {a} et {b}.",
                "Le plus grand est celui qui est le plus loin quand on avance sur la file numérique.",
                $"Le plus grand nombre est {answer}."),
            "placeValue" => ExplainPlaceValue(values, answer, answerArguments),
            "roundToTen" => Join(
                $"On cherche la dizaine la plus proche de {a}.",
                "Si le chiffre des unités est 5 ou plus, on monte à la dizaine suivante ; sinon on descend.",
                $"La dizaine la plus proche est {answer}."),
            "fractionOfSet" => Join(
                $"On veut prendre {a}/{b} de {c}.",
                $"On partage {c} en {b} parts égales : une part vaut {c / Math.Max(1, b)}.",
                $"On prend {a} part(s), donc le résultat est {answer}."),
            "multiplication" => Join(
                $"Multiplier {a} par {b}, c'est prendre {a}, {b} fois.",
                $"Calcul : {a} × {b} = {answer}."),
            "division" => Join(
                $"Diviser {a} par {b}, c'est chercher combien de groupes de {b} on peut faire avec {a}.",
                $"Calcul : {a} ÷ {b} = {answer}."),
            "addition" => ExplainAddition(values, answer),
            "subtraction" or "mentalSubtraction" => ExplainSubtraction(values, answer),
            "repeatedMultiplication2" => ExplainPower(a, 2, answer),
            "repeatedMultiplication3" => ExplainPower(a, 3, answer),
            "repeatedMultiplication4" => ExplainPower(a, 4, answer),
            "lengthReading" => Join(
                "La longueur d'un segment sur une règle est l'écart entre l'arrivée et le départ.",
                $"Il commence à {a} cm et finit à {b} cm.",
                $"Calcul : {b} − {a} = {answer} cm."),
            "lengthConversion" => Join(
                "Un mètre contient 100 centimètres.",
                $"Donc {a} m = {a} × 100 = {answer} cm."),
            "massReading" => Join(
                "La balance additionne toutes les masses posées dessus.",
                $"Calcul : {a} g + {b} g = {answer} g."),
            "massConversion" => Join(
                "Un kilogramme contient 1000 grammes.",
                $"Donc {a} kg = {a} × 1000 = {answer} g."),
            "timeReading" => Join(
                "Une heure contient 60 minutes.",
                $"Donc {a} h = {a} × 60 = {answer} min."),
            "timeCalculation" => Join(
                "On additionne les deux durées exprimées en minutes.",
                $"Calcul : {a} + {b} = {answer} minutes."),
            "unitSquareArea" or "rectangleArea" => Join(
                "L'aire d'un rectangle se calcule avec longueur × largeur.",
                $"Calcul : {a} × {b} = {answer}."),
            "proportionality" => Join(
                $"Un lot contient {a} objets.",
                $"Pour {b} lots, on multiplie : {a} × {b} = {answer}."),
            "circleDiameter" => Join(
                "Le diamètre d'un cercle vaut deux fois le rayon.",
                $"Calcul : {a} × 2 = {answer}."),
            "rectanglePerimeter" => Join(
                "Le périmètre d'un rectangle est le tour de la figure.",
                $"On additionne deux longueurs et deux largeurs : 2 × ({a} + {b}) = {answer}."),
            "cuboidVolume" => Join(
                "Le volume d'un pavé droit se calcule avec longueur × largeur × hauteur.",
                $"Calcul : {a} × {b} × {c} = {answer}."),
            "dataTotal" => Join(
                "Pour trouver un total dans un tableau, on additionne les catégories.",
                $"Calcul : {a} + {b} + {c} = {answer}."),
            "chartMaximum" => Join(
                $"On compare les valeurs {a}, {b} et {c}.",
                $"La plus grande valeur est {answer}."),
            _ => ExplainKnownFact(key, answer)
        };
    }

    /// <summary>
    /// Explique les exercices du collège.
    /// </summary>
    private static string ExplainMiddle(
        string key,
        string[] values,
        string answer)
    {
        var a = I(values, 0);
        var b = I(values, 1);
        var c = I(values, 2);
        var d = I(values, 3);

        return key switch
        {
            "fractionComparison" => Join(
                $"Les deux fractions ont le même dénominateur {c}.",
                "Quand les dénominateurs sont égaux, la plus grande fraction a le plus grand numérateur.",
                $"On compare {a} et {b} : le plus grand numérateur est {answer}."),
            "signedDifference" => Join(
                "Soustraire un nombre revient à mesurer l'écart orienté entre les deux valeurs.",
                $"Calcul : {a} − {b} = {answer}."),
            "scientificExponent" => Join(
                "Dans 10ⁿ, l'exposant n indique le nombre de zéros après le 1.",
                $"{a} s'écrit 1 suivi de {answer} zéro(s).",
                $"Donc n = {answer}."),
            "euclideanQuotient" => ExplainEuclideanDivision(a, b, answer, askRemainder: false),
            "remainder" => ExplainEuclideanDivision(a, b, answer, askRemainder: true),
            "divisibility" => ExplainDivisibility(a, b, answer),
            "smallestDivisor" => Join(
                $"On cherche le plus petit nombre supérieur à 1 qui divise {a} sans reste.",
                $"En testant dans l'ordre, le premier diviseur trouvé est {answer}."),
            "primeFactorExponent" => Join(
                $"On décompose {a} en facteurs premiers.",
                "On compte combien de fois le facteur 2 apparaît.",
                $"L'exposant de 2 est {answer}."),
            "leastCommonMultiple" => Join(
                $"Le PPCM de {a} et {b} est le plus petit nombre non nul multiple des deux.",
                $"Le premier multiple commun obtenu est {answer}."),
            "operationPriority" => Join(
                "La multiplication est prioritaire sur l'addition.",
                $"On calcule d'abord {b} × {c} = {b * c}.",
                $"Puis {a} + {b * c} = {answer}."),
            "parentheses" => Join(
                "Les parenthèses se calculent en premier.",
                $"On calcule {a} + {b} = {a + b}.",
                $"Puis {a + b} × {c} = {answer}."),
            "decimalTenths" => Join(
                "Les deux nombres sont exprimés en dixièmes.",
                $"On additionne les numérateurs : {a} + {b} = {answer}.",
                $"Le numérateur du résultat est donc {answer}."),
            "fractionNumerator" => Join(
                $"Les fractions ont le même dénominateur {c}.",
                $"On additionne les numérateurs : {a} + {b} = {answer}."),
            "fractionProductNumerator" => Join(
                "Pour multiplier deux fractions, on multiplie les numérateurs entre eux.",
                $"Calcul : {a} × {b} = {answer}."),
            "greatestCommonDivisor" => Join(
                $"Le PGCD est le plus grand diviseur commun de {a} et {b}.",
                $"Le plus grand diviseur qui convient est {answer}."),
            "percentage" => Join(
                $"{a} % signifie {a} pour 100.",
                $"On calcule {a} % de {b} : {b} × {a} ÷ 100 = {answer}."),
            "percentageIncrease" => Join(
                $"On augmente {a} de {b} %.",
                $"La hausse vaut {a} × {b} ÷ 100 = {a * b / 100}.",
                $"Valeur finale : {a} + {a * b / 100} = {answer}."),
            "roundToTen" => ExplainPrimary("roundToTen", values, answer, []),
            "power" => ExplainPower(a, b, answer),
            "powerProductExponent" => Join(
                "Quand on multiplie deux puissances de même base, on additionne les exposants.",
                $"Donc 2^{a} × 2^{b} = 2^({a} + {b}) = 2^{answer}."),
            "squareRoot" => Join(
                "La racine carrée positive de ce nombre est le nombre qui, multiplié par lui-même, redonne ce nombre.",
                $"{answer} × {answer} = {a}.",
                $"Donc √{a} = {answer}."),
            "areaConversion" => Join(
                "Pour convertir des m² en cm², chaque dimension est multipliée par 100.",
                "L'aire est donc multipliée par 100 × 100 = 10 000.",
                $"{a} m² = {a} × 10 000 = {answer} cm²."),
            "volume" => Join(
                "Le volume d'un pavé droit se calcule avec longueur × largeur × hauteur.",
                $"Calcul : {a} × {b} × {c} = {answer}."),
            "pyramidVolume" => ExplainPyramidVolume(a, b, answer),
            "volumeConversion" => Join(
                "Pour convertir des m³ en dm³, chaque dimension est multipliée par 10.",
                "Le volume est donc multiplié par 10 × 10 × 10 = 1000.",
                $"{a} m³ = {a} × 1000 = {answer} dm³."),
            "speed" => Join(
                "La vitesse se calcule avec distance ÷ durée.",
                $"Calcul : {a} ÷ {b} = {answer} km/h."),
            "scale" or "proportion" or "thales" => Join(
                "On utilise un coefficient multiplicateur.",
                $"Calcul : {a} × {b} = {answer}."),
            "triangleArea" => Join(
                "L'aire d'un triangle vaut base × hauteur ÷ 2.",
                $"Calcul : {a} × {b} ÷ 2 = {answer}."),
            "circleArea" => Join(
                "L'aire d'un disque vaut π × rayon².",
                $"Le coefficient de π est donc {a}² = {answer}."),
            "cylinderVolume" => Join(
                "Le volume d'un cylindre vaut π × rayon² × hauteur.",
                $"Le coefficient de π est {a}² × {b} = {answer}."),
            "coneVolume" => Join(
                "Le volume d'un cône vaut π × rayon² × hauteur ÷ 3.",
                $"Le coefficient de π est {a}² × {b} ÷ 3 = {answer}."),
            "sphereVolume" => Join(
                "Le volume d'une sphère vaut 4 × π × rayon³ ÷ 3.",
                $"Le coefficient de π est 4 × {a}³ ÷ 3 = {answer}."),
            "centralSymmetry" => Join(
                "Une symétrie centrale équivaut à un demi-tour.",
                "Deux demi-tours ramènent la figure à sa position initiale.",
                $"La réponse est donc {answer}."),
            "centralSymmetryAngle" or "halfTurn" => Join(
                "Un demi-tour correspond à 180 degrés.",
                $"La réponse est donc {answer}."),
            "fullTurn" => Join(
                "Un tour complet correspond à 360 degrés.",
                $"La réponse est donc {answer}."),
            "enlargement" => Join(
                "Un agrandissement de coefficient 2 multiplie la longueur par 2.",
                $"Calcul : {a} × 2 = {answer}."),
            "translation" or "coordinate" => Join(
                "Une translation vers la droite augmente l'abscisse.",
                $"Calcul : {a} + {b} = {answer}."),
            "pythagoras" => Join(
                "Dans un triangle rectangle, l'hypoténuse vérifie a² + b² = c².",
                $"Ici : {a}² + {b}² = {a * a + b * b}.",
                $"La longueur de l'hypoténuse est {answer}."),
            "algebra" => Join(
                $"On remplace x par {a} dans 3x + {b}.",
                $"Calcul : 3 × {a} + {b} = {answer}."),
            "equation" => Join(
                $"L'équation est x + {b} = {a}.",
                $"On retire {b} des deux côtés : x = {a} − {b} = {answer}."),
            "function" => Join(
                $"On remplace x par {a} dans f(x) = 2x + {b}.",
                $"Calcul : 2 × {a} + {b} = {answer}."),
            "graphOrdinate" => Join(
                $"Le point donné est ({a} ; {b}).",
                $"À l'abscisse {a}, l'ordonnée lue est donc {answer}."),
            "functionPreimage" => Join(
                $"On cherche x tel que 2x + {b} = {a}.",
                $"On retire {b}, puis on divise par 2.",
                $"x = ({a} − {b}) ÷ 2 = {answer}."),
            "linearFunction" => Join(
                $"Pour une fonction linéaire f(x) = 3x, on multiplie l'entrée par 3.",
                $"Calcul : 3 × {a} = {answer}."),
            "trigonometry" => Join(
                "Dans ce triangle rectangle, on utilise le théorème de Pythagore.",
                $"Côté manquant² = {a}² − {b}².",
                $"La longueur du côté manquant est {answer}."),
            "mean" => Join(
                "La moyenne est la somme des valeurs divisée par le nombre de valeurs.",
                $"Somme : {a} + {b} + {c} + {d} = {a + b + c + d}.",
                $"Moyenne : {a + b + c + d} ÷ 4 = {answer}."),
            "median" => Join(
                "La médiane est la valeur du milieu dans une série ordonnée.",
                $"Dans {a}, {b}, {c}, la valeur du milieu est {answer}."),
            "range" => Join(
                "L'étendue est la plus grande valeur moins la plus petite.",
                $"Calcul : {c} − {a} = {answer}."),
            "firstQuartile" => Join(
                "Le premier quartile est la première valeur atteignant au moins un quart de la série.",
                $"Avec cette convention, la valeur obtenue est {answer}."),
            "totalCount" => Join(
                "L'effectif total est la somme des effectifs des catégories.",
                $"Calcul : {a} + {b} + {c} = {answer}."),
            "probabilityPercent" => Join(
                "Une probabilité en pourcentage se calcule avec issues favorables ÷ issues totales × 100.",
                $"Calcul : {a} ÷ {b} × 100 = {answer} %."),
            "isEven" => Join(
                "Un nombre pair est divisible par 2.",
                $"{a} {(a % 2 == 0 ? "est" : "n'est pas")} divisible par 2.",
                $"On répond donc {answer}."),
            "trueStatement" => Join(
                "Le symbole < signifie « est inférieur à ».",
                $"On compare donc {a} et {b}.",
                $"{a} est plus petit que {b}, donc l'affirmation {a} < {b} est vraie.",
                $"Comme il faut répondre 1 pour vrai, la bonne réponse est {answer}."),
            "combinatorics" => Join(
                "Pour chaque premier choix, il existe plusieurs seconds choix.",
                $"On multiplie : {a} × {b} = {answer}."),
            "condition" => Join(
                "L'algorithme compare les deux valeurs et affiche la plus grande.",
                $"Entre {a} et {b}, la plus grande valeur est {answer}."),
            "loop" => Join(
                $"La boucle ajoute {a}, {b} fois.",
                $"Cela revient à calculer {a} × {b} = {answer}."),
            "variable" => Join(
                "La variable change quand on lui ajoute une valeur.",
                $"Calcul : {a} + {b} = {answer}."),
            _ => ExplainFallback(values, answer)
        };
    }

    /// <summary>
    /// Explique les exercices du lycée avec la formule et les calculs intermédiaires.
    /// </summary>
    private static string ExplainHigh(
        string key,
        string[] values,
        string answer)
    {
        var a = I(values, 0);
        var b = I(values, 1);
        var c = I(values, 2);
        var d = I(values, 3);
        var e = I(values, 4);
        var f = I(values, 5);

        return key switch
        {
            "intervalMembership" => Join(
                $"L'intervalle [{b} ; {c}] contient tous les nombres compris entre {b} et {c}, bornes incluses.",
                $"On vérifie donc les deux conditions : {b} ≤ {a} et {a} ≤ {c}.",
                a >= b && a <= c
                    ? $"Les deux conditions sont vraies : {a} appartient à l'intervalle. On répond 1."
                    : $"Au moins une condition est fausse : {a} n'appartient pas à l'intervalle. On répond 0.",
                $"La réponse correcte est {answer}."),
            "divisibility" => ExplainDivisibility(a, b, answer),
            "absoluteDistance" => Join(
                "La distance entre deux nombres est la valeur absolue de leur différence.",
                $"Distance = |{a} − {b}|.",
                $"{a} − {b} = {a - b}, puis |{a - b}| = {answer}.",
                $"La distance est donc {answer}."),
            "expandedConstant" => Join(
                $"On développe (x + {a})(x + {b}) en multipliant chaque terme du premier facteur par chaque terme du second.",
                $"(x + {a})(x + {b}) = x² + {b}x + {a}x + {a} × {b}.",
                $"Le terme sans x est {a} × {b} = {answer}.",
                $"Le terme constant est donc {answer}."),
            "vectorAbscissa" => Join(
                "La coordonnée du vecteur AB se calcule par arrivée moins départ.",
                $"Abscisse de AB = xB − xA = {b} − {a}.",
                $"{b} − {a} = {answer}.",
                $"L'abscisse du vecteur AB est donc {answer}."),
            "lineSlope" => Join(
                "Le coefficient directeur d'une droite passant par A et B vaut (yB − yA) ÷ (xB − xA).",
                $"Ici : ({d} − {b}) ÷ ({c} − {a}).",
                $"Numérateur : {d} − {b} = {d - b}. Dénominateur : {c} − {a} = {c - a}.",
                $"Coefficient directeur = {d - b} ÷ {c - a} = {answer}."),
            "functionImage" => Join(
                $"Pour calculer l'image de {a}, on remplace x par {a} dans f(x) = 2x + {b}.",
                $"f({a}) = 2 × {a} + {b}.",
                $"2 × {a} = {2 * a}, puis {2 * a} + {b} = {answer}.",
                $"L'image de {a} est donc {answer}."),
            "affineIncreasing" => Join(
                "Une fonction affine f(x) = ax + b est croissante lorsque son coefficient directeur a est positif.",
                $"Ici, le coefficient directeur est {a}.",
                a > 0
                    ? $"Comme {a} > 0, la fonction est croissante : on répond 1."
                    : $"Comme {a} < 0, la fonction est décroissante : on répond 0.",
                $"La réponse correcte est {answer}."),
            "mean" => Join(
                "La moyenne est la somme des valeurs divisée par leur nombre.",
                $"Somme : {a} + {b} + {c} = {a + b + c}.",
                $"Il y a 3 valeurs : {a + b + c} ÷ 3 = {answer}.",
                $"La moyenne est donc {answer}."),
            "probabilityPercent" => Join(
                "Une probabilité se calcule avec issues favorables ÷ issues totales.",
                $"Ici : {a} ÷ {b}.",
                $"Pour obtenir un pourcentage : {a} ÷ {b} × 100 = {answer} %.",
                $"La probabilité vaut donc {answer} %."),
            "loop" => Join(
                $"La boucle ajoute {a} à chacune de ses {b} répétitions.",
                $"Une addition répétée revient à multiplier : {a} × {b}.",
                $"{a} × {b} = {answer}.",
                $"Le total obtenu est donc {answer}."),
            "listSum" => Join(
                "Pour calculer la somme d'une liste, on additionne successivement tous ses éléments.",
                $"Liste : [{a}, {b}, {c}].",
                $"{a} + {b} = {a + b}, puis {a + b} + {c} = {answer}.",
                $"La somme de la liste est donc {answer}."),
            "binaryConversion" => ExplainBinaryConversion(
                values.ElementAtOrDefault(0) ?? string.Empty,
                answer),
            "arithmeticSequence" => Join(
                "Pour une suite arithmétique, u(n) = u(0) + n × r.",
                $"Ici : u(0) = {a}, r = {b} et n = {c}.",
                $"u({c}) = {a} + {c} × {b} = {a} + {c * b} = {answer}.",
                $"Le terme demandé vaut donc {answer}."),
            "geometricSequence" => Join(
                "Pour une suite géométrique, u(n) = u(0) × qⁿ.",
                $"Ici : u(0) = {a}, q = {b} et n = {c}.",
                $"u({c}) = {a} × {b}^{c} = {a} × {(int)Math.Pow(b, c)} = {answer}.",
                $"Le terme demandé vaut donc {answer}."),
            "discriminant" => Join(
                "Pour ax² + bx + c, le discriminant est Δ = b² − 4ac.",
                $"On remplace : Δ = {b}² − 4 × {a} × {c}.",
                $"{b}² = {b * b} et 4 × {a} × {c} = {4 * a * c}.",
                $"Δ = {b * b} − {4 * a * c} = {answer}."),
            "rootSum" => Join(
                "La question donne directement les deux racines du trinôme.",
                $"On additionne les racines : {a} + {b}.",
                $"{a} + {b} = {answer}.",
                $"La somme des racines est donc {answer}."),
            "derivativePowerCoefficient" => Join(
                "La dérivée de axⁿ est a × n × xⁿ⁻¹.",
                $"Ici, f(x) = {a}x^{b}.",
                $"f'(x) = {a} × {b} × x^{b - 1} = {answer}x^{c}.",
                $"Le coefficient demandé est donc {answer}."),
            "tangentSlopeSquare" => Join(
                "La pente de la tangente au point d'abscisse x₀ est le nombre dérivé f'(x₀).",
                "Pour f(x) = x², la fonction dérivée est f'(x) = 2x.",
                $"Au point d'abscisse {a} : f'({a}) = 2 × {a}.",
                $"2 × {a} = {answer}. La pente de la tangente est donc {answer}."),
            "exponentialSum" => Join(
                "Pour multiplier deux exponentielles, on additionne leurs exposants : exp(x) × exp(y) = exp(x + y).",
                $"exp({a}) × exp({b}) = exp({a} + {b}).",
                $"{a} + {b} = {answer}.",
                $"L'exposant n vaut donc {answer}."),
            "trigonometricRemarkable" => Join(
                "Sur le cercle trigonométrique, sin(π/2) est l'ordonnée du point situé en haut du cercle.",
                "Cette ordonnée vaut 1.",
                $"Donc sin(π/2) = {answer}."),
            "trigonometricValue" => Join(
                "Une valeur remarquable se lit sur le cercle trigonométrique : le cosinus est l'abscisse et le sinus est l'ordonnée.",
                $"On cherche {values.ElementAtOrDefault(0)}({values.ElementAtOrDefault(1)}).",
                $"La coordonnée correspondante sur le cercle vaut {answer}.",
                $"La valeur demandée est donc {answer}."),
            "dotProduct" => Join(
                "Dans le plan, u · v = ux × vx + uy × vy.",
                $"On remplace : {a} × {c} + {b} × {d}.",
                $"Produits : {a * c} et {b * d}.",
                $"Somme : {a * c} + {b * d} = {answer}."),
            "distanceHorizontal" => Join(
                "Sur une droite graduée, la distance est la valeur absolue de la différence des abscisses.",
                $"Distance = |{b} − {a}|.",
                $"|{b - a}| = {answer}.",
                $"La distance vaut donc {answer}."),
            "conditionalProbability" => Join(
                "La probabilité conditionnelle P_A(B) vaut P(A ∩ B) ÷ P(A).",
                $"Parmi {b} cas où A est réalisé, {a} réalisent aussi B.",
                $"P_A(B) = {a} ÷ {b} = {answer} ÷ 100.",
                $"En pourcentage, cela donne {answer} %."),
            "expectedValue" => Join(
                "Avec deux valeurs équiprobables, l'espérance est leur moyenne.",
                $"E(X) = ({a} + {b}) ÷ 2.",
                $"{a} + {b} = {a + b}, puis {a + b} ÷ 2 = {answer}.",
                $"L'espérance vaut donc {answer}."),
            "combination" => Join(
                "Le nombre de choix de k éléments parmi n est C(n,k) = n! ÷ (k!(n−k)!).",
                $"Ici : n = {a} et k = {b}.",
                $"C({a},{b}) = {a}! ÷ ({b}! × {a - b}!).",
                $"Après simplification, on obtient {answer} possibilités."),
            "spaceVectorZ" => Join(
                "La troisième coordonnée du vecteur AB se calcule par zB − zA.",
                $"zB − zA = {b} − {a}.",
                $"{b} − {a} = {answer}.",
                $"La troisième coordonnée vaut donc {answer}."),
            "planeNormalZ" => Join(
                "Dans l'équation ax + by + cz + d = 0, le vecteur (a ; b ; c) est normal au plan.",
                $"Le plan donné possède les coefficients {a}, {b} et {c} devant x, y et z.",
                $"Un vecteur normal est donc ({a} ; {b} ; {c}).",
                $"Sa troisième coordonnée est {answer}."),
            "spaceDotProduct" => Join(
                "Dans l'espace, u · v = ux × vx + uy × vy + uz × vz.",
                $"On remplace : {a} × {d} + {b} × {e} + {c} × {f}.",
                $"Produits : {a * d}, {b * e} et {c * f}.",
                $"Somme : {a * d} + {b * e} + {c * f} = {answer}."),
            "geometricLimit" => Join(
                "Une suite géométrique de raison q converge vers 0 lorsque |q| < 1.",
                "Ici q = 1/2, donc |q| = 1/2 < 1.",
                $"La suite tend donc vers {answer}."),
            "geometricLimitRatio" => Join(
                "Une suite géométrique de raison q converge vers 0 lorsque |q| < 1.",
                $"Ici q = {values.ElementAtOrDefault(0)} et sa valeur absolue est strictement inférieure à 1.",
                "Les puissances successives de q deviennent de plus en plus proches de 0.",
                $"La limite est donc {answer}."),
            "inductionInitial" => Join(
                "L'initialisation d'une récurrence consiste à vérifier la propriété au premier rang.",
                $"La suite est définie par u(n) = {a} + {b}n et le premier rang demandé est n = 0.",
                $"u(0) = {a} + {b} × 0 = {a} + 0 = {answer}.",
                $"La valeur initiale est donc {answer}."),
            "functionLimitAtInfinity" => Join(
                $"Quand x devient très grand, le numérateur {a} reste constant tandis que le dénominateur x grandit sans borne.",
                $"La fraction {a}/x devient donc de plus en plus petite en valeur absolue.",
                $"Elle se rapproche de {answer}.",
                $"Ainsi, la limite de {a}/x quand x tend vers +∞ est {answer}."),
            "intermediateValueExistence" => Join(
                "Pour une fonction continue, un changement de signe entre deux valeurs garantit le passage par 0.",
                $"Les valeurs aux bornes sont {a} et {b}.",
                a * b < 0
                    ? "Elles sont de signes opposés : le théorème des valeurs intermédiaires garantit au moins une solution. On répond 1."
                    : "Elles ont le même signe : ces seules données ne garantissent pas un passage par 0. On répond 0.",
                $"La réponse correcte est {answer}."),
            "logarithmProduct" => Join(
                "Le logarithme népérien est la fonction réciproque de l'exponentielle : ln(eˣ) = x.",
                $"Donc ln(e^{a}) = {a} et ln(e^{b}) = {b}.",
                $"On additionne : {a} + {b} = {answer}.",
                $"La valeur de n est donc {answer}."),
            "integralTwoX" => Join(
                "Une primitive de 2x est x².",
                $"L'intégrale entre 0 et {a} vaut [x²] de 0 à {a}.",
                $"On calcule {a}² − 0² = {a * a} − 0 = {answer}.",
                $"L'intégrale vaut donc {answer}."),
            "differentialExponent" => Join(
                "Les solutions de y' = ay sont de la forme y(x) = Ce^(ax).",
                $"Dans l'équation donnée, le coefficient qui multiplie y est {a}.",
                $"Le même coefficient apparaît devant x dans l'exponentielle.",
                $"L'exposant est donc {answer}x."),
            "binomialExpectation" => Join(
                "Pour une loi binomiale de paramètres n et p, l'espérance vaut E(X) = np.",
                $"Le calcul demandé dans cette ancienne question est {a} × {b}.",
                $"{a} × {b} = {answer}."),
            "binomialExpectationFraction" => Join(
                "Pour une loi binomiale de paramètres n et p, l'espérance vaut E(X) = np.",
                $"Ici : n = {a} et p = {b}/{c}.",
                $"E(X) = {a} × {b}/{c} = {a * b}/{c}.",
                $"Comme {a * b} ÷ {c} = {answer}, l'espérance vaut {answer}."),
            "poissonExpectation" => Join(
                "Pour une loi de Poisson de paramètre λ, l'espérance vaut E(X) = λ.",
                $"Ici λ = {a}.",
                $"L'espérance vaut donc directement {answer}."),
            "geometricExpectation" => Join(
                "Pour une loi géométrique de probabilité de succès p, l'espérance du rang du premier succès vaut E(X) = 1/p.",
                $"Ici p = 1/{a}.",
                $"E(X) = 1 ÷ (1/{a}) = {answer}.",
                $"Il faut donc attendre en moyenne {answer} essais."),
            "varianceSum" => Join(
                "Pour deux variables aléatoires indépendantes, Var(X + Y) = Var(X) + Var(Y).",
                $"On remplace : Var(X + Y) = {a} + {b}.",
                $"{a} + {b} = {answer}.",
                $"La variance de la somme vaut donc {answer}."),
            "trueStatement" or "largeNumbersStabilization" => Join(
                "La loi des grands nombres décrit le comportement d'une moyenne lorsque le nombre d'essais augmente.",
                "La moyenne observée se rapproche de l'espérance, même si de petites fluctuations restent possibles.",
                $"L'affirmation est donc vraie : la réponse correcte est {answer}."),
            "largeNumbersExact" => Join(
                "La loi des grands nombres annonce une stabilisation autour de l'espérance, pas une égalité parfaite après un nombre fixé d'essais.",
                "Des fluctuations aléatoires peuvent encore subsister.",
                $"L'affirmation est donc fausse : la réponse correcte est {answer}."),
            "midpoint" => Join(
                "Avant de diviser par 2 pour trouver le milieu, on additionne les deux bornes.",
                $"Somme des bornes : {a} + {b} = {answer}.",
                "Cette ancienne question demande seulement cette somme, pas encore le milieu."),
            "intervalMidpoint" => Join(
                "Le milieu d'un intervalle [a ; b] se calcule avec (a + b) ÷ 2.",
                $"On remplace : ({a} + {b}) ÷ 2.",
                $"{a} + {b} = {a + b}, puis {a + b} ÷ 2 = {answer}.",
                $"Le milieu de l'intervalle est donc {answer}."),
            _ => ExplainFallback(values, answer)
        };
    }

    /// <summary>
    /// Décompose une écriture binaire en puissances de deux.
    /// </summary>
    private static string ExplainBinaryConversion(string binary, string answer)
    {
        var terms = binary
            .Reverse()
            .Select((digit, exponent) => $"{digit} × 2^{exponent}")
            .Reverse();
        return Join(
            "En base deux, chaque position correspond à une puissance de 2.",
            $"{binary}₂ = {string.Join(" + ", terms)}.",
            $"En additionnant les valeurs des positions contenant 1, on obtient {answer}.",
            $"Donc {binary}₂ = {answer} en base dix.");
    }

    /// <summary>
    /// Explique la valeur d'un chiffre selon sa position.
    /// </summary>
    private static string ExplainPlaceValue(
        string[] values,
        string answer,
        string[] answerArguments)
    {
        var number = I(values, 0);
        var digit = I(values, 1);
        var place = I(answerArguments, 0);
        var placeName = place switch
        {
            1 => "unité",
            10 => "dizaine",
            100 => "centaine",
            1000 => "millier",
            _ => $"position {place}"
        };
        return Join(
            $"Dans {number}, on cherche la valeur du chiffre {digit}.",
            $"Ce chiffre est placé dans la colonne des {placeName}s.",
            $"Sa valeur est donc {digit} × {place} = {answer}.",
            $"Le nom de position « {placeName} » est aussi une réponse correcte.");
    }

    /// <summary>
    /// Explique une multiplication répétée.
    /// </summary>
    private static string ExplainPower(int value, int repetitions, string answer)
    {
        var factors = string.Join(" × ", Enumerable.Repeat(value.ToString(), repetitions));
        return Join(
            $"On multiplie {value} par lui-même {repetitions} fois.",
            $"Calcul : {factors} = {answer}.",
            $"Le résultat est donc {answer}.");
    }

    /// <summary>
    /// Explique les petites connaissances de géométrie et de logique du primaire.
    /// </summary>
    private static string ExplainKnownFact(string key, string answer)
    {
        var sentence = key switch
        {
            "segmentEndpoints" => "Un segment est limité par deux extrémités.",
            "segmentBetweenPoints" => "Entre deux points distincts, on trace un seul segment direct.",
            "parallelIntersections" => "Deux droites parallèles ne se croisent jamais.",
            "perpendicularAngle" => "Deux droites perpendiculaires forment un angle droit.",
            "rightAngle" => "Un angle droit mesure 90 degrés.",
            "straightAngle" => "Un angle plat mesure 180 degrés.",
            "angleReading" => "La mesure demandée est celle indiquée sur l'angle.",
            "triangleSides" => "Un triangle possède trois côtés.",
            "triangleVertices" => "Un triangle possède trois sommets.",
            "quadrilateralSides" => "Un quadrilatère possède quatre côtés.",
            "quadrilateralVertices" => "Un quadrilatère possède quatre sommets.",
            "pentagonSides" => "Un pentagone possède cinq côtés.",
            "hexagonSides" => "Un hexagone possède six côtés.",
            "octagonSides" => "Un octogone possède huit côtés.",
            "squareSymmetryAxes" => "Un carré possède quatre axes de symétrie.",
            "rectangleSymmetryAxes" => "Un rectangle non carré possède deux axes de symétrie.",
            "cubeFaces" => "Un cube possède six faces.",
            "cubeVertices" => "Un cube possède huit sommets.",
            "cubeEdges" => "Un cube possède douze arêtes.",
            "cubeNetFaces" or "cubeNetSquares" => "Un cube est formé de six faces carrées.",
            _ => "On utilise la propriété indiquée par l'énoncé."
        };
        return Join(sentence, $"La réponse attendue est {answer}.");
    }

    /// <summary>
    /// Explique une division euclidienne.
    /// </summary>
    private static string ExplainEuclideanDivision(
        int dividend,
        int divisor,
        string answer,
        bool askRemainder)
    {
        var quotient = dividend / divisor;
        var remainder = dividend % divisor;
        return Join(
            $"On cherche combien de fois {divisor} rentre dans {dividend}.",
            $"{divisor} × {quotient} = {divisor * quotient}.",
            $"{dividend} − {divisor * quotient} = {remainder}.",
            $"Le quotient est {quotient} et le reste est {remainder}.",
            askRemainder
                ? $"La question demande le reste : {answer}."
                : $"La question demande le quotient entier : {answer}.");
    }

    /// <summary>
    /// Explique un test de divisibilité.
    /// </summary>
    private static string ExplainDivisibility(
        int number,
        int divisor,
        string answer)
    {
        var remainder = number % divisor;
        return Join(
            $"Pour savoir si {number} est divisible par {divisor}, on regarde le reste de la division.",
            $"{number} ÷ {divisor} donne un reste de {remainder}.",
            remainder == 0
                ? $"Le reste est 0, donc on répond 1."
                : $"Le reste n'est pas 0, donc on répond 0.",
            $"La réponse attendue est {answer}.");
    }

    /// <summary>
    /// Explique le volume d'une pyramide, avec l'erreur classique sans division.
    /// </summary>
    private static string ExplainPyramidVolume(int baseArea, int height, string answer)
    {
        var product = baseArea * height;
        return Join(
            "Pour calculer le volume d'une pyramide, on utilise la formule :",
            "Volume = aire de la base × hauteur ÷ 3.",
            $"Ici : aire de la base = {baseArea} et hauteur = {height}.",
            $"On remplace : Volume = {baseArea} × {height} ÷ 3.",
            $"On calcule : {baseArea} × {height} = {product}.",
            $"{product} ÷ 3 = {answer}.",
            $"Le volume de la pyramide est donc {answer}.");
    }

    /// <summary>
    /// Ajoute une remarque sur l'erreur classique quand elle est reconnaissable.
    /// </summary>
    private static string CreateMisconception(ExerciseResult result)
    {
        if (result.IsCorrect)
        {
            return "Conclusion : la réponse donnée est correcte.";
        }

        if (string.IsNullOrWhiteSpace(result.GivenAnswer))
        {
            return "Conclusion : aucune réponse n'a été donnée. Reprends les étapes ci-dessus pour construire le raisonnement complet.";
        }

        var key = result.Exercise.QuestionKey ?? string.Empty;
        var values = result.Exercise.QuestionArguments?.ToArray() ?? [];
        var given = result.GivenAnswer.Trim();

        if (key == "exercise.middle.pyramidVolume"
            && int.TryParse(given, out var givenNumber)
            && givenNumber == I(values, 0) * I(values, 1))
        {
            return "La réponse donnée correspond seulement à aire de base × hauteur. Pour une pyramide, il manque la division par 3.";
        }

        if (key == "exercise.middle.trueStatement")
        {
            return "Si la réponse est fausse, vérifie bien le sens du symbole < : il signifie « est inférieur à ».";
        }

        return "Conclusion : compare ta réponse au résultat attendu et repère l'étape où le calcul ou le raisonnement change.";
    }

    /// <summary>
    /// Fournit un détail minimal lorsque la clé n'est pas encore spécialisée.
    /// </summary>
    private static string ExplainFallback(string[] values, string answer)
    {
        var data = values.Length == 0
            ? "aucune valeur numérique supplémentaire"
            : string.Join(", ", values);
        return Join(
            $"Données utilisées : {data}.",
            $"En suivant la question, le résultat attendu est {answer}.");
    }

    /// <summary>
    /// Convertit une valeur de tableau en entier.
    /// </summary>
    private static int I(string[] values, int index)
    {
        return index < values.Length
            && int.TryParse(
                values[index],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var value)
            ? value
            : 0;
    }

    /// <summary>
    /// Assemble des lignes en paragraphes lisibles.
    /// </summary>
    private static string Join(params string[] lines)
    {
        return string.Join(Environment.NewLine, lines);
    }
}
