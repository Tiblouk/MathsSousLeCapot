# Audit des composants tiers

**Dernier audit :** 11 août 2026

**Cibles examinées :** Windows et Android

**Fichier distribué :** [`../../THIRD-PARTY-NOTICES.txt`](../../THIRD-PARTY-NOTICES.txt)

## Méthode

L'inventaire est établi à partir du graphe NuGet réellement restauré dans
`src/MathsSousLeCapot.App/obj/project.assets.json`, puis comparé aux fichiers de
licence et de notices présents dans les packages. Le script
`scripts/Update-ThirdPartyNotices.ps1` reproduit les textes juridiques sans les
traduire et déduplique les copies identiques.

Commande de régénération :

```powershell
dotnet restore .\src\MathsSousLeCapot.App\MathsSousLeCapot.App.csproj
.\scripts\Update-ThirdPartyNotices.ps1
```

Le fichier doit être régénéré et relu après toute modification d'une version de
.NET, d'un workload MAUI, d'un `PackageReference` ou d'une cible de publication.

## Composants distribués

### Socle commun

- .NET MAUI 9.0.120 ;
- .NET Runtime et Mono 9.0.15 ;
- bibliothèques `Microsoft.Extensions` 9.0.9.

### Windows

- Microsoft Windows App SDK 1.7.250909003 ;
- Microsoft Edge WebView2 1.0.3179.45 ;
- Microsoft.Graphics.Win2D 1.3.2 ;
- Microsoft.Maui.Graphics.Win2D.WinUI.Desktop 9.0.120 ;
- Microsoft.IO.RecyclableMemoryStream 3.0.1.

### Android

- bindings .NET pour AndroidX et Google Material ;
- Glide 4.16, Gson 2.13, Tink 1.17 et leurs bindings ;
- bibliothèques Kotlin, KotlinX, JetBrains et JSpecify ;
- dépendances Android transitives détaillées avec leur version exacte dans
  `THIRD-PARTY-NOTICES.txt`.

Les licences rencontrées comprennent principalement MIT, Apache-2.0 et
BSD-2-Clause. WebView2 et Windows App SDK fournissent leurs propres termes et
notices, reproduits depuis les packages restaurés.

## Dépendances non distribuées

Les packages suivants servent uniquement à la compilation ou aux tests et ne
sont pas embarqués comme bibliothèques de l'application :

- Microsoft.Maui.Controls.Build.Tasks ;
- Microsoft.Maui.Resizetizer ;
- Microsoft.NET.ILLink.Tasks ;
- Microsoft.Windows.SDK.BuildTools ;
- coverlet.collector ;
- Microsoft.NET.Test.Sdk et Microsoft.CodeCoverage ;
- xUnit, son runner, ses analyseurs et ses bibliothèques transitives ;
- Newtonsoft.Json utilisé transitivement par l'infrastructure de test.

Les outils de build apparaissent séparément dans la notice pour conserver un
inventaire transparent. Les packages exclusivement liés aux tests ne sont pas
inclus dans la distribution commerciale de l'application.

## Ressources graphiques et polices

L'audit du dossier `Resources` ne révèle actuellement :

- aucune police tierce embarquée ;
- aucune photographie ou illustration tierce ;
- aucun fichier audio ou vidéo tiers.

L'icône et l'écran de démarrage sont des SVG internes composés de formes et de
texte. Les dossiers `Resources/Fonts` et `Resources/Images` ne contiennent que
leurs fichiers `.gitkeep`.

## Points de distribution vérifiés

- le projet MAUI embarque `THIRD-PARTY-NOTICES.txt` comme ressource brute dans
  l'application Android ;
- le script Windows portable copie la notice à côté de l'exécutable ;
- la licence propre à Maths Sous le capot reste séparée des licences tierces.

Cet inventaire facilite la conformité mais ne remplace pas une revue juridique
professionnelle avant une première commercialisation.
