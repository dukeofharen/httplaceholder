Param(
    [Parameter(Mandatory = $True)]
    [string]$srcFolder
)

. "$PSScriptRoot\Functions.ps1"

$clientCsprojFile = "$srcFolder\HttPlaceholder.Client\HttPlaceholder.Client.csproj"

# Building NuGet packages
& dotnet pack $clientCsprojFile -c Release
Assert-Cmd-Ok