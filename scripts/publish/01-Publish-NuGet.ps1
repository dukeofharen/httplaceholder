<#
A script for publishing all NuGet packages
#>
Param(
    [Parameter(Mandatory = $True)]
    [string]$distFolder
)

# Find and .nupkg files
$nupkgs = Get-ChildItem -Path $distFolder -Filter *.nupkg -Recurse
foreach ($nupkg in $nupkgs) {
    $nupkgPath = $nupkg.FullName
    Write-Host "Publishing NuGet package $nupkgPath"
    & dotnet nuget push $nupkgPath -k $env:nuget_api_key -s https://api.nuget.org/v3/index.json
    Assert-Cmd-Ok
}