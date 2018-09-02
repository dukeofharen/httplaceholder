. "$PSScriptRoot\functions.ps1"

$ErrorActionPreference = 'Stop'

$rootFolder = "$PSScriptRoot\..\gui"
$distFolder = "$rootFolder\dist"

# Run npm install
& cmd /c "cd $rootFolder && npm install"
Assert-Cmd-Ok

# Run Vue release build
& cmd /c "cd $rootFolder && npm run build"
Assert-Cmd-Ok