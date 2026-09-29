# Documentation du projet

Ce dossier rassemble les documents nécessaires pour comprendre, maintenir et
faire évoluer **Maths Sous le capot**. Il distingue volontairement la vision du
produit, l'état réellement vérifié, l'architecture technique et les contenus
pédagogiques.

**Dernier audit documentaire :** 29 septembre 2026

**Point de référence du code :** version `0.1.1`, publiée sous le tag Git
`v0.1.1`.

## Ordre de lecture conseillé

1. [`projet.md`](projet.md) : vision, objectifs et règles fonctionnelles.
2. [`etat-du-projet.md`](etat-du-projet.md) : fonctionnalités réellement
   présentes, validations effectuées et limites connues.
3. [`architecture.md`](architecture.md) : composants, dépendances et flux
   techniques actuels.
4. [`../README.md`](../README.md) : prérequis, commandes de compilation,
   lancement et tests.
5. [`cours.md`](cours.md) : organisation pédagogique générale et catalogue
   historique des cours.
6. [`cours_avancer.md`](cours_avancer.md) : cadrage pédagogique détaillé du
   lycée.
7. [`Philosophie de code.md`](Philosophie%20de%20code.md) : principes généraux
   de développement appliqués avec Codex.
8. [`../LICENSE.md`](../LICENSE.md) : droits d'usage personnel, de modification,
   de redistribution gratuite et conditions applicables aux organisations.
9. [`../CONTRIBUTING.md`](../CONTRIBUTING.md) : règles de contribution et
   formalisation des droits avant intégration au projet officiel.
10. [`../CHANGELOG.md`](../CHANGELOG.md) : contenu des versions publiées.

## Sources de vérité

| Sujet | Document ou emplacement de référence |
|---|---|
| Finalité et règles du produit | [`projet.md`](projet.md) |
| État fonctionnel actuel | [`etat-du-projet.md`](etat-du-projet.md) |
| Architecture implémentée | [`architecture.md`](architecture.md) |
| Commandes de développement | [`../README.md`](../README.md) |
| Historique des versions | [`../CHANGELOG.md`](../CHANGELOG.md) |
| Packaging Windows portable | [`packaging-windows.md`](packaging-windows.md) |
| Progression et contenu pédagogique | [`cours.md`](cours.md) et [`cours_avancer.md`](cours_avancer.md) |
| Conventions générales de code | [`Philosophie de code.md`](Philosophie%20de%20code.md) |
| Licence et droits d'utilisation | [`../LICENSE.md`](../LICENSE.md) |
| Licences professionnelles et organisationnelles | [`../COMMERCIAL-LICENSING.md`](../COMMERCIAL-LICENSING.md) |
| Contributions et cession des droits | [`../CONTRIBUTING.md`](../CONTRIBUTING.md) et [`legal/CONTRIBUTOR-ASSIGNMENT-AGREEMENT.md`](legal/CONTRIBUTOR-ASSIGNMENT-AGREEMENT.md) |
| Composants tiers et leurs licences | [`../THIRD-PARTY-NOTICES.txt`](../THIRD-PARTY-NOTICES.txt) et [`legal/third-party-audit.md`](legal/third-party-audit.md) |
| Bugs suivis | [`bugs.md`](bugs.md) |
| Idées et suivi de leur réalisation | [`idées.md`](id%C3%A9es.md) |
| Comportement exécutable | `src/` et `tests/` |

En cas de contradiction sur une fonctionnalité déjà développée, le code et les
tests priment, puis `etat-du-projet.md`. Les catalogues pédagogiques peuvent
décrire une cible future : ils ne prouvent pas à eux seuls qu'un module est
implémenté.

## Rôle des documents

### Documents de référence

- `projet.md` change lorsqu'une règle produit ou un objectif durable évolue.
- `etat-du-projet.md` est une photographie factuelle et datée du produit.
- `architecture.md` décrit uniquement l'organisation réellement présente dans
  le code, avec ses contraintes et ses points d'extension.
- le `README.md` racine reste une porte d'entrée courte pour construire et
  lancer l'application.
- `LICENSE.md` définit les autorisations accordées au public et réserve les
  usages professionnels, organisationnels et commerciaux ;
- `CONTRIBUTING.md` et le modèle placé dans `docs/legal/` encadrent l'intégration
  de code tiers à la version officielle.

### Documents pédagogiques

- `cours.md` organise la progression générale et fournit le modèle d'un cours ;
- `cours_avancer.md` détaille le programme du lycée et les approfondissements
  possibles.

Leur remise en cohérence complète avec les catalogues C# constitue une étape
distincte. Tant qu'elle n'est pas terminée, `etat-du-projet.md` indique les
quantités et fonctionnalités effectivement présentes.

### Documents de suivi

- `bugs.md` doit contenir les défauts confirmés et reproductibles ;
- `idées.md` conserve les propositions, leur état et leur devenir sans les
  confondre avec les engagements décrits dans `projet.md`.

Un fichier vide est ambigu. Lors de la prochaine phase documentaire, ces deux
fichiers recevront un modèle explicite permettant d'indiquer soit les éléments
ouverts, soit qu'aucun élément n'est connu.

## Règles de maintenance

Une modification doit mettre à jour :

- `etat-du-projet.md` lorsqu'elle ajoute, retire ou limite une fonctionnalité ;
- `architecture.md` lorsqu'elle change une responsabilité, une dépendance, un
  flux de chargement ou un format de stockage ;
- le `README.md` racine lorsqu'elle change un prérequis, une commande ou une
  procédure visible ;
- les documents de cours lorsqu'elle change le contenu ou la progression
  pédagogique ;
- `packaging-windows.md` lorsqu'elle change la production ou le contenu de
  l'archive portable.
- les documents juridiques lorsqu'elle change les droits d'utilisation, de
  redistribution, de contribution ou de commercialisation.
- `THIRD-PARTY-NOTICES.txt` et son audit après toute modification de .NET, des
  workloads, des packages ou des ressources externes.

Chaque affirmation de compatibilité ou de fonctionnement doit préciser ce qui
a été vérifié. Une compilation réussie ne remplace pas un essai sur un appareil
physique, et un cours techniquement disponible n'implique pas que sa profondeur
pédagogique a été entièrement relue.

## Convention d'état

Les documents de suivi utilisent les états suivants :

- **Implémenté et vérifié** : présent dans le code et couvert par une validation
  adaptée ;
- **Implémenté partiellement** : utilisable, mais incomplet ou encore inégal ;
- **À revoir** : présent, mais une correction ou une révision est identifiée ;
- **Planifié** : cadré, sans implémentation complète ;
- **Non commencé** : aucun travail significatif n'est présent ;
- **Non vérifié** : l'implémentation existe, mais la validation annoncée n'a pas
  été exécutée dans l'environnement concerné.
