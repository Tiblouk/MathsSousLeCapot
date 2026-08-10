namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Catalogue ordonné des notions du lycée, de la Seconde à la Terminale.
/// </summary>
public static class HighSchoolCourseCatalog
{
    /// <summary>
    /// Définitions classées par niveau puis selon la progression de cours avancés.
    /// </summary>
    public static IReadOnlyList<HighSchoolCourseDefinition> Definitions { get; } =
    [
        // Seconde : consolider le calcul, les fonctions, le repère et les données.
        D("number-sets-and-intervals", "understand-numbers", "second", "Ensembles de nombres et intervalles",
            "rational-numbers", "x ∈ [2 ; 7]", "sets", HighSchoolExerciseKind.IntervalMembership),
        D("integer-arithmetic", "arithmetic", "second", "Arithmétique des entiers",
            "prime-factorization", "84 = 2² × 3 × 7", "arithmetic", HighSchoolExerciseKind.Divisibility),
        D("real-numbers-roots-absolute-value", "understand-numbers", "second", "Nombres réels, racines et valeur absolue",
            "square-roots", "|−5| = 5", "real", HighSchoolExerciseKind.SquareRootDistance),
        D("literal-calculation-equations", "algebra", "second", "Calcul littéral, équations et inéquations",
            "linear-equations", "(x + 3)(x + 4) = x² + 7x + 12", "algebra", HighSchoolExerciseKind.AlgebraExpansion),
        D("plane-vectors", "coordinate-geometry", "second", "Vecteurs du plan",
            "point-coordinates", "AB(5 ; −2)", "vectors", HighSchoolExerciseKind.VectorCoordinates),
        D("line-equations", "coordinate-geometry", "second", "Droites du plan",
            "affine-function", "y = 3x − 2", "lines", HighSchoolExerciseKind.LineSlope),
        D("high-functions-introduction", "functions", "second", "Notion de fonction",
            "functions-introduction", "f(4) = 2 × 4 + 1", "functions", HighSchoolExerciseKind.FunctionImage),
        D("function-variations", "functions", "second", "Variations et extremums",
            "read-a-graph", "Une fonction croissante garde l'ordre des nombres", "variations", HighSchoolExerciseKind.FunctionVariation),
        D("descriptive-statistics", "statistics", "second", "Information chiffrée et statistique descriptive",
            "quartiles", "moyenne, médiane, quartiles et étendue", "statistics", HighSchoolExerciseKind.StatisticsMean),
        D("simple-probabilities", "probability", "second", "Probabilités",
            "simple-probability", "3 issues favorables sur 10", "probability", HighSchoolExerciseKind.SimpleProbability),
        D("python-variables-conditions-loops", "algorithms", "second", "Variables, conditions, boucles et fonctions",
            "algorithm-loops", "répéter 5 fois l'ajout de 3", "algorithms", HighSchoolExerciseKind.AlgorithmLoop),
        D("number-base-conversion", "understand-numbers", "second", "Convertir entre bases numériques",
            CourseCatalog.BinaryCourseId, "1010₂ = 10", "bases", HighSchoolExerciseKind.BaseConversion),

        // Première spécialité : installer les outils centraux du cycle terminal.
        D("sequences-introduction", "sequences", "first", "Suites numériques et modèles discrets",
            "high-functions-introduction", "u(n+1) = u(n) + 4", "sequences", HighSchoolExerciseKind.ArithmeticSequenceTerm),
        D("arithmetic-sequences", "sequences", "first", "Suites arithmétiques",
            "sequences-introduction", "u(n) = u(0) + nr", "sequences", HighSchoolExerciseKind.ArithmeticSequenceTerm),
        D("geometric-sequences", "sequences", "first", "Suites géométriques",
            "arithmetic-sequences", "u(n) = u(0)qⁿ", "sequences", HighSchoolExerciseKind.GeometricSequenceTerm),
        D("quadratic-functions", "algebra", "first", "Fonctions du second degré",
            "literal-calculation-equations", "f(x) = ax² + bx + c", "quadratic", HighSchoolExerciseKind.QuadraticDiscriminant),
        D("quadratic-equations-discriminant", "algebra", "first", "Discriminant et racines",
            "quadratic-functions", "Δ = b² − 4ac", "quadratic", HighSchoolExerciseKind.QuadraticRootSum),
        D("derivative-introduction", "analysis", "first", "Dérivation locale",
            "function-variations", "la tangente porte la pente instantanée", "derivatives", HighSchoolExerciseKind.TangentSlope),
        D("derivative-and-variations", "analysis", "first", "Calcul des dérivées et variations",
            "derivative-introduction", "si f' > 0, alors f croît", "derivatives", HighSchoolExerciseKind.DerivativePower),
        D("exponential-function", "functions", "first", "Fonction exponentielle",
            "geometric-sequences", "exp(x + y) = exp(x)exp(y)", "exponential", HighSchoolExerciseKind.ExponentialRule),
        D("unit-circle", "trigonometry", "first", "Cercle trigonométrique",
            "cosine-introduction", "cos²(x) + sin²(x) = 1", "trigonometry", HighSchoolExerciseKind.TrigonometricValue),
        D("dot-product", "coordinate-geometry", "first", "Produit scalaire",
            "plane-vectors", "u · v = xx' + yy'", "dotProduct", HighSchoolExerciseKind.DotProduct),
        D("analytic-geometry-plane", "coordinate-geometry", "first", "Géométrie repérée",
            "line-equations", "AB = √((xB − xA)² + (yB − yA)²)", "analyticGeometry", HighSchoolExerciseKind.Distance),
        D("conditional-probabilities", "probability", "first", "Probabilités conditionnelles et indépendance",
            "simple-probabilities", "P_A(B) = P(A ∩ B) / P(A)", "conditionalProbability", HighSchoolExerciseKind.ConditionalProbability),
        D("random-variables", "probability", "first", "Variables aléatoires",
            "conditional-probabilities", "E(X) = Σ xi P(X = xi)", "randomVariables", HighSchoolExerciseKind.ExpectedValue),
        D("list-algorithms-simulations", "algorithms", "first", "Listes, boucles et simulations",
            "python-variables-conditions-loops", "parcourir une liste pour calculer une moyenne", "algorithms", HighSchoolExerciseKind.AlgorithmList),

        // Terminale spécialité : approfondir l'analyse, les probabilités et l'espace.
        D("combinatorics-principles", "combinatorics", "terminal", "Combinatoire et dénombrement",
            "choice-trees", "3 choix puis 4 choix donnent 12 possibilités", "combinatorics", HighSchoolExerciseKind.Combination),
        D("binomial-coefficients", "combinatorics", "terminal", "Coefficients binomiaux",
            "combinatorics-principles", "C(n,k) = n! / (k!(n-k)!)", "combinatorics", HighSchoolExerciseKind.Combination),
        D("vectors-in-space", "geometry-space", "terminal", "Vecteurs de l'espace",
            "plane-vectors", "u(x ; y ; z)", "spaceGeometry", HighSchoolExerciseKind.SpaceVectorCoordinate),
        D("lines-and-planes-in-space", "geometry-space", "terminal", "Droites et plans dans l'espace",
            "vectors-in-space", "M = A + t u", "spaceGeometry", HighSchoolExerciseKind.SpacePlaneNormal),
        D("dot-product-in-space", "geometry-space", "terminal", "Orthogonalité, distances et équations cartésiennes",
            "dot-product", "ax + by + cz + d = 0", "spaceGeometry", HighSchoolExerciseKind.SpaceDotProduct),
        D("sequence-limits", "sequences", "terminal", "Suites et limites",
            "geometric-sequences", "une suite croissante majorée converge", "limits", HighSchoolExerciseKind.GeometricSequenceLimit),
        D("mathematical-induction", "logic", "terminal", "Raisonnement par récurrence",
            "sequences-introduction", "initialisation, hérédité, conclusion", "proofs", HighSchoolExerciseKind.Induction),
        D("function-limits", "analysis", "terminal", "Limites de fonctions",
            "derivative-and-variations", "comportement de f(x) quand x devient très grand", "limits", HighSchoolExerciseKind.FunctionLimit),
        D("continuity-intermediate-value", "analysis", "terminal", "Continuité et théorème des valeurs intermédiaires",
            "function-limits", "une courbe continue prend toutes les valeurs entre deux hauteurs", "continuity", HighSchoolExerciseKind.Continuity),
        D("convexity", "analysis", "terminal", "Compléments de dérivation et convexité",
            "derivative-and-variations", "si f'' ≥ 0, la fonction est convexe", "convexity", HighSchoolExerciseKind.DerivativePower),
        D("logarithm-function", "functions", "terminal", "Fonction logarithme",
            "exponential-function", "ln(ab) = ln(a) + ln(b)", "logarithm", HighSchoolExerciseKind.LogarithmRule),
        D("sine-cosine-functions", "trigonometry", "terminal", "Fonctions sinus et cosinus",
            "unit-circle", "sin(x + 2π) = sin(x)", "trigonometry", HighSchoolExerciseKind.TrigonometricValue),
        D("antiderivatives", "analysis", "terminal", "Primitives",
            "derivative-and-variations", "une primitive de 2x est x²", "integral", HighSchoolExerciseKind.IntegralPower),
        D("differential-equations-introduction", "analysis", "terminal", "Équations différentielles simples",
            "exponential-function", "y' = ay donne y = Ce^(ax)", "differential", HighSchoolExerciseKind.DifferentialEquation),
        D("integral-introduction", "analysis", "terminal", "Calcul intégral",
            "antiderivatives", "∫[a,b] f(x) dx = F(b) − F(a)", "integral", HighSchoolExerciseKind.IntegralPower),
        D("binomial-distribution", "probability", "terminal", "Loi binomiale",
            "binomial-coefficients", "P(X = k) = C(n,k)p^k(1-p)^(n-k)", "binomial", HighSchoolExerciseKind.BinomialExpectation),
        D("geometric-and-poisson-laws", "probability", "terminal", "Lois géométrique et de Poisson",
            "binomial-distribution", "E(X) = 1/p pour une loi géométrique", "poisson", HighSchoolExerciseKind.PoissonExpectation),
        D("sums-of-random-variables", "probability", "terminal", "Sommes de variables aléatoires",
            "random-variables", "E(X + Y) = E(X) + E(Y)", "randomVariables", HighSchoolExerciseKind.VarianceSum),
        D("concentration-large-numbers", "probability", "terminal", "Concentration et loi des grands nombres",
            "sums-of-random-variables", "les moyennes se stabilisent autour de l'espérance", "largeNumbers", HighSchoolExerciseKind.LargeNumbers),
        D("numerical-methods", "algorithms", "terminal", "Récurrence, dichotomie, Newton et simulations",
            "python-variables-conditions-loops", "approcher une solution par dichotomie", "algorithms", HighSchoolExerciseKind.NumericalMethod)
    ];

    /// <summary>
    /// Recherche une définition par son identifiant stable.
    /// </summary>
    public static HighSchoolCourseDefinition Get(string courseId)
    {
        return Definitions.Single(definition => definition.Id == courseId);
    }

    /// <summary>
    /// Indique si un identifiant appartient au catalogue du lycée.
    /// </summary>
    public static bool TryGet(
        string courseId,
        out HighSchoolCourseDefinition definition)
    {
        definition = Definitions.FirstOrDefault(item => item.Id == courseId)!;
        return definition is not null;
    }

    /// <summary>
    /// Réduit la répétition lors de la déclaration des métadonnées d'un cours.
    /// </summary>
    private static HighSchoolCourseDefinition D(
        string id,
        string chapterId,
        string grade,
        string frenchTitle,
        string? prerequisiteId,
        string example,
        string discovery,
        HighSchoolExerciseKind exerciseKind)
    {
        return new HighSchoolCourseDefinition(
            id,
            chapterId,
            $"chapter.level.{grade}",
            $"high.course.{id}.title",
            prerequisiteId,
            example,
            $"high.discovery.{discovery}",
            exerciseKind);
    }
}
