<#
A script to build the Windows version of HttPlaceholder and create several distributables.
#>

Param(
    [Parameter(Mandatory = $True)][string]$rootFolder,
    [Parameter(Mandatory = $True)][string]$srcFolder,
    [Parameter(Mandatory = $True)][string]$distFolder,
    [Parameter(Mandatory = $True)][string]$mainProjectFile
)

$nsiPath = "$PSScriptRoot/httplaceholder.nsi"
$binDir = "$srcFolder/HttPlaceholder/bin/release/netcoreapp3.0/win-x64/publish"
$docsFolder = "$rootFolder/docs"
$installScriptsPath = "$PSScriptRoot/installscripts/windows"

# Patching main .csproj file
[xml]$csproj = Get-Content $mainProjectFile
$propertyGroupNode = $csproj.SelectSingleNode("/Project/PropertyGroup[1]")
$propertyGroupNode.PackAsTool = "false"
$csproj.Save($mainProjectFile)

# Cleaning publish dir
Remove-Item $binDir -Force -Confirm:$false -Recurse -ErrorAction Ignore

# Create Windows package
Write-Host "Packing up for Windows" -ForegroundColor Green
& dotnet publish $mainProjectFile --configuration=release --runtime=win-x64 /p:PublishTrimmed=true
Assert-Cmd-Ok

# Moving install scripts for Windows
Copy-Item "$installScriptsPath/**" $binDir -Recurse

# Moving docs folder to bin path
Copy-Item $docsFolder "$binDir/docs" -Recurse -Container

# Renaming config files
Rename-Item -Path "$binDir/web.config" "_web.config" -Force

# Creating ZIP file
Compress-Archive -Path "$binDir/*" -DestinationPath "$distFolder/httplaceholder_win-x64.zip"

# Making installer
Write-Host "Building installer $nsiPath"
[version]$version = $env:RELEASE_VERSION
$env:VersionMajor = $version.Major
$env:VersionMinor = $version.Minor
$env:VersionBuild = $version.Build
if ($IsLinux) {
    $env:BuildOutputBinDirectory = $(winepath --windows "$binDir")
    $env:InstallerLocation = $(winepath --windows "$distFolder/httplaceholder_install.exe")
    & wine "C:\Program Files (x86)\NSIS\makensis.exe" $(winepath --windows "$nsiPath")
} else {
    $env:BuildOutputBinDirectory = $binDir
    $env:InstallerLocation = "$distFolder/httplaceholder_install.exe"
    & "C:\Program Files (x86)\NSIS\Bin\makensis.exe" $nsiPath
}

Assert-Cmd-Ok

