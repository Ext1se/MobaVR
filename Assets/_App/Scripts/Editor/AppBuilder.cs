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

public class AppBuilder
{
    private const string TAG = nameof(AppBuilder);

    private const string OCULUS_LOADER = "Unity.XR.Oculus.OculusLoader";
    private const string OPENXR_LOADER = "UnityEngine.XR.OpenXR.OpenXRLoader";
    
    public static readonly string DEFAULT_OUT_PATH = "Builds/";
    public static readonly string DEFAULT_NAME = "Heroes Arena";
    public static readonly string CITY_PATH = "Assets/_App/Resources/Api/Settings/AppSettingCity.asset";

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
        //"SkyArena",
        //"SkyArena_Rift",
        //"Dungeon",
        
        //PVE
        //"Necropolis",
        
        //TD
        //"Tower",
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cityName"></param>
    /// <param name="targetName"></param>
    /// <param name="isAdmin"></param>
    /// <param name="outPath">Example: "C:\Builds</param>
    //public static void Build(string cityName, BuildTarget target, bool isAdmin = false, string outPath = null)
    public static void Build(
        string cityName, 
        string targetName, 
        bool useVR = false,
        bool isAdmin = false, 
        bool isDevelopmentBuild = false, 
        bool useLogs = false, 
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

        TryBuild(targetName, !isAdmin, fullPath, scenes);
    }

    private static void TryBuild(string targetName, bool useVR, string outPath, string[] scenes)
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
                    xrTypes.Add(typeof(OculusTouchControllerProfile));
                    xrTypes.Add(typeof(PICOTouchControllerProfile));
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

        EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
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
        

        Debug.Log($"{TAG}: start to build");

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"{TAG}: build is completed. OK.");
        }
        else
        {
            Debug.LogError($"{TAG}: build is failed");
        }
    }

    private static void TryBuild(BuildTarget target, string outPath, string[] scenes)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outPath,
            target = target,
            options = BuildOptions.None
        };

        Debug.LogError($"{TAG}: start to build");

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.LogError($"{TAG}: build is completed. OK.");
        }
        else
        {
            Debug.LogError($"{TAG}: build is failed");
        }
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

    private static bool SetAppSettings(string cityName, bool isAdmin, PlatformType platformType, bool useVR, bool isDevBuild, bool useLogs)
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
        settings.AppData.IsDevelopmentBuild = isDevBuild;
        settings.AppData.UseLogs = useLogs;
        settings.AppData.UseVR = useVR;

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        return true;
    }
    
    private static bool CheckDirectory(PlatformType platformType, string outPath, string appName, out string fullPath)
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

        fullPath = $"{outPath}{appName}";
        
        if (!Directory.Exists(outPath))
        {
            Debug.Log($"{TAG}: {fullPath} is not exists. Try to create it.");

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
}