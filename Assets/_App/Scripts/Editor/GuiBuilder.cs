using UnityEditor;
using UnityEngine;

namespace MobaVR
{
    [CustomEditor(typeof(BuildSetting))]
    public class GuiBuilder : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(32);
            
            if (GUILayout.Button("Build Selected Platform"))
            {
                Build();
            }

            if (GUILayout.Button("Build All Platform"))
            {
                Build(PlatformType.WINDOWS);
                Build(PlatformType.ANDROID);
                Build(PlatformType.META);
                Build(PlatformType.PICO);
            }
            
            GUILayout.Space(32);


            if (GUILayout.Button("Build Windows 64"))
            {
                Build(PlatformType.WINDOWS);
            }
            

            if (GUILayout.Button("Build Android"))
            {
                Build(PlatformType.ANDROID);
            }

            if (GUILayout.Button("Build Meta"))
            {
                Build(PlatformType.META);
            }

            if (GUILayout.Button("Build Pico"))
            {
                Build(PlatformType.PICO);
            }
        }

        public void Build(BuildSetting buildSetting)
        {
            AppBuilder.Build(
                buildSetting.AppData.City,
                buildSetting.AppData.Platform.ToString(),
                buildSetting.AppData.UseVR,
                buildSetting.AppData.IsAdmin,
                buildSetting.AppData.IsDevBuild,
                buildSetting.AppData.UseLogs,
                buildSetting.Path,
                buildSetting.Name);
        }

        public void Build()
        {
            BuildSetting buildSetting = serializedObject.targetObject as BuildSetting;
            if (buildSetting == null)
            {
                return;
            }

            Build(buildSetting);
        }

        public void Build(PlatformType platformType)
        {
            BuildSetting buildSetting = serializedObject.targetObject as BuildSetting;
            if (buildSetting == null)
            {
                return;
            }

            buildSetting.AppData.Platform = platformType;
            Build(buildSetting);
        }
    }
}