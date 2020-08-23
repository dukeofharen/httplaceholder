Param(
    [Parameter(Mandatory = $true)][string]$apiKey,
    [Parameter(Mandatory = $true)][string]$distFolder,
    [Parameter(Mandatory = $true)][string]$version,
    [Parameter(Mandatory = $true)][string]$commitHash
)

if (!(Test-Path $distFolder))
{
    Write-Error "Dist path not found: $distFolder"
    Exit 1
}

$owner = "dukeofharen"
$repo = "httplaceholder"
$userAgent = "$owner/$repo"
$accept = "application/vnd.github.v3+json"

# Create GitHub release
$tagName = "v$version"
$release = @{ tag_name = $tagName; target_commitish = $commitHash; name = $tagName; body = ""; draft = $true }
$releaseJson = ConvertTo-Json $release
$createReleaseResponse = Invoke-WebRequest -Uri "https://api.github.com/repos/$owner/$repo/releases" `
    -Method Post `
    -Body $releaseJson `
    -Headers @{ "Authorization" = "token $apiKey"; "User-Agent" = $userAgent; "Accept" = $accept } `
    -UseBasicParsing
$createReleaseJson = ConvertFrom-Json $createReleaseResponse.Content
$releaseId = $createReleaseJson.id
$uploadUrl = "https://uploads.github.com/repos/$owner/$repo/releases/$releaseId/assets"

# Upload release files
$files = Get-ChildItem $distFolder
foreach ($file in $files)
{
    $filename = [System.IO.Path]::GetFileName($file)
    $fullUploadUrl = "$($uploadUrl)?name=$filename"
    Write-Host "Uploading file $file to GitHub Releases on URL $fullUploadUrl"
    Invoke-WebRequest -Uri $fullUploadUrl `
        -Method Post `
        -Headers @{ "Authorization" = "token $apiKey"; "User-Agent" = $userAgent; "Accept" = $accept; "Content-Type" = "application/octet-stream" } `
        -UseBasicParsing `
        -InFile $file
}