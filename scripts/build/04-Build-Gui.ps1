<#
A script for building the user interface of HttPlaceholder and copying the files to the HttPlaceholder project.
#>
Param(
    [Parameter(Mandatory = $True)][string]$srcFolder,
    [Parameter(Mandatory = $True)][string]$rootFolder
)

$guiProjectFolder = "$rootFolder/gui"
$guiDistFolder = "$guiProjectFolder/dist"
$guiDestinationFolder = "$srcFolder/HttPlaceholder/gui"

# Build Vue project
Set-Location $guiProjectFolder

& npm install
Assert-Cmd-Ok

& npm run build
Assert-Cmd-Ok

# Moving GUI dist folder to bin path
Copy-item "$guiDistFolder/*" $guiDestinationFolder -Recurse -Container -Force