@echo off
set unityPath="C:\Program Files\Unity\Hub\Editor\2021.3.1f1\Editor\Unity.exe"
set projectPath="C:\Projects\Unity\VR\VRIF"
set logPath="C:\Projects\Unity\VR\VRIF\Builds\Scripts\CommandLine\build.txt"
set appName="Arena Heroes"

echo Start Build Windows Client
%unityPath% ^
-quit ^
-batchmode ^
-projectpath %projectPath% ^
-executeMethod AppBuilder.Build ^
-targetName "WINDOWS" ^
-appVersion "1.0.0a" ^
-useVR false ^
-isAdmin false ^
-idDev false ^
-useLogs true ^
-outPath "D:\Projects\Unity\ArenaHeroes\Builds\v2\WindowsClient\" ^
-appName %appName% ^
-logFile %logPath%
echo Print Logs
powershell -Command "& {Get-Content %logPath% | Select-String -Pattern "^AppBuilder"}"

echo Start Build Windows Admin
%unityPath% ^
-quit ^
-batchmode ^
-projectpath %projectPath% ^
-executeMethod AppBuilder.Build ^
-targetName "WINDOWS" ^
-appVersion "1.0.0a" ^
-useVR false ^
-isAdmin true ^
-idDev false ^
-useLogs true ^
-outPath "D:\Projects\Unity\ArenaHeroes\Builds\v2\WindowsAdmin\" ^
-appName %appName% ^
-logFile %logPath%
echo Print Logs
powershell -Command "& {Get-Content %logPath% | Select-String -Pattern "^AppBuilder"}"

echo Start Build Meta
%unityPath% ^
-quit ^
-batchmode ^
-projectpath %projectPath% ^
-executeMethod AppBuilder.Build ^
-targetName "META" ^
-appVersion "1.0.0a" ^
-useVR true ^
-isAdmin false ^
-idDev false ^
-useLogs true ^
-outPath "D:\Projects\Unity\ArenaHeroes\Builds\v2\Meta\" ^
-appName %appName% ^
-logFile %logPath%
echo Print Logs
powershell -Command "& {Get-Content %logPath% | Select-String -Pattern "^AppBuilder"}"


echo Complete Build
pause