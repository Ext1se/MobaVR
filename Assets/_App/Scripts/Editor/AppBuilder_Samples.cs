using System;
using System.Collections.Generic;
using System.IO;
using MobaVR;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.OculusQuestSupport;
using UnityEngine.XR.OpenXR.Features.PICOSupport;

public partial class AppBuilder
{
    #region Build Variants

    public static void BuildWindows(
        string version,
        bool isAdmin,
        bool isDevelopmentBuild,
        bool useLogs,
        string outPath,
        string appName)
    {
        Build();
    }

    public static void BuildWindows()
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: App Settings is null or invalid path");
            return;
        }

        string cityName = settings.AppData.City;
        string roomName = settings.AppData.Room;
        int idClub = settings.IdClub;
        int idGame = settings.IdGame;
        string targetName = PlatformType.WINDOWS.ToString();
        bool useVR = false;

        if (!TryGetArgumentValue(KEY_VERSION, out string version))
        {
            version = settings.GameVersion;
        }

        if (!TryGetArgumentValue(KEY_IS_ADMIN, out bool isAdmin))
        {
            isAdmin = false;
        }

        if (!TryGetArgumentValue(KEY_IS_DEV_BUILD, out bool isDevelopmentBuild))
        {
            isDevelopmentBuild = false;
        }

        if (!TryGetArgumentValue(KEY_USE_LOGS, out bool useLogs))
        {
            useLogs = false;
        }

        if (!TryGetArgumentValue(KEY_OUT_PATH, out string outPath))
        {
            outPath = DEFAULT_OUT_PATH;
        }

        if (!TryGetArgumentValue(KEY_APP_NAME, out string appName))
        {
            appName = DEFAULT_NAME;
        }

        Console.WriteLine($"{TAG}: ---");
        Console.WriteLine($"{TAG}: Input Console Arguments: ");
        Console.WriteLine($"{TAG}: cityName = {cityName};");
        Console.WriteLine($"{TAG}: idClub = {idClub};");
        Console.WriteLine($"{TAG}: idGame = {idGame};");
        Console.WriteLine($"{TAG}: targetName = {targetName};");
        Console.WriteLine($"{TAG}: version = {version};");
        Console.WriteLine($"{TAG}: useVR = {useVR};");
        Console.WriteLine($"{TAG}: isAdmin = {isAdmin};");
        Console.WriteLine($"{TAG}: isDevelopmentBuild = {isDevelopmentBuild};");
        Console.WriteLine($"{TAG}: useLogs = {useLogs};");
        Console.WriteLine($"{TAG}: outPath = {outPath};");
        Console.WriteLine($"{TAG}: appName = {appName};");
        Console.WriteLine($"{TAG}: ---");


        Build(cityName,
              roomName,
              targetName,
              version,
              idClub,
              idGame,
              useVR,
              isAdmin,
              isDevelopmentBuild,
              useLogs,
              outPath,
              appName);
    }

    public static void BuildAndroid()
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: App Settings is null or invalid path");
            return;
        }

        string cityName = settings.AppData.City;
        string roomName = settings.AppData.Room;
        int idClub = settings.IdClub;
        int idGame = settings.IdGame;
        string targetName = PlatformType.ANDROID.ToString();
        bool useVR = false;
        bool isAdmin = true;

        if (!TryGetArgumentValue(KEY_VERSION, out string version))
        {
            version = settings.GameVersion;
        }

        if (!TryGetArgumentValue(KEY_IS_DEV_BUILD, out bool isDevelopmentBuild))
        {
            isDevelopmentBuild = false;
        }

        if (!TryGetArgumentValue(KEY_USE_LOGS, out bool useLogs))
        {
            useLogs = false;
        }

        if (!TryGetArgumentValue(KEY_OUT_PATH, out string outPath))
        {
            outPath = DEFAULT_OUT_PATH;
        }

        if (!TryGetArgumentValue(KEY_APP_NAME, out string appName))
        {
            appName = DEFAULT_NAME;
        }

        Console.WriteLine($"{TAG}: ---");
        Console.WriteLine($"{TAG}: Input Console Arguments: ");
        Console.WriteLine($"{TAG}: cityName = {cityName};");
        Console.WriteLine($"{TAG}: idClub = {idClub};");
        Console.WriteLine($"{TAG}: idGame = {idGame};");
        Console.WriteLine($"{TAG}: targetName = {targetName};");
        Console.WriteLine($"{TAG}: version = {version};");
        Console.WriteLine($"{TAG}: useVR = {useVR};");
        Console.WriteLine($"{TAG}: isAdmin = {isAdmin};");
        Console.WriteLine($"{TAG}: isDevelopmentBuild = {isDevelopmentBuild};");
        Console.WriteLine($"{TAG}: useLogs = {useLogs};");
        Console.WriteLine($"{TAG}: outPath = {outPath};");
        Console.WriteLine($"{TAG}: appName = {appName};");
        Console.WriteLine($"{TAG}: ---");


        Build(cityName,
              roomName,
              targetName,
              version,
              idClub,
              idGame,
              useVR,
              isAdmin,
              isDevelopmentBuild,
              useLogs,
              outPath,
              appName);
    }
    
    public static void BuildPico()
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: App Settings is null or invalid path");
            return;
        }

        string cityName = settings.AppData.City;
        string roomName = settings.AppData.Room;
        int idClub = settings.IdClub;
        int idGame = settings.IdGame;
        string targetName = PlatformType.PICO.ToString();
        bool useVR = true;
        bool isAdmin = false;

        if (!TryGetArgumentValue(KEY_VERSION, out string version))
        {
            version = settings.GameVersion;
        }

        if (!TryGetArgumentValue(KEY_IS_DEV_BUILD, out bool isDevelopmentBuild))
        {
            isDevelopmentBuild = false;
        }

        if (!TryGetArgumentValue(KEY_USE_LOGS, out bool useLogs))
        {
            useLogs = false;
        }

        if (!TryGetArgumentValue(KEY_OUT_PATH, out string outPath))
        {
            outPath = DEFAULT_OUT_PATH;
        }

        if (!TryGetArgumentValue(KEY_APP_NAME, out string appName))
        {
            appName = DEFAULT_NAME;
        }

        Console.WriteLine($"{TAG}: ---");
        Console.WriteLine($"{TAG}: Input Console Arguments: ");
        Console.WriteLine($"{TAG}: cityName = {cityName};");
        Console.WriteLine($"{TAG}: idClub = {idClub};");
        Console.WriteLine($"{TAG}: idGame = {idGame};");
        Console.WriteLine($"{TAG}: targetName = {targetName};");
        Console.WriteLine($"{TAG}: version = {version};");
        Console.WriteLine($"{TAG}: useVR = {useVR};");
        Console.WriteLine($"{TAG}: isAdmin = {isAdmin};");
        Console.WriteLine($"{TAG}: isDevelopmentBuild = {isDevelopmentBuild};");
        Console.WriteLine($"{TAG}: useLogs = {useLogs};");
        Console.WriteLine($"{TAG}: outPath = {outPath};");
        Console.WriteLine($"{TAG}: appName = {appName};");
        Console.WriteLine($"{TAG}: ---");


        Build(cityName,
              roomName,
              targetName,
              version,
              idClub,
              idGame,
              useVR,
              isAdmin,
              isDevelopmentBuild,
              useLogs,
              outPath,
              appName);
    }
    
    public static void BuildMeta()
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: App Settings is null or invalid path");
            return;
        }

        string cityName = settings.AppData.City;
        string roomName = settings.AppData.Room;
        int idClub = settings.IdClub;
        int idGame = settings.IdGame;
        string targetName = PlatformType.META.ToString();
        bool useVR = true;
        bool isAdmin = false;

        if (!TryGetArgumentValue(KEY_VERSION, out string version))
        {
            version = settings.GameVersion;
        }

        if (!TryGetArgumentValue(KEY_IS_DEV_BUILD, out bool isDevelopmentBuild))
        {
            isDevelopmentBuild = false;
        }

        if (!TryGetArgumentValue(KEY_USE_LOGS, out bool useLogs))
        {
            useLogs = false;
        }

        if (!TryGetArgumentValue(KEY_OUT_PATH, out string outPath))
        {
            outPath = DEFAULT_OUT_PATH;
        }

        if (!TryGetArgumentValue(KEY_APP_NAME, out string appName))
        {
            appName = DEFAULT_NAME;
        }

        Console.WriteLine($"{TAG}: ---");
        Console.WriteLine($"{TAG}: Input Console Arguments: ");
        Console.WriteLine($"{TAG}: cityName = {cityName};");
        Console.WriteLine($"{TAG}: idClub = {idClub};");
        Console.WriteLine($"{TAG}: idGame = {idGame};");
        Console.WriteLine($"{TAG}: targetName = {targetName};");
        Console.WriteLine($"{TAG}: version = {version};");
        Console.WriteLine($"{TAG}: useVR = {useVR};");
        Console.WriteLine($"{TAG}: isAdmin = {isAdmin};");
        Console.WriteLine($"{TAG}: isDevelopmentBuild = {isDevelopmentBuild};");
        Console.WriteLine($"{TAG}: useLogs = {useLogs};");
        Console.WriteLine($"{TAG}: outPath = {outPath};");
        Console.WriteLine($"{TAG}: appName = {appName};");
        Console.WriteLine($"{TAG}: ---");


        Build(cityName,
              roomName,
              targetName,
              version,
              idClub,
              idGame,
              useVR,
              isAdmin,
              isDevelopmentBuild,
              useLogs,
              outPath,
              appName);
    }
    
    
    public static void SimpleBuild()
    {
        Debug.Log("SimpleBuild: from Debug.Log");
        Console.WriteLine("SimpleBuild");

        TryGetArgumentValue(KEY_CITY_NAME, out string cityName);
        TryGetArgumentValue(KEY_CLUB_ID, out string idClub);
        TryGetArgumentValue(KEY_GAME_ID, out string idGame);
        TryGetArgumentValue(KEY_PLATFORM_NAME, out string targetName);
        TryGetArgumentValue(KEY_VERSION, out string version);
        TryGetArgumentValue(KEY_USE_VR, out bool useVR);
        TryGetArgumentValue(KEY_IS_ADMIN, out bool isAdmin);
        TryGetArgumentValue(KEY_IS_DEV_BUILD, out bool isDevelopmentBuild);
        TryGetArgumentValue(KEY_USE_LOGS, out bool useLogs);
        TryGetArgumentValue(KEY_OUT_PATH, out string outPath);
        TryGetArgumentValue(KEY_APP_NAME, out string appName);

        Console.WriteLine($"SimpleBuild: " +
                          $"cityName = {cityName}; " +
                          $"idClub = {idClub}; " +
                          $"idGame = {idGame}; " +
                          $"targetName = {targetName}; " +
                          $"version = {version}; " +
                          $"useVR = {useVR}; " +
                          $"isAdmin = {isAdmin}; " +
                          $"isDevelopmentBuild = {isDevelopmentBuild}; " +
                          $"useLogs = {useLogs}; " +
                          $"outPath = {outPath}; " +
                          $"appName = {appName}; "
        );
    }


    #endregion
}