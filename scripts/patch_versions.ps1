$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\functions.ps1"

$rootFolder = Join-Path -Path $PSScriptRoot ".."
$srcFolder = Join-Path -Path $rootFolder "src"
$csprojPath = Join-Path -Path $srcFolder "HttPlaceholder\HttPlaceholder.csproj"

Write-Host "Reading file '$csprojPath'"
[xml]$mainCsproj = Get-Content $csprojPath
$propertyGroupNode = $mainCsproj.SelectSingleNode("/Project/PropertyGroup[1]")
$version = [version]$propertyGroupNode.Version

Write-Host "Current version number: '$version'"

$versionString = "{0}.{1}.{2}.{3}" -f $version.Major, $version.Minor, $version.Build, $env:APPVEYOR_BUILD_NUMBER

Write-Host "New version number: '$versionString'"

$env:versionString = $versionString

$csprojFiles = Get-ChildItem -Path $srcFolder -Filter *.csproj -Recurse
foreach($csprojFile in $csprojFiles)
{
    Write-Host "Parsing .csproj file $($csprojFile.FullName)"
    [xml]$csprojContents = Get-Content $csprojFile.FullName
    $propertyGroupNode = $csprojContents.SelectSingleNode("/Project/PropertyGroup[1]")
    $propertyGroupNode.Version = $versionString
    $propertyGroupNode.AssemblyVersion = $versionString
    $propertyGroupNode.FileVersion = $versionString
    $csprojContents.Save($csprojFile.FullName)
}