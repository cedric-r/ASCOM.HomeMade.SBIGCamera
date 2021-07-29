[Setup]
AppID={{3a7e63ad-c913-44f0-9489-e1744c9c2991}}
AppName=ASCOM HomeMade Camera
AppVerName=ASCOM HomeMade SBig Camera Driver 0.16.14
AppVersion=0.16.14
AppPublisher=Cedric Raguenaud <cedric@raguenaud.earth>
AppPublisherURL=mailto:cedric@raguenaud.earth
AppSupportURL=https://github.com/cedric-r/ASCOM.HomeMade.SBIGCamera
AppUpdatesURL=https://github.com/cedric-r/ASCOM.HomeMade.SBIGCamera
VersionInfoVersion=0.16.14
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
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Astrometry.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Attributes.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Controls.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.DeviceInterfaces.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Exceptions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.HomeMade.SBIGCommon.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.HomeMade.SBIGImagingCamera.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.HomeMade.SBIGImagingCamera.dll.config"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGFW\bin\Debug\ASCOM.HomeMade.SBIGFW.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGFW\bin\Debug\ASCOM.HomeMade.SBIGFW.dll.config"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Internal.Extensions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.SettingsProvider.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Utilities.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\ASCOM.Utilities.Video.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\Microsoft.Win32.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\netstandard.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\SbigSharp.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\SBIGUDrv.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.AppContext.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Collections.Concurrent.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Collections.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Collections.NonGeneric.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Collections.Specialized.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.ComponentModel.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.ComponentModel.EventBasedAsync.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.ComponentModel.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.ComponentModel.TypeConverter.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Console.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Data.Common.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.Contracts.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.Debug.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.FileVersionInfo.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.Process.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.StackTrace.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.TextWriterTraceListener.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.Tools.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.TraceSource.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Diagnostics.Tracing.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Drawing.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Dynamic.Runtime.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Globalization.Calendars.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Globalization.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Globalization.Extensions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.Compression.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.Compression.ZipFile.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.FileSystem.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.FileSystem.DriveInfo.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.FileSystem.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.FileSystem.Watcher.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.IsolatedStorage.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.MemoryMappedFiles.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.Pipes.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.IO.UnmanagedMemoryStream.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Linq.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Linq.Expressions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Linq.Parallel.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Linq.Queryable.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.Http.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.NameResolution.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.NetworkInformation.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.Ping.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.Requests.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.Security.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.Sockets.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.WebHeaderCollection.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.WebSockets.Client.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Net.WebSockets.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.ObjectModel.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Reflection.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Reflection.Extensions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Reflection.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Resources.Reader.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Resources.ResourceManager.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Resources.Writer.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.CompilerServices.VisualC.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.Extensions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.Handles.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.InteropServices.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.InteropServices.RuntimeInformation.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.Numerics.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.Serialization.Formatters.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.Serialization.Json.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.Serialization.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Runtime.Serialization.Xml.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.Claims.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.Cryptography.Algorithms.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.Cryptography.Csp.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.Cryptography.Encoding.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.Cryptography.Primitives.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.Cryptography.X509Certificates.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.Principal.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Security.SecureString.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Text.Encoding.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Text.Encoding.Extensions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Text.RegularExpressions.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Threading.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Threading.Overlapped.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Threading.Tasks.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Threading.Tasks.Parallel.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Threading.Thread.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Threading.ThreadPool.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Threading.Timer.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.ValueTuple.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Xml.ReaderWriter.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Xml.XDocument.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Xml.XmlDocument.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Xml.XmlSerializer.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Xml.XPath.dll"; DestDir: "{app}"
Source: "k:\astro\ASCOM.HomeMade.SBIGCamera\ASCOM.HomeMade.SBIGImagingCamera\bin\Debug\System.Xml.XPath.XDocument.dll"; DestDir: "{app}"

; Only if driver is .NET
[Run]
; Only for .NET assembly/in-proc drivers
Filename: "{dotnet4032}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 32bit
; Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64

Filename: "{dotnet4032}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 32bit
; Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 64bit; Check: IsWin64


; Only if driver is .NET
[UninstallRun]
; Only for .NET assembly/in-proc drivers
Filename: "{dotnet4032}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 32bit
; This helps to give a clean uninstall
; Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGImagingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64
; Filename: "{dotnet4064}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGImgaingCamera.dll"""; Flags: runhidden 64bit; Check: IsWin64

Filename: "{dotnet4032}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 32bit
; This helps to give a clean uninstall
; Filename: "{dotnet4064}\regasm.exe"; Parameters: "/codebase ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 64bit; Check: IsWin64
; Filename: "{dotnet4064}\regasm.exe"; Parameters: "-u ""{app}\ASCOM.HomeMade.SBIGFW.dll"""; Flags: runhidden 64bit; Check: IsWin64


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

