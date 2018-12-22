Param(
    [Parameter(Mandatory = $True)]
    [string]$srcFolder
)

. "$PSScriptRoot\Functions.ps1"

# Find and .nupkg files
$nupkgs = Get-ChildItem -Path $srcFolder -Filter *.nupkg -Recurse
foreach($nupkg in $nupkgs)
{
    $nupkgPath = $nupkg.FullName
    Write-Host "Publishing NuGet package $nupkgPath"
    & dotnet nuget push $nupkgPath -k $env:nuget_api_key -s https://api.nuget.org/v3/index.json
    Assert-Cmd-Ok
}