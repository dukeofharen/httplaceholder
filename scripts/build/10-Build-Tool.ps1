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
if ($IsLinux) {
    # For some reason, the MySqlConnector NuGet package has last write time set to "1 january 1980", which NuGet doesn't like.
    # For now, set the last write time of this package to now to make it work.
    Write-Host "Updating MySqlConnector last write time"
    & touch "$($env:HOME)/.nuget/packages/mysqlconnector/0.57.0/lib/netcoreapp3.0/MySqlConnector.dll"
    Assert-Cmd-Ok
}

& dotnet pack $mainProjectFile -c Tool
Assert-Cmd-Ok

$binFolder = "$srcFolder/HttPlaceholder/bin/Tool"
Copy-Item -Path "$binFolder/*.nupkg" -Destination $distFolder