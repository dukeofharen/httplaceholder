# Make sure the install folder is added to the system path variable after install.
Param(
    [Parameter(Mandatory = $True)]
    [string]$installFolder
)

$environmentLocation = "hklm:\SYSTEM\CurrentControlSet\Control\Session Manager\Environment"
$environment = Get-ItemProperty $environmentLocation
$pathVar = $environment.Path
if (!($pathVar -like "*$installFolder*"))
{
    $pathVar = "$pathVar;$installFolder"
    New-ItemProperty -Path $environmentLocation -Name "Path" -Value $pathVar -Force
}