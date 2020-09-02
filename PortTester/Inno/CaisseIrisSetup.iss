// setup
#define MyAppName "PortTester"
#define MyAppPublisher "Synapsy"
#define MyAppCopyright 'Copyright © Synapsy 2020'
#define MyAppURL "https://www.synapsy.fr/"
#define MyAppExeName "PortTester.exe"
#define MyAppVsConfig "Release"
#define MyAppSourceFolder "..\bin"
#define MyAppVersion GetFileVersion(MyAppSourceFolder +'\'+ MyAppVsConfig +'\'+ MyAppExeName)

// comment out product defines to disable installing them
#define use_msi31
#define use_msi45
#define use_wic

#define use_dotnetfx45

// supported languages
#include "scripts\lang\french.iss"

// shared code for installing the products
#include "scripts\products.iss"

// helper functions
#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\dotnetfxversion.iss"

// actual products
#ifdef use_msi31
#include "scripts\products\msi31.iss"
#endif
#ifdef use_msi45
#include "scripts\products\msi45.iss"
#endif
#ifdef use_wic
#include "scripts\products\wic.iss"
#endif

#ifdef use_dotnetfx45
#include "scripts\products\dotnetfx45.iss"
#endif

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{257EBA87-623E-4F76-B5B0-FF0044CC00AA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppCopyright={#MyAppCopyright}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputBaseFilename={#MyAppName}-{#MyAppVersion}-Setup
DefaultGroupName={#MyAppName}
DefaultDirName= C:\{#MyAppPublisher}\{#MyAppName}
OutputDir={#MyAppSourceFolder}\{#MyAppVsConfig}\Inno
SourceDir=.
SetupIconFile=iris_inst.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
WizardImageFile=wizard.bmp
WizardSmallImageFile=wizard_small.bmp

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#MyAppSourceFolder}\{#MyAppVsConfig}\PortTester.exe"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall

[CustomMessages]
DependenciesDir=MyProgramDependencies
WindowsServicePack=Windows %1 Service Pack %2

[Code]
function InitializeSetup(): Boolean;
begin
	// initialize windows version
	initwinversion();

#ifdef use_msi31
	msi31('3.1'); // install if version < 3.1
#endif
#ifdef use_msi45
	msi45('4.5'); // install if version < 4.5
#endif
#ifdef use_wic
	wic();
#endif

#ifdef use_dotnetfx45
	dotnetfx45(50); // install if version < 4.6.0
#endif

	Result := true;
end;