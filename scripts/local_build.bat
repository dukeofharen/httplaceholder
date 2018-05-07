SET APPVEYOR_BUILD_NUMBER=0
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& %~dp0patch_versions.ps1"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& %~dp0build.ps1"
pause