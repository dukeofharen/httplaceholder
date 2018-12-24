Param(
    [Parameter(Mandatory = $True)]
    [string]$srcFolder
)

. "$PSScriptRoot\Functions.ps1"

$rootFolder = "$PSScriptRoot\..\gui"
$guiDistFolder = "$rootFolder\dist"
$guiDestinationFolder = "$srcFolder\HttPlaceholder\gui"

# Run npm install
& cmd /c "cd $rootFolder && npm install"
Assert-Cmd-Ok

# Run Vue release build
& cmd /c "cd $rootFolder && npm run build"
Assert-Cmd-Ok

# Moving GUI dist folder to bin path
Copy-item "$guiDistFolder\*" $guiDestinationFolder -Recurse -Container -Force