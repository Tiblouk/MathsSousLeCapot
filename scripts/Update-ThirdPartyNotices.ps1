[CmdletBinding()]
param(
    [string]$AppProject = "src\MathsSousLeCapot.App\MathsSousLeCapot.App.csproj",
    [string]$OutputPath = "THIRD-PARTY-NOTICES.txt"
)

$ErrorActionPreference = "Stop"

# Les chemins sont résolus depuis le dépôt pour accepter un lancement depuis n'importe quel dossier.
$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$ResolvedProjectPath = [System.IO.Path]::GetFullPath((Join-Path $RepositoryRoot $AppProject))
$ResolvedOutputPath = [System.IO.Path]::GetFullPath((Join-Path $RepositoryRoot $OutputPath))
$AssetsPath = Join-Path (Split-Path $ResolvedProjectPath -Parent) "obj\project.assets.json"

if (-not (Test-Path -LiteralPath $AssetsPath)) {
    throw "Inventaire NuGet introuvable. Exécutez dotnet restore avant ce script : $AssetsPath"
}

# Ces packages participent à la compilation mais ne sont pas distribués comme bibliothèques de l'application.
$BuildOnlyPackages = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase)
@(
    "Microsoft.Maui.Controls.Build.Tasks",
    "Microsoft.Maui.Resizetizer",
    "Microsoft.NET.ILLink.Tasks",
    "Microsoft.Windows.SDK.BuildTools"
) | ForEach-Object { [void]$BuildOnlyPackages.Add($_) }

# Les contenus juridiques sont dédupliqués après normalisation des fins de ligne.
$LegalTexts = @{}

function ConvertTo-NormalizedText {
    param([string]$Text)

    $normalizedLines = ($Text -replace "`r`n", "`n") -split "`n" |
        ForEach-Object { $_.TrimEnd() }
    return ($normalizedLines -join "`n").TrimEnd()
}

function Get-NormalizedText {
    param([string]$Path)

    return ConvertTo-NormalizedText -Text (Get-Content -LiteralPath $Path -Raw)
}

function Get-TextHash {
    param([string]$Text)

    $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
    $hash = [System.Security.Cryptography.SHA256]::HashData($bytes)
    return [Convert]::ToHexString($hash)
}

function Add-LegalText {
    param(
        [string]$Source,
        [string]$Text
    )

    if ([string]::IsNullOrWhiteSpace($Text)) {
        return
    }

    $normalizedText = ConvertTo-NormalizedText -Text $Text
    $hash = Get-TextHash -Text $normalizedText
    if (-not $LegalTexts.ContainsKey($hash)) {
        $LegalTexts[$hash] = [pscustomobject]@{
            Sources = [System.Collections.Generic.List[string]]::new()
            Text = $normalizedText
        }
    }

    if (-not $LegalTexts[$hash].Sources.Contains($Source)) {
        $LegalTexts[$hash].Sources.Add($Source)
    }
}

function Add-LegalFilesFromDirectory {
    param(
        [string]$Directory,
        [string]$SourcePrefix
    )

    if (-not (Test-Path -LiteralPath $Directory)) {
        return
    }

    $legalFiles = Get-ChildItem -LiteralPath $Directory -Recurse -File |
        Where-Object {
            $_.Name -match '(?i)(license|licence|notice|copying)' -and
            $_.Extension -notin @('.nupkg', '.sha512', '.p7s')
        }

    foreach ($file in $legalFiles) {
        $relativePath = $file.FullName.Substring($Directory.Length).TrimStart('\', '/')
        Add-LegalText -Source "$SourcePrefix - $relativePath" -Text (Get-NormalizedText -Path $file.FullName)
    }
}

function Get-PackageMetadata {
    param(
        [string]$PackageId,
        [string]$Version,
        [string]$Directory,
        [string[]]$Platforms
    )

    $nuspecPath = Get-ChildItem -LiteralPath $Directory -Filter '*.nuspec' -File |
        Select-Object -First 1 -ExpandProperty FullName
    if (-not $nuspecPath) {
        throw "Métadonnées NuGet introuvables pour $PackageId $Version."
    }

    [xml]$nuspec = Get-Content -LiteralPath $nuspecPath -Raw
    $metadata = $nuspec.package.metadata
    $license = if ($metadata.license) { $metadata.license.'#text' } else { $metadata.licenseUrl }
    $licenseType = if ($metadata.license) { $metadata.license.type } else { "url" }

    return [pscustomobject]@{
        Id = $PackageId
        Version = $Version
        Platforms = $Platforms
        License = $license
        LicenseType = $licenseType
        Copyright = [string]$metadata.copyright
        ProjectUrl = [string]$metadata.projectUrl
        IsBuildOnly = $BuildOnlyPackages.Contains($PackageId)
        Directory = $Directory
    }
}

$assets = Get-Content -LiteralPath $AssetsPath -Raw | ConvertFrom-Json -AsHashtable
$packagesRoot = ($assets.packageFolders.Keys | Select-Object -First 1)
if (-not $packagesRoot) {
    throw "Dossier global des packages NuGet introuvable dans $AssetsPath."
}

# Chaque package est associé aux plateformes sur lesquelles NuGet l'a réellement résolu.
$platformsByPackage = @{}
foreach ($target in $assets.targets.GetEnumerator()) {
    # La partie située avant le RID représente le framework ; un RID croisé ne doit pas changer la plateforme.
    $targetFramework = ($target.Key -split '/', 2)[0]
    $platform = if ($targetFramework -match '(?i)android') {
        "Android"
    }
    elseif ($targetFramework -match '(?i)windows') {
        "Windows"
    }
    else {
        $targetFramework
    }

    foreach ($libraryKey in $target.Value.Keys) {
        if (-not $assets.libraries.ContainsKey($libraryKey) -or
            $assets.libraries[$libraryKey].type -ne "package") {
            continue
        }

        if (-not $platformsByPackage.ContainsKey($libraryKey)) {
            $platformsByPackage[$libraryKey] = [System.Collections.Generic.HashSet[string]]::new()
        }

        [void]$platformsByPackage[$libraryKey].Add($platform)
    }
}

$packages = foreach ($libraryKey in ($platformsByPackage.Keys | Sort-Object)) {
    $parts = $libraryKey -split '/', 2
    $packageId = $parts[0]
    $version = $parts[1]
    $directory = Join-Path $packagesRoot (Join-Path $packageId.ToLowerInvariant() $version)
    if (-not (Test-Path -LiteralPath $directory)) {
        throw "Package restauré introuvable : $packageId $version ($directory)."
    }

    $package = Get-PackageMetadata `
        -PackageId $packageId `
        -Version $version `
        -Directory $directory `
        -Platforms ($platformsByPackage[$libraryKey] | Sort-Object)

    Add-LegalFilesFromDirectory `
        -Directory $directory `
        -SourcePrefix "$packageId $version"

    $package
}

# La publication Windows autonome embarque le runtime .NET en plus des packages du graphe NuGet.
$runtimeVersion = ($packages | Where-Object Id -eq "Microsoft.NET.ILLink.Tasks" |
    Select-Object -First 1 -ExpandProperty Version)
if ($runtimeVersion) {
    $windowsRuntimeDirectory = Join-Path $packagesRoot "microsoft.netcore.app.runtime.win-x64\$runtimeVersion"
    Add-LegalFilesFromDirectory `
        -Directory $windowsRuntimeDirectory `
        -SourcePrefix ".NET Runtime $runtimeVersion - Windows self-contained"

    $dotnetRoot = Split-Path (Get-Command dotnet).Source -Parent
    $androidRuntimeDirectory = Join-Path $dotnetRoot "packs\Microsoft.NETCore.App.Runtime.Mono.android-arm64\$runtimeVersion"
    Add-LegalFilesFromDirectory `
        -Directory $androidRuntimeDirectory `
        -SourcePrefix ".NET Runtime Mono $runtimeVersion - Android"
}

# Le package Win2D 1.3.2 ne contient pas son fichier de licence, mais son dépôt officiel publie cette licence MIT.
$win2DLicense = @"
Win2D

Copyright (c) Microsoft Corporation. All rights reserved.

MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
"@
Add-LegalText `
    -Source "Microsoft.Graphics.Win2D - https://github.com/microsoft/Win2D/blob/winappsdk/main/LICENSE.txt" `
    -Text $win2DLicense

$builder = [System.Text.StringBuilder]::new()
[void]$builder.AppendLine("MATHS SOUS LE CAPOT")
[void]$builder.AppendLine("THIRD-PARTY SOFTWARE NOTICES AND INFORMATION")
[void]$builder.AppendLine()
[void]$builder.AppendLine("This file lists third-party software resolved for the Windows and Android application.")
[void]$builder.AppendLine("The notices reproduced below come from the exact restored packages whenever available.")
[void]$builder.AppendLine("Do not translate or localize the upstream license and notice texts.")
[void]$builder.AppendLine()
[void]$builder.AppendLine("Maths Sous le capot is licensed separately under LICENSE.md.")
[void]$builder.AppendLine("Third-party components remain governed by their own terms.")
[void]$builder.AppendLine()
[void]$builder.AppendLine("======================================================================")
[void]$builder.AppendLine("RUNTIME PACKAGE INVENTORY")
[void]$builder.AppendLine("======================================================================")
[void]$builder.AppendLine()

foreach ($package in ($packages | Where-Object { -not $_.IsBuildOnly } | Sort-Object Id)) {
    $platforms = $package.Platforms -join ", "
    $license = if ($package.License) { $package.License } else { "not declared in package metadata" }
    [void]$builder.AppendLine("- $($package.Id) $($package.Version) [$platforms] - License: $license")
    if ($package.Copyright) {
        [void]$builder.AppendLine("  Copyright: $($package.Copyright)")
    }
    if ($package.ProjectUrl) {
        [void]$builder.AppendLine("  Project: $($package.ProjectUrl)")
    }
}

[void]$builder.AppendLine()
[void]$builder.AppendLine("Additional runtime components:")
[void]$builder.AppendLine("- .NET Runtime $runtimeVersion, embedded by self-contained Windows and Android builds.")
[void]$builder.AppendLine("- Microsoft Windows App SDK and Microsoft Edge WebView2 runtime components on Windows.")
[void]$builder.AppendLine("- AndroidX, Google Material, Glide, Gson, Tink, Kotlin and related Java libraries on Android.")
[void]$builder.AppendLine()
[void]$builder.AppendLine("======================================================================")
[void]$builder.AppendLine("BUILD-TIME PACKAGES NOT SHIPPED AS APPLICATION LIBRARIES")
[void]$builder.AppendLine("======================================================================")
[void]$builder.AppendLine()

foreach ($package in ($packages | Where-Object IsBuildOnly | Sort-Object Id)) {
    $platforms = $package.Platforms -join ", "
    $license = if ($package.License) { $package.License } else { "not declared in package metadata" }
    [void]$builder.AppendLine("- $($package.Id) $($package.Version) [$platforms] - License: $license")
}

[void]$builder.AppendLine()
[void]$builder.AppendLine("======================================================================")
[void]$builder.AppendLine("LICENSES AND UPSTREAM NOTICES")
[void]$builder.AppendLine("======================================================================")

$noticeNumber = 0
foreach ($entry in ($LegalTexts.GetEnumerator() | Sort-Object { $_.Value.Sources[0] })) {
    $noticeNumber++
    [void]$builder.AppendLine()
    [void]$builder.AppendLine("----------------------------------------------------------------------")
    [void]$builder.AppendLine("NOTICE $noticeNumber")
    [void]$builder.AppendLine("Source copies with identical content: $($entry.Value.Sources.Count)")
    foreach ($source in ($entry.Value.Sources | Sort-Object)) {
        [void]$builder.AppendLine("- $source")
    }
    [void]$builder.AppendLine("----------------------------------------------------------------------")
    [void]$builder.AppendLine()
    [void]$builder.AppendLine($entry.Value.Text)
}

$outputDirectory = Split-Path $ResolvedOutputPath -Parent
if (-not (Test-Path -LiteralPath $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

[System.IO.File]::WriteAllText(
    $ResolvedOutputPath,
    $builder.ToString().TrimEnd() + "`n",
    [System.Text.UTF8Encoding]::new($false))

Write-Host "Notices tierces générées : $ResolvedOutputPath"
Write-Host "Packages inventoriés : $($packages.Count)"
Write-Host "Textes juridiques uniques : $($LegalTexts.Count)"
