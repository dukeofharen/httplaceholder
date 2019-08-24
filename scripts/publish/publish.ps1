. "$PSScriptRoot/../Functions.ps1"

$distFolder = "$PSScriptRoot/../../dist"
. "$PSScriptRoot/01-Publish-NuGet.ps1" -distFolder $distFolder