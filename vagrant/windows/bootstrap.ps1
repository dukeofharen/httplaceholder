# Setup common vars and functions.
function Give-Full-Rights($path, $user)
{
    Write-Host "Giving full rights to user $user on path $path"
    $acl = Get-Acl $path
    if (Test-Path -Path $path -PathType Leaf)
    {
        $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($user, "FullControl", "None", "None", "Allow")
    }
    else
    {
        $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($user, "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    }

    $acl.SetAccessRule($rule)
    Set-Acl $path $acl
}

function Add-Env-Variable($envVarsElement, $name, $value)
{
    $element = $envVarsElement.OwnerDocument.CreateElement("environmentVariable")

    $nameAttr = $envVarsElement.OwnerDocument.CreateAttribute("name")
    $nameAttr.Value = $name

    $valueAttr = $envVarsElement.OwnerDocument.CreateAttribute("value")
    $valueAttr.Value = $value

    $element.Attributes.Append($nameAttr);
    $element.Attributes.Append($valueAttr);

    $envVarsElement.AppendChild($element)
}

$iisUser = "IIS_IUSRS"
$tmpFolder = $env:TEMP
$iisSite = "HttPlaceholder"
$httplRootPath = "C:\httplaceholder"
$distPath = Join-Path $httplRootPath "dist\httplaceholder_win-x64.zip"
$windowsVagrantPath = Join-Path $httplRootPath "vagrant\windows"

if (!(Test-Path $distPath))
{
    Write-Host "Path $distPath not found."
    Exit 1
}

# First, install IIS (help from https://weblog.west-wind.com/posts/2017/may/25/automating-iis-feature-installation-with-powershell).
Write-Host "Installing Windows Features for IIS"
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment

# Install dependencies
& choco install dotnet-windowshosting --version=7.0.10 -y

# Stop HttPlaceholder IIS site if it exists.
Stop-Website -Name $iisSite -ErrorAction SilentlyContinue

# Creating HttPlaceholder folder
$installPath = "C:\bin\httplaceholder"
if (Test-Path $installPath)
{
    Write-Host "Deleting path $installPath"
    Remove-Item $installPath -Recurse -Force
}

Write-Host "Creating folder $installPath"
New-Item -ItemType Directory $installPath

# Copy dist to temp folder
$tempPath = Join-Path $env:TEMP "httplaceholder_win-x64.zip"
Write-Host "Copying $distPath to $tempPath"
Copy-Item $distPath $tempPath

# Extract binaries to installation path
Write-Host "Extracting file $distPath to $installPath"
Expand-Archive $distPath -DestinationPath $installPath

# Create logs folder and set rights.
$logsPath = "C:\logs\httplaceholder"
if (!(Test-Path $logsPath))
{
    Write-Host "Creating directory $logsPath"
    New-Item -ItemType Directory $logsPath
}

Give-Full-Rights -path $logsPath -user $iisUser

# Create HttPlaceholder data folder and set rights.
$dataPath = "C:\HttPlaceholderData"
if (!(Test-Path $dataPath))
{
    Write-Host "Creating folder $dataPath"
    New-Item -ItemType Directory $dataPath
    Give-Full-Rights -path $dataPath -user $iisUser
}

# Move config file to correct location
Copy-Item (Join-Path $windowsVagrantPath "config.json") $dataPath

# Updating web.config of HttPlaceholder with the correct variables.
Move-Item (Join-Path $installPath "_web.config") (Join-Path $installPath "web.config")
$webConfigPath = Join-Path -Path $installPath "web.config"
Write-Host "Updating file $webConfigPath"
[xml]$webConfig = Get-Content $webConfigPath
$aspNetCore = $webConfig.configuration.location.'system.webServer'.aspNetCore
$aspNetCore.stdoutLogEnabled = "true"
$aspNetCore.stdoutLogFile = $logsPath

$environmentVariables = $webConfig.CreateElement("environmentVariables")
Add-Env-Variable -envVarsElement $environmentVariables -name "ASPNETCORE_ENVIRONMENT" -value "Production"
Add-Env-Variable -envVarsElement $environmentVariables -name "configjsonlocation" -value "$dataPath\config.json"

$aspNetCore.AppendChild($environmentVariables)

$webConfig.Save($webConfigPath)

# Remove default site
Remove-Website -Name "Default Web Site" -ErrorAction SilentlyContinue

# Create a self signed certificate
$store = "cert:\LocalMachine\My"
$cn = "CN=localhost"
$certs = Get-ChildItem $store | where { $_.Subject -eq $cn }
$cert = $null
if ($certs.Length -ge 1)
{
    Write-Host "Certificate with CN $cn already exists"
    $cert = $certs[0]
}
else
{
    Write-Host "Creating new self-signed certificate for CN $cn"
    New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation $store
    $certs = Get-ChildItem $store | where { $_.Subject -eq $cn }
    $cert = $certs[0]
}

# Add site to IIS.
$httpPort = 80
$httpsPort = 443
$httpsProto = "https"
$sitePath = "C:\bin\httplaceholder"

Write-Host "Adding site to IIS."
New-WebSite -Name $iisSite -Port $httpPort -PhysicalPath $sitePath -Force
$binding = Get-WebBinding -Name $iisSite -Port $httpsPort -Protocol $httpsProto
if ($binding -eq $null) {
    New-WebBinding -Name $iisSite -Port $httpsPort -Protocol "https" -SslFlags 0
    $binding = Get-WebBinding -Name $iisSite -Port $httpsPort -Protocol $httpsProto
}

$binding.AddSslCertificate($certs[0].Thumbprint, "my")
Start-Website -Name $iisSite -ErrorAction SilentlyContinue