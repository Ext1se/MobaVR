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
    private const string TAG = nameof(AppBuilder);

    private const string OCULUS_LOADER = "Unity.XR.Oculus.OculusLoader";
    private const string OPENXR_LOADER = "UnityEngine.XR.OpenXR.OpenXRLoader";

    public const string KEY_CITY_NAME = "-cityName";
    public const string KEY_CLUB_ID = "-idClub";
    public const string KEY_GAME_ID = "-idGame";
    public const string KEY_PLATFORM_NAME = "-targetName";
    public const string KEY_VERSION = "-appVersion";
    public const string KEY_USE_VR = "-useVR";
    public const string KEY_IS_ADMIN = "-isAdmin";
    public const string KEY_IS_DEV_BUILD = "-isDev";
    public const string KEY_USE_LOGS = "-useLogs";
    public const string KEY_OUT_PATH = "-outPath";
    public const string KEY_APP_NAME = "-appName";

    public static readonly string DEFAULT_OUT_PATH = "Builds/";
    public static readonly string DEFAULT_NAME = "Arena Heroes";
    public static readonly string DEFAULT_COMPANY = "PortalVR";
    public static readonly string CITY_PATH = "Assets/_App/Resources/Api/Settings/AppSettingCity.asset";
    public static readonly string BUILD_GROUP_PATH = "Assets/_App/Resources/Api/Builds/Groups/BuildGroup.asset";


    public static readonly string[] ADMIN_SCENES = new[]
    {
        "Assets/_App/Scenes/Login.unity",
    };

    public static readonly string[] COMMON_SCENES = new string[]
    {
        "Assets/_App/Scenes/Init.unity",
        "Assets/_App/Scenes/Menu.unity",
        "Assets/_App/Scenes/Lobby.unity"
    };

    public static readonly string[] CITY_SCENES = new string[]
    {
        //Lobby
        "Taverna",

        //PVP
        "SkyArena",
        "Dungeon",

        //PVE
        "Necropolis",

        //TD
        "Tower",
    };

    public static void OpenDialogAndBuild(string cityName,
                                          string targetName,
                                          string version,
                                          bool useVR = false,
                                          bool isAdmin = false,
                                          bool isDevelopmentBuild = false,
                                          bool useLogs = false)
    {
        string extension = "";
        targetName = targetName.ToUpper();
        if (!Enum.TryParse(targetName, out PlatformType platformType))
        {
            Debug.LogError($"{TAG}: is not valid platform");
            return;
        }

        switch (platformType)
        {
            case PlatformType.WINDOWS:
                extension += "exe";
                break;
            case PlatformType.ANDROID:
            case PlatformType.META:
            case PlatformType.PICO:
                extension += "apk";
                break;
            default:
                Debug.LogError($"{TAG}: is not valid platform");
                return;
        }

        string pathToSaveFile = EditorUtility.SaveFilePanel(
            $"Build application: {platformType}",
            "",
            "Arena Heroes",
            extension);

        if (string.IsNullOrEmpty(pathToSaveFile) || pathToSaveFile.Length == 0)
        {
            return;
        }

        string outPath = pathToSaveFile.Substring(0, pathToSaveFile.LastIndexOf('/') + 1);
        string appName = pathToSaveFile.Substring(pathToSaveFile.LastIndexOf('/') + 1);
        appName = appName.Substring(0, appName.LastIndexOf('.') - 1);

        Build(
            cityName,
            targetName,
            useVR,
            isAdmin,
            isDevelopmentBuild,
            useLogs,
            version,
            outPath,
            appName);
    }

    public static void Build()
    {
        Debug.Log($"{TAG}: Build: from Debug.Log");
        Console.WriteLine($"{TAG}: Build: from Debug.Log");

        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: App Settings is null or invalid path");
            return;
        }

        if (!TryGetArgumentValue(KEY_CITY_NAME, out string cityName))
        {
            cityName = settings.AppData.City;
        }

        if (!TryGetArgumentValue(KEY_CLUB_ID, out int idClub))
        {
            idClub = settings.IdClub;
        }

        if (!TryGetArgumentValue(KEY_GAME_ID, out int idGame))
        {
            idGame = settings.IdGame;
        }

        if (!TryGetArgumentValue(KEY_PLATFORM_NAME, out string targetName))
        {
            targetName = settings.AppData.Platform.ToString();
        }

        if (!TryGetArgumentValue(KEY_VERSION, out string version))
        {
            version = settings.GameVersion;
        }

        if (!TryGetArgumentValue(KEY_USE_VR, out bool useVR))
        {
            useVR = false;
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

        /*
        Console.WriteLine($"{TAG}: Build Arguments: " +
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
        */

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

    public static void Build(
        string cityName,
        string targetName,
        string version,
        int idClub,
        int idGame,
        bool useVR,
        bool isAdmin,
        bool isDevelopmentBuild,
        bool useLogs,
        string outPath,
        string appName)
    {
        targetName = targetName.ToUpper();
        if (!Enum.TryParse(targetName, out PlatformType platformType))
        {
            Debug.LogError($"{TAG}: {targetName} is not valid platform");
            return;
        }

        if (string.IsNullOrEmpty(outPath))
        {
            outPath = DEFAULT_OUT_PATH;
        }

        if (string.IsNullOrEmpty(appName))
        {
            appName = DEFAULT_NAME;
        }

        if (!CheckDirectory(platformType, outPath, appName, out string fullPath))
        {
            return;
        }

        if (!SetAppSettings(cityName, isAdmin, platformType, idClub, idGame, version, useVR, isDevelopmentBuild,
                            useLogs))
        {
            return;
        }

        if (!SetScenes(cityName, platformType, isAdmin, out string[] scenes))
        {
            return;
        }

        Console.WriteLine($"{TAG}: ---");
        Console.WriteLine($"{TAG}: Arguments before build: ");
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
        Console.WriteLine($"{TAG}: fullPath = {fullPath};");
        Console.WriteLine($"{TAG}: ---");

        TryBuild(targetName, version, useVR, fullPath, scenes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cityName"></param>
    /// <param name="targetName"></param>
    /// <param name="useVR"></param>
    /// <param name="isAdmin"></param>
    /// <param name="isDevelopmentBuild"></param>
    /// <param name="useLogs"></param>
    /// <param name="version"></param>
    /// <param name="outPath"></param>
    /// <param name="appName"></param>
    public static void Build(
        string cityName,
        string targetName,
        bool useVR = false,
        bool isAdmin = false,
        bool isDevelopmentBuild = false,
        bool useLogs = false,
        string version = "0",
        string outPath = null,
        string appName = null)
    {
        targetName = targetName.ToUpper();
        if (!Enum.TryParse(targetName, out PlatformType platformType))
        {
            Debug.LogError($"{TAG}: is not valid platform");
            return;
        }

        if (string.IsNullOrEmpty(outPath))
        {
            outPath = DEFAULT_OUT_PATH;
        }

        if (string.IsNullOrEmpty(appName))
        {
            appName = DEFAULT_NAME;
        }

        if (!CheckDirectory(platformType, outPath, appName, out string fullPath))
        {
            return;
        }

        if (!SetAppSettings(cityName, isAdmin, platformType, useVR, isDevelopmentBuild, useLogs))
        {
            return;
        }

        if (!SetScenes(cityName, platformType, isAdmin, out string[] scenes))
        {
            return;
        }

        /*
Console.WriteLine($"{TAG}: Build Arguments: " +
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
                  $"appName = {appName}; " +
                  $"fullPath = {fullPath}"
);
*/
        
        TryBuild(targetName, version, useVR, fullPath, scenes);
    }

    private static void TryBuild(string targetName, string version, bool useVR, string outPath, string[] scenes)
    {
        //EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        BuildTarget buildTarget;
        BuildTargetGroup buildTargetGroup;
        List<Type> xrTypes = new List<Type>();

        targetName = targetName.ToUpper();
        switch (targetName)
        {
            case (nameof(PlatformType.WINDOWS)):
                buildTarget = BuildTarget.StandaloneWindows64;
                buildTargetGroup = BuildTargetGroup.Standalone;
                if (useVR)
                {
                    //xrTypes.Add(typeof(OculusTouchControllerProfile));
                    //xrTypes.Add(typeof(PICOTouchControllerProfile));

                    xrTypes.Add(typeof(OculusQuestFeature));
                    xrTypes.Add(typeof(OculusTouchControllerProfile));
                }

                break;
            case (nameof(PlatformType.ANDROID)):
                buildTarget = BuildTarget.Android;
                buildTargetGroup = BuildTargetGroup.Android;
                useVR = false;
                break;
            case (nameof(PlatformType.META)):
                buildTarget = BuildTarget.Android;
                buildTargetGroup = BuildTargetGroup.Android;
                useVR = true;
                xrTypes.Add(typeof(OculusQuestFeature));
                xrTypes.Add(typeof(OculusTouchControllerProfile));
                break;
            case (nameof(PlatformType.PICO)):
                buildTarget = BuildTarget.Android;
                buildTargetGroup = BuildTargetGroup.Android;
                useVR = true;
                xrTypes.Add(typeof(PICOFeature));
                xrTypes.Add(typeof(PICOTouchControllerProfile));
                break;
            default:
                Debug.LogError($"{TAG}: is not valid platform");
                return;
        }

        EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey,
                                               out XRGeneralSettingsPerBuildTarget buildTargetSettings);
        XRGeneralSettings xrGeneralSettings = buildTargetSettings.SettingsForBuildTarget(buildTargetGroup);

        if (useVR)
        {
            XRPackageMetadataStore.RemoveLoader(xrGeneralSettings.Manager, OCULUS_LOADER, buildTargetGroup);
            XRPackageMetadataStore.AssignLoader(xrGeneralSettings.Manager, OPENXR_LOADER, buildTargetGroup);
        }
        else
        {
            XRPackageMetadataStore.RemoveLoader(xrGeneralSettings.Manager, OCULUS_LOADER, buildTargetGroup);
            XRPackageMetadataStore.RemoveLoader(xrGeneralSettings.Manager, OPENXR_LOADER, buildTargetGroup);
        }

        OpenXRSettings openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
        foreach (OpenXRFeature xrFeature in openXRSettings.GetFeatures())
        {
            xrFeature.enabled = xrTypes.Contains(xrFeature.GetType());
        }

        EditorUtility.SetDirty(buildTargetSettings);
        EditorUtility.SetDirty(xrGeneralSettings);
        EditorUtility.SetDirty(openXRSettings);
        AssetDatabase.SaveAssets();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outPath,
            target = buildTarget,
            options = BuildOptions.None
        };

        //TODO: Update Player Settings
        string prevVersion = Application.version;
        string prevName = PlayerSettings.productName;
        string prevPackageName = PlayerSettings.applicationIdentifier;
        
        PlayerSettings.bundleVersion = version;
        PlayerSettings.productName = $"{DEFAULT_NAME} {version}";
        PlayerSettings.applicationIdentifier = $"com.portal_vr.arena_heroes_{version}";

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"{TAG}: build is completed. OK.");
        }
        else
        {
            Debug.LogError($"{TAG}: build is failed");
        }

        PlayerSettings.bundleVersion = prevVersion;
        PlayerSettings.productName = prevName;
        PlayerSettings.applicationIdentifier = prevPackageName;
    }

    private static bool SetScenes(string cityName, PlatformType platformType, bool isAdmin, out string[] scenes)
    {
        switch (platformType)
        {
            case PlatformType.WINDOWS:
                break;
            case PlatformType.ANDROID:
                if (!isAdmin)
                {
                    Debug.LogError($"{TAG}: Android Table can not be client");
                    scenes = null;
                    return false;
                }

                break;
            case PlatformType.META:
            case PlatformType.PICO:
                if (isAdmin)
                {
                    Debug.LogError($"{TAG}: Android VR can not be admin");
                    scenes = null;
                    return false;
                }

                break;
            default:
                scenes = null;
                return false;
        }

        string cityPath = $"Assets/_App/Scenes/City/{cityName}";
        if (!Directory.Exists(cityPath))
        {
            Debug.LogError($"{TAG}: City by Path: {cityPath} is not exist");
            scenes = null;
            return false;
        }

        int scenesCount = 0;
        //scenesCount += isAdmin ? ADMIN_SCENES.Length : 0;
        scenesCount += ADMIN_SCENES.Length;
        scenesCount += COMMON_SCENES.Length;
        scenesCount += CITY_SCENES.Length;
        scenes = new string[scenesCount];

        int offset = 0;

        COMMON_SCENES.CopyTo(scenes, offset);
        offset += COMMON_SCENES.Length;

        //if (isAdmin)
        {
            ADMIN_SCENES.CopyTo(scenes, offset);
            offset += ADMIN_SCENES.Length;
        }

        int cityScenePosition = 0;
        for (int i = offset; i < scenes.Length; i++)
        {
            string scenePath = $"{cityPath}/{CITY_SCENES[cityScenePosition]}_{cityName}.unity";
            scenes[i] = scenePath;
            cityScenePosition++;
        }

        AddScenesToBuildEditor(scenes);
        return true;
    }

    private static bool SetAppSettings(string cityName,
                                       bool isAdmin,
                                       PlatformType platformType,
                                       int idClub,
                                       int idGame,
                                       string version,
                                       bool useVR,
                                       bool isDevBuild,
                                       bool useLogs)
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: settings is null or invalid path");
            return false;
        }

        settings.IdClub = idClub;
        settings.IdGame = idGame;
        settings.GameVersion = version;

        settings.AppData.City = cityName;
        settings.AppData.City = cityName;
        settings.AppData.IsAdmin = isAdmin;
        settings.AppData.Platform = platformType;
        settings.AppData.IsDevBuild = isDevBuild;
        settings.AppData.UseLogs = useLogs;
        settings.AppData.UseVR = useVR;

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        return true;
    }

    private static bool SetAppSettings(string cityName,
                                       bool isAdmin,
                                       PlatformType platformType,
                                       bool useVR,
                                       bool isDevBuild,
                                       bool useLogs)
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: city settings is null or invalid path");
            return false;
        }

        settings.AppData.City = cityName;
        settings.AppData.IsAdmin = isAdmin;
        settings.AppData.Platform = platformType;
        settings.AppData.IsDevBuild = isDevBuild;
        settings.AppData.UseLogs = useLogs;
        settings.AppData.UseVR = useVR;

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        return true;
    }

    private static bool CheckDirectory(PlatformType platformType,
                                       string outPath,
                                       string appName,
                                       out string fullPath)
    {
        switch (platformType)
        {
            case PlatformType.WINDOWS:
                appName += ".exe";
                break;
            case PlatformType.ANDROID:
            case PlatformType.META:
            case PlatformType.PICO:
                appName += ".apk";
                break;
            default:
                Debug.LogError($"{TAG}: is not valid platform");

                fullPath = null;
                return false;
        }

        //fullPath = $"{outPath}{appName}";
        fullPath = Path.Combine(outPath, appName);

        if (!Directory.Exists(outPath))
        {
            Debug.Log($"{TAG}: {outPath} is not exists. Try to create it.");

            DirectoryInfo info = Directory.CreateDirectory(outPath);
            if (info.Exists)
            {
                Debug.Log($"{TAG}: {outPath} is created: OK");
            }
            else
            {
                Debug.LogError($"{TAG}: {outPath} is not created: FAIL. Stop build.");
                return false;
            }
        }

        return true;
    }

    private static void AddScenesToBuildEditor(string[] scenes)
    {
        EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[scenes.Length];

        for (var i = 0; i < scenes.Length; i++)
        {
            editorBuildSettingsScenes[i] = new EditorBuildSettingsScene(scenes[i], true);
        }

        EditorBuildSettings.scenes = editorBuildSettingsScenes;
    }
    
    #region Console

    private static bool TryGetArgumentValue<T>(string name, out T value) where T : IConvertible
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(name) && (args.Length > i + 1))
            {
                try
                {
                    value = (T)Convert.ChangeType(args[i + 1], typeof(T));
                    return true;
                }
                catch (Exception e)
                {
                    value = default;
                    return false;
                }
            }
        }

        value = default;
        return false;
    }

    #endregion
}