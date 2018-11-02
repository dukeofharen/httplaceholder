# Determine version
$date = Get-Date
$versionString = "{0}.{1}.{2}.{3}" -f $date.Year, $date.Month, $date.Day, $env:APPVEYOR_BUILD_NUMBER
Write-Host "New version: $versionString"
$env:versionString = $versionString

# Determine deployment variables
$description = $env:APPVEYOR_REPO_COMMIT_MESSAGE
Write-Host "Deployment description: $description"
$env:description = $description