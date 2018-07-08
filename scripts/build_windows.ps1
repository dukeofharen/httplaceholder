Param(
    [Parameter(Mandatory = $True)]
    [string]$srcFolder
)

$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\functions.ps1"

$nsiPath = Join-Path $PSScriptRoot "httplaceholder.nsi"
$binDir = Join-Path $srcFolder "HttPlaceholder\bin\release\netcoreapp2.1\win10-x64\publish"
$docsFolder = Join-Path $srcFolder "..\docs"
$installScriptsPath = Join-Path -Path $PSScriptRoot "installscripts\windows"

# Create Windows package
Write-Host "Packing up for Windows" -ForegroundColor Green
& dotnet publish $mainProjectFile --configuration=release --runtime=win10-x64
Assert-Cmd-Ok

# Moving install scripts for Windows
Copy-Item (Join-Path $installScriptsPath "**") $binDir -Recurse

# Moving docs folder to bin path
Copy-Item $docsFolder (Join-Path $binDir "docs") -Recurse -Container

# Making installer
$env:VersionMajor = $version.Major
$env:VersionMinor = $version.Minor
$env:VersionBuild = $version.Build
$env:BuildOutputBinDirectory = $binDir
$env:BuildOutputDirectory = $binDir
Write-Host "Building installer $nsiPath"
& makensis $nsiPath
Assert-Cmd-Ok