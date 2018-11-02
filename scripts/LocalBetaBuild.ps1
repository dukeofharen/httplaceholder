$env:APPVEYOR_BUILD_NUMBER = "1234"
$env:APPVEYOR_REPO_COMMIT_MESSAGE = "[beta] New version"
$env:APPVEYOR_REPO_BRANCH = "dev"

. "$PSScriptRoot\Determine-Variables.ps1"
. "$PSScriptRoot\Patch-Versions.ps1"
. "$PSScriptRoot\Build.ps1"