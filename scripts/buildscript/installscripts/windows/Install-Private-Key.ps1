$pfxFile = Join-Path -Path $PSScriptRoot "key.pfx"
if (!(Test-Path $pfxFile))
{
    Write-Error "File $pfxFile not found."
    Exit -1
}

$password = ConvertTo-SecureString "1234" -AsPlainText -Force
Import-PfxCertificate -FilePath $pfxFile -CertStoreLocation Cert:\LocalMachine\Root -Password $password