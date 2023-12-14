﻿using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

namespace MobaVR
{
    [CustomEditor(typeof(BuildSettingGroup))]
    public class GuiBuilders : OdinEditor
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

            string folderPath = EditorUtility.SaveFolderPanel(
                $"Build application", 
                "",
                "Arena Heroes");

            if (folderPath.IsNullOrEmpty() || folderPath.Length == 0)
            {
                return;
            }
            
            foreach (BuildSetting buildSetting in buildSettingGroup.BuildSettings)
            {
                string cityName = buildSettingGroup.IsOverrideCity ? buildSettingGroup.CityName : buildSetting.AppData.City;
                string roomName = buildSettingGroup.IsOverrideRoom ? buildSettingGroup.RoomName : buildSetting.AppData.Room;
                string path = $"{folderPath}/{buildSetting.name}";
                //if (buildSettingGroup.IsOverridePath)
                {
                  //  path = $"{buildSettingGroup.BasePath}{buildSetting.name}";
                }
                AppBuilder.Build(
                    //buildSetting.AppData.City,
                    cityName,
                    buildSetting.AppData.Platform.ToString(),
                    roomName,
                    buildSetting.AppData.UseVR,
                    buildSetting.AppData.IsAdmin,
                    buildSetting.AppData.IsDevBuild,
                    buildSetting.AppData.UseLogs,
                    Application.version,
                    path,
                    buildSetting.Name);
            }
        }
    }
}