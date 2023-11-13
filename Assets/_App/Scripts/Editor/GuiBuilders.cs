using UnityEditor;
using UnityEngine;

namespace MobaVR
{
    [CustomEditor(typeof(BuildSettingGroup))]
    public class GuiBuilders : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Build"))
            {
                Build();
            }
        }

        public void Build()
        {
            BuildSettingGroup buildSettingGroup = serializedObject.targetObject as BuildSettingGroup;
            if (buildSettingGroup == null)
            {
                return;
            }

            
            foreach (BuildSetting buildSetting in buildSettingGroup.BuildSettings)
            {
                string cityName = buildSettingGroup.IsOverrideCity ? buildSettingGroup.CityName : buildSetting.AppData.City;
                string path = buildSetting.Path;
                if (buildSettingGroup.IsOverridePath)
                {
                    path = $"{buildSettingGroup.BasePath}{buildSetting.name}";
                }
                
                AppBuilder.Build(
                    //buildSetting.AppData.City,
                    cityName,
                    buildSetting.AppData.Platform.ToString(),
                    buildSetting.AppData.UseVR,
                    buildSetting.AppData.IsAdmin,
                    buildSetting.AppData.IsDevBuild,
                    buildSetting.AppData.UseLogs,
                    "1.0.0",
                    path,
                    buildSetting.Name);
            }
        }
    }
}