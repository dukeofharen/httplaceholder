# Determine deployment variables
[string]$description = $env:APPVEYOR_REPO_COMMIT_MESSAGE
Write-Host "Deployment description: $description"
$env:description = $description

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
    $env:draft = "false"
    $env:prerelease = "true"
} else {
    $env:draft = "true"
    $env:prerelease = "false"
}

# Determine version
$date = Get-Date
$versionString = "{0}.{1}.{2}.{3}" -f $date.Year, $date.Month, $date.Day, $env:APPVEYOR_BUILD_NUMBER
Write-Host "New version: $versionString"
$env:versionString = $versionString
$releaseName = "v$versionString"
if ($beta) {
    $releaseName = "$releaseName-beta"
}

Write-Host "Release name: $releaseName"
$env:releaseName = $releaseName