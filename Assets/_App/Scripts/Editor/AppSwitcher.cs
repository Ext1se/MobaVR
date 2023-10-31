using MobaVR;
using UnityEditor;
using UnityEngine;

public class AppSwitcher
{
    private const string TAG = nameof(AppSwitcher);

    public static void SwitchAdminMode(bool isAdmin)
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(AppBuilder.CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: city settings is null or invalid path");
            return;
        }

        settings.AppData.IsAdmin = isAdmin;
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();

        EditorBuildSettingsScene[] editorBuildSettingsScenes = EditorBuildSettings.scenes;
        foreach (EditorBuildSettingsScene editorBuildSettingsScene in editorBuildSettingsScenes)
        {
            foreach (string adminScene in AppBuilder.ADMIN_SCENES)
            {
                if (editorBuildSettingsScene.path.Equals(adminScene))
                {
                    editorBuildSettingsScene.enabled = isAdmin;
                }
            }
        }

        EditorBuildSettings.scenes = editorBuildSettingsScenes;
    }

    [MenuItem("MobaVR/Mode/Set Admin")]
    public static void SetAdmin()
    {
        SwitchAdminMode(true);
    }

    [MenuItem("MobaVR/Mode/Set Client")]
    public static void SetClient()
    {
        SwitchAdminMode(false);
    }

    private static void SwitchDevelopmentMode(bool isDevelopment)
    {
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(AppBuilder.CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: city settings is null or invalid path");
            return;
        }

        settings.AppData.IsDevelopmentBuild = isDevelopment;
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("MobaVR/Mode/Set Development")]
    public static void SetDevelopmentMode()
    {
        SwitchDevelopmentMode(true);
    }

    [MenuItem("MobaVR/Mode/Set Release")]
    public static void SetReleaseMode()
    {
        SwitchDevelopmentMode(false);
    }

    [MenuItem("MobaVR/Mode/Select App Settings", priority = 1)]
    public static void SelectAppSettingsFile()
    {
        SwitchDevelopmentMode(false);
        
        AppSetting settings = AssetDatabase.LoadAssetAtPath<AppSetting>(AppBuilder.CITY_PATH);
        if (settings == null)
        {
            Debug.LogError($"{TAG}: city settings is null or invalid path");
            return;
        }

        EditorGUIUtility.PingObject(settings);
        Selection.activeObject = settings;
    }
}