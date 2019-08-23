<#
A script to build the Mac OS X version of HttPlaceholder and create several distributables.
#>

Param(
    [Parameter(Mandatory = $True)][string]$rootFolder,
    [Parameter(Mandatory = $True)][string]$srcFolder,
    [Parameter(Mandatory = $True)][string]$distFolder,
    [Parameter(Mandatory = $True)][string]$mainProjectFile
)

$binDir = Join-Path $srcFolder "HttPlaceholder\bin\release\netcoreapp2.2\osx-x64\publish"
$docsFolder = "$rootFolder/docs"

# Patching main .csproj file
[xml]$csproj = Get-Content $mainProjectFile
$propertyGroupNode = $csproj.SelectSingleNode("/Project/PropertyGroup[1]")
$propertyGroupNode.PackAsTool = "false"
$csproj.Save($mainProjectFile)

# Cleaning publish dir
Remove-Item $binDir -Force -Confirm:$false -Recurse -ErrorAction Ignore

# Create OS X package
Write-Host "Packing up for OS X" -ForegroundColor Green
& dotnet publish $mainProjectFile --configuration=release --runtime=osx-x64
Assert-Cmd-Ok

# Moving docs folder to bin path
Copy-Item $docsFolder (Join-Path $binDir "docs") -Recurse -Container

# Creating .tar.gz file of binaries.
if (!$IsLinux) {
    & "C:\Program Files\7-Zip\7z.exe" a -ttar "$distFolder\httplaceholder_osx-x64.tar" "$binDir\**"
    Assert-Cmd-Ok

    & "C:\Program Files\7-Zip\7z.exe" a -tgzip "$distFolder\..\..\httplaceholder_osx-x64.tar.gz" "$distFolder\httplaceholder_osx-x64.tar"
    Assert-Cmd-Ok

    Remove-Item "$distFolder\httplaceholder_osx-x64.tar"
}
else {
    & tar -czvf $distFolder/httplaceholder_osx-x64.tar.gz -C $binDir .
    Assert-Cmd-Ok -alsoAcceptable 1
}