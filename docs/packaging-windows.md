# Packaging Windows portable

L'application Windows est une application .NET MAUI / WinUI. La version
portable est donc générée avec `dotnet publish` en mode Windows non empaqueté,
puis archivée dans un fichier ZIP.

## Générer l'archive

Depuis la racine de la solution :

```powershell
.\scripts\Package-WindowsPortable.ps1
```

Le script produit :

```text
artifacts/windows-portable/MathsSousLeCapot-Windows-Portable-1.0.0.zip
```

L'archive contient également `LICENSE.md` et `COMMERCIAL-LICENSING.md` afin que
les conditions d'utilisation accompagnent l'exécutable distribué.

## Utilisation côté utilisateur

L'utilisateur doit seulement :

1. télécharger l'archive ZIP ;
2. la décompresser ;
3. lancer `MathsSousLeCapot.exe`.

Aucun terminal n'est nécessaire pour lancer l'application.

## Données utilisateur

Dans la version portable, le dossier `Data` est placé à côté de l'exécutable.
Il contient les fichiers sauvegardables :

- `settings.json` ;
- `course_progress.json` ;
- `training_history.json`.

Pour sauvegarder ou déplacer l'application portable, il suffit de copier le
dossier extrait complet, y compris `Data`.

Hors version portable, les mêmes fichiers sont placés dans :

```text
%LOCALAPPDATA%\MathsSousLeCapot\Data
```

## Compatibilité visée

- Windows 10 64 bits ;
- Windows 11 64 bits.

Le script publie une version `win-x64` non empaquetée et autonome avec le
runtime .NET et le Windows App SDK embarqués. L'option `UseMonoRuntime=false`
évite que la publication Windows demande un runtime Mono prévu pour d'autres
cibles MAUI.
