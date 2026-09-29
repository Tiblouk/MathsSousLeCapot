# Maths Sous le capot

Application éducative locale développée avec .NET MAUI pour comprendre les
mécanismes mathématiques par la manipulation et la visualisation.

L'application fonctionne hors connexion et ne nécessite ni compte, ni serveur,
ni abonnement.

## Développement assisté par IA

Le projet est conçu et dirigé par Yamine Kebaili. Son développement a été
réalisé avec l'aide d'outils d'intelligence artificielle, notamment ChatGPT et
Codex d'OpenAI. Ces outils ont participé à la rédaction, à l'implémentation, à
la documentation et à la vérification du code sous la direction du responsable
du projet.

## Licence

Une personne physique peut utiliser et modifier gratuitement l'application dans
un cadre personnel. Elle peut publier gratuitement sa version modifiée, à
condition de citer le projet et de ne pas la monétiser. Toute utilisation
professionnelle ou réalisée pour le compte d'une école, d'une entreprise, d'une
association, d'une administration ou d'une autre organisation nécessite une
licence payante préalable.

Les conditions complètes figurent dans [`LICENSE.md`](LICENSE.md). Les demandes
professionnelles et organisationnelles sont expliquées dans
[`COMMERCIAL-LICENSING.md`](COMMERCIAL-LICENSING.md). Les contributions au dépôt
officiel suivent [`CONTRIBUTING.md`](CONTRIBUTING.md).

Les composants externes conservent leurs propres licences et sont recensés dans
[`THIRD-PARTY-NOTICES.txt`](THIRD-PARTY-NOTICES.txt). Cette notice doit
accompagner toute distribution de l'application.

## Documentation

Le point d'entrée de la documentation de conception et de maintenance est
[`docs/README.md`](docs/README.md). Il indique l'ordre de lecture, la source de
vérité de chaque sujet et renvoie notamment vers l'état réel du projet et son
architecture actuelle.

## État actuel

Le niveau **Fondations** contient actuellement :

- compter et nommer les nombres ;
- additionner et soustraire des quantités.

L'**école primaire** contient 39 cours classés précisément :

- 3 cours de CP ;
- 5 cours de CE1 ;
- 12 cours de CE2, dont le compteur positionnel ;
- 13 cours de CM1, dont les nombres décimaux ;
- 6 cours de CM2.

Ces cours restent répartis en sous-parties :

- nombres et numération, dont le compteur positionnel décimal ;
- opérations et calcul mental ;
- multiplication répétée ;
- mesures et conversions ;
- proportionnalité ;
- géométrie plane ;
- géométrie dans l'espace ;
- données et graphiques.

Le niveau **Collège** contient 94 cours disponibles et classés par classe :

- 37 cours de 6e ;
- 27 cours de 5e, dont les nombres négatifs ;
- 19 cours de 4e ;
- 11 cours de 3e.

Ils couvrent les nombres, le calcul, les puissances, les mesures, la
proportionnalité, la géométrie, l'algèbre, les fonctions, la trigonométrie,
les statistiques, les probabilités, la logique, l'arithmétique, le repérage,
le dénombrement et l'algorithmique.

Le niveau **Lycée** contient 47 cours disponibles et classés par classe :

- 13 cours de Seconde, dont la base deux et les premières notions de fonctions,
  vecteurs, intervalles, statistiques, probabilités et algorithmique ;
- 14 cours de Première, centrés sur la spécialité mathématiques : suites,
  second degré, dérivation, exponentielle, trigonométrie, produit scalaire et
  probabilités ;
- 20 cours de Terminale spécialité, couvrant limites, continuité, logarithme,
  intégrales, équations différentielles, combinatoire, géométrie de l'espace et
  probabilités avancées.

Les cours sont affichés par niveau, puis regroupés par sous-partie : les nombres,
les opérations, l'algèbre, l'analyse, les fonctions, la géométrie et les
probabilités restent donc distincts tout en appartenant au même niveau.
La liste utilise un accordéon replié par défaut. Les grands ensembles Primaire,
Collège et Lycée déploient leurs classes, chaque classe déploie ses catégories,
puis chaque catégorie affiche ses cours. Fondations ouvre directement ses
catégories puisqu'il ne contient pas de classes scolaires.

Le chargement de cette liste est progressif : le démarrage ne crée que les
boutons des grands ensembles. Les classes sont préparées à l'ouverture de leur
ensemble, les objets de cours à l'ouverture d'une classe et les cartes
graphiques à l'ouverture d'une catégorie.

Une recherche locale permet de retrouver un cours par son titre, sa notion, sa
classe, sa catégorie ou ses tags. Son index ne contient que les métadonnées : le
contenu pédagogique reste chargé uniquement après l'ouverture du cours.

Plusieurs profils peuvent partager le même appareil sans créer de compte.
Chaque profil possède sa progression, son historique d'entraînement, six défis
locaux et son propre score. L'écran **Mon parcours** affiche les statistiques et
charge l'historique progressivement. Le classement des scores reste strictement
local et aucune donnée n'est envoyée sur Internet.

Les cartes de cours indiquent aussi le meilleur état atteint par une couleur et
un badge textuel : bleu pour un cours lu, vert pour une session sans faute et
or pour une session sans faute en difficulté élevée. Le résumé des badges est
séparé de l'historique détaillé afin que le menu n'ait pas à charger les
exercices déjà réalisés.

Une session d'entraînement vérifiée rapporte des points une seule fois par cours
et par difficulté. La base vaut `1` point en Fondations/Primaire, `3` au Collège
et `5` au Lycée ; la difficulté ajoute respectivement `0`, `1` ou `3` points en
facile, modéré ou difficile. Refaire le même niveau de difficulté ne permet donc
pas d'accumuler des points.

Chaque classe disponible propose aussi une section **Contrôles intermédiaires**.
La première version fournit un contrôle général généré au démarrage de la
tentative, à partir des cours disponibles dans la classe ouverte. Les questions
mélangent plusieurs notions quand le niveau le permet, puis affichent un score,
une correction et un bilan des notions maîtrisées ou à revoir.

Une archive Windows portable peut être générée avec
`.\scripts\Package-WindowsPortable.ps1`. Elle contient un exécutable
`MathsSousLeCapot.exe` lançable après décompression, ainsi qu'un dossier `Data`
où sont stockés les paramètres et le registre des profils. La progression et
l'historique, ainsi que le résumé léger des badges, sont isolés sous
`Data/Profiles/<id>/`.

Les cours de mesure concernés affichent un tableau commun des longueurs, masses,
litres et aires. Il distingue les conversions par `10` des conversions d'aires
par `100` et explique l'effet du carré sur les deux dimensions.

Le compteur permet des déplacements de `-100`, `-10`, `-1`, `+1`, `+10` et
`+100`. Les retenues et emprunts sont visualisés et expliqués.

Chaque cours possède :

- une fiche d'entrée ;
- des étapes courtes ;
- des interactions pédagogiques ;
- une question finale ;
- un entraînement de cinq exercices ;
- une sauvegarde locale de la progression et des sessions.

Les cours primaires génériques distinguent désormais trois étapes :

- une découverte avec une définition propre à la notion et un exemple ;
- un exercice guidé plus exigeant ;
- une validation composée de deux exercices aléatoires distincts.

Les cours du collège suivent le même contrat pédagogique, avec un catalogue
propre et des familles d'exercices adaptées à chaque notion. Les validations
génèrent toujours deux questions distinctes et les entraînements en génèrent
cinq selon la difficulté choisie.

Les cours du lycée suivent le même contrat pédagogique, avec un catalogue séparé
et des familles d'exercices adaptées aux notions avancées. Le cadrage détaillé
du lycée est documenté dans `docs/cours_avancer.md`.

Les découvertes du primaire, du collège et du lycée sont structurées en trois
blocs visibles : définition précise, méthode adaptée au niveau et points à
retenir. Elles expliquent le sens des notations, les étapes du raisonnement, les
conditions d'application et les confusions courantes, au lieu de présenter
seulement une formule. Un composant d'affichage partagé rend ces blocs de façon
cohérente dans les trois parcours sans dupliquer l'interface.

Pour les niveaux Fondations, école primaire, 6e et 5e, les additions, soustractions,
multiplications et divisions pertinentes sont présentées sous forme de calcul
posé. La question masque le résultat ; la correction affiche les retenues,
emprunts, produits intermédiaires ou étapes de division.

Un bouton **Aide au calcul posé** est disponible dans les cours et les
entraînements du primaire. Il ouvre une pose interactive dans laquelle l'élève
écrit directement chaque chiffre dans la case active : résultat d'une colonne,
retenue, emprunt, produit partiel, quotient, valeur soustraite ou reste. Une
valeur juste reste verte dans la disposition et la case suivante devient active ;
une valeur fausse est signalée en rouge sans révéler la réponse. Lorsqu'un
exercice de calcul est sélectionné, ses nombres sont repris automatiquement.

La valeur d'un chiffre peut être donnée par sa valeur numérique ou par le nom
de sa position. Par exemple, pour le chiffre `9` dans `921`, `900`, `neuf
cents` et `centaine` sont acceptés.

Les entraînements proposent les niveaux facile, modéré et difficile. Leur
récapitulatif affiche chaque question, la réponse donnée, la réponse correcte et
l'explication associée, puis permet de quitter ou de continuer avec cinq
nouveaux exercices.

Les réponses entières peuvent être saisies en chiffres ou en toutes lettres
dans la langue active. Par exemple, en français, `21`, `vingt et un` et
`vingt-et-un` sont équivalents. Pour les nombres décimaux, la virgule et le
point sont acceptés. Les réponses binaires restent écrites avec `0` et `1`,
car `100₂` représente quatre et non cent.

## Architecture

```text
MathsSousLeCapot/
├── src/
│   ├── MathsSousLeCapot.App/        Interface .NET MAUI
│   │   ├── Features/                Modules pédagogiques
│   │   │   ├── Challenges/          Défis et classement local
│   │   │   ├── FoundationNumbers/   Nombres négatifs et décimaux
│   │   │   ├── HighSchool/          Pages génériques des cours du lycée
│   │   │   ├── MiddleSchool/        Pages génériques des cours du collège
│   │   │   ├── Profiles/            Gestion des profils locaux
│   │   │   ├── Progress/            Parcours et historique
│   │   ├── Controls/                Contrôles visuels réutilisables
│   │   ├── Localization/            Chargement des traductions JSON
│   │   ├── Resources/Raw/lang/      Catalogues de langue et modules traduits
│   │   ├── Services/                Navigation et stockage local
│   │   └── Platforms/               Entrées Windows et Android
│   └── MathsSousLeCapot.Core/       Logique mathématique indépendante
└── tests/
    └── MathsSousLeCapot.Core.Tests/ Tests unitaires du moteur
```

La logique mathématique ne dépend pas de MAUI. Les chapitres et cours sont
déclarés dans `CourseCatalog`, ce qui permet d'ajouter un module sans modifier
les cours existants. La liste du chapitre est construite depuis ce catalogue et
les routes des modules sont regroupées dans `NavigationService`.

Les contrats du cœur acceptent progressivement un contenu pédagogique enrichi.
Un `Course` peut porter des objectifs, du vocabulaire, des points à retenir et
des erreurs fréquentes. Chaque `CourseStep` peut ordonner des blocs de définition,
propriété, théorème, méthode, exemple résolu ou visualisation. Un `Exercise` peut
également fournir une solution détaillée composée d'étapes et d'erreurs classiques.
Ces données restent facultatives afin que les cours, générateurs et historiques
créés avec le format précédent continuent de fonctionner pendant leur migration.

## Prérequis

- .NET SDK 9 ;
- workload .NET MAUI ;
- Visual Studio 2022 avec les composants Windows et Android pour tester les
  deux plateformes ;
- SDK Android et JDK pour la cible Android.

Installation des workloads déclarés par le projet :

```powershell
dotnet workload restore
```

## Compiler

Depuis le dossier contenant `MathsSousLeCapot.sln` :

```powershell
dotnet build .\src\MathsSousLeCapot.App\MathsSousLeCapot.App.csproj -f net9.0-windows10.0.19041.0
dotnet build .\src\MathsSousLeCapot.App\MathsSousLeCapot.App.csproj -f net9.0-android
```

Le projet désactive le Fast Deployment pour Android et embarque les assemblies
dans l'APK. Le fichier signé placé dans `bin/Debug/net9.0-android` peut donc être
transféré et installé directement sur un téléphone sans dépendre de Visual
Studio.

## Lancer sous Windows

Le target MAUI `-t:Run` est volontairement évité : avec WinUI non empaqueté, il
peut chercher un nom d'exécutable obsolète dans `bin` et échouer avec le code
`9009`.

Commande fiable :

```powershell
dotnet build .\src\MathsSousLeCapot.App\MathsSousLeCapot.App.csproj -f net9.0-windows10.0.19041.0
Start-Process .\src\MathsSousLeCapot.App\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\MathsSousLeCapot.exe
```

On peut aussi ouvrir ce fichier directement dans l'Explorateur Windows après la
compilation.

Si le build échoue en indiquant que `MathsSousLeCapot.exe` est utilisé par un
autre processus, il faut fermer l'application déjà ouverte puis relancer la
commande `dotnet build`.

## Générer la version portable Windows

Pour produire une archive ZIP lançable sans terminal par l'utilisateur final :

```powershell
.\scripts\Package-WindowsPortable.ps1
```

L'archive est créée dans :

```text
artifacts/windows-portable/MathsSousLeCapot-Windows-Portable-0.1.1.zip
```

Après extraction, l'utilisateur lance simplement `MathsSousLeCapot.exe`.

## Tests

```powershell
dotnet test .\tests\MathsSousLeCapot.Core.Tests\MathsSousLeCapot.Core.Tests.csproj
```

Les tests couvrent notamment :

- les retenues et emprunts ;
- les déplacements par unités, dizaines et centaines ;
- le changement de base sans perte de valeur ;
- la lecture française des nombres ;
- l'écriture et la reconnaissance des nombres dans les cinq langues ;
- la génération des entraînements ;
- les 37 générateurs génériques du niveau primaire ;
- les 93 cours et générateurs du niveau collège ;
- les additions et soustractions sur les nombres naturels ;
- l'ordre pédagogique des cours ;
- la robustesse de la sérialisation locale ;
- l'index de recherche et ses tags ;
- les règles de gestion des profils locaux ;
- le calcul des statistiques, défis et scores ;
- la priorité des badges de cours et l'absence de points répétés ;
- la validité des catalogues et de leurs paramètres de formatage.

## Traductions JSON

Les traductions sont embarquées dans :

```text
src/MathsSousLeCapot.App/Resources/Raw/lang/
├── index.json
├── en_US.json
├── es_ES.json
├── fr_FR.json
├── it_IT.json
├── ja_JP.json
├── primary/
├── primary-assistant/
├── highschool/
└── progress/
```

`index.json` contient la langue par défaut, la liste des langues disponibles et
leurs catalogues complémentaires. Les dossiers thématiques isolent les textes
du primaire, de l'assistant, du lycée et du suivi local afin qu'un nouveau
module puisse ajouter ses traductions sans transformer le fichier principal en
catalogue monolithique.
Au démarrage, l'application charge d'abord le catalogue français complet, puis
remplace les clés disponibles par celles de la langue choisie. Une nouvelle
notion reste donc lisible en français tant que sa traduction n'a pas encore été
produite. L'application recherche la culture du système, par exemple
`fr-FR`, la convertit en `fr_FR`, puis charge le fichier correspondant. Si cette
langue exacte n'existe pas, elle recherche une variante partageant le même code
court, par exemple `en-GB` vers `en_US`, puis utilise le français par défaut.

Les langues fournies sont le français, l'anglais, l'espagnol, l'italien et le
japonais. Les intitulés des difficultés, les cours, les entraînements et les
nombres écrits en toutes lettres suivent la langue sélectionnée.

Un test automatisé vérifie que chaque langue déclarée utilise uniquement des
clés françaises connues et conserve les mêmes paramètres de formatage pour les
textes qu'elle traduit.

### Processus de traduction

Le français reste la langue source du projet. Lorsqu'un texte ou une clé est
ajouté, les cinq catalogues peuvent désormais être mis à jour ensemble. Le test
de parité vérifie ensuite que les clés et paramètres sont identiques en
français, anglais, espagnol, italien et japonais.

Tous les fichiers JSON utilisent le même format : UTF-8, indentation de deux
espaces et une propriété par ligne.

Une fonctionnalité transversale peut utiliser un supplément dédié, comme le
dossier `progress`, plutôt que d'allonger les catalogues principaux.

### Ajouter une langue

1. Dupliquer `fr_FR.json`, par exemple en `en_US.json`.
2. Traduire toutes les valeurs sans modifier les clés.
3. Ajouter la langue dans `index.json` :

```json
{
  "code": "en_US",
  "name": "English (United States)",
  "file": "en_US.json",
  "supplements": [ "primary/en_US.json" ]
}
```

4. Recompiler l'application.

Dans le XAML, une traduction est appelée ainsi :

```xml
Text="{loc:Translate course.counting.title}"
```

Dans le code C# :

```csharp
TranslationService.Current.Get("course.counting.title");
TranslationService.Current.Format("feedback.score", correctCount);
```

Une clé absente est affichée entre crochets, par exemple
`[course.counting.title]`, afin de rendre l'erreur visible.

## Paramètres

Le bouton **Paramètres** de la liste des cours permet de choisir :

- la langue parmi celles déclarées dans `lang/index.json` ;
- le thème clair ;
- le thème sombre ;
- le thème sépia.

Les thèmes sont déclarés dans `ThemeCatalog`. Chaque palette fournit toutes les
couleurs sémantiques de l'application : couleurs principales, fonds, textes,
bordures, succès et erreurs. Ajouter un thème ne nécessite donc pas de modifier
séparément l'écran des paramètres et le moteur d'application.

Les choix sont enregistrés dans les préférences locales. Lorsqu'ils sont
appliqués, le Shell est reconstruit afin que les pages XAML utilisent
immédiatement la nouvelle langue et la nouvelle palette.

Le compteur fournit également des descriptions sémantiques pour ses commandes
principales et empêche une nouvelle manipulation pendant une animation.

## Conventions de code

- Les classes, méthodes et champs qui portent un comportement ou un état sont
  documentés avec des commentaires XML.
- Les variables locales ne reçoivent un commentaire que lorsque leur rôle
  n'est pas évident, afin d'éviter des commentaires qui répètent simplement le
  code.
- Les textes visibles ne doivent pas être ajoutés directement dans le XAML ou
  le C# : ils doivent recevoir une clé dans les fichiers de langue.
- Les calculs restent dans `MathsSousLeCapot.Core`; l'interface se limite à la
  présentation et à l'interaction.
