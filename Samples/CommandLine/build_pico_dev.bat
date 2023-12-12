@echo off
set unityPath="C:\Program Files\Unity\Hub\Editor\2021.3.1f1\Editor\Unity.exe"
set projectPath="C:\Projects\Unity\VR\VRIF"
set logPath="C:\Projects\Unity\VR\VRIF\Builds\Scripts\CommandLine\build.txt"
set outPath="D:\Projects\Unity\ArenaHeroes\Builds\Pico_Dev\"
set appName="Arena Heroes"

echo Start Build

%unityPath% ^
-quit ^
-batchmode ^
-projectpath %projectPath% ^
-executeMethod AppBuilder.Build ^
-targetName "PICO" ^
-appVersion "1.0.0a" ^
-useVR true ^
-isAdmin false ^
-isDev true ^
-useLogs true ^
-outPath %outPath% ^
-appName %appName% ^
-logFile %logPath%

echo Print Logs
powershell -Command "& {Get-Content %logPath% | Select-String -Pattern "^AppBuilder"}"
echo Complete Build
pause