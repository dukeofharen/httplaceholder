<#
A script for building and packing the HttPlaceholder .NET global tool NuGet package.
#>
Param(
    [Parameter(Mandatory = $True)][string]$srcFolder,
    [Parameter(Mandatory = $True)][string]$mainProjectFile,
    [Parameter(Mandatory = $True)][string]$distFolder
)

Write-Host "Building dotnet tool"

# Patching main .csproj file
[xml]$csproj = Get-Content $mainProjectFile
$propertyGroupNode = $csproj.SelectSingleNode("/Project/PropertyGroup[1]")
$propertyGroupNode.PackAsTool = "true"
$csproj.Save($mainProjectFile)

# Building dotnet tool
& dotnet pack $mainProjectFile -c Tool
Assert-Cmd-Ok

$binFolder = "$srcFolder/HttPlaceholder/bin/Tool"
Copy-Item -Path "$binFolder/*.nupkg" -Destination $distFolder