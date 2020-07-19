# Setup common vars and functions.
function Give-Full-Rights($path, $user) {
    Write-Host "Giving full rights to user $user on path $path"
    $acl = Get-Acl $path
    if (Test-Path -Path $path -PathType Leaf) {
        $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($user, "FullControl", "None", "None", "Allow")
    }
    else {
        $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($user, "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    }

    $acl.SetAccessRule($rule)
    Set-Acl $path $acl
}

function Add-Env-Variable($envVarsElement, $name, $value) {
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

# First, install IIS (help from https://weblog.west-wind.com/posts/2017/may/25/automating-iis-feature-installation-with-powershell).
Write-Host "Installing Windows Features for IIS"
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment

# Install dependencies
& choco install dotnetcore-windowshosting -y
& choco install dotnetcore-sdk -y
& choco install nodejs -y

# Set correct path
$env:PATH = "C:\Program Files\nodejs;C:\Program Files\dotnet;$($env:PATH)"

# Stop HttPlaceholder IIS site if it exists.
Stop-Website -Name HttPlaceholder -ErrorAction SilentlyContinue

# Creating HttPlaceholder folder
$installPath = "C:\bin\httplaceholder"
if (Test-Path $installPath) {
    Write-Host "Deleting path $installPath"
    Remove-Item $installPath -Recurse -Force
}

Write-Host "Creating folder $installPath"
New-Item -ItemType Directory $installPath

# Copy source code to profile folder
$sourcePath = "$($env:USERPROFILE)\httplaceholder"
if (Test-Path $sourcePath) {
    Write-Host "Deleting path $sourcePath"
    Remove-Item $sourcePath -Recurse -Force
}

& robocopy C:\httplaceholder $sourcePath /E /Z /ZB /R:5 /W:5 /TBD /NP /V /XD bin obj node_modules

# Build HttPlaceholder
$projectPath = "$sourcePath\src\HttPlaceholder\HttPlaceholder.csproj"
& dotnet publish $projectPath -c Release --runtime win-x64 -o $installPath

# Build HttPlaceholder GUI
$guiProjectPath = "$sourcePath\gui"
cd $guiProjectPath
& npm install
& npm run build
Copy-item "$guiProjectPath/dist/*" "$installPath/gui" -Recurse -Container -Force

# Create logs folder and set rights.
$logsPath = "C:\logs\httplaceholder"
if (!(Test-Path $logsPath)) {
    Write-Host "Creating directory $logsPath"
    New-Item -ItemType Directory $logsPath   
}

Give-Full-Rights -path $logsPath -user $iisUser

# Create HttPlaceholder data folder and set rights.
$dataPath = "C:\HttPlaceholderData"
if (!(Test-Path $dataPath)) {
    Write-Host "Creating folder $dataPath"
    New-Item -ItemType Directory $dataPath
    Give-Full-Rights -path $dataPath -user $iisUser
}

# Move config file to correct location
Copy-Item "C:\httplaceholder\vagrant\windows\config.json" $dataPath

# Updating web.config of HttPlaceholder with the correct variables.
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

# Add site to IIS.
Write-Host "Adding site to IIS."
New-WebSite -Name HttPlaceholder -Port 80 -PhysicalPath "C:\bin\httplaceholder" -Force
Start-Website -Name HttPlaceholder -ErrorAction SilentlyContinue