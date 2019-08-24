<#
A script for running all unit tests in the .NET solution.
#>

Param(
    [Parameter(Mandatory = $true)][string]$srcFolder
)

# Run unit tests
$unitTestProjects = Get-ChildItem -Path $srcFolder -Filter *.Tests.csproj -Recurse
Write-Host "Running unit tests"
foreach ($unitTest in $unitTestProjects) {
    Write-Host $unitTest

    & dotnet restore $unitTest.FullName

    & dotnet test $unitTest.FullName
    Assert-Cmd-Ok
}