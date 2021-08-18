[Setup]
AppID={{3a7e63ad-c913-44f0-9489-e1744c9c2991}}
AppName=ASCOM HomeMade Camera
AppVerName=ASCOM HomeMade SBig Camera Driver 0.21.40
AppVersion=0.21.40
AppPublisher=Cedric Raguenaud <cedric@raguenaud.earth>
AppPublisherURL=mailto:cedric@raguenaud.earth
AppSupportURL=https://github.com/cedric-r/ASCOM.HomeMade.SBIGCamera
AppUpdatesURL=https://github.com/cedric-r/ASCOM.HomeMade.SBIGCamera
VersionInfoVersion=0.21.40
MinVersion=0,6.1
DefaultDirName="{cf}\ASCOM\Camera\HomeMade SBIGCamera\"
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir="."
OutputBaseFilename="HomeMade SBIG Camera Setup"
Compression=lzma
SolidCompression=yes
; Put there by Platform if Driver Installer Support selected
WizardImageFile="C:\Program Files (x86)\ASCOM\Platform 6 Developer Components\Installer Generator\Resources\WizardImage.bmp"
LicenseFile="k:\astro\ASCOM.HomeMade.SBIGCamera\License"
; {cf}\ASCOM\Uninstall\SBIGCamera folder created by Platform, always
UninstallFilesDir="{cf}\ASCOM\Uninstall\Camera\HomeMade SBIGCamera\"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
Name: "{cf}\ASCOM\Uninstall\Camera\HomeMade SBIGCamera\"
; TODO: Add subfolders below {app} as needed (e.g. Name: "{app}\MyFolder")

[Files]
; Require a read-me HTML to appear after installation, maybe driver's Help doc
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\readme.txt"; DestDir: "{app}"; Flags: isreadme
; TODO: Add other files needed by your driver here (add subfolders above)
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGCommon\bin\Debug\sensors.txt"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Astrometry.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Attributes.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Controls.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.DeviceInterfaces.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Exceptions.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.HomeMade.SBIGCommon.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.HomeMade.SBIGClient.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.HomeMade.SBIGImagingCamera.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.HomeMade.SBIGImagingCamera.dll.config"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGFW\bin\Debug\ASCOM.HomeMade.SBIGFW.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGFW\bin\Debug\ASCOM.HomeMade.SBIGFW.dll.config"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGHub\bin\Debug\ASCOM.HomeMade.SBIGHub.Server.exe"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGHub\bin\Debug\ASCOM.HomeMade.SBIGHub.Server.exe.config"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGGuidingCamera\bin\Debug\ASCOM.HomeMade.SBIGGuidingCamera.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGGuidingCamera\bin\Debug\ASCOM.HomeMade.SBIGGuidingCamera.dll.config"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Internal.Extensions.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.SettingsProvider.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Utilities.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Utilities.Video.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\SbigSharp.dll"; DestDir: "{app}"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\x86\HomeMade.SBIGUDrv.dll"; DestDir: "{app}\x86\"; Flags: comparetimestamp overwritereadonly ignoreversion
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\x64\HomeMade.SBIGUDrv.dll"; DestDir: "{app}\x64\"; Flags: comparetimestamp overwritereadonly ignoreversion

; Only if driver is .NET
[Run]
; Only for .NET assembly/in-proc drivers
;Filename: "{dotnet4032}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 32bit
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64

;Filename: "{dotnet4032}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGGuidingCamera.dll"""; Flags: runhidden 32bit
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGGuidingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64

;Filename: "{dotnet4032}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 32bit
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 64bit; Check: IsWin64

;Filename: {sys}\sc.exe; Parameters: "stop SBIGService" ; Flags: runhidden
;Filename: {sys}\sc.exe; Parameters: "create SBIGService start= auto binPath= ""{app}\ASCOM.HomeMade.SBIGWindowsService.exe""" ; Flags: runhidden
;Filename: {sys}\sc.exe; Parameters: "start SBIGService" ; Flags: runhidden

;Filename: "{dotnet4032}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGHub.Server.exe"""; Flags: runhidden 32bit
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGHub.Server.exe"""; Flags: runhidden 64bit; Check: IsWin64
Filename: {app}\ASCOM.HomeMade.SBIGHub.Server.exe; Parameters: "/register" ; Flags: runhidden 32bit
;Filename: {app}\ASCOM.HomeMade.SBIGHub.Server.exe; Parameters: "/register" ; Flags: runhidden 64bit; Check: IsWin64


; Only if driver is .NET
[UninstallRun]
; Only for .NET assembly/in-proc drivers
;Filename: "{dotnet4032}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 32bit
; This helps to give a clean uninstall
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGImgaingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64

;Filename: "{dotnet4032}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGGuidingCamera.dll"""; Flags: runhidden 32bit
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGGuidingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGGuidingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64

;Filename: "{dotnet4032}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 32bit
; This helps to give a clean uninstall
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 64bit; Check: IsWin64
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 64bit; Check: IsWin64

;Filename: {sys}\sc.exe; Parameters: "stop SBIGService" ; Flags: runhidden
;Filename: {sys}\sc.exe; Parameters: "delete SBIGService" ; Flags: runhidden

;Filename: "{dotnet4032}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGHub.Server.exe"""; Flags: runhidden 32bit
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGHub.Server.exe"""; Flags: runhidden 64bit; Check: IsWin64
;Filename: "{dotnet4064}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGHub.Server.exe"""; Flags: runhidden 64bit; Check: IsWin64
Filename: {app}\ASCOM.HomeMade.SBIGHub.Server.exe; Parameters: "/unregister" ; Flags: runhidden 32bit
;Filename: {app}\ASCOM.HomeMade.SBIGHub.Server.exe; Parameters: "/unregister" ; Flags: runhidden 64bit; Check: IsWin64


[CODE]
//
// Before the installer UI appears, verify that the (prerequisite)
// ASCOM Platform 6.2 or greater is installed, including both Helper
// components. Utility is required for all types (COM and .NET)!
//
function InitializeSetup(): Boolean;
var
   U : Variant;
   H : Variant;
begin
   Result := TRUE;  // Assume failure
   // check that the DriverHelper and Utilities objects exist, report errors if they don't
   // try
   //   H := CreateOLEObject('DriverHelper.Util');
   // except
   //   MsgBox('The ASCOM DriverHelper object has failed to load, this indicates a serious problem with the ASCOM installation', mbInformation, MB_OK);
   // end;
   // try
   //   U := CreateOLEObject('ASCOM.Utilities.Util');
   // except
   //   MsgBox('The ASCOM Utilities object has failed to load, this indicates that the ASCOM Platform has not been installed correctly', mbInformation, MB_OK);
   // end;
   // try
   //   if (U.IsMinimumRequiredVersion(6,2)) then	// this will work in all locales
   //      Result := TRUE;
   // except
   // end;
   // if(not Result) then
   //   MsgBox('The ASCOM Platform 6.2 or greater is required for this driver.', mbInformation, MB_OK);
end;

// Code to enable the installer to uninstall previous versions of itself when a new version is installed
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
  UninstallExe: String;
  UninstallRegistry: String;
begin
  if (CurStep = ssInstall) then // Install step has started
	begin
      // Create the correct registry location name, which is based on the AppId
      UninstallRegistry := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#SetupSetting("AppId")}' + '_is1');
      // Check whether an extry exists
      if RegQueryStringValue(HKLM, UninstallRegistry, 'UninstallString', UninstallExe) then
        begin // Entry exists and previous version is installed so run its uninstaller quietly after informing the user
          MsgBox('Setup will now remove the previous version.', mbInformation, MB_OK);
          Exec(RemoveQuotes(UninstallExe), ' /SILENT', '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
          sleep(1000);    //Give enough time for the install screen to be repainted before continuing
        end
  end;
end;

