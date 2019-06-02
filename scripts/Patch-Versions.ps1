$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\functions.ps1"

Write-Host "Found version number $($env:versionString)"

$rootFolder = Join-Path -Path $PSScriptRoot ".."
$srcFolder = Join-Path -Path $rootFolder "src"

$csprojFiles = Get-ChildItem -Path $srcFolder -Filter *.csproj -Recurse
foreach ($csprojFile in $csprojFiles) {
    Write-Host "Parsing .csproj file $($csprojFile.FullName)"
    [xml]$csprojContents = Get-Content $csprojFile.FullName
    $propertyGroupNode = $csprojContents.SelectSingleNode("/Project/PropertyGroup[1]")
    if ($null -ne $propertyGroupNode.Version) {
        $propertyGroupNode.Version = $env:versionString
        $propertyGroupNode.AssemblyVersion = $env:versionString
        $propertyGroupNode.FileVersion = $env:versionString
        $csprojContents.Save($csprojFile.FullName)
    }
}