Param(
    [Parameter(Mandatory = $True)]
    [string]$srcFolder,

    [Parameter(Mandatory = $True)]
    [string]$mainProjectFile
)

. "$PSScriptRoot\Functions.ps1"

Write-Host "Building dotnet tool"

# Patching main .csproj file
[xml]$csproj = Get-Content $mainProjectFile
$propertyGroupNode = $csproj.SelectSingleNode("/Project/PropertyGroup[1]")
$propertyGroupNode.PackAsTool = "true"
$csproj.Save($mainProjectFile)

# Building dotnet tool
& dotnet pack $mainProjectFile -c Tool
Assert-Cmd-Ok