<#
A script to build the Linux version of HttPlaceholder and create several distributables.
#>

Param(
    [Parameter(Mandatory = $True)][string]$rootFolder,
    [Parameter(Mandatory = $True)][string]$srcFolder,
    [Parameter(Mandatory = $True)][string]$distFolder,
    [Parameter(Mandatory = $True)][string]$mainProjectFile
)

$binDir = Join-Path $srcFolder "HttPlaceholder\bin\release\netcoreapp3.1\linux-x64\publish"
$docsFolder = "$rootFolder/docs"
$installScriptsPath = "$PSScriptRoot/installscripts/linux"

# Patching main .csproj file
[xml]$csproj = Get-Content $mainProjectFile
$propertyGroupNode = $csproj.SelectSingleNode("/Project/PropertyGroup[1]")
$propertyGroupNode.PackAsTool = "false"
$csproj.Save($mainProjectFile)

# Cleaning publish dir
Remove-Item $binDir -Force -Confirm:$false -Recurse -ErrorAction Ignore

# Create Linux package
Write-Host "Packing up for Linux" -ForegroundColor Green
& dotnet publish $mainProjectFile --configuration=release --runtime=linux-x64 /p:PublishTrimmed=true
Assert-Cmd-Ok

# Moving install scripts
Copy-Item "$installScriptsPath/**" $binDir -Recurse

# Moving docs folder to bin path
Copy-Item $docsFolder (Join-Path $binDir "docs") -Recurse -Container

# Creating .tar.gz file of binaries.
if (!$IsLinux) {
    & "C:\Program Files\7-Zip\7z.exe" a -ttar "$distFolder\httplaceholder_linux-x64.tar" "$binDir\**"
    Assert-Cmd-Ok

    & "C:\Program Files\7-Zip\7z.exe" a -tgzip "$distFolder\httplaceholder_linux-x64.tar.gz" "$distFolder\httplaceholder_linux-x64.tar"
    Assert-Cmd-Ok

    Remove-Item "$distFolder\httplaceholder_linux-x64.tar"
}
else {
    & tar -czvf $distFolder/httplaceholder_linux-x64.tar.gz -C $binDir .
    Assert-Cmd-Ok -alsoAcceptable 1
}