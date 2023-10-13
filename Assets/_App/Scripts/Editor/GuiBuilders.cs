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
                AppBuilder.Build(
                    buildSetting.City,
                    buildSetting.Platform.ToString(),
                    buildSetting.IsAdmin,
                    buildSetting.IsDevelopmentBuild,
                    buildSetting.Path,
                    buildSetting.Name);
            }
        }
    }
}