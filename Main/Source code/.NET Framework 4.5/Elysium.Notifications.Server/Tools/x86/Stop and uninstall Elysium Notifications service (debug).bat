@echo off
set OriginalPATH=%PATH%
set PATH=%OriginalPATH%;%windir%\Microsoft.NET\Framework\v4.0.30319
chdir /d %~dp0
cd ..\..\..\..\
@echo on
net stop ElysiumNotifications-v1.5.35.0-v4.5
installutil /u "%CD%\Binary\.NET Framework 4.5\Debug\x86\Elysium.Notifications.Server.exe"
pause