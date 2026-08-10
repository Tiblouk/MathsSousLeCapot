# Cours avancés — Lycée

Ce document regroupe les cours de mathématiques du lycée à prévoir pour
l'application.

Il complète `cours.md` sans le remplacer. Son rôle est de donner une vision
plus détaillée des contenus du lycée, afin de préparer ensuite des cours
modulaires, interactifs et progressifs.

Le périmètre principal retenu est :

- **Seconde générale et technologique** ;
- **Première générale — spécialité mathématiques** ;
- **Terminale générale — spécialité mathématiques**.

Les parcours de **mathématiques complémentaires** et de **mathématiques
expertes** sont mentionnés comme extensions possibles, mais ils ne doivent pas
être mélangés avec le parcours de spécialité si le projet développe d'abord un
tronc lycée cohérent.

## Références de cadrage

Les programmes officiels consultés indiquent que les nouveaux programmes de
Seconde et de Première entrent en application à la rentrée scolaire 2026-2027,
et ceux de Terminale à la rentrée scolaire 2027-2028.

Sources de cadrage :

- https://eduscol.education.fr/5817/programmes-et-ressources-en-mathematiques-voie-gt
- https://www.education.gouv.fr/bo/2026/Hebdo14/MENE2602914A
- https://www.education.gouv.fr/bo/2026/Hebdo14/MENE2602917A
- https://www.education.gouv.fr/bo/2026/Hebdo14/MENE2602919A

## Convention de structure

Chaque niveau est organisé ainsi :

1. grands thèmes ;
2. chapitres ;
3. définitions et objets à connaître ;
4. propriétés, théorèmes et formules ;
5. méthodes et techniques indispensables ;
6. pistes de manipulation ou de visualisation.

Les chapitres listés ici sont des chapitres pédagogiques. Ils ne sont pas encore
tous des modules développés dans l'application.

---

## Seconde

**But du niveau :** consolider le collège, installer les premiers outils du
lycée et préparer le choix d'un parcours en Première.

**Parcours concerné :** enseignement commun de mathématiques.

**Notions transversales :**

- vocabulaire ensembliste et logique ;
- raisonnement, contre-exemple, implication, réciproque ;
- calcul exact et calcul approché ;
- algorithmique et programmation en Python ;
- automatismes de calcul numérique, algébrique et graphique.

### Thème 1 — Nombres, calculs et algèbre

#### Ensembles de nombres et intervalles

**But :** savoir situer un nombre dans un ensemble et représenter des ensembles
de solutions.

**Définitions :**

- `N` : entiers naturels ;
- `Z` : entiers relatifs ;
- `D` : nombres décimaux ;
- `Q` : nombres rationnels ;
- `R` : nombres réels ;
- intervalle ouvert, fermé ou semi-ouvert ;
- appartenance `x ∈ A` ;
- inclusion `A ⊂ B` ;
- réunion `A ∪ B` ;
- intersection `A ∩ B`.

**Propriétés et formules :**

- `N ⊂ Z ⊂ D ⊂ Q ⊂ R` ;
- un intervalle représente une partie continue de la droite réelle ;
- l'intersection garde ce qui est commun ;
- la réunion rassemble ce qui appartient à au moins un ensemble.

**Méthodes indispensables :**

- traduire une inégalité en intervalle ;
- placer un intervalle sur une droite graduée ;
- lire une réunion ou une intersection d'intervalles ;
- utiliser un contre-exemple pour réfuter une affirmation.

**Visualisation utile :**

- droite numérique interactive ;
- zones colorées pour les intervalles ;
- superposition de deux ensembles pour montrer réunion et intersection.

#### Arithmétique des entiers

**But :** comprendre les propriétés de divisibilité avant les raisonnements plus
abstraits.

**Définitions :**

- multiple ;
- diviseur ;
- nombre pair et impair ;
- nombre premier ;
- décomposition en facteurs premiers ;
- division euclidienne.

**Propriétés et formules :**

- si `a = bq + r` avec `0 <= r < b`, alors `q` est le quotient et `r` le reste ;
- un entier naturel supérieur ou égal à 2 est soit premier, soit décomposable en
  produit de nombres premiers ;
- si `d` divise `a` et `b`, alors `d` divise `a + b` et `a - b`.

**Méthodes indispensables :**

- tester une divisibilité ;
- effectuer une division euclidienne ;
- décomposer un entier en facteurs premiers ;
- simplifier une fraction avec des facteurs communs.

**Visualisation utile :**

- grille de multiples ;
- arbre de factorisation ;
- division euclidienne avec quotient et reste visibles.

#### Nombres réels, racines et valeur absolue

**But :** relier calcul exact, distance et approximation.

**Définitions :**

- nombre réel ;
- racine carrée d'un nombre positif ;
- valeur absolue ;
- distance entre deux réels ;
- encadrement ;
- valeur approchée.

**Propriétés et formules :**

- si `a >= 0`, alors `sqrt(a)` est le nombre positif dont le carré vaut `a` ;
- `|x|` est la distance de `x` à `0` ;
- `|a - b|` est la distance entre `a` et `b` ;
- `sqrt(a²) = |a|`.

**Méthodes indispensables :**

- distinguer `sqrt(a²)` et `a` ;
- encadrer une racine carrée ;
- comparer deux nombres en utilisant une approximation ou un carré ;
- contrôler la vraisemblance d'un résultat.

**Visualisation utile :**

- carré dont l'aire vaut `a` pour introduire `sqrt(a)` ;
- droite graduée pour la valeur absolue.

#### Calcul littéral, équations et inéquations

**But :** manipuler des expressions sans perdre le sens des opérations.

**Définitions :**

- expression littérale ;
- développement ;
- factorisation ;
- équation ;
- inéquation ;
- solution d'une équation ou d'une inéquation.

**Propriétés et formules :**

- distributivité : `a(b + c) = ab + ac` ;
- identités remarquables :
  - `(a + b)² = a² + 2ab + b²` ;
  - `(a - b)² = a² - 2ab + b²` ;
  - `(a + b)(a - b) = a² - b²` ;
- équation produit nul : si `AB = 0`, alors `A = 0` ou `B = 0`.

**Méthodes indispensables :**

- réduire une expression ;
- choisir entre développer et factoriser ;
- résoudre une équation du premier degré ;
- résoudre une inéquation du premier degré ;
- vérifier une solution par substitution.

**Visualisation utile :**

- balance algébrique pour les équations ;
- tuiles algébriques pour la distributivité et les identités remarquables.

### Thème 2 — Géométrie

#### Vecteurs du plan

**But :** représenter un déplacement et calculer avec des directions.

**Définitions :**

- vecteur ;
- direction, sens, norme ;
- vecteurs égaux ;
- vecteur nul ;
- somme de vecteurs ;
- produit d'un vecteur par un nombre ;
- colinéarité.

**Propriétés et formules :**

- coordonnées de `AB` : `AB(xB - xA ; yB - yA)` ;
- norme dans un repère orthonormé :
  `||u|| = sqrt(x² + y²)` pour `u(x ; y)` ;
- deux vecteurs `u(x ; y)` et `v(x' ; y')` sont colinéaires lorsque
  `xy' - yx' = 0`.

**Méthodes indispensables :**

- construire une somme de vecteurs ;
- calculer les coordonnées d'un vecteur ;
- prouver un alignement par colinéarité ;
- prouver qu'un quadrilatère est un parallélogramme.

**Visualisation utile :**

- glisser-déposer de flèches ;
- parallélogramme dynamique pour l'addition vectorielle.

#### Droites du plan

**But :** passer entre équation, représentation graphique et propriétés
géométriques.

**Définitions :**

- coefficient directeur ;
- ordonnée à l'origine ;
- équation réduite `y = mx + p` ;
- équation cartésienne `ax + by + c = 0`.

**Propriétés et formules :**

- coefficient directeur entre deux points :
  `m = (yB - yA) / (xB - xA)` si `xA != xB` ;
- deux droites non verticales sont parallèles si elles ont le même coefficient
  directeur ;
- l'intersection de deux droites correspond à la solution d'un système de deux
  équations.

**Méthodes indispensables :**

- tracer une droite à partir de son équation ;
- lire graphiquement une équation réduite ;
- déterminer l'équation d'une droite passant par deux points ;
- résoudre un système simple par substitution ou combinaison.

**Visualisation utile :**

- repère interactif ;
- curseurs pour faire varier `m` et `p`.

### Thème 3 — Fonctions

#### Notion de fonction

**But :** comprendre qu'une fonction associe une sortie à une entrée.

**Définitions :**

- fonction ;
- variable ;
- image ;
- antécédent ;
- courbe représentative ;
- domaine de définition.

**Propriétés et formules :**

- `f(a)` désigne l'image de `a` par `f` ;
- résoudre `f(x) = k` revient à chercher les antécédents de `k` ;
- une courbe permet de lire approximativement images et antécédents.

**Méthodes indispensables :**

- calculer une image ;
- lire une image et un antécédent sur un graphique ;
- construire un tableau de valeurs ;
- passer d'une formule à une courbe.

**Visualisation utile :**

- machine à fonctions ;
- point mobile sur la courbe synchronisé avec un tableau.

#### Variations et extremums

**But :** décrire comment une grandeur évolue.

**Définitions :**

- fonction croissante ;
- fonction décroissante ;
- maximum ;
- minimum ;
- tableau de variations.

**Propriétés et formules :**

- si `a < b` et `f(a) <= f(b)`, la fonction est croissante sur l'intervalle
  considéré ;
- un extremum correspond à la plus grande ou la plus petite valeur atteinte sur
  un ensemble donné.

**Méthodes indispensables :**

- lire un tableau de variations ;
- dresser un tableau de variations à partir d'une courbe ;
- résoudre graphiquement une équation ou une inéquation ;
- utiliser les variations pour comparer deux images.

**Visualisation utile :**

- courbe avec portions montantes et descendantes colorées ;
- curseur montrant la variation de `f(x)`.

### Thème 4 — Statistiques et probabilités

#### Information chiffrée et statistique descriptive

**But :** résumer et interpréter une série de données.

**Définitions :**

- effectif ;
- fréquence ;
- moyenne ;
- médiane ;
- quartiles ;
- étendue ;
- diagramme en barres, histogramme, diagramme en boîte.

**Propriétés et formules :**

- fréquence : `effectif / effectif total` ;
- moyenne pondérée :
  `(n1x1 + n2x2 + ... + nkxk) / (n1 + n2 + ... + nk)` ;
- l'étendue vaut `maximum - minimum`.

**Méthodes indispensables :**

- calculer et interpréter une moyenne ;
- comparer médiane et moyenne ;
- construire ou lire un diagramme ;
- détecter une valeur aberrante ou une dispersion importante.

**Visualisation utile :**

- tableau de données transformé en graphique ;
- diagramme en boîte construit étape par étape.

#### Probabilités

**But :** modéliser une expérience aléatoire simple.

**Définitions :**

- expérience aléatoire ;
- issue ;
- univers ;
- événement ;
- événement contraire ;
- probabilité.

**Propriétés et formules :**

- `0 <= P(A) <= 1` ;
- `P(univers) = 1` ;
- `P(A barre) = 1 - P(A)` ;
- si `A` et `B` sont incompatibles, alors `P(A ∪ B) = P(A) + P(B)`.

**Méthodes indispensables :**

- lister les issues ;
- calculer une probabilité en situation d'équiprobabilité ;
- utiliser un tableau ou un arbre ;
- distinguer fréquence observée et probabilité théorique.

**Visualisation utile :**

- arbre de probabilités ;
- simulation répétée pour observer la stabilisation des fréquences.

### Thème 5 — Algorithmique et programmation

#### Variables, conditions, boucles et fonctions

**But :** automatiser un calcul ou une simulation mathématique.

**Définitions :**

- variable informatique ;
- affectation ;
- condition ;
- boucle bornée ;
- boucle non bornée ;
- fonction Python ;
- valeur renvoyée.

**Propriétés et méthodes :**

- une variable garde une valeur jusqu'à sa prochaine affectation ;
- une condition choisit entre plusieurs chemins ;
- une boucle répète une instruction ;
- une fonction isole un calcul réutilisable.

**Techniques indispensables :**

- écrire un algorithme en langage naturel ;
- traduire un algorithme simple en Python ;
- compléter un programme ;
- tester un programme sur plusieurs valeurs ;
- simuler une expérience aléatoire.

**Visualisation utile :**

- exécution pas à pas ;
- affichage de l'état des variables après chaque instruction.

---

## Première

**But du niveau :** installer les outils centraux du cycle terminal :
suites, second degré, dérivation, exponentielle, trigonométrie, produit scalaire
et probabilités conditionnelles.

**Parcours concerné :** spécialité mathématiques de Première générale.

### Thème 1 — Algèbre

#### Suites numériques et modèles discrets

**But :** modéliser une évolution qui avance par étapes.

**Définitions :**

- suite numérique ;
- terme d'indice `n` ;
- définition explicite ;
- définition par récurrence ;
- suite arithmétique ;
- suite géométrique ;
- raison d'une suite ;
- somme de termes.

**Propriétés et formules :**

- suite arithmétique : `u(n+1) = u(n) + r` ;
- terme général arithmétique : `u(n) = u(0) + nr` ;
- suite géométrique : `u(n+1) = q u(n)` ;
- terme général géométrique : `u(n) = u(0) q^n` ;
- somme arithmétique : `1 + 2 + ... + n = n(n + 1) / 2` ;
- somme géométrique : si `q != 1`,
  `1 + q + ... + q^n = (1 - q^(n+1)) / (1 - q)`.

**Méthodes indispensables :**

- reconnaître un modèle arithmétique ou géométrique ;
- calculer un terme à partir d'une formule explicite ;
- calculer des termes par récurrence ;
- déterminer le sens de variation d'une suite simple ;
- modéliser une évolution à accroissements constants ou à taux constant ;
- chercher un seuil par algorithme.

**Visualisation utile :**

- points `(n ; u(n))` ;
- comparaison dynamique entre croissance linéaire et croissance géométrique.

#### Second degré

**But :** résoudre et interpréter les équations et fonctions polynômes de degré
2.

**Définitions :**

- fonction polynôme du second degré ;
- racine ;
- forme développée ;
- forme factorisée ;
- forme canonique ;
- discriminant ;
- parabole.

**Propriétés et formules :**

- forme générale : `f(x) = ax² + bx + c`, avec `a != 0` ;
- discriminant : `Δ = b² - 4ac` ;
- si `Δ > 0`, deux racines :
  `x1 = (-b - sqrt(Δ)) / (2a)` et `x2 = (-b + sqrt(Δ)) / (2a)` ;
- si `Δ = 0`, une racine double : `x0 = -b / (2a)` ;
- si `Δ < 0`, aucune racine réelle ;
- sommet : abscisse `-b / (2a)` ;
- forme canonique : `a(x - α)² + β`.

**Méthodes indispensables :**

- choisir la forme adaptée à la question ;
- résoudre une équation du second degré ;
- étudier le signe d'un trinôme ;
- lire les racines et le sommet sur une parabole ;
- factoriser lorsque c'est possible ;
- résoudre un problème d'optimisation simple.

**Visualisation utile :**

- parabole modifiée par les coefficients `a`, `b`, `c` ;
- passage animé entre formes développée, factorisée et canonique.

### Thème 2 — Analyse et fonctions

#### Dérivation locale

**But :** comprendre la dérivée comme pente instantanée.

**Définitions :**

- taux de variation ;
- sécante ;
- nombre dérivé ;
- tangente ;
- approximation affine locale.

**Propriétés et formules :**

- taux de variation entre `a` et `a + h` :
  `(f(a + h) - f(a)) / h` ;
- équation de la tangente en `a` :
  `y = f(a) + f'(a)(x - a)` ;
- approximation locale :
  `f(a + h) ≈ f(a) + f'(a)h`.

**Méthodes indispensables :**

- calculer un taux de variation ;
- estimer graphiquement un nombre dérivé ;
- écrire l'équation d'une tangente ;
- interpréter une dérivée comme vitesse, coût marginal ou pente.

**Visualisation utile :**

- sécante qui devient tangente lorsque `h` se rapproche de `0`.

#### Calcul des dérivées et variations

**But :** utiliser la dérivée pour étudier une fonction.

**Définitions :**

- fonction dérivable ;
- fonction dérivée ;
- extremum local ;
- fonction paire ;
- fonction impaire.

**Propriétés et formules :**

- `(u + v)' = u' + v'` ;
- `(uv)' = u'v + uv'` ;
- `(1/u)' = -u' / u²` si `u` ne s'annule pas ;
- `(u/v)' = (u'v - uv') / v²` si `v` ne s'annule pas ;
- `(x^n)' = nx^(n-1)` pour les puissances usuelles ;
- si `f' > 0` sur un intervalle, alors `f` est croissante ;
- si `f' < 0` sur un intervalle, alors `f` est décroissante ;
- si `f' = 0` sur un intervalle, alors `f` est constante.

**Méthodes indispensables :**

- dériver une expression ;
- dresser un tableau de signes de `f'` ;
- en déduire un tableau de variations ;
- déterminer un maximum ou un minimum ;
- comparer deux courbes grâce à une différence de fonctions.

**Visualisation utile :**

- courbe de `f` et courbe de `f'` synchronisées ;
- zones croissantes et décroissantes selon le signe de `f'`.

#### Fonction exponentielle

**But :** modéliser une croissance proportionnelle à la quantité présente.

**Définitions :**

- fonction exponentielle ;
- nombre `e` ;
- croissance exponentielle.

**Propriétés et formules :**

- `exp(0) = 1` ;
- `exp'(x) = exp(x)` ;
- `exp(x + y) = exp(x) exp(y)` ;
- `exp(-x) = 1 / exp(x)` ;
- `e^x` est une autre notation de `exp(x)` ;
- `exp(x) > 0` pour tout réel `x`.

**Méthodes indispensables :**

- transformer une somme dans l'exposant en produit ;
- résoudre des équations exponentielles simples par lecture ou approximation ;
- utiliser l'exponentielle dans un modèle d'évolution ;
- comparer une suite géométrique et une fonction exponentielle.

**Visualisation utile :**

- courbe exponentielle avec pente égale à la hauteur ;
- méthode d'Euler pour construire une approximation.

#### Trigonométrie

**But :** passer du triangle rectangle au cercle trigonométrique.

**Définitions :**

- radian ;
- cercle trigonométrique ;
- enroulement de la droite réelle ;
- cosinus d'un réel ;
- sinus d'un réel ;
- angles associés.

**Propriétés et formules :**

- `cos²(x) + sin²(x) = 1` ;
- valeurs remarquables pour `0`, `π/6`, `π/4`, `π/3`, `π/2` ;
- `cos(-x) = cos(x)` ;
- `sin(-x) = -sin(x)`.

**Méthodes indispensables :**

- placer un angle sur le cercle trigonométrique ;
- lire un cosinus ou un sinus remarquable ;
- convertir une mesure intuitive en radians ;
- relier trigonométrie du triangle et trigonométrie du cercle.

**Visualisation utile :**

- cercle trigonométrique interactif ;
- projection du point sur les axes pour afficher cosinus et sinus.

### Thème 3 — Géométrie

#### Produit scalaire

**But :** mesurer l'alignement ou l'orthogonalité de deux directions.

**Définitions :**

- produit scalaire ;
- projection orthogonale ;
- norme ;
- orthogonalité.

**Propriétés et formules :**

- `u · v = ||u|| ||v|| cos(θ)` ;
- dans une base orthonormée :
  `u(x ; y) · v(x' ; y') = xx' + yy'` ;
- `u` et `v` sont orthogonaux si `u · v = 0` ;
- `||u||² = u · u` ;
- formule d'Al-Kashi :
  `a² = b² + c² - 2bc cos(A)`.

**Méthodes indispensables :**

- calculer un produit scalaire avec les coordonnées ;
- démontrer une orthogonalité ;
- calculer une longueur ou un angle ;
- utiliser Al-Kashi dans un triangle non rectangle.

**Visualisation utile :**

- projection d'un vecteur sur un autre ;
- angle qui fait varier le produit scalaire.

#### Géométrie repérée

**But :** résoudre un problème géométrique par le calcul.

**Définitions :**

- repère orthonormé ;
- coordonnées d'un point ;
- milieu ;
- distance ;
- équation de cercle ;
- équation de droite.

**Propriétés et formules :**

- milieu de `AB` :
  `((xA + xB) / 2 ; (yA + yB) / 2)` ;
- distance :
  `AB = sqrt((xB - xA)² + (yB - yA)²)` ;
- cercle de centre `A(a ; b)` et de rayon `r` :
  `(x - a)² + (y - b)² = r²`.

**Méthodes indispensables :**

- choisir un repère adapté ;
- calculer une distance ou un milieu ;
- vérifier qu'un point appartient à une droite ou un cercle ;
- traduire un lieu géométrique en équation.

**Visualisation utile :**

- repère avec points déplaçables ;
- cercle dont l'équation se met à jour automatiquement.

### Thème 4 — Probabilités et variables aléatoires

#### Probabilités conditionnelles et indépendance

**But :** raisonner lorsque l'information disponible change.

**Définitions :**

- probabilité conditionnelle ;
- événements indépendants ;
- partition de l'univers ;
- système complet d'événements ;
- arbre pondéré.

**Propriétés et formules :**

- si `P(A) != 0`, alors `P_A(B) = P(A ∩ B) / P(A)` ;
- `P(A ∩ B) = P(A) P_A(B)` ;
- événements indépendants :
  `P(A ∩ B) = P(A)P(B)` ;
- formule des probabilités totales :
  `P(B) = P(A1 ∩ B) + ... + P(An ∩ B)` pour une partition.

**Méthodes indispensables :**

- compléter un arbre pondéré ;
- choisir entre probabilité simple et conditionnelle ;
- reconnaître une situation d'indépendance ;
- calculer une probabilité totale.

**Visualisation utile :**

- arbre pondéré interactif ;
- tableau croisé reliant effectifs et probabilités.

#### Variables aléatoires

**But :** associer un nombre à chaque issue d'une expérience aléatoire.

**Définitions :**

- variable aléatoire réelle ;
- loi de probabilité ;
- espérance ;
- variance ;
- écart type.

**Propriétés et formules :**

- espérance : `E(X) = Σ xi P(X = xi)` ;
- variance : `V(X) = E((X - E(X))²)` ;
- autre formule : `V(X) = E(X²) - E(X)²` ;
- écart type : `σ(X) = sqrt(V(X))`.

**Méthodes indispensables :**

- construire la loi d'une variable aléatoire ;
- calculer espérance, variance et écart type ;
- interpréter l'espérance comme moyenne à long terme ;
- utiliser une variable aléatoire pour modéliser un gain, une perte ou un
  score.

**Visualisation utile :**

- distribution sous forme de barres ;
- simulation de répétitions et moyenne cumulée.

### Thème 5 — Algorithmique et listes

#### Listes, boucles et simulations

**But :** stocker plusieurs valeurs et automatiser des calculs répétitifs.

**Définitions :**

- liste ;
- indice ;
- parcours de liste ;
- accumulation ;
- simulation ;
- seuil.

**Méthodes indispensables :**

- créer une liste de valeurs ;
- parcourir une liste ;
- calculer une somme ou une moyenne ;
- générer les premiers termes d'une suite ;
- simuler une variable aléatoire ;
- chercher le premier rang qui dépasse un seuil.

**Visualisation utile :**

- tableau dynamique des valeurs d'une liste ;
- exécution pas à pas d'une boucle.

---

## Terminale

**But du niveau :** approfondir les outils de spécialité et préparer les études
supérieures : raisonnement, analyse, probabilités, combinatoire et géométrie de
l'espace.

**Parcours concerné :** spécialité mathématiques de Terminale générale.

**Note de transition :** le programme officiel 2026 de Terminale entre en
application à la rentrée scolaire 2027-2028. Pour une application évolutive, les
chapitres ci-dessous sont organisés pour rester compatibles avec le coeur du
parcours de spécialité tout en anticipant cette structuration.

### Thème 1 — Algèbre, combinatoire et géométrie

#### Combinatoire et dénombrement

**But :** compter sans énumérer un par un.

**Définitions :**

- principe additif ;
- principe multiplicatif ;
- liste ordonnée ;
- permutation ;
- combinaison ;
- coefficient binomial ;
- triangle de Pascal.

**Propriétés et formules :**

- nombre de listes de longueur `k` avec répétition parmi `n` choix : `n^k` ;
- nombre de permutations de `n` objets : `n!` ;
- nombre de combinaisons de `k` éléments parmi `n` :
  `C(n,k) = n! / (k!(n-k)!)` ;
- symétrie : `C(n,k) = C(n,n-k)` ;
- relation de Pascal : `C(n+1,k) = C(n,k-1) + C(n,k)` ;
- somme des coefficients binomiaux :
  `C(n,0) + C(n,1) + ... + C(n,n) = 2^n`.

**Méthodes indispensables :**

- choisir entre liste, permutation et combinaison ;
- représenter une situation par un arbre ;
- éviter les doubles comptages ;
- utiliser le triangle de Pascal ;
- relier dénombrement et probabilités.

**Visualisation utile :**

- arbre de choix ;
- générateur de combinaisons ;
- triangle de Pascal interactif.

#### Vecteurs, droites et plans de l'espace

**But :** étendre le calcul vectoriel au repérage dans l'espace.

**Définitions :**

- vecteur de l'espace ;
- vecteurs coplanaires ;
- combinaison linéaire ;
- base de l'espace ;
- droite de l'espace ;
- plan de l'espace ;
- représentation paramétrique.

**Propriétés et formules :**

- un point d'une droite de vecteur directeur `u` s'écrit
  `M = A + t u` ;
- un point d'un plan dirigé par deux vecteurs non colinéaires `u` et `v`
  s'écrit `M = A + s u + t v` ;
- trois vecteurs non coplanaires forment une base de l'espace.

**Méthodes indispensables :**

- déterminer une représentation paramétrique de droite ;
- déterminer une représentation paramétrique de plan ;
- tester l'appartenance d'un point à une droite ou un plan ;
- étudier la position relative de droites et de plans ;
- interpréter géométriquement un système d'équations.

**Visualisation utile :**

- scène 3D manipulable ;
- vecteurs directeurs affichés sur droites et plans.

#### Orthogonalité, distances et équations cartésiennes

**But :** calculer angles, distances et positions dans l'espace.

**Définitions :**

- produit scalaire dans l'espace ;
- vecteur normal ;
- plan normal à un vecteur ;
- projection orthogonale ;
- distance d'un point à un plan.

**Propriétés et formules :**

- dans une base orthonormée :
  `u(x ; y ; z) · v(x' ; y' ; z') = xx' + yy' + zz'` ;
- norme :
  `||u|| = sqrt(x² + y² + z²)` ;
- équation cartésienne d'un plan :
  `ax + by + cz + d = 0` ;
- un vecteur normal au plan est `n(a ; b ; c)`.

**Méthodes indispensables :**

- démontrer une orthogonalité dans l'espace ;
- trouver un vecteur normal ;
- passer d'une représentation paramétrique à une équation cartésienne dans des
  cas simples ;
- calculer l'intersection d'une droite et d'un plan ;
- choisir une représentation adaptée au problème.

**Visualisation utile :**

- plan transparent avec vecteur normal ;
- projection orthogonale d'un point sur un plan.

### Thème 2 — Analyse

#### Suites et limites

**But :** comprendre le comportement à long terme d'une suite.

**Définitions :**

- suite convergente ;
- suite divergente ;
- limite finie ;
- limite infinie ;
- suite majorée, minorée, bornée ;
- suite monotone ;
- raisonnement par récurrence.

**Propriétés et théorèmes :**

- une suite croissante et majorée converge ;
- une suite décroissante et minorée converge ;
- opérations usuelles sur les limites ;
- limites de référence des suites géométriques selon la valeur de `q` ;
- principe de récurrence : initialisation, hérédité, conclusion.

**Méthodes indispensables :**

- conjecturer une limite avec un tableau ou un graphique ;
- démontrer une propriété par récurrence ;
- étudier la monotonie d'une suite ;
- encadrer une suite ;
- utiliser un algorithme de seuil.

**Visualisation utile :**

- nuage de points `(n ; u(n))` ;
- bande d'encadrement autour d'une limite.

#### Limites de fonctions

**But :** décrire le comportement d'une fonction près d'un point ou à l'infini.

**Définitions :**

- limite finie ;
- limite infinie ;
- limite à droite ou à gauche ;
- asymptote horizontale ;
- asymptote verticale ;
- asymptote oblique.

**Propriétés et formules :**

- opérations usuelles sur les limites ;
- formes indéterminées à reconnaître ;
- limites de référence des fonctions polynômes, rationnelles, racine,
  exponentielle et logarithme ;
- une asymptote verticale correspond à une limite infinie au voisinage d'une
  abscisse.

**Méthodes indispensables :**

- factoriser pour lever une indétermination ;
- utiliser le terme dominant à l'infini ;
- repérer une asymptote ;
- interpréter graphiquement une limite.

**Visualisation utile :**

- zoom autour d'un point ;
- curseur faisant tendre `x` vers une valeur ou vers l'infini.

#### Compléments de dérivation et convexité

**But :** affiner l'étude des variations et de la forme d'une courbe.

**Définitions :**

- dérivée seconde ;
- convexité ;
- concavité ;
- point d'inflexion ;
- tangente.

**Propriétés et formules :**

- si `f'' >= 0`, alors `f` est convexe ;
- si `f'' <= 0`, alors `f` est concave ;
- une fonction convexe est située au-dessus de ses tangentes ;
- la dérivée de `f(g(x))` se calcule par la règle de chaîne :
  `(f ∘ g)' = g' × (f' ∘ g)`.

**Méthodes indispensables :**

- calculer une dérivée composée ;
- utiliser `f'` pour les variations ;
- utiliser `f''` pour la convexité ;
- identifier un point d'inflexion ;
- résoudre un problème d'optimisation.

**Visualisation utile :**

- tangentes mobiles ;
- courbure colorée selon le signe de `f''`.

#### Continuité et théorème des valeurs intermédiaires

**But :** justifier l'existence d'une solution.

**Définitions :**

- fonction continue en un point ;
- fonction continue sur un intervalle ;
- solution d'une équation `f(x) = k`.

**Théorèmes et propriétés :**

- toute fonction dérivable est continue ;
- théorème des valeurs intermédiaires : si une fonction continue passe d'une
  valeur `a` à une valeur `b`, elle prend toutes les valeurs intermédiaires ;
- si la fonction est continue et strictement monotone, la solution est unique.

**Méthodes indispensables :**

- prouver l'existence d'une solution ;
- prouver l'unicité avec la monotonie ;
- encadrer une solution ;
- appliquer une méthode de dichotomie.

**Visualisation utile :**

- courbe traversant une droite horizontale ;
- dichotomie animée sur un intervalle.

#### Fonction logarithme

**But :** comprendre la fonction réciproque de l'exponentielle.

**Définitions :**

- logarithme népérien `ln` ;
- fonction réciproque ;
- domaine `]0 ; +∞[` ;
- croissance comparée.

**Propriétés et formules :**

- `ln(1) = 0` ;
- `ln(e) = 1` ;
- `ln(ab) = ln(a) + ln(b)` pour `a > 0` et `b > 0` ;
- `ln(a/b) = ln(a) - ln(b)` ;
- `ln(a^n) = n ln(a)` ;
- `(ln x)' = 1/x` ;
- `ln(exp(x)) = x` et `exp(ln(x)) = x` pour `x > 0`.

**Méthodes indispensables :**

- transformer une équation avec logarithme ou exponentielle ;
- résoudre une inéquation logarithmique en respectant le domaine ;
- utiliser les propriétés algébriques de `ln` ;
- comparer logarithme, puissance et exponentielle.

**Visualisation utile :**

- symétrie des courbes de `exp` et `ln` par rapport à la droite `y = x`.

#### Fonctions sinus et cosinus

**But :** étudier les fonctions trigonométriques comme fonctions périodiques.

**Définitions :**

- fonction sinus ;
- fonction cosinus ;
- période ;
- parité ;
- amplitude.

**Propriétés et formules :**

- `sin` est impaire ;
- `cos` est paire ;
- `sin(x + 2π) = sin(x)` ;
- `cos(x + 2π) = cos(x)` ;
- `(sin x)' = cos x` ;
- `(cos x)' = -sin x`.

**Méthodes indispensables :**

- lire et tracer une sinusoïde ;
- résoudre des équations trigonométriques simples ;
- utiliser la périodicité ;
- étudier une fonction contenant sinus ou cosinus.

**Visualisation utile :**

- point qui tourne sur le cercle et courbes sinus/cosinus synchronisées.

#### Primitives et équations différentielles

**But :** inverser la dérivation et modéliser une évolution continue.

**Définitions :**

- primitive ;
- équation différentielle ;
- solution générale ;
- solution particulière ;
- condition initiale.

**Propriétés et formules :**

- deux primitives d'une même fonction sur un intervalle diffèrent d'une
  constante ;
- primitive de `x^n` :
  `x^(n+1) / (n+1)` si `n != -1` ;
- primitive de `1/x` sur `]0 ; +∞[` : `ln(x)` ;
- solutions de `y' = ay` : `y = Ce^(ax)` ;
- pour `y' = ay + b` avec `a != 0`, une solution constante est `-b/a`.

**Méthodes indispensables :**

- lire une primitive dans un tableau de dérivées ;
- déterminer une constante avec une condition initiale ;
- résoudre `y' = ay` ;
- résoudre `y' = ay + b` ;
- approcher une solution avec la méthode d'Euler.

**Visualisation utile :**

- champ de pentes ;
- construction d'Euler pas à pas.

#### Calcul intégral

**But :** relier aire, accumulation et primitive.

**Définitions :**

- intégrale ;
- aire sous la courbe ;
- valeur moyenne d'une fonction ;
- primitive utilisée pour calculer une intégrale.

**Propriétés et formules :**

- si `F` est une primitive de `f`, alors
  `∫[a,b] f(x) dx = F(b) - F(a)` ;
- linéarité :
  `∫(af + bg) = a∫f + b∫g` ;
- relation de Chasles :
  `∫[a,c] f = ∫[a,b] f + ∫[b,c] f` ;
- valeur moyenne :
  `(1 / (b - a)) ∫[a,b] f(x) dx` ;
- intégration par parties :
  `∫ u'v = uv - ∫ uv'`.

**Méthodes indispensables :**

- estimer une intégrale graphiquement ;
- calculer une aire avec une primitive ;
- calculer l'aire entre deux courbes ;
- utiliser l'intégration par parties ;
- interpréter une intégrale dans un contexte physique ou statistique.

**Visualisation utile :**

- rectangles, trapèzes et aire exacte ;
- accumulation progressive de l'aire.

### Thème 3 — Probabilités

#### Schéma de Bernoulli et loi binomiale

**But :** modéliser une répétition indépendante d'une même expérience à deux
issues.

**Définitions :**

- épreuve de Bernoulli ;
- succès ;
- échec ;
- schéma de Bernoulli ;
- loi binomiale ;
- variable aléatoire de comptage.

**Propriétés et formules :**

- si `X` suit la loi binomiale `B(n,p)`, alors
  `P(X = k) = C(n,k) p^k (1-p)^(n-k)` ;
- `E(X) = np` ;
- `V(X) = np(1-p)` ;
- `σ(X) = sqrt(np(1-p))`.

**Méthodes indispensables :**

- reconnaître un schéma de Bernoulli ;
- identifier `n`, `p` et `k` ;
- calculer une probabilité exacte ;
- calculer une probabilité cumulée ;
- interpréter espérance et écart type.

**Visualisation utile :**

- histogramme de la loi binomiale ;
- simulation de répétitions indépendantes.

#### Lois géométrique et de Poisson

**But :** modéliser une attente jusqu'au premier succès ou des événements rares.

**Définitions :**

- loi géométrique ;
- loi de Poisson ;
- événement rare ;
- paramètre d'une loi.

**Propriétés et formules :**

- loi géométrique : `P(X = k) = (1-p)^(k-1)p` ;
- espérance géométrique : `E(X) = 1/p` ;
- loi de Poisson de paramètre `λ` :
  `P(X = k) = e^(-λ) λ^k / k!` ;
- pour une loi de Poisson : `E(X) = V(X) = λ`.

**Méthodes indispensables :**

- reconnaître une attente du premier succès ;
- reconnaître un modèle d'événements rares ;
- choisir entre binomiale, géométrique et Poisson ;
- vérifier les hypothèses du modèle.

**Visualisation utile :**

- comparaison entre loi binomiale et approximation de Poisson.

#### Sommes de variables aléatoires

**But :** comprendre comment plusieurs grandeurs aléatoires se combinent.

**Définitions :**

- variables aléatoires indépendantes ;
- somme de variables aléatoires ;
- échantillon ;
- moyenne empirique.

**Propriétés et formules :**

- linéarité de l'espérance :
  `E(X + Y) = E(X) + E(Y)` ;
- `E(aX) = aE(X)` ;
- si `X` et `Y` sont indépendantes :
  `V(X + Y) = V(X) + V(Y)` ;
- `V(aX) = a²V(X)`.

**Méthodes indispensables :**

- représenter une variable comme somme de variables plus simples ;
- calculer une espérance sans déterminer toute la loi ;
- calculer une variance par indépendance ;
- appliquer ces propriétés à une loi binomiale.

**Visualisation utile :**

- empilement de variables de Bernoulli ;
- distribution de la somme obtenue par simulation.

#### Concentration et loi des grands nombres

**But :** comprendre pourquoi les moyennes se stabilisent.

**Définitions :**

- écart à l'espérance ;
- concentration ;
- inégalité de Bienaymé-Tchebychev ;
- loi des grands nombres.

**Propriétés et formules :**

- inégalité de Bienaymé-Tchebychev :
  `P(|X - E(X)| >= a) <= V(X) / a²` pour `a > 0` ;
- la moyenne d'un grand nombre de variables indépendantes de même loi se
  rapproche de l'espérance.

**Méthodes indispensables :**

- utiliser une variance pour majorer une probabilité d'écart ;
- interpréter un résultat de concentration ;
- simuler la stabilisation d'une fréquence ;
- relier probabilité théorique et observation.

**Visualisation utile :**

- moyenne cumulée qui se stabilise ;
- bande autour de l'espérance.

### Thème 4 — Algorithmique et raisonnement

#### Récurrence, dichotomie, Newton et simulations

**But :** utiliser l'algorithmique comme outil de raisonnement et de calcul.

**Définitions :**

- invariant ;
- boucle ;
- seuil ;
- dichotomie ;
- méthode de Newton ;
- méthode d'Euler ;
- simulation de Monte-Carlo.

**Méthodes indispensables :**

- écrire une preuve par récurrence ;
- programmer une recherche de seuil ;
- approcher une solution par dichotomie ;
- approcher une solution par Newton dans un cas favorable ;
- approcher une solution d'équation différentielle par Euler ;
- approximer une aire par rectangles, trapèzes ou Monte-Carlo.

**Visualisation utile :**

- tableau des itérations ;
- affichage de l'erreur à chaque étape.

---

## Extensions possibles du lycée

Ces extensions ne doivent être intégrées que si le projet décide de couvrir les
parcours correspondants.

### Mathématiques complémentaires — Terminale

**Public visé :** élèves qui n'ont pas conservé la spécialité mathématiques en
Terminale, mais qui ont besoin de mathématiques pour leur poursuite d'études.

**Organisation recommandée :**

- modèles définis par une fonction d'une variable ;
- modèles d'évolution ;
- probabilités et statistiques appliquées ;
- optimisation ;
- calcul intégral contextualisé ;
- suites et modèles discrets ;
- équations différentielles simples ;
- fonctions convexes ;
- logarithme et exponentielle dans des contextes.

**Principe pédagogique :** privilégier la modélisation, les problèmes issus
d'autres disciplines et les interprétations concrètes.

### Mathématiques expertes — Terminale

**Public visé :** élèves conservant la spécialité mathématiques et souhaitant un
approfondissement.

**Chapitres possibles :**

- nombres complexes ;
- arithmétique avancée ;
- matrices et graphes ;
- structures algébriques élémentaires ;
- applications aux transformations, suites, systèmes et problèmes de
  dénombrement.

**Principe pédagogique :** ce parcours doit rester séparé du tronc de spécialité
pour éviter de rendre les cours de Terminale indispensables plus lourds que
nécessaire.

---

## Techniques indispensables à renforcer tout au long du lycée

- Lire précisément un énoncé et identifier la nature de la question.
- Choisir une représentation : calcul, tableau, courbe, figure, arbre ou
  algorithme.
- Distinguer définition, propriété, théorème, formule et méthode.
- Vérifier les conditions d'application d'un théorème.
- Contrôler les domaines de définition avant de résoudre une équation.
- Vérifier une solution dans l'énoncé initial.
- Passer d'un résultat exact à une approximation lorsque c'est pertinent.
- Interpréter un résultat dans son contexte.
- Produire un contre-exemple lorsqu'une affirmation est fausse.
- Rédiger une démonstration avec hypothèse, raisonnement et conclusion.

## Priorités de développement conseillées

Pour rester fidèle à l'architecture progressive du projet, le lycée peut être
développé dans cet ordre :

1. **Seconde — Fonctions et repère** : images, antécédents, variations,
   droites, vecteurs.
2. **Seconde — Algèbre** : intervalles, calcul littéral, équations,
   inéquations.
3. **Seconde — Statistiques et probabilités** : données, fréquences,
   probabilités simples.
4. **Première — Suites** : suites arithmétiques, géométriques et modèles
   discrets.
5. **Première — Second degré** : discriminant, racines, signe, parabole.
6. **Première — Dérivation** : tangente, dérivée, variations, optimisation.
7. **Première — Exponentielle et trigonométrie**.
8. **Première — Produit scalaire et probabilités conditionnelles**.
9. **Terminale — Limites, continuité et logarithme**.
10. **Terminale — Intégrales, équations différentielles et probabilités
    avancées**.

Chaque chapitre devra ensuite être transformé en cours selon le modèle de
`cours.md` :

- but ;
- prérequis ;
- contenu découpé en petites étapes ;
- manipulation ou visualisation ;
- exemples essentiels ;
- question finale ;
- entraînement ;
- limites actuelles ;
- notes pédagogiques.
