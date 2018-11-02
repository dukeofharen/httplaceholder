SET APPVEYOR_BUILD_NUMBER=1234
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& %~dp0Patch-Versions.ps1"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& %~dp0Build.ps1"
pause