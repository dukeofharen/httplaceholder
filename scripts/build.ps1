$ErrorActionPreference = 'Stop'

$rootFolder = Join-Path -Path $PSScriptRoot ".."
$srcFolder = Join-Path -Path $rootFolder "src"
$mainProjectFile = Join-Path $srcFolder "Placeholder\Placeholder.csproj"
$solutionFile = Join-Path -Path $srcFolder "Placeholder.sln"

$nsisPath = "C:\Program Files (x86)\NSIS\Bin"

. "$PSScriptRoot\functions.ps1"

# Updating path variable
Write-Host "Updating path variable"
$env:PATH = "$env:PATH;$nsisPath"

# Remove all bin and obj folders
Write-Host "Cleaning the solution"
Get-ChildItem $srcFolder -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }

# Perform a debug build
& dotnet build $solutionFile /p:DebugType=Full

# Run unit tests
$unitTestProjects = Get-ChildItem -Path $srcFolder -Filter *.Tests.csproj -Recurse
Write-Host "Running unit tests"
foreach($unitTest in $unitTestProjects)
{
    Write-Host $unitTest

    & dotnet restore $unitTest.FullName
    Assert-Cmd-Ok

    & dotnet test $unitTest.FullName
    Assert-Cmd-Ok
}

# Release package build
Write-Host "Building a release package"

& dotnet restore $mainProjectFile
Assert-Cmd-Ok

# Reading version number
Write-Host "Reading version from $mainProjectFile"
[xml]$csproj = Get-Content $mainProjectFile
$propertyGroupNode = $csproj.SelectSingleNode("/Project/PropertyGroup[1]")
$version = [version]$propertyGroupNode.Version
Write-Host "Found version $version"

. "$PSScriptRoot\build_windows.ps1" -srcFolder $srcFolder