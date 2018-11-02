!include "FileFunc.nsh"

!define APPNAME "HttPlaceholder"
!define COMPANYNAME "Ducode"
!define DESCRIPTION "A very flexible cross platform HTTP stub application."
!define VERSIONMAJOR "$%VersionMajor%"
!define VERSIONMINOR "$%VersionMinor%"
!define VERSIONBUILD "$%VersionBuild%"
!define BINDIRECTORY "$%BuildOutputBinDirectory%"
!define HELPURL "https://ducode.org"
!define UPDATEURL "https://ducode.org"
!define ABOUTURL "https://ducode.org"
 
RequestExecutionLevel admin
 
InstallDir "$PROGRAMFILES\${COMPANYNAME}\${APPNAME}"

LicenseData "..\LICENSE.txt"
Name "${COMPANYNAME} - ${APPNAME}"
outFile "$%BuildOutputDirectory%\httplaceholder_install-$%versionString%.exe"
 
!include LogicLib.nsh

page license
page directory
Page instfiles
 
!macro VerifyUserIsAdmin
UserInfo::GetAccountType
pop $0
${If} $0 != "admin"
        messageBox mb_iconstop "Administrator rights required!"
        setErrorLevel 740
        quit
${EndIf}
!macroend
 
function .onInit
	setShellVarContext all
	!insertmacro VerifyUserIsAdmin
functionEnd
 
section "install"
	setOutPath $INSTDIR
	file /r "${BINDIRECTORY}\*"
 	writeUninstaller "$INSTDIR\uninstall.exe"
 
	createDirectory "$SMPROGRAMS\${COMPANYNAME}"
    createShortCut "$SMPROGRAMS\${COMPANYNAME}\${APPNAME}.lnk" "$INSTDIR\HttPlaceholder.exe"
	createShortCut "$SMPROGRAMS\${COMPANYNAME}\Uninstall ${APPNAME}.lnk" "$INSTDIR\uninstall.exe"

	# Calculate installed size
	${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
	IntFmt $0 "0x%08X" $0
 
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayName" "${COMPANYNAME} - ${APPNAME} - ${DESCRIPTION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayIcon" "$INSTDIR\HttPlaceholder.exe"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "Publisher" "${COMPANYNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "HelpLink" "${HELPURL}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "URLUpdateInfo" "${UPDATEURL}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "URLInfoAbout" "${ABOUTURL}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayVersion" "${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMajor" ${VERSIONMAJOR}
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMinor" ${VERSIONMINOR}
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoRepair" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "EstimatedSize" $0

    ExecWait 'powershell.exe -NoProfile -ExecutionPolicy Unrestricted -Command "& \"$INSTDIR\Add-Path-Var.ps1\"" -installFolder \"$INSTDIR\"'
sectionEnd
 
function un.onInit
	SetShellVarContext all
 
	MessageBox MB_YESNO "Permanantly remove ${APPNAME}?" /SD IDYES IDYES next IDNO abort
	abort:
		Abort
	next:
	!insertmacro VerifyUserIsAdmin
functionEnd
 
section "uninstall"
	ExecWait 'powershell.exe -NoProfile -ExecutionPolicy Unrestricted -Command "& \"$INSTDIR\Remove-Path-Var.ps1\"" -installFolder \"$INSTDIR\"'

	rmDir /r "$SMPROGRAMS\${COMPANYNAME}"
 
	rmDir /r $INSTDIR
 
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}"
sectionEnd