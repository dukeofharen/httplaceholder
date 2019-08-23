<#
This script determines various build and release time variables.
#>

$env:RELEASE_DESCRIPTION = $env:APPVEYOR_REPO_COMMIT_MESSAGE

$branch = $env:APPVEYOR_REPO_BRANCH
Write-Host "Branch: $branch"
$beta = $false
if ($branch -eq "dev") {
    Write-Host "This build is a beta build."
    $beta = $true
}
elseif ($branch -eq "master") {
    Write-Host "This build is a production build."
}

if ($beta) {
    $env:RELEASE_IS_PRERELEASE = "true"
} else {
    $env:RELEASE_IS_PRERELEASE = "false"
}

# Determine version
$date = Get-Date
$versionString = "{0}.{1}.{2}.{3}" -f $date.Year, $date.Month, $date.Day, $env:APPVEYOR_BUILD_NUMBER
Write-Host "New version: $versionString"
$env:RELEASE_VERSION = $versionString
$releaseName = "v$versionString"
if ($beta) {
    $releaseName = "$releaseName-beta"
}

Write-Host "Release name: $releaseName"
$env:RELEASE_NAME = $releaseName