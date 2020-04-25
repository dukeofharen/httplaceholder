Param(
    $runUnitTests = $true
)

$rootFolder = "$PSScriptRoot/../.."
$distFolder = "$rootFolder/dist"
$srcFolder = "$rootFolder/src"
$mainProjectFile = "$srcFolder/HttPlaceholder/HttPlaceholder.csproj"
$solutionFile = "$srcFolder/HttPlaceholder.sln"
if (Test-Path $distFolder) {
    Remove-Item $distFolder -Confirm:$false -Recurse -Force
}

New-Item -ItemType Directory $distFolder

# & dotnet restore $solutionFile
# & dotnet build $solutionFile /p:DebugType=Full

. "$PSScriptRoot/../Functions.ps1"
. "$PSScriptRoot/01-Set-Vars.ps1"
#. "$PSScriptRoot/02-Patch-Csproj-Versions.ps1" -srcFolder $srcFolder
#if ($runUnitTests) {
#    . "$PSScriptRoot/03-Run-Tests.ps1" -srcFolder $srcFolder
#}
#
#. "$PSScriptRoot/04-Build-Gui.ps1" -srcFolder $srcFolder -rootFolder $rootFolder
#. "$PSScriptRoot/05-Build-Windows.ps1" `
#    -srcFolder $srcFolder `
#    -mainProjectFile $mainProjectFile `
#    -distFolder $distFolder `
#    -rootFolder $rootFolder
#. "$PSScriptRoot\06-Build-Linux.ps1" `
#    -srcFolder $srcFolder `
#    -mainProjectFile $mainProjectFile `
#    -distFolder $distFolder `
#    -rootFolder $rootFolder
#. "$PSScriptRoot\07-Build-OsX.ps1" `
#    -srcFolder $srcFolder `
#    -mainProjectFile $mainProjectFile `
#    -distFolder $distFolder `
#    -rootFolder $rootFolder
#. "$PSScriptRoot\08-Create-OpenAPI-File.ps1" `
#    -srcFolder $srcFolder `
#    -distFolder $distFolder
#. "$PSScriptRoot\09-Build-NuGet.ps1" `
#    -srcFolder $srcFolder `
#    -distFolder $distFolder
#. "$PSScriptRoot\10-Build-Tool.ps1" `
#    -srcFolder $srcFolder `
#    -mainProjectFile $mainProjectFile `
#    -distFolder $distFolder
. "$PSScriptRoot\11-Build-Publish-Docker.ps1"