# Projet : Maths — Sous le capot

## 1. Nom provisoire

Maths — Sous le capot

Le nom pourra évoluer ultérieurement.

L’idée centrale est de montrer les mécanismes internes des mathématiques plutôt que de demander à l’utilisateur de mémoriser mécaniquement des règles.

---

## 2. Technologie choisie

Framework : .NET MAUI
Langage principal : C#
Interface : XAML et C#
Version cible : version stable actuelle de .NET compatible avec .NET MAUI

Cibles prioritaires :

* Windows
* Android

Cibles envisageables ultérieurement :

* Linux

Le projet doit utiliser une base de code commune autant que possible.

L’interface doit être responsive et adaptée :

* aux fenêtres redimensionnables sur Windows ;
* aux écrans tactiles sur mobile ;
* à l’utilisation avec une souris ;
* aux orientations portrait et paysage lorsque cela est pertinent.

---

## 3. Architecture générale

L’application doit fonctionner localement et hors connexion.

Ne pas créer de serveur web pour la première version.

Architecture attendue :

Application .NET MAUI
│
├── Interface utilisateur
├── Modules pédagogiques
├── Visualisations interactives
├── Animations
├── Moteur mathématique local en C#
├── Cours intégrés localement
├── Exercices intégrés localement
└── Sauvegarde locale facultative de la progression

La logique mathématique doit être séparée de l’interface graphique.

---

## 4. But réel du projet

Créer une application éducative interactive permettant de comprendre les mathématiques de manière visuelle, logique et progressive.

L’application ne doit pas uniquement proposer :

* des formules ;
* des définitions ;
* des cours écrits ;
* des exercices scolaires classiques ;
* des questionnaires à choix multiples.

Elle doit montrer les mécanismes qui se cachent derrière les règles.

L’utilisateur doit pouvoir :

* manipuler des éléments ;
* observer une transformation ;
* comprendre pourquoi une règle fonctionne ;
* lire ensuite une explication courte ;
* s’entraîner avec des exercices associés.

Principe fondamental :

« Ne jamais demander à l’utilisateur de mémoriser une règle avant de lui avoir donné une chance de la comprendre. »

---

## 5. Accès aux cours

Pour la première version :

* aucun compte utilisateur ;
* aucune connexion Internet requise ;
* aucun abonnement ;
* aucune restriction artificielle ;
* tous les cours disponibles directement ;
* une partie exercices associée à chaque notion.

Les cours et les exercices doivent être embarqués localement dans l’application.

Une sauvegarde locale de la progression pourra être ajoutée sans imposer de compte.

---

## 6. Moteur mathématique

Le moteur mathématique doit être développé en C# et fonctionner localement.

Utiliser selon les besoins :

* les types numériques standards de .NET ;
* BigInteger pour les entiers arbitrairement grands ;
* une classe Fraction dédiée pour conserver les valeurs exactes ;
* Complex pour les nombres complexes ;
* Math.NET Numerics pour l’algèbre linéaire, les matrices, les vecteurs, les statistiques et les calculs numériques avancés ;
* éventuellement AngouriMath ultérieurement pour les manipulations symboliques.

Toujours distinguer :

* une valeur exacte ;
* une valeur approchée ;
* une représentation visuelle ;
* une valeur utilisée uniquement pour le rendu graphique.

Exemple :

Valeur exacte : √2
Valeur approchée : 1,41421356…

---

## 7. Visualisations

Utiliser les outils graphiques de .NET MAUI, notamment GraphicsView, pour afficher et manipuler des représentations mathématiques.

Exemples prévus :

* compteur positionnel ;
* retenues et emprunts ;
* groupes de blocs ;
* fractions représentées visuellement ;
* carrés et cubes ;
* racines carrées ;
* triangles ;
* surfaces ;
* théorème de Pythagore ;
* théorème de Thalès ;
* repères ;
* courbes ;
* vecteurs ;
* matrices.

Les animations doivent toujours servir l’explication.

Elles ne doivent pas être uniquement décoratives.

---

## 8. Structure pédagogique envisagée

### Chapitre 1 — Comprendre les nombres

* unités, dizaines, centaines et milliers ;
* numération positionnelle ;
* retenues ;
* base dix ;
* base deux ;
* bases numériques ;
* nombres négatifs ;
* nombres décimaux.

### Chapitre 2 — Comprendre les opérations

* addition ;
* soustraction ;
* multiplication ;
* division ;
* fractions ;
* priorités opératoires.

### Chapitre 3 — Comprendre les puissances

* multiplication répétée ;
* carrés ;
* cubes ;
* puissances ;
* racines carrées ;
* relations entre multiplication, division, fractions et puissances.

### Chapitre 4 — Comprendre la géométrie

* longueurs ;
* angles ;
* périmètres ;
* surfaces ;
* volumes ;
* théorème de Pythagore ;
* théorème de Thalès ;
* proportions ;
* agrandissements et réductions.

### Chapitres ultérieurs

* équations ;
* fonctions ;
* trigonométrie ;
* probabilités ;
* statistiques ;
* vecteurs ;
* matrices ;
* algèbre linéaire ;
* notions supplémentaires à définir progressivement.

Cette liste représente une vision à long terme.

Ne pas développer tous les chapitres immédiatement.

---

## 9. Première version minimale

Créer uniquement un premier module fonctionnel : le compteur positionnel.

Fonctionnalités attendues :

* affichage de plusieurs colonnes numériques ;
* unités ;
* dizaines ;
* centaines ;
* milliers ;
* bouton +1 ;
* bouton -1 ;
* animation lors d’une retenue ;
* explication textuelle courte ;
* sélecteur de base ;
* base dix ;
* base deux ;
* interface responsive Windows et Android.

Exemples à visualiser :

Base dix :

009
devient
010

099
devient
100

Base deux :

011
devient
100

Afficher une explication contextuelle :

« La colonne est pleine. Elle revient à zéro et ajoute une unité à la colonne suivante. »

---

## 10. Première tâche demandée

Avant de produire une quantité importante de code :

1. proposer l’arborescence initiale du projet ;
2. expliquer brièvement le rôle de chaque fichier ;
3. créer la solution .NET MAUI ;
4. séparer la logique mathématique de l’interface ;
5. implémenter le premier module du compteur positionnel ;
6. prévoir une architecture modulaire permettant d’ajouter de futurs cours et exercices sans modifier inutilement les modules existants.

Ne pas ajouter :

* de serveur ;
* de création de compte ;
* de connexion ;
* de publicité ;
* d’abonnement ;
* de boutique ;
* de système complexe de progression ;
* de fonctions en ligne ;
* de modules non demandés.

## 11. Fonctionnement des cours, entraînements et sauvegardes locales

### 11.1 Accès aux cours

L’application s’adresse à tout public.

Les notions vont des bases jusqu’au niveau bac +3.

Tous les cours sont accessibles librement dès le départ.
L’utilisateur peut commencer par la notion de son choix.

Les cours sont néanmoins classés dans un ordre croissant de complexité, avec de grandes sections visibles :

* Bases
* Primaire
* Collège
* Lycée
* Bac +1
* Bac +2
* Bac +3

---

### 11.2 Fiche d’entrée d’un cours

Avant de commencer un cours, afficher une fiche courte contenant :

* titre ;
* niveau ;
* objectif ;
* résumé rapide ;
* prérequis conseillés ;
* boutons d’action.

Les prérequis doivent être cliquables afin d’ouvrir directement les cours correspondants.

Deux boutons sont affichés :

* Lire le cours
* S’entraîner

---

### 11.3 Structure d’un cours

Un cours est séparé en petites étapes.

Chaque étape doit contenir :

* une seule idée principale ;
* une explication courte ;
* un exemple concret ;
* si pertinent, une visualisation ou une interaction ;
* des boutons précédent et suivant.

Éviter les murs de texte.

Lors du premier passage, certaines étapes peuvent proposer de petites questions facultatives afin de vérifier la compréhension.

Pour ces questions intégrées :

* l’utilisateur peut continuer sans répondre ;
* après une erreur, afficher un indice ;
* autoriser jusqu’à trois essais ;
* après trois erreurs, afficher la réponse expliquée ;
* après une bonne réponse, expliquer également pourquoi elle est correcte.

Lorsqu’un cours est relu après sa validation, ces petites questions intermédiaires sont masquées par défaut afin d’éviter les répétitions inutiles.

---

### 11.4 Validation d’un cours

À la fin de chaque cours, proposer une question finale :

* fixe ;
* simple ;
* directement liée à la notion présentée.

Le cours est marqué comme terminé uniquement lorsque l’utilisateur répond correctement à cette question finale.

Lors de la première validation, sauvegarder localement :

* la date ;
* l’heure.

Cette date de première validation ne doit jamais être remplacée lors d’une relecture ultérieure.

Sauvegarder également le nombre total de lectures du cours.

---

### 11.5 Mode « S’entraîner »

Le bouton « S’entraîner » permet de pratiquer une notion indépendamment de la lecture du cours.

Avant de commencer, demander le niveau souhaité :

* facile ;
* modéré ;
* difficile.

Une session contient cinq exercices générés aléatoirement.

Chaque exercice doit être associé au cours sélectionné.

Pendant la session :

* ne pas afficher immédiatement si la réponse est bonne ou mauvaise ;
* conserver les réponses saisies ;
* attendre que l’utilisateur clique sur « Vérifier ».

À la fin de la session, afficher un récapitulatif complet :

* nombre de bonnes réponses ;
* nombre de mauvaises réponses ;
* chaque question posée ;
* réponse donnée par l’utilisateur ;
* réponse correcte ;
* indication visuelle bonne ou mauvaise réponse ;
* explication du raisonnement pour chaque question.

Après le récapitulatif, afficher deux choix :

* Quitter
* Continuer avec cinq nouveaux exercices

---

### 11.6 Historique local des entraînements

Chaque session vérifiée doit être sauvegardée localement.

L’historique doit être regroupé par cours.

Pour chaque session, sauvegarder :

* cours concerné ;
* niveau choisi ;
* date ;
* heure ;
* cinq questions ;
* réponses données ;
* réponses correctes ;
* bonnes réponses ;
* mauvaises réponses ;
* explications associées.

---

### 11.7 Suppression de l’historique

Prévoir plusieurs niveaux de suppression :

* supprimer une session précise ;
* supprimer tout l’historique d’un cours ;
* supprimer tout l’historique de l’application.

Avant chaque suppression, afficher une confirmation.

Exemple :

« Êtes-vous sûr de vouloir supprimer cet historique ?
Cette action est irréversible. »

---

### 11.8 Progression locale

Aucun compte utilisateur n’est requis.

La progression doit être sauvegardée localement.

Afficher notamment :

* cours consultés ;
* cours terminés ;
* date et heure de première validation ;
* nombre de lectures de chaque cours ;
* historique des entraînements.

Prévoir éventuellement plus tard une section « Défis » permettant d’encourager l’utilisateur à compléter tous les cours.

Cette fonctionnalité n’est pas prioritaire pour la première version.