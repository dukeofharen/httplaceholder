<#
A script for patching the version of all .csproj files.
#>

Param(
    [Parameter(Mandatory = $true)][string]$srcFolder
)

Write-Host "Found version number $($env:RELEASE_VERSION)"
$csprojFiles = Get-ChildItem -Path $srcFolder -Filter *.csproj -Recurse
foreach ($csprojFile in $csprojFiles) {
    Write-Host "Parsing .csproj file $($csprojFile.FullName)"
    [xml]$csprojContents = Get-Content $csprojFile.FullName
    $propertyGroupNode = $csprojContents.SelectSingleNode("/Project/PropertyGroup[1]")
    if ($null -ne $propertyGroupNode.Version) {
        $propertyGroupNode.Version = $env:RELEASE_VERSION
        $propertyGroupNode.AssemblyVersion = $env:RELEASE_VERSION
        $propertyGroupNode.FileVersion = $env:RELEASE_VERSION
        $csprojContents.Save($csprojFile.FullName)
    }
}