namespace MathsSousLeCapot.Core.Courses;

/// <summary>
/// Catalogue ordonné des notions du collège, de la sixième à la troisième.
/// </summary>
public static class MiddleSchoolCourseCatalog
{
    /// <summary>
    /// Définitions classées par niveau puis selon leur progression pédagogique.
    /// </summary>
    public static IReadOnlyList<MiddleSchoolCourseDefinition> Definitions { get; } =
    [
        // Sixième : nombres, calculs et premières représentations.
        D("compare-fractions", "understand-numbers", "sixth", "Comparer des fractions",
            "fractions-as-numbers", "3/7 < 5/7", "fractions", MiddleSchoolExerciseKind.FractionComparison),
        D("euclidean-division", "understand-operations", "sixth", "La division euclidienne",
            "written-division", "47 = 6 × 7 + 5", "division", MiddleSchoolExerciseKind.EuclideanDivision),
        D("quotient-and-remainder", "understand-operations", "sixth", "Quotient et reste",
            "euclidean-division", "83 ÷ 9 : quotient 9, reste 2", "division", MiddleSchoolExerciseKind.Remainder),
        D("multiples-and-divisors", "understand-operations", "sixth", "Les multiples et les diviseurs",
            "multiplication-tables", "24 est un multiple de 6", "divisibility", MiddleSchoolExerciseKind.Divisibility),
        D("prime-numbers", "understand-operations", "sixth", "Les nombres premiers",
            "multiples-and-divisors", "29 n'a que 1 et 29 comme diviseurs", "divisibility", MiddleSchoolExerciseKind.PrimeNumber),
        D("decimal-operations", "understand-operations", "sixth", "Calculer avec des nombres décimaux",
            "written-addition", "2,4 + 1,7 = 4,1", "decimal", MiddleSchoolExerciseKind.DecimalOperation),
        D("percentages", "understand-operations", "sixth", "Les pourcentages",
            "fractions-as-numbers", "25 % de 80 = 20", "percentage", MiddleSchoolExerciseKind.Percentage),
        D("exact-and-approximate-values", "understand-operations", "sixth", "Valeurs exactes et valeurs approchées",
            "rounding-and-estimation", "19,8 ≈ 20 à l'unité", "approximation", MiddleSchoolExerciseKind.Approximation),
        D("area-unit-conversion", "measurements", "sixth", "Convertir les unités d'aire",
            "area-measurement", "2 m² = 20 000 cm²", "measurements", MiddleSchoolExerciseKind.AreaConversion),
        D("volume-measurement", "measurements", "sixth", "Mesurer un volume",
            "cuboid-volume", "3 × 4 × 5 = 60 cm³", "measurements", MiddleSchoolExerciseKind.Volume),
        D("volume-unit-conversion", "measurements", "sixth", "Convertir les unités de volume",
            "volume-measurement", "2 m³ = 2 000 dm³", "measurements", MiddleSchoolExerciseKind.VolumeConversion),
        D("liters-and-cubic-meters", "measurements", "sixth", "Les litres et les mètres cubes",
            "volume-unit-conversion", "1 L = 1 dm³", "measurements", MiddleSchoolExerciseKind.VolumeConversion),
        D("scales", "measurements", "sixth", "Les échelles",
            "length-unit-conversion", "1 cm pour 100 cm réels : échelle 1:100", "scale", MiddleSchoolExerciseKind.Scale),
        D("proportionality-coefficient", "proportionality", "sixth", "Le coefficient de proportionnalité",
            "recognize-proportionality", "3 → 12 : coefficient 4", "proportion", MiddleSchoolExerciseKind.Proportion),
        D("rule-of-three", "proportionality", "sixth", "La règle de trois",
            "proportionality-coefficient", "3 objets coûtent 12 €, 5 coûtent 20 €", "proportion", MiddleSchoolExerciseKind.Proportion),
        D("triangle-area", "geometry-plane", "sixth", "Aire d'un triangle",
            "rectangle-area", "A = base × hauteur ÷ 2", "geometry", MiddleSchoolExerciseKind.TriangleArea),
        D("circle-area", "geometry-plane", "sixth", "Aire d'un disque",
            "circle-introduction", "A = π × 4² = 16π", "geometry", MiddleSchoolExerciseKind.CircleArea),
        D("central-symmetry", "geometry-plane", "sixth", "La symétrie centrale",
            "axial-symmetry", "Un demi-tour autour du centre", "transformations", MiddleSchoolExerciseKind.Transformation),
        D("prism-volume", "geometry-space", "sixth", "Volume d'un prisme",
            "volume-measurement", "V = aire de base × hauteur", "solids", MiddleSchoolExerciseKind.Volume),
        D("variables-introduction", "algebra", "sixth", "Une lettre peut représenter un nombre",
            "multiplication-basics", "Pour x = 4, x + 3 = 7", "algebra", MiddleSchoolExerciseKind.Algebra),
        D("translate-to-expression", "algebra", "sixth", "Traduire une phrase en expression",
            "variables-introduction", "Le double de x plus 3 : 2x + 3", "algebra", MiddleSchoolExerciseKind.Algebra),
        D("simple-equations", "algebra", "sixth", "Résoudre une équation simple",
            "variables-introduction", "x + 7 = 12, donc x = 5", "equations", MiddleSchoolExerciseKind.Equation),
        D("counts-and-frequencies", "statistics", "sixth", "Effectifs et fréquences",
            "read-data-table", "8 réponses sur 20 donnent 40 %", "statistics", MiddleSchoolExerciseKind.Statistics),
        D("mean", "statistics", "sixth", "La moyenne",
            "counts-and-frequencies", "(8 + 10 + 12) ÷ 3 = 10", "statistics", MiddleSchoolExerciseKind.Statistics),
        D("random-experiment", "probability", "sixth", "Comprendre une expérience aléatoire",
            null, "Lancer un dé produit une issue de 1 à 6", "probability", MiddleSchoolExerciseKind.Probability),
        D("outcomes-and-events", "probability", "sixth", "Issues et événements",
            "random-experiment", "Obtenir un nombre pair : {2, 4, 6}", "probability", MiddleSchoolExerciseKind.Probability),
        D("simple-probability", "probability", "sixth", "Calculer une probabilité simple",
            "outcomes-and-events", "3 issues favorables sur 6 : 50 %", "probability", MiddleSchoolExerciseKind.Probability),
        D("mathematical-statements", "logic", "sixth", "Une affirmation mathématique",
            null, "« 8 est pair » est une affirmation vraie", "logic", MiddleSchoolExerciseKind.Logic),
        D("truth-and-counterexamples", "logic", "sixth", "Vrai, faux et contre-exemple",
            "mathematical-statements", "3 est un contre-exemple à « tous les nombres sont pairs »", "logic", MiddleSchoolExerciseKind.Logic),
        D("coordinate-plane", "coordinate-geometry", "sixth", "Se repérer dans un plan",
            "points-lines-segments", "L'axe horizontal porte les abscisses", "coordinates", MiddleSchoolExerciseKind.Coordinate),
        D("point-coordinates", "coordinate-geometry", "sixth", "Coordonnées d'un point",
            "coordinate-plane", "A(3 ; −2)", "coordinates", MiddleSchoolExerciseKind.Coordinate),
        D("counting-possibilities", "combinatorics", "sixth", "Compter des possibilités",
            "multiplication-basics", "3 hauts et 2 bas donnent 6 tenues", "counting", MiddleSchoolExerciseKind.Combinatorics),
        D("choice-trees", "combinatorics", "sixth", "Arbres de choix",
            "counting-possibilities", "Chaque branche représente un choix", "counting", MiddleSchoolExerciseKind.Combinatorics),
        D("algorithms-introduction", "algorithms", "sixth", "Comprendre un algorithme",
            null, "Lire, calculer, puis afficher", "algorithms", MiddleSchoolExerciseKind.Algorithm),
        D("algorithm-variables", "algorithms", "sixth", "Variables",
            "algorithms-introduction", "x ← 4 puis x ← x + 3 donne 7", "algorithms", MiddleSchoolExerciseKind.Algorithm),
        D("algorithm-conditions", "algorithms", "sixth", "Conditions",
            "algorithm-variables", "Si x > 0, afficher « positif »", "algorithms", MiddleSchoolExerciseKind.Algorithm),
        D("algorithm-loops", "algorithms", "sixth", "Boucles",
            "algorithm-conditions", "Répéter 4 fois l'ajout de 3 donne 12", "algorithms", MiddleSchoolExerciseKind.Algorithm),

        // Cinquième : nombres relatifs, expressions et premières fonctions.
        D("rational-numbers", "understand-numbers", "fifth", "Les nombres rationnels",
            "compare-fractions", "−3/4 est un nombre rationnel", "fractions", MiddleSchoolExerciseKind.RationalOperation),
        D("operation-priority", "understand-operations", "fifth", "Les priorités opératoires",
            "multiplication-tables", "2 + 3 × 4 = 14", "calculation", MiddleSchoolExerciseKind.OperationPriority),
        D("parentheses", "understand-operations", "fifth", "Les parenthèses",
            "operation-priority", "(2 + 3) × 4 = 20", "calculation", MiddleSchoolExerciseKind.OperationPriority),
        D("negative-number-operations", "understand-operations", "fifth", "Calculer avec des nombres négatifs",
            "rational-numbers", "−4 + 7 = 3", "signed", MiddleSchoolExerciseKind.SignedOperation),
        D("fraction-addition-subtraction", "understand-operations", "fifth", "Additionner et soustraire des fractions",
            "compare-fractions", "3/8 + 2/8 = 5/8", "fractions", MiddleSchoolExerciseKind.FractionOperation),
        D("simplify-fractions", "understand-operations", "fifth", "Simplifier une fraction",
            "multiples-and-divisors", "12/18 = 2/3", "fractions", MiddleSchoolExerciseKind.SimplifyFraction),
        D("powers-introduction", "understand-powers", "fifth", "Comprendre les puissances",
            "repeated-multiplication", "3⁴ = 3 × 3 × 3 × 3", "powers", MiddleSchoolExerciseKind.Power),
        D("squares", "understand-powers", "fifth", "Les carrés",
            "powers-introduction", "7² = 49", "powers", MiddleSchoolExerciseKind.Power),
        D("cubes", "understand-powers", "fifth", "Les cubes",
            "powers-introduction", "4³ = 64", "powers", MiddleSchoolExerciseKind.Power),
        D("powers-of-ten", "understand-powers", "fifth", "Les puissances de dix",
            "powers-introduction", "10⁵ = 100 000", "powers", MiddleSchoolExerciseKind.ScientificNotation),
        D("speed-distance-time", "measurements", "fifth", "Vitesse, distance et durée",
            "proportionality-coefficient", "120 km en 2 h donnent 60 km/h", "speed", MiddleSchoolExerciseKind.Speed),
        D("proportionality-and-percentages", "proportionality", "fifth", "Proportionnalité et pourcentages",
            "percentages", "15 % de 200 = 30", "percentage", MiddleSchoolExerciseKind.Percentage),
        D("proportionality-and-scales", "proportionality", "fifth", "Proportionnalité et échelles",
            "scales", "2 cm à l'échelle 1:500 représentent 10 m", "scale", MiddleSchoolExerciseKind.Scale),
        D("proportionality-graph", "proportionality", "fifth", "Représentation graphique d'une proportionnalité",
            "proportionality-coefficient", "Les points sont alignés avec l'origine", "proportion", MiddleSchoolExerciseKind.Proportion),
        D("translations", "geometry-plane", "fifth", "Les translations",
            "central-symmetry", "Chaque point se déplace du même vecteur", "transformations", MiddleSchoolExerciseKind.Transformation),
        D("enlargement-and-reduction", "geometry-plane", "fifth", "Agrandissements et réductions",
            "scales", "Un coefficient 2 double toutes les longueurs", "transformations", MiddleSchoolExerciseKind.Scale),
        D("simplify-expression", "algebra", "fifth", "Réduire une expression",
            "translate-to-expression", "3x + 2x = 5x", "algebra", MiddleSchoolExerciseKind.Algebra),
        D("distributivity", "algebra", "fifth", "La distributivité",
            "simplify-expression", "3(x + 4) = 3x + 12", "algebra", MiddleSchoolExerciseKind.Algebra),
        D("functions-introduction", "functions", "fifth", "Comprendre la notion de fonction",
            "variables-introduction", "f(x) = 2x + 1", "functions", MiddleSchoolExerciseKind.Function),
        D("read-a-graph", "functions", "fifth", "Lire une courbe",
            "coordinate-plane", "À l'abscisse 3, on lit l'ordonnée 7", "functions", MiddleSchoolExerciseKind.Function),
        D("image-and-preimage", "functions", "fifth", "Image et antécédent",
            "functions-introduction", "Si f(3) = 7, 7 est l'image de 3", "functions", MiddleSchoolExerciseKind.Function),
        D("median", "statistics", "fifth", "La médiane",
            "mean", "Dans 2, 5, 9, la médiane est 5", "statistics", MiddleSchoolExerciseKind.Statistics),
        D("range-and-dispersion", "statistics", "fifth", "Étendue et dispersion",
            "counts-and-frequencies", "12 − 3 = 9 d'étendue", "statistics", MiddleSchoolExerciseKind.Statistics),
        D("frequency-and-probability", "probability", "fifth", "Fréquence et probabilité",
            "simple-probability", "48 succès sur 100 donnent une fréquence de 48 %", "probability", MiddleSchoolExerciseKind.Probability),
        D("arithmetic-multiples-divisors", "arithmetic", "fifth", "Multiples et diviseurs",
            "multiples-and-divisors", "36 est divisible par 4 et par 9", "divisibility", MiddleSchoolExerciseKind.Divisibility),
        D("arithmetic-prime-numbers", "arithmetic", "fifth", "Nombres premiers",
            "prime-numbers", "31 est premier", "divisibility", MiddleSchoolExerciseKind.PrimeNumber),

        // Quatrième : calcul littéral, théorèmes et approfondissements numériques.
        D("fraction-multiplication-division", "understand-operations", "fourth", "Multiplier et diviser des fractions",
            "fraction-addition-subtraction", "2/3 × 5/4 = 10/12", "fractions", MiddleSchoolExerciseKind.FractionOperation),
        D("percentage-changes", "understand-operations", "fourth", "Augmentations et diminutions en pourcentage",
            "percentages", "80 augmenté de 25 % donne 100", "percentage", MiddleSchoolExerciseKind.PercentageChange),
        D("power-rules", "understand-powers", "fourth", "Les règles de calcul sur les puissances",
            "powers-introduction", "2³ × 2⁴ = 2⁷", "powers", MiddleSchoolExerciseKind.Power),
        D("square-roots", "understand-powers", "fourth", "Les racines carrées",
            "squares", "√81 = 9", "roots", MiddleSchoolExerciseKind.SquareRoot),
        D("cross-multiplication", "proportionality", "fourth", "Le produit en croix",
            "rule-of-three", "3/5 = x/20, donc x = 12", "proportion", MiddleSchoolExerciseKind.Proportion),
        D("rotations", "geometry-plane", "fourth", "Les rotations",
            "central-symmetry", "Une rotation conserve longueurs et angles", "transformations", MiddleSchoolExerciseKind.Transformation),
        D("pythagorean-theorem", "geometry-plane", "fourth", "Le théorème de Pythagore",
            "right-triangle-ratios", "3² + 4² = 5²", "pythagoras", MiddleSchoolExerciseKind.Pythagoras),
        D("pythagorean-converse", "geometry-plane", "fourth", "La réciproque de Pythagore",
            "pythagorean-theorem", "6² + 8² = 10² : le triangle est rectangle", "pythagoras", MiddleSchoolExerciseKind.Pythagoras),
        D("cylinder-volume", "geometry-space", "fourth", "Volume d'un cylindre",
            "circle-area", "V = π × r² × h", "solids", MiddleSchoolExerciseKind.CircleArea),
        D("expand-expression", "algebra", "fourth", "Développer une expression",
            "distributivity", "4(x + 3) = 4x + 12", "algebra", MiddleSchoolExerciseKind.Algebra),
        D("factor-expression", "algebra", "fourth", "Factoriser une expression",
            "distributivity", "5x + 15 = 5(x + 3)", "algebra", MiddleSchoolExerciseKind.Algebra),
        D("linear-equations", "algebra", "fourth", "Résoudre une équation du premier degré",
            "simple-equations", "3x + 2 = 14, donc x = 4", "equations", MiddleSchoolExerciseKind.Equation),
        D("linear-function", "functions", "fourth", "La fonction linéaire",
            "functions-introduction", "f(x) = 3x", "functions", MiddleSchoolExerciseKind.Function),
        D("right-triangle-ratios", "trigonometry", "fourth", "Triangle rectangle et rapports de longueurs",
            "triangles", "adjacent / hypoténuse", "trigonometry", MiddleSchoolExerciseKind.Trigonometry),
        D("cosine-introduction", "trigonometry", "fourth", "Cosinus",
            "right-triangle-ratios", "cos(α) = adjacent / hypoténuse", "trigonometry", MiddleSchoolExerciseKind.Trigonometry),
        D("quartiles", "statistics", "fourth", "Les quartiles",
            "median", "Q1 sépare le premier quart des données", "statistics", MiddleSchoolExerciseKind.Statistics),
        D("prime-factorization", "arithmetic", "fourth", "Décomposition en facteurs premiers",
            "arithmetic-prime-numbers", "84 = 2² × 3 × 7", "divisibility", MiddleSchoolExerciseKind.PrimeFactorization),
        D("greatest-common-divisor", "arithmetic", "fourth", "PGCD",
            "prime-factorization", "PGCD(18, 24) = 6", "divisibility", MiddleSchoolExerciseKind.SimplifyFraction),
        D("least-common-multiple", "arithmetic", "fourth", "PPCM",
            "prime-factorization", "PPCM(6, 8) = 24", "divisibility", MiddleSchoolExerciseKind.LeastCommonMultiple),

        // Troisième : synthèse du collège et préparation au lycée.
        D("scientific-notation", "understand-numbers", "third", "L'écriture scientifique",
            "powers-of-ten", "4 500 000 = 4,5 × 10⁶", "scientific", MiddleSchoolExerciseKind.ScientificNotation),
        D("homothety", "geometry-plane", "third", "Les homothéties",
            "enlargement-and-reduction", "Un rapport −2 double et inverse la direction", "transformations", MiddleSchoolExerciseKind.Transformation),
        D("thales-theorem", "geometry-plane", "third", "Le théorème de Thalès",
            "proportionality-coefficient", "AM/AB = AN/AC", "thales", MiddleSchoolExerciseKind.Thales),
        D("thales-converse", "geometry-plane", "third", "La réciproque de Thalès",
            "thales-theorem", "Des rapports égaux permettent de prouver le parallélisme", "thales", MiddleSchoolExerciseKind.Thales),
        D("geometric-transformations", "geometry-plane", "third", "Les transformations géométriques",
            "rotations", "Symétrie, translation, rotation et homothétie", "transformations", MiddleSchoolExerciseKind.Transformation),
        D("geometry-proofs", "geometry-plane", "third", "Démontrer en géométrie",
            "pythagorean-converse", "Données, propriété, calcul, conclusion", "proofs", MiddleSchoolExerciseKind.Logic),
        D("pyramid-volume", "geometry-space", "third", "Volume d'une pyramide",
            "prism-volume", "V = aire de base × hauteur ÷ 3", "solids", MiddleSchoolExerciseKind.Volume),
        D("cone-volume", "geometry-space", "third", "Volume d'un cône",
            "cylinder-volume", "V = π × r² × h ÷ 3", "solids", MiddleSchoolExerciseKind.CircleArea),
        D("sphere-volume", "geometry-space", "third", "Volume d'une sphère",
            "circle-area", "V = 4πr³ ÷ 3", "solids", MiddleSchoolExerciseKind.CircleArea),
        D("affine-function", "functions", "third", "La fonction affine",
            "linear-function", "f(x) = 2x + 3", "functions", MiddleSchoolExerciseKind.Function),
        D("implication-and-converse", "logic", "third", "Implication et réciproque",
            "truth-and-counterexamples", "Pythagore et sa réciproque sont deux énoncés distincts", "logic", MiddleSchoolExerciseKind.Logic)
    ];

    /// <summary>
    /// Recherche une définition par son identifiant stable.
    /// </summary>
    public static MiddleSchoolCourseDefinition Get(string courseId)
    {
        return Definitions.Single(definition => definition.Id == courseId);
    }

    /// <summary>
    /// Indique si un identifiant appartient au catalogue du collège.
    /// </summary>
    public static bool TryGet(
        string courseId,
        out MiddleSchoolCourseDefinition definition)
    {
        definition = Definitions.FirstOrDefault(item => item.Id == courseId)!;
        return definition is not null;
    }

    /// <summary>
    /// Réduit la répétition lors de la déclaration des métadonnées d'un cours.
    /// </summary>
    private static MiddleSchoolCourseDefinition D(
        string id,
        string chapterId,
        string grade,
        string frenchTitle,
        string? prerequisiteId,
        string example,
        string discovery,
        MiddleSchoolExerciseKind exerciseKind)
    {
        return new MiddleSchoolCourseDefinition(
            id,
            chapterId,
            $"chapter.level.{grade}",
            $"middle.course.{id}.title",
            prerequisiteId,
            example,
            $"middle.discovery.{discovery}",
            exerciseKind);
    }
}
