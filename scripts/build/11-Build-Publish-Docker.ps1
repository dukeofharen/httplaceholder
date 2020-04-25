<#
A script for building and publishing packing HttPlaceholder Docker image. 
#>
$currentDir = $PSScriptRoot
$version = $env:RELEASE_VERSION
$repoName = "dukeofharen/httplaceholder"

Set-Location "$currentDir/../.."
Write-Host "$($env:docker_password)" | docker login -u "$($env:docker_username)" --password-stdin
docker build -t "$($repoName):$($version)" .
docker tag "$($repoName):$($version)" "$($repoName):latest"
docker push "$($repoName):$($version)"
docker push "$($repoName):latest"
Set-Location $currentDir