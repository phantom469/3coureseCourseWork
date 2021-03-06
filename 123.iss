; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "����������"
#define MyAppVersion "1.5"
#define MyAppPublisher "My Company, Inc."
#define MyAppURL "http://www.example.com/"
#define MyAppExeName "CourseWork.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{0595CEF9-75B8-4CF5-8E66-D306D0A6EBFA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=����������\{#MyAppName}
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=D:\onedrive\������ 2,0\Output
OutputBaseFilename=����������
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\Debug\CourseWork.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\Debug\CourseWork.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\Debug\CourseWork.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\Debug\Emploees.mdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\Debug\Emploees_log.ldf"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\Debug\itextsharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\Debug\No-Image-Available.png"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\onedrive\������ 2,0\Kominternemploee\CourseWork\bin\�����\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\���-��\Desktop\Frame.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\���-��\Desktop\SqlLocalDB.msi"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\SqlLocalDB.msi"; Description: "��������� ��������� SQLLocalDB (�����������) ����� ���������?";  Flags:  nowait postinstall shellexec;
Filename: "{app}\Frame.exe"; Description: "��������� ��������� .NET Framework 4.8 ����� ���������?";  Flags:  nowait postinstall shellexec;

