Param(
    [Parameter(Mandatory = $True)]
    [string]$srcFolder,

    [Parameter(Mandatory = $True)]
    [string]$mainProjectFile
)

. "$PSScriptRoot\Functions.ps1"

$binDir = Join-Path $srcFolder "HttPlaceholder\bin\release\netcoreapp2.2\osx-x64\publish"
$docsFolder = Join-Path $srcFolder "..\docs"

# Create Linux package
Write-Host "Packing up for Linux" -ForegroundColor Green
& dotnet publish $mainProjectFile --configuration=release --runtime=osx-x64
Assert-Cmd-Ok

# Moving docs folder to bin path
Copy-Item $docsFolder (Join-Path $binDir "docs") -Recurse -Container

# Creating ZIP file
Compress-Archive -Path $binDir -DestinationPath "$binDir\..\..\httplaceholder_osx-x64.zip"