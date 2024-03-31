# This is added to ensure that the whole script is downloaded before running it.
if ($True)
{
    $global:ProgressPreference = 'SilentlyContinue'
    $ErrorActionPreference = "Stop"
    $tempDir = [System.IO.Path]::GetTempPath()

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

    Write-Host "Attempting to download and install HttPlaceholder"

    $releasesUrl = "https://api.github.com/repos/dukeofharen/httplaceholder/releases"
    $releasesJson = (Invoke-WebRequest $releasesUrl -UseBasicParsing).Content
    $parsedJson = ConvertFrom-Json $releasesJson
    $tag = $parsedJson[0].tag_name
    Write-Host "Latest version is $tag"

    $release = $parsedJson[0].assets | Where-Object { $_.name -match "httplaceholder_win" }
    $downloadUrl = $release.browser_download_url
    $downloadPath = Join-Path $tempDir "httplaceholder_win_x64.zip"
    Write-Host "Downloading binaries from $downloadUrl to $downloadPath"
    Invoke-WebRequest -Uri $downloadUrl -UseBasicParsing -OutFile $downloadPath

    $mainDrive = (Get-Item $env:windir).PSDrive.Name
    $installLocation = "$( $mainDrive ):\httplaceholder"
    if (Test-Path $installLocation)
    {
        Write-Host "Removing directory $installLocation"
        Remove-Item -Path $installLocation -Recurse -Force
    }

    Write-Host "Extracting archive $downloadPath to $installLocation"
    Expand-Archive -Path $downloadPath -DestinationPath $installLocation

    $pathVar = [Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::Machine)
    if (!($pathVar -match [regex]::Escape($installLocation)))
    {
        Write-Host "Adding path $installLocation to the path variable"
        $newPath = "$pathVar;$installLocation"
        [Environment]::SetEnvironmentVariable("Path", $newPath, [System.EnvironmentVariableTarget]::Machine)
    }

    Write-Host "HttPlaceholder version $tag is installed. Start application by running 'httplaceholder'."
}