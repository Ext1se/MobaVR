@echo off
set unityPath="C:\Program Files\Unity\Hub\Editor\2021.3.1f1\Editor\Unity.exe"
set projectPath="C:\Projects\Unity\VR\VRIF"
set logPath="C:\Projects\Unity\VR\VRIF\Builds\Scripts\CommandLine\build.txt"
set outPath="D:\Projects\Unity\ArenaHeroes\Builds\"
set appName="Arena Heroes"

echo start

%unityPath% ^
-quit ^
-batchmode ^
-logFile %logPath% ^
-stackTraceLogType Full ^
-projectpath %projectPath% ^
-executeMethod AppBuilder.Build ^
-cityName "ARMA" ^
-idClub 1 ^
-idGame 4 ^
-targetName "PICO" ^
-appVersion "1.0.0a" ^
-useVR false ^
-isAdmin true ^
-idDev true ^
-useLogs true ^
-outPath %outPath% ^
-appName %appName%

echo end
pause