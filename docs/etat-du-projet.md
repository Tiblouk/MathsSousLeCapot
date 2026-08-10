# État du projet

Ce document décrit l'état réellement observé de **Maths Sous le capot**. Il ne
remplace ni la vision de `projet.md`, ni le catalogue pédagogique de `cours.md`.

**Date de l'audit :** 10 août 2026

**Référence initiale :** commit Git `4d0b935`

**Version applicative déclarée :** `1.0` (`ApplicationVersion` 1)

## Lecture des états

- **Implémenté et vérifié** : présent et validé par des tests, une compilation
  ou un parcours manuel adapté.
- **Implémenté partiellement** : utilisable, mais incomplet ou de qualité
  inégale.
- **À revoir** : présent, avec une correction ou une révision déjà identifiée.
- **Planifié** : documenté sans implémentation complète.
- **Non vérifié** : présent dans le code, sans validation suffisante sur la
  plateforme ou le scénario annoncé.

## Socle technique

| Élément | État constaté |
|---|---|
| Framework | .NET MAUI avec .NET 9 |
| Interface | XAML et C# |
| Cœur métier | Projet C# `MathsSousLeCapot.Core` indépendant de MAUI |
| Cibles | `net9.0-android` et `net9.0-windows10.0.19041.0` |
| Windows minimal déclaré | Windows 10, version `10.0.17763.0` |
| Android minimal déclaré | API 21 |
| Dépendance applicative directe | `Microsoft.Maui.Controls` 9.0.120 |
| Réseau et compte | Aucun serveur, compte ou accès réseau nécessaire |
| Stockage | Fichiers JSON locaux |

## Catalogue disponible

Les nombres ci-dessous proviennent des catalogues C# et des tests de cohérence.
Ils décrivent des cours techniquement accessibles, pas une validation complète
de leur qualité pédagogique.

| Ensemble | Niveau | Nombre de cours |
|---|---|---:|
| Fondations | Fondations | 3 |
| Primaire | CP | 3 |
| Primaire | CE1 | 5 |
| Primaire | CE2 | 12 |
| Primaire | CM1 | 13 |
| Primaire | CM2 | 6 |
| Collège | 6e | 37 |
| Collège | 5e | 27 |
| Collège | 4e | 19 |
| Collège | 3e | 11 |
| Lycée | Seconde | 13 |
| Lycée | Première spécialité | 14 |
| Lycée | Terminale spécialité | 20 |
| **Total** |  | **183** |

Le catalogue technique comprend 37 définitions génériques du primaire,
93 du collège et 46 du lycée. Le compteur décimal, les nombres décimaux, les
nombres négatifs et la base deux sont ajoutés par leurs modules spécialisés,
ce qui explique les totaux visibles dans l'application.

## Fonctionnalités

| Fonctionnalité | État | Preuves et limites |
|---|---|---|
| Menu par ensemble, classe et catégorie | Implémenté et vérifié | Accordéons repliés par défaut et navigation Windows parcourue manuellement. |
| Chargement progressif du menu | Implémenté et vérifié | Les ensembles sont créés au démarrage ; niveaux, catégories et cartes sont matérialisés au premier déploiement. |
| Fiche d'entrée d'un cours | Implémenté et vérifié | Titre, niveau, objectif, résumé, prérequis et accès au cours ou à l'entraînement. |
| Cours par étapes | Implémenté et vérifié techniquement | Pages dédiées pour les premiers modules et pages génériques pour primaire, collège et lycée. |
| Profondeur pédagogique | Implémenté partiellement | Le modèle enrichi existe, mais de nombreux cours avancés utilisent encore des explications trop courtes ou génériques. |
| Validation de notion | Implémenté et vérifié | Deux exercices variables pour les familles génériques ; parcours spécialisés conservés pour les premiers modules. |
| Entraînement | Implémenté et vérifié | Difficultés facile, modérée et difficile ; cinq exercices par session. |
| Contrôle intermédiaire par classe | Implémenté et vérifié | Dix questions par défaut, limitées aux cours de la classe, avec variété et bilan par notion. |
| Validation des réponses | Implémenté et vérifié | Nombres localisés, variantes textuelles raisonnables, positions décimales et valeurs de chiffres. |
| Calcul posé dans les questions | Implémenté et vérifié | Addition, soustraction, multiplication et division avec retenues, emprunts, produits et restes. |
| Assistant primaire de calcul posé | Implémenté et vérifié sur Windows | Saisie directe dans les cases, progression sans révélation du résultat et affichage partagé entre opérations. La compilation Android est validée, mais aucun test automatisé d'interface mobile n'existe. |
| Corrections courtes | Implémenté et vérifié | Conservées dans les cartes de résultat. |
| Corrections détaillées ouvrables | Implémenté partiellement | Les cartes ouvrent une page détaillée, mais certaines clés utilisent encore une explication de repli minimale. |
| Compteur positionnel | Implémenté et vérifié | Bases dix et deux, déplacements par 1, 10 et 100, retenues et emprunts. |
| Mesures | Implémenté et vérifié techniquement | Tableau commun pour longueurs, masses, contenances et aires. |
| Traductions | Implémenté et vérifié structurellement | Français, anglais, espagnol, italien et japonais ; parité des clés connues et paramètres contrôlés par tests. La qualité linguistique complète n'est pas auditée automatiquement. |
| Thèmes | Implémenté et vérifié | Clair, sombre et sépia avec palettes sémantiques centralisées. |
| Sauvegarde de progression | Implémenté et vérifié par tests | Nombre de lectures et date de première validation dans `course_progress.json`. |
| Historique d'entraînement | Implémenté partiellement | Les sessions sont écrites dans `training_history.json`, mais aucune interface complète de consultation et suppression n'est présente. |
| Paramètres persistants | Implémenté et vérifié | Langue et thème dans `settings.json`, avec migration des anciennes préférences MAUI. |
| Version Windows portable | Implémenté, validation externe restante | Script et archive autonome prévus ; le démarrage sur des machines Windows 10 et 11 propres reste à certifier. |
| APK Android autonome | Implémenté et compilable | Assemblies embarquées et format APK ; la matrice d'appareils physiques reste manuelle. |
| Études supérieures | Planifié | Mentionnées dans la vision, absentes du catalogue affiché actuel. |

## Données locales

| Fichier | Contenu |
|---|---|
| `settings.json` | Langue et thème sélectionnés |
| `course_progress.json` | Lectures et première validation de chaque cours |
| `training_history.json` | Sessions d'entraînement vérifiées |

Sur Windows portable, le dossier `Data` situé à côté de l'exécutable est
utilisé lorsqu'il existe. Sur Windows classique, les fichiers sont placés dans
`%LOCALAPPDATA%\MathsSousLeCapot\Data`. Sur Android, ils sont placés sous le
dossier de données privé de l'application.

## Validation technique

La ligne doit être actualisée après toute évolution importante du socle, des
catalogues ou des générateurs.

| Vérification | Dernier résultat connu | Date |
|---|---|---|
| Tests `MathsSousLeCapot.Core.Tests` | 247 réussis, 0 échec | 10 août 2026 |
| Build Windows Debug | Réussi, 0 avertissement | 10 août 2026 |
| Build Android Debug | Réussi, 0 avertissement | 10 août 2026 |
| Parcours principal Windows | Menu, cours, entraînement et assistant parcourus manuellement | 9 août 2026 |
| Android physique | Utilisation signalée, sans campagne reproductible documentée | Non certifié |
| Portable Windows 10 propre | Non vérifié | À faire |
| Portable Windows 11 propre | Non vérifié | À faire |

## Limites et dette connues

1. `cours.md` et `cours_avancer.md` décrivent encore plusieurs modules comme
   planifiés alors qu'ils sont présents dans les catalogues C#.
2. La profondeur pédagogique reste inégale, surtout au lycée et dans certaines
   corrections détaillées.
3. `DetailedExerciseExplanation` possède encore un texte de repli générique ;
   toutes les familles d'exercices ne fournissent donc pas le même niveau de
   raisonnement détaillé.
4. Les tests automatisés ciblent principalement le cœur métier. Il n'existe pas
   de suite automatisée pour l'interface MAUI sur Windows et Android.
5. L'historique est sauvegardé, mais sa consultation et ses différents niveaux
   de suppression ne sont pas encore exposés dans l'interface.
6. Plusieurs responsabilités sont concentrées dans de grands fichiers,
   notamment le catalogue principal, les générateurs, les corrections
   détaillées et le rendu du calcul guidé.
7. `bugs.md` et `idées.md` n'ont pas encore de format de suivi explicite.

## Mise à jour de ce document

Pour chaque changement fonctionnel important :

1. actualiser l'état de la fonctionnalité concernée ;
2. citer la limite qui reste, sans confondre compilation et validation réelle ;
3. mettre à jour les nombres de cours depuis les catalogues et leurs tests ;
4. dater les validations effectivement exécutées ;
5. conserver les fonctionnalités futures comme planifiées tant qu'elles ne sont
   pas accessibles dans l'application.
