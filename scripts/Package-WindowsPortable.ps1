[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$Version = "1.0.0",
    [switch]$KeepPublishDirectory
)

$ErrorActionPreference = "Stop"

# Racine du dépôt, résolue depuis le dossier du script pour accepter les chemins avec espaces ou accents.
$RepositoryRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$ProjectPath = Join-Path $RepositoryRoot "src\MathsSousLeCapot.App\MathsSousLeCapot.App.csproj"
$ArtifactsRoot = Join-Path $RepositoryRoot "artifacts\windows-portable"
$PublishDirectory = Join-Path $ArtifactsRoot "publish"
$PackageName = "MathsSousLeCapot-Windows-Portable-$Version"
$PortableDirectory = Join-Path $ArtifactsRoot $PackageName
$ZipPath = Join-Path $ArtifactsRoot "$PackageName.zip"

function Remove-DirectoryInRepository {
    param([string]$Path)

    # Vérifie que la suppression reste dans le dossier de travail attendu.
    if (-not (Test-Path $Path)) {
        return
    }

    $resolvedPath = Resolve-Path $Path
    if (-not $resolvedPath.Path.StartsWith($RepositoryRoot.Path, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refus de supprimer un dossier hors du projet : $($resolvedPath.Path)"
    }

    Remove-Item -LiteralPath $resolvedPath.Path -Recurse -Force
}

function New-ReadmeFile {
    param([string]$Path)

    # Le texte reste volontairement simple : il sera lu par des personnes qui ne veulent pas utiliser le terminal.
    @"
Maths Sous le capot - version portable Windows

Utilisation :
1. Decompressez toute l'archive dans un dossier de votre choix.
2. Lancez MathsSousLeCapot.exe.

Donnees utilisateur :
- Les reglages, la progression et l'historique sont stockes dans le dossier Data.
- Pour sauvegarder ou deplacer l'application, copiez tout le dossier, y compris Data.

Compatibilite visee :
- Windows 10 64 bits
- Windows 11 64 bits

Licence :
- Consultez LICENSE.md avant toute utilisation ou redistribution.
- Une organisation ou un professionnel doit obtenir une licence payante.

Si Windows bloque l'application au premier lancement, ouvrez les proprietes du fichier ZIP ou de l'executable et choisissez Debloquer, puis relancez.
"@ | Set-Content -LiteralPath $Path -Encoding UTF8
}

Write-Host "Publication Windows portable..."
Remove-DirectoryInRepository -Path $ArtifactsRoot
New-Item -ItemType Directory -Path $PublishDirectory | Out-Null

dotnet publish $ProjectPath `
    -c $Configuration `
    -f net9.0-windows10.0.19041.0 `
    -r $RuntimeIdentifier `
    --self-contained true `
    -p:UseMonoRuntime=false `
    -p:WindowsPackageType=None `
    -p:WindowsAppSDKSelfContained=true `
    -p:PublishSingleFile=false `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -p:AppxPackage=false `
    -p:GenerateAppxPackageOnBuild=false `
    -o $PublishDirectory
if ($LASTEXITCODE -ne 0) {
    throw "La publication Windows portable a échoué avec le code $LASTEXITCODE."
}

Write-Host "Preparation du dossier portable..."
New-Item -ItemType Directory -Path $PortableDirectory | Out-Null
Copy-Item -Path (Join-Path $PublishDirectory "*") -Destination $PortableDirectory -Recurse -Force

# La licence accompagne chaque distribution officielle de l'application.
Copy-Item -LiteralPath (Join-Path $RepositoryRoot "LICENSE.md") -Destination $PortableDirectory -Force
Copy-Item -LiteralPath (Join-Path $RepositoryRoot "COMMERCIAL-LICENSING.md") -Destination $PortableDirectory -Force

$DataDirectory = Join-Path $PortableDirectory "Data"
New-Item -ItemType Directory -Path $DataDirectory | Out-Null
New-ReadmeFile -Path (Join-Path $PortableDirectory "Lisez-moi.txt")
Set-Content -LiteralPath (Join-Path $DataDirectory "README.txt") -Encoding UTF8 -Value @"
Ce dossier contient les donnees utilisateur de la version portable :
- settings.json
- course_progress.json
- training_history.json

Il peut etre sauvegarde ou copie avec l'application.
"@

$ExecutablePath = Join-Path $PortableDirectory "MathsSousLeCapot.exe"
if (-not (Test-Path $ExecutablePath)) {
    throw "Executable introuvable : $ExecutablePath"
}

Write-Host "Creation de l'archive ZIP..."
if (Test-Path $ZipPath) {
    Remove-Item -LiteralPath $ZipPath -Force
}
Compress-Archive -Path $PortableDirectory -DestinationPath $ZipPath -Force

if (-not $KeepPublishDirectory) {
    Remove-DirectoryInRepository -Path $PublishDirectory
}

Write-Host "Archive portable creee : $ZipPath"
