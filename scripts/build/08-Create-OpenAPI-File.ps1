<#
A script for creating the OpenAPI file for the API of HttPlaceholder.
#>
Param(
    [Parameter(Mandatory = $True)][string]$srcFolder,
    [Parameter(Mandatory = $True)][string]$distFolder
)

$swaggerGenFolder = "$srcFolder/HttPlaceholder.SwaggerGenerator"
Set-Location $swaggerGenFolder

& dotnet run -c Release
Assert-Cmd-Ok

$swaggerLocation = "$swaggerGenFolder/bin/Release/netcoreapp2.2/swagger.json"
Copy-Item $swaggerLocation $distFolder