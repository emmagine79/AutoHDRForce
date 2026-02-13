!include "MUI2.nsh"

; General
Name "Auto HDR Force"
OutFile "AutoHDRForce-Setup.exe"
InstallDir "$PROGRAMFILES\AutoHDRForce"
InstallDirRegKey HKLM "Software\AutoHDRForce" "InstallDir"
RequestExecutionLevel admin

; UI
!define MUI_ICON "app.ico"
!define MUI_UNICON "app.ico"
!define MUI_ABORTWARNING

; Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\AutoHDRForce.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Launch Auto HDR Force"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language
!insertmacro MUI_LANGUAGE "English"

; Installer
Section "Install"
    SetOutPath "$INSTDIR"

    ; Main files
    File "AutoHDRForce.exe"
    File "app.ico"

    ; Create uninstaller
    WriteUninstaller "$INSTDIR\Uninstall.exe"

    ; Start Menu shortcuts
    CreateDirectory "$SMPROGRAMS\Auto HDR Force"
    CreateShortCut "$SMPROGRAMS\Auto HDR Force\Auto HDR Force.lnk" "$INSTDIR\AutoHDRForce.exe" "" "$INSTDIR\app.ico"
    CreateShortCut "$SMPROGRAMS\Auto HDR Force\Uninstall.lnk" "$INSTDIR\Uninstall.exe"

    ; Registry info for Add/Remove Programs
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce" "DisplayName" "Auto HDR Force"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce" "UninstallString" '"$INSTDIR\Uninstall.exe"'
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce" "DisplayIcon" "$INSTDIR\app.ico"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce" "Publisher" "emmagine79"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce" "DisplayVersion" "1.1"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce" "NoRepair" 1

    ; Save install dir
    WriteRegStr HKLM "Software\AutoHDRForce" "InstallDir" "$INSTDIR"
SectionEnd

; Uninstaller
Section "Uninstall"
    ; Remove files
    Delete "$INSTDIR\AutoHDRForce.exe"
    Delete "$INSTDIR\app.ico"
    Delete "$INSTDIR\Uninstall.exe"
    RMDir "$INSTDIR"

    ; Remove Start Menu
    Delete "$SMPROGRAMS\Auto HDR Force\Auto HDR Force.lnk"
    Delete "$SMPROGRAMS\Auto HDR Force\Uninstall.lnk"
    RMDir "$SMPROGRAMS\Auto HDR Force"

    ; Remove registry
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\AutoHDRForce"
    DeleteRegKey HKLM "Software\AutoHDRForce"
SectionEnd
