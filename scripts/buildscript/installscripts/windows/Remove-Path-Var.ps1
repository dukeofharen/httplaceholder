# Make sure the install folder is removed from the path variable after deinstallation.
Param(
    [Parameter(Mandatory=$True)]
    [string]$installFolder
)

$environmentLocation = "hklm:\SYSTEM\CurrentControlSet\Control\Session Manager\Environment"
$environment = Get-ItemProperty $environmentLocation
$pathVar = $environment.Path
$pathVar = $pathVar.Replace(";$installFolder", "")
New-ItemProperty -Path $environmentLocation -Name "Path" -Value $pathVar -Force