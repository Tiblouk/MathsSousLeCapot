# Architecture actuelle

Ce document décrit l'architecture réellement implémentée de **Maths Sous le
capot** au 10 août 2026. Il sert de carte de lecture du code et doit évoluer
lorsqu'une responsabilité, une dépendance ou un flux structurant change.

## Vue générale

```mermaid
flowchart LR
    U["Utilisateur Windows ou Android"] --> UI["MathsSousLeCapot.App"]
    UI --> F["Pages Features et contrôles MAUI"]
    F --> C["MathsSousLeCapot.Core"]
    F --> L["Traductions JSON embarquées"]
    F --> T["Palettes de thème"]
    F --> S["Services de navigation et stockage"]
    S --> D["Fichiers JSON locaux"]
    TEST["MathsSousLeCapot.Core.Tests"] --> C
```

La dépendance principale est à sens unique : l'application MAUI référence le
cœur, tandis que le cœur ne référence ni MAUI, ni Windows, ni Android.

## Projets de la solution

### `MathsSousLeCapot.App`

Responsable de :

- l'interface XAML et C# ;
- la navigation Shell ;
- la création différée des vues ;
- les interactions, animations et visualisations ;
- la localisation des textes ;
- l'application des thèmes ;
- le choix de l'emplacement des données locales ;
- les points d'entrée Windows et Android.

### `MathsSousLeCapot.Core`

Responsable de :

- la description des cours et de leur progression ;
- les modèles pédagogiques ;
- les générateurs d'exercices et de contrôles ;
- la validation des réponses ;
- les calculs, conversions et représentations mathématiques ;
- les modèles et la sérialisation des données locales.

Le cœur doit rester testable sans initialiser MAUI.

### `MathsSousLeCapot.Core.Tests`

Projet xUnit couvrant les catalogues, modèles pédagogiques, générateurs,
validateurs, calculs et formats de stockage. Il ne constitue pas une suite de
tests d'interface MAUI.

## Dossiers principaux

| Dossier | Responsabilité |
|---|---|
| `src/MathsSousLeCapot.App/Features` | Pages regroupées par parcours fonctionnel |
| `src/MathsSousLeCapot.App/Controls` | Composants visuels réutilisables |
| `src/MathsSousLeCapot.App/Localization` | Chargement et utilisation des traductions |
| `src/MathsSousLeCapot.App/Services` | Navigation, thèmes, préférences et stockage |
| `src/MathsSousLeCapot.App/Resources` | Styles, icônes, contenus et JSON embarqués |
| `src/MathsSousLeCapot.App/Platforms` | Configuration propre à Windows et Android |
| `src/MathsSousLeCapot.Core/Courses` | Catalogues et modèles de cours |
| `src/MathsSousLeCapot.Core/Training` | Exercices, sessions, contrôles et validation |
| `src/MathsSousLeCapot.Core/Mathematics` | Logique mathématique indépendante |
| `src/MathsSousLeCapot.Core/Progress` | Modèles et sérialisation de la progression |
| `tests/MathsSousLeCapot.Core.Tests` | Tests unitaires et de cohérence du cœur |

## Démarrage de l'application

```mermaid
sequenceDiagram
    participant App
    participant Preferences as UserPreferencesService
    participant Translation as TranslationService
    participant Theme as ThemeService
    participant Shell as AppShell
    participant Menu as ChapterPage

    App->>Preferences: Lire settings.json ou migrer Preferences
    App->>Translation: Appliquer la langue enregistrée
    App->>Theme: Appliquer la palette enregistrée
    App->>Shell: Créer la fenêtre principale
    Shell->>Shell: Enregistrer les routes
    Shell->>Menu: Afficher la page racine
    Menu->>Menu: Créer uniquement les grands ensembles repliés
```

Les services ne passent pas actuellement par un conteneur d'injection de
dépendances. Les services simples sont construits directement et le service de
traduction expose une instance partagée.

## Chargement progressif du menu

`ChapterPage` utilise un état de déploiement possédant une fabrique exécutée une
seule fois par `EnsureLoaded()`.

1. Au démarrage, seuls les boutons des grands ensembles sont créés.
2. À l'ouverture d'un ensemble, les niveaux qui contiennent des cours sont
   ajoutés.
3. À l'ouverture d'un niveau, les cours de ce niveau sont obtenus depuis
   `CourseCatalog` et regroupés par catégorie.
4. À l'ouverture d'une catégorie, les cartes des cours sont créées.
5. À la sélection d'un cours, seule sa fiche et ensuite sa page sont construites.
6. Les exercices sont générés lors de la validation ou du démarrage d'une
   session, pas au lancement de l'application.

Ce comportement est une contrainte d'architecture : un nouveau module ne doit
pas reconstruire tous les cours ou toutes les visualisations au démarrage.

## Catalogues de cours

```mermaid
flowchart TD
    SG["SchoolGroupCatalog"] --> CC["CourseCatalog, façade commune"]
    PC["PrimaryCourseCatalog"] --> CC
    MC["MiddleSchoolCourseCatalog"] --> CC
    HC["HighSchoolCourseCatalog"] --> CC
    FC["FoundationNumberCourseCatalog"] --> CC
    CC --> COURSE["Course"]
    COURSE --> STEP["CourseStep"]
    STEP --> BLOCK["CourseContentBlock facultatif"]
```

- `SchoolGroupCatalog` ordonne Fondations, Primaire, Collège et Lycée.
- `CourseCatalog` fournit les chapitres, résout un identifiant et rassemble les
  cours d'un niveau.
- les catalogues primaire, collège et lycée portent les grandes familles
  génériques ;
- `FoundationNumberCourseCatalog` est un nom technique historique pour les
  modules spécialisés des nombres négatifs et décimaux. Ces cours restent
  classés respectivement en 5e et CM1.
- le comptage, les opérations de fondation, le compteur décimal et la base deux
  disposent également de constructions spécialisées dans `CourseCatalog`.

Les identifiants de cours sont des contrats persistants : ils sont utilisés par
la navigation, les exercices, les prérequis, la progression et l'historique. Un
identifiant existant ne doit pas être renommé sans migration.

## Modèle pédagogique

Un `Course` contient :

- ses métadonnées et prérequis ;
- une liste ordonnée de `CourseStep` ;
- un indicateur de disponibilité ;
- un `CoursePedagogicalContent` facultatif.

Un `CourseStep` porte une idée principale et un type d'interaction. Ses
`CourseContentBlock` facultatifs permettent d'ordonner définition, intuition,
propriété, théorème, formule, méthode, exemple résolu, erreur fréquente,
visualisation et résumé.

Les champs enrichis sont facultatifs pour conserver la compatibilité avec les
cours créés selon l'ancien modèle. Les pages doivent donc accepter à la fois le
contenu structuré et l'ancien contenu court pendant la migration pédagogique.

## Familles de pages

Les premiers modules possèdent des pages dédiées :

- comptage ;
- compteur positionnel ;
- base deux ;
- addition et soustraction de fondation.

Les catalogues plus étendus réutilisent trois familles de pages génériques :

- `PrimaryCourses` ;
- `MiddleSchool` ;
- `HighSchool`.

Chaque famille fournit une fiche d'entrée, un cours et un entraînement. Les
nombres négatifs et décimaux utilisent la famille spécialisée
`FoundationNumbers`. `NavigationService` choisit la famille à partir de
l'identifiant du cours et centralise les routes Shell.

## Exercices et entraînements

```mermaid
flowchart LR
    COURSE["Cours sélectionné"] --> GEN["Générateur de sa famille"]
    GEN --> EX["Exercise"]
    EX --> SESSION["TrainingSession ou validation"]
    SESSION --> VALID["ExerciseAnswerValidator"]
    VALID --> SUMMARY["Récapitulatif"]
    SUMMARY --> DETAIL["DetailedCorrectionPage"]
    SESSION --> STORE["LocalStorageService"]
```

Un `Exercise` conserve la question, la réponse correcte, le type de réponse,
les clés de traduction et, si nécessaire, un calcul posé ou une
`ExerciseSolution` structurée. Les anciens exercices restent valides grâce aux
champs enrichis facultatifs.

`ExerciseAnswerValidator` centralise la reconnaissance des réponses afin
d'éviter que chaque page réimplémente les règles sur les espaces, accents,
signes, nombres écrits ou valeurs de position.

La correction détaillée utilise en priorité `ExerciseSolution`. Pour les
exercices encore anciens, `DetailedExerciseExplanation` construit une
explication à partir de la clé et des arguments. Une explication minimale de
repli subsiste : sa suppression dépend de la migration pédagogique de toutes les
familles d'exercices.

## Contrôles intermédiaires

`ClassAssessmentGenerator` reçoit une clé de niveau, demande uniquement les
cours disponibles pour ce niveau, puis délègue chaque question au générateur de
la famille correspondante.

Il produit dix questions par défaut, alterne les cours disponibles et rejette
les signatures identiques dans la limite d'un nombre maximal de tentatives. Une
classe sans cours renvoie un contrôle vide au lieu de provoquer une erreur.

L'interface `ClassAssessmentPage` calcule le score et regroupe les résultats par
notion afin d'indiquer ce qui est maîtrisé ou à revoir.

## Calcul posé

Deux représentations complémentaires existent :

- `WrittenCalculationBuilder` et `WrittenCalculationView` produisent une pose
  complète destinée aux questions et corrections ;
- `GuidedCalculationPlanBuilder` et `GuidedWrittenCalculationView` découpent la
  pose en cases successives pour l'assistant primaire.

L'assistant ne renseigne pas le résultat à l'avance. Il active une case, valide
sa valeur, conserve les réponses correctes en vert puis passe à l'étape suivante.
Les étapes possibles couvrent chiffres de résultat, retenues, emprunts, produits
partiels, quotient, valeurs soustraites et restes.

La logique de construction du plan reste dans `Core`; le contrôle MAUI se charge
uniquement de la disposition, de la saisie et des couleurs.

## Traductions

Les traductions sont des ressources JSON embarquées sous
`Resources/Raw/lang`.

```text
lang/
├── index.json
├── fr_FR.json, en_US.json, es_ES.json, it_IT.json, ja_JP.json
├── primary/
├── primary-assistant/
└── highschool/
```

`index.json` déclare la langue française par défaut et les suppléments de chaque
langue. `TranslationService` charge d'abord le français complet, puis remplace
les clés disponibles par celles de la langue active. Une traduction manquante
peut ainsi retomber sur le français.

Les clés sont utilisées dans le XAML par `TranslateExtension` et dans le C# par
`Get` ou `Format`. Les paramètres comme `{0}` font partie du contrat de la clé.

## Thèmes

`ThemeCatalog` déclare les thèmes clair, sombre et sépia. Chaque
`ThemeDefinition` fournit une palette sémantique complète : couleurs
principales, fonds, textes, bordures, succès et erreurs.

`ThemeService` reporte la palette dans les ressources MAUI. Les pages et
contrôles doivent demander une couleur sémantique au lieu d'introduire une
couleur locale sans justification.

## Stockage local

```mermaid
flowchart TD
    PREF["UserPreferencesService"] --> SETTINGS["settings.json"]
    STORAGE["LocalStorageService"] --> PROGRESS["course_progress.json"]
    STORAGE --> HISTORY["training_history.json"]
    PATH["AppDataPathService"] --> PREF
    PATH --> STORAGE
```

`AppDataPathService` choisit le dossier :

- `Data` à côté de l'exécutable pour la version Windows portable lorsque ce
  dossier existe ;
- `%LOCALAPPDATA%\MathsSousLeCapot\Data` pour Windows classique ;
- le dossier privé MAUI de l'application sur Android.

Les services migrent les anciennes valeurs MAUI Preferences lorsqu'aucun
fichier JSON n'existe. Les modèles persistés doivent rester compatibles ou être
accompagnés d'une migration explicite.

## Invariants à préserver

1. `MathsSousLeCapot.Core` ne dépend pas de MAUI.
2. Les identifiants de cours et les clés de traduction restent stables.
3. Un contrôle de classe ne pioche que dans son niveau.
4. Les exercices ne sont générés qu'au moment où ils sont nécessaires.
5. Les cartes de cours et visualisations ne sont pas toutes créées au démarrage.
6. Les réponses sont validées par les règles partagées, pas par des comparaisons
   dispersées dans les pages.
7. Les nouveaux champs des modèles restent facultatifs pendant les migrations.
8. Les données locales existantes ne sont jamais supprimées implicitement.
9. Les couleurs visibles proviennent des palettes sémantiques.
10. Une fonctionnalité annoncée comme vérifiée doit citer la validation
    réellement exécutée.

## Points d'extension principaux

| Besoin | Point d'entrée |
|---|---|
| Ajouter ou classer un cours | Catalogues sous `Core/Courses` |
| Ajouter un type d'exercice | Enumération et générateur sous `Core/Training` |
| Modifier la reconnaissance d'une réponse | `ExerciseAnswerValidator` et convertisseurs de nombres |
| Ajouter une page spécialisée | `App/Features` et `NavigationService` |
| Ajouter un composant visuel partagé | `App/Controls` |
| Ajouter une langue ou un supplément | `Resources/Raw/lang/index.json` et catalogues JSON |
| Ajouter un thème | `ThemeCatalog` et `ThemeDefinition` |
| Faire évoluer les données locales | Modèles `Core/Progress`, sérialiseur et services de stockage |
| Ajouter une règle mathématique | Sous-dossier adapté de `Core/Mathematics` avec tests |

La procédure détaillée d'ajout d'un cours sera documentée dans une phase
suivante. En attendant, toute extension doit être accompagnée des tests de
catalogue, de génération, de traduction et de navigation concernés.
