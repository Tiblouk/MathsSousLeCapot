# Philosophie de code

Ce document définit une philosophie générale de développement à appliquer lors
de tout projet réalisé avec Codex. Il ne décrit pas un produit particulier :
il sert de référence commune pour concevoir, modifier, tester et documenter du code de manière durable.

## Champ d'application

Les principes généraux de ce document s'appliquent à tous les projets.
Certaines règles concernent uniquement les projets pour lesquels elles sont pertinentes, notamment celles relatives à l'interface graphique, aux thèmes, aux traductions, aux saisies utilisateur et au stockage persistant.

Lorsqu'une règle n'est pas applicable au projet actuel, elle ne doit pas entraîner l'ajout prématuré d'une fonctionnalité ou d'une infrastructure inutile.

L'objectif de ce document n'est pas d'imposer systématiquement toutes les
pratiques à chaque modification. Les règles doivent être appliquées de manière proportionnée au périmètre, au risque et à la maturité du projet.

## 1. Comprendre avant de coder

- Lire entièrement la documentation fournie avant de commencer.
- Examiner l'architecture et le code existants avant de proposer une solution.
- Respecter les choix techniques, les conventions et les limites déjà établis.
- Ne pas supposer qu'une fonctionnalité est souhaitée lorsqu'elle n'est pas
  clairement demandée.
- Demander confirmation avant d'ajouter une fonctionnalité qui dépasse le
  périmètre décrit.
- En cas d'ambiguïté importante, poser une question plutôt que d'imposer une décision difficile à annuler.
- Ne demander confirmation que lorsque la décision possède un impact réel, difficilement réversible ou susceptible de modifier le besoin exprimé.
- Pour les détails mineurs et réversibles, choisir une solution cohérente avec le projet existant et signaler brièvement le choix effectué.
- Pour une première étape, privilégier une base fonctionnelle et cohérente
  plutôt qu'une implémentation précipitée de toutes les fonctionnalités.
- Lorsqu'un problème est signalé, rechercher sa cause racine avant de modifier
  le comportement ou l'apparence qui en révèle seulement le symptôme.
- Expliquer brièvement la cause identifiée lorsque la demande porte sur la
  correction d'un dysfonctionnement.

## 2. Développer progressivement

- Découper le travail en étapes compréhensibles et vérifiables.
- Commencer par le besoin prioritaire ou le premier parcours complet.
- Terminer correctement une fonctionnalité avant d'étendre inutilement son
  périmètre.
- Éviter de coder tous les modules d'un projet dès la première version.
- Préparer les extensions futures sans construire prématurément une
  architecture excessivement complexe.
- Informer clairement de ce qui est implémenté, de ce qui reste à faire et des
  éventuelles limites connues.

## 3. Concevoir une architecture modulaire

- Séparer les responsabilités techniques et fonctionnelles.
- Isoler la logique métier de l'interface, du stockage et des services externes.
- Organiser le code par modules ou fonctionnalités cohérentes.
- Permettre l'ajout d'un module sans devoir modifier inutilement les modules
  existants.
- Limiter les dépendances entre modules.
- Centraliser les comportements véritablement partagés.
- Réutiliser les composants communs au lieu de dupliquer leur fonctionnement.
- N'ajouter une abstraction que lorsqu'elle réduit une complexité réelle ou
  correspond à une structure déjà établie.
- Préférer des contrats clairs entre les composants aux accès implicites ou
  dispersés.

## 4. Respecter le projet existant

- Suivre les conventions, bibliothèques et modèles déjà utilisés dans le dépôt.
- Limiter les changements aux fichiers nécessaires pour accomplir la demande.
- Ne pas profiter d'une modification pour effectuer des refactorisations sans
  rapport.
- Ne pas supprimer ou annuler des changements existants sans autorisation.
- Considérer que les modifications inconnues peuvent appartenir à une autre
  personne et travailler avec elles.
- Préserver la compatibilité existante sauf lorsqu'une rupture est explicitement
  demandée et acceptée.
- Vérifier qu'un correctif ne dégrade pas les parcours voisins ou les
  comportements qui fonctionnaient déjà.
- Respecter strictement le dossier de travail autorisé.
- Ne pas créer, modifier, déplacer ou supprimer un fichier en dehors de cet
  espace sans demande explicite.
- Vérifier le chemin cible avant toute opération destructive ou difficile à
  annuler.

## 5. Écrire du code lisible

- Utiliser des noms explicites pour les classes, fonctions, méthodes,
  propriétés, champs et variables.
- Garder chaque élément centré sur une responsabilité identifiable.
- Préférer une solution simple et directe à une construction abstraite sans
  bénéfice concret.
- Éviter les valeurs magiques et les chaînes dupliquées lorsqu'elles possèdent
  une signification commune.
- Utiliser des types et des structures adaptés aux données plutôt que des
  manipulations de texte fragiles.
- Traiter explicitement les erreurs et les cas limites importants.
- Favoriser un code facile à relire et à modifier par une autre personne.

## 6. Commenter utilement

- Documenter les classes et composants importants en expliquant leur
  responsabilité.
- Documenter les méthodes en précisant leur rôle, particulièrement lorsqu'elles
  font partie d'une API ou portent un comportement métier.
- Documenter les champs et propriétés qui représentent un état, une dépendance
  ou une règle importante.
- Commenter une variable locale lorsque sa signification n'est pas évidente.
- Ajouter un commentaire court avant une logique complexe lorsque cela évite
  une lecture laborieuse.
- Expliquer le pourquoi d'une décision plutôt que paraphraser le code.
- Éviter les commentaires inutiles qui répètent une instruction simple.
- Maintenir les commentaires lors des modifications afin qu'ils ne deviennent pas trompeurs.
- Ne pas surcharger le code de commentaires ou de documentation lorsque les noms et la structure suffisent déjà à expliquer clairement le comportement.
- Documenter en priorité les éléments publics, les responsabilités
  structurantes, les règles métier et les décisions dont la raison ne peut pas
  être déduite directement du code.

## 7. Prévoir la traduction

- Ne pas disperser les textes visibles directement dans le code ou
  l'interface.
- Utiliser des clés de traduction stables.
- Stocker les traductions dans des fichiers JSON séparés par langue lorsqu'un
  projet doit être multilingue.
- Utiliser un fichier d'index JSON pour répertorier les langues disponibles,
  leur code, leur nom et leur fichier.
- Conserver exactement les mêmes clés dans tous les fichiers de langue.
- Préserver les paramètres de formatage comme `{0}` et `{1}` dans chaque
  traduction.
- Prévoir une langue par défaut lorsque la langue demandée n'est pas disponible.
- Faire suivre la langue active aux textes statiques comme aux textes générés
  dynamiquement.
- Actualiser correctement l'interface après un changement de langue.
- Vérifier automatiquement la validité des JSON, la parité des clés et les
  paramètres de formatage.

## 8. Concevoir une interface robuste

- Ne pas dépendre aveuglément des couleurs ou styles implicites du système.
- Définir les fonds, textes et contrôles nécessaires à une lecture correcte.
- Prévoir une architecture permettant de gérer les thèmes lorsque l'interface graphique est destinée à évoluer.
- Ajouter un thème clair et un thème sombre lorsqu'ils sont utiles au produit ou demandés dans le périmètre de la version actuelle.
- Vérifier le contraste et la lisibilité dans chaque thème.
- Utiliser des conventions visuelles constantes pour les succès, erreurs, avertissements et informations.
- Prévoir une navigation claire, notamment un moyen de revenir à l'écran précédent ou principal.
- Adapter l'interface aux différentes tailles d'écran, méthodes de saisie et plateformes ciblées.
- Ne pas utiliser une animation uniquement pour décorer lorsqu'elle risque de gêner la compréhension ou l'utilisation.
- Faire dépendre l'état des commandes de l'état réel de l'application.
- Empêcher les actions impossibles, incompatibles ou déjà en cours.
- Expliquer, notamment par une infobulle lorsque cela convient, pourquoi une commande ou une option est indisponible.
- Distinguer clairement les données obligatoires des données facultatives.
- Lorsqu'une valeur n'est pas destinée à être modifiée directement par l'utilisateur, rendre son état visuellement compréhensible sans empêcher sa mise à jour par le programme.
- Prévoir un état vide ou indisponible compréhensible pour les fonctionnalités facultatives qui ne sont pas configurées.
- Définir explicitement le comportement de l'interface lorsque l'espace
  disponible diminue : redimensionnement, retour à la ligne, défilement ou masquage contrôlé.
- Conserver des dimensions cohérentes pour les contrôles qui possèdent le même rôle et éviter que leur contenu dynamique déplace la mise en page.

## 9. Accepter les saisies raisonnables

- Ne pas rejeter une réponse correcte uniquement à cause d'une différence de
  présentation sans importance.
- Normaliser les saisies lorsque le contexte le permet : casse, espaces,
  accents, apostrophes, séparateurs ou traits d'union.
- Accepter plusieurs représentations légitimes d'une même valeur.
- Maintenir une distinction claire lorsque deux formats similaires n'ont pas la
  même signification.
- Adapter l'analyse des saisies à la langue et au domaine fonctionnel.
- Afficher un retour explicite et compréhensible après validation.
- Utiliser une indication visuelle cohérente, par exemple vert pour une réussite
  et rouge pour une erreur, sans dépendre uniquement de la couleur.
- Valider autant que possible la nature réelle d'une ressource, et pas
  seulement l'existence de son chemin ou la syntaxe de son adresse.

## 10. Stocker les données avec discernement

- Ne conserver que les données réellement utiles au fonctionnement demandé.
- Préférer un stockage local lorsqu'aucun serveur n'est nécessaire.
- Ne pas imposer de compte, de connexion Internet ou de service distant sans
  justification fonctionnelle.
- Séparer la logique de stockage de l'interface et de la logique métier.
- Protéger les données existantes lors des migrations ou changements de format.
- Demander une confirmation avant toute suppression irréversible.
- Documenter les données sauvegardées et leur durée de conservation lorsque
  cela est pertinent.

## 11. Tester selon le risque

- Ajouter des tests aux règles métier et aux comportements importants.
- Tester les cas nominaux, les limites et les erreurs prévisibles.
- Augmenter la couverture lorsque la modification touche un composant partagé
  ou un parcours utilisateur important.
- Vérifier les différentes plateformes ciblées lorsqu'elles peuvent se
  comporter différemment.
- Pour une interface adaptative, vérifier les tailles minimale, maximisée et
  intermédiaires ainsi que les résolutions et facteurs d'échelle pertinents.
- Contrôler l'absence de contenu coupé, de chevauchement, d'espace vide anormal
  et de barres de défilement superflues.
- Compiler le projet après une modification significative.
- Exécuter les tests existants avant de considérer le travail comme terminé.
- Vérifier le démarrage ou le parcours principal lorsque la modification touche
  l'initialisation, les ressources, la navigation ou la configuration.
- Ne pas masquer un test en échec ou un avertissement important.
- Signaler clairement toute validation qui n'a pas pu être exécutée.
- Ajouter un test de régression lorsqu'un défaut corrigé peut être reproduit de
  manière automatique.

## 12. Maintenir la documentation

- Mettre à jour le fichier `README.md` lorsqu'une modification affecte l'installation, l'utilisation, l'architecture, les commandes, les prérequis ou un comportement visible important.
- Documenter l'installation, les prérequis, les commandes de compilation, les
  tests et le lancement.
- Décrire l'architecture générale et le rôle des principaux dossiers.
- Mettre à jour la documentation lorsqu'une langue, un thème, une plateforme,
  une dépendance ou une commande change.
- Créer un document fonctionnel distinct lorsque le projet possède des règles
  métier ou un objectif détaillé.
- Utiliser le présent document comme référence générale des pratiques de
  développement.
- Ajouter ici toute nouvelle règle durable destinée à s'appliquer aux futurs
  projets.
- Ne documenter comme présent, pris en charge ou sécurisé qu'un comportement
  vérifié dans le code, la configuration ou les résultats de test.
- Séparer clairement les faits confirmés, les limitations connues, les
  hypothèses et les informations que l'auteur doit encore compléter.

## 13. Communiquer clairement pendant le travail

- Expliquer brièvement ce qui va être examiné avant de modifier le projet.
- Donner des nouvelles lorsqu'une tâche comporte plusieurs étapes.
- Signaler les décisions techniques importantes et leurs conséquences.
- Ne pas prétendre qu'une fonctionnalité fonctionne sans l'avoir vérifiée.
- Présenter les erreurs et les limites honnêtement.
- À la fin, résumer les changements réellement effectués et les validations
  exécutées.
- Ne pas demander à l'utilisateur d'effectuer une étape que Codex peut réaliser
  directement dans l'environnement partagé.
- Ne pas interrompre inutilement le travail pour une décision locale,
  réversible et raisonnablement déductible du projet.

## 14. Définition d'un travail terminé

Une tâche est considérée comme terminée lorsque :

- la demande a été comprise et respectée ;
- le périmètre n'a pas été étendu sans accord ;
- les modifications sont limitées à ce qui est nécessaire ;
- l'implémentation suit l'architecture du projet ;
- le code est lisible et documenté lorsque cela apporte une réelle valeur ;
- les traductions, thèmes, plateformes et formats de données concernés ont été pris en compte lorsqu'ils sont applicables ;
- les validations pertinentes ont été exécutées ;
- les cibles concernées compilent lorsque l'environnement le permet ;
- les risques de régression et les parcours voisins concernés ont été vérifiés ;
- l'interface a été contrôlée aux tailles et facteurs d'échelle pertinents
  lorsqu'elle a été modifiée ;
- la documentation utile a été mise à jour si nécessaire ;
- les limites restantes et les validations non exécutées ont été clairement signalées.

## 15. Préserver la réversibilité

- Examiner l'état du dépôt avant toute modification importante.
- Identifier les fichiers déjà modifiés afin de ne pas écraser un travail en cours.
- Préférer des changements ciblés et faciles à relire.
- Ne pas effectuer d'opération destructive sans autorisation explicite.
- Ne pas supprimer un fichier, une donnée ou une fonctionnalité existante sans vérifier qu'elle n'est plus utilisée.
- Prévoir une migration ou une compatibilité descendante lorsqu'un format de données existant évolue.
- Signaler clairement les modifications difficiles à annuler.

## 16. Externaliser la configuration

- Ne pas inscrire directement dans le code les chemins, adresses, ports ou paramètres susceptibles de changer selon l'installation.
- Regrouper les paramètres configurables dans un emplacement clairement identifié.
- Fournir des valeurs par défaut raisonnables lorsque cela est possible.
- Valider les paramètres chargés et afficher une erreur compréhensible lorsqu'ils sont invalides.
- Ne pas stocker de secret ou d'information sensible directement dans le dépôt.
- Documenter les paramètres disponibles et leur effet.


## 17. Faciliter le diagnostic

- Afficher des messages d'erreur compréhensibles pour l'utilisateur.
- Conserver les informations techniques utiles au diagnostic dans des journaux.
- Inclure suffisamment de contexte pour identifier l'étape ayant échoué.
- Ne pas masquer silencieusement une exception importante.
- Éviter d'exposer inutilement des données sensibles dans les journaux.
- Prévoir un moyen simple de retrouver ou d'exporter les journaux lorsqu'un diagnostic utilisateur est nécessaire.
- Classer les journaux selon leur nature réelle : information, réussite,
avertissement ou erreur.
- Lorsqu'une interface de filtrage existe, garantir que chaque entrée apparaît dans les vues correspondant à sa catégorie sans disparaître de la vue générale.
- Pour un lien réseau, distinguer une adresse syntaxiquement valide d'un
service réellement accessible et expliquer précisément l'échec constaté.

## 18. Ajouter des dépendances avec discernement

- Ne pas ajouter une dépendance externe lorsque le besoin peut être satisfait simplement avec les outils déjà présents.
- Vérifier qu'une dépendance est maintenue, compatible avec le projet et adaptée à son usage.
- Évaluer son impact sur la compilation, la distribution et les plateformes ciblées.
- Documenter toute nouvelle dépendance importante.
- Supprimer les dépendances devenues inutiles.

## 19. Optimiser à partir de besoins réels

- Privilégier d'abord la clarté et la correction du comportement.
- Ne pas complexifier le code pour résoudre un problème de performance non observé.
- Mesurer les performances avant d'entreprendre une optimisation importante.
- Documenter les compromis lorsqu'une optimisation réduit la lisibilité ou la simplicité.
- Pour une distribution, contrôler aussi le poids des dépendances et ressources
  réellement embarquées sans sacrifier la correction ou la maintenabilité.

## 20. Concevoir une interface accessible

- Ne pas transmettre une information uniquement par la couleur.
- Maintenir un contraste suffisant entre les textes, les fonds et les contrôles.
- Utiliser des libellés compréhensibles et des messages d'erreur explicites.
- Prévoir une navigation utilisable au clavier lorsque la plateforme le permet.
- Éviter les animations gênantes, trop rapides ou indispensables à la compréhension.
- Vérifier que l'interface reste lisible avec une taille de texte différente lorsque la plateforme le permet.

## 21. Gérer les processus externes explicitement

- Lorsqu'une application lance ou supervise des processus externes.
- Définir clairement qui lance, surveille, masque, restaure et ferme chaque processus externe.
- Distinguer une fermeture normale, une fermeture demandée et un arrêt inattendu.
- Ne pas laisser actif après la fermeture de l'application un processus dont elle est responsable, sauf comportement volontaire et documenté.
- Ne pas interrompre un processus préexistant ou indépendant sans certitude qu'il appartient au cycle de vie géré par l'application.
- Afficher un état fidèle lorsqu'un processus démarre, s'arrête, échoue ou devient indisponible.

## 22. Préparer une publication publique responsable

- Vérifier la présence et la pertinence de la licence du projet.
- Identifier les licences, notices et droits applicables aux dépendances,
  ressources et logiciels tiers.
- Ne pas présenter un projet comme affilié, approuvé ou officiel sans preuve et
  autorisation explicites.
- Décrire uniquement les fichiers lus ou écrits, les processus lancés, les
  communications réseau et la télémétrie qui ont été vérifiés dans le code.
- Retirer des fichiers publiés les secrets, chemins personnels, configurations
  locales, journaux et sorties de compilation inutiles.
- Distinguer clairement ce qui appartient au projet de ce qui doit être fourni,
  installé ou configuré séparément par l'utilisateur.

## 23. Produire une distribution légère et reproductible

- Ne distribuer que les exécutables, dépendances, ressources et documents
  nécessaires au fonctionnement.
- Vérifier qu'aucun fichier de développement, cache, journal ou artefact
  intermédiaire inutile n'est inclus.
- Documenter les commandes et prérequis permettant de reproduire la
  compilation et le paquet publié.
- Vérifier le contenu final de la distribution, et pas seulement la réussite de
  la compilation.

## 24. Gérer les variantes et la compatibilité

- Lorsqu'un logiciel, format ou environnement possède plusieurs versions,
  centraliser les règles de détection et de compatibilité.
- Rechercher les variantes connues de manière structurée plutôt que disperser des noms, chemins ou conditions historiques dans le code et l'interface.
- Conserver une stratégie de repli explicite lorsqu'une variante ne peut pas être détectée automatiquement.
- Tester les variantes réellement prises en charge et documenter les limites de compatibilité.

## 25. Appliquer une sécurité proportionnée au risque

- Identifier les données, entrées et opérations susceptibles d'être exposées à une utilisation incorrecte ou malveillante.
- Valider les entrées provenant de l'utilisateur, de fichiers ou de services externes.
- Ne pas accorder plus de permissions que nécessaire.
- Ne pas introduire une complexité excessive pour protéger un risque inexistant ou négligeable.
- Signaler clairement les limites de sécurité qui n'ont pas pu être vérifiées.