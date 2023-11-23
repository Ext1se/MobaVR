using Photon.Realtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    [CustomEditor(typeof(AppSetting))]
    public class GuiAppSettings : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(32);
            GUILayout.Label("Click to find and add new scenes by City Name to Build Settings");
            if (GUILayout.Button("Update Scenes for Build Settings"))
            {
                AddScene();
            }
            
            GUILayout.Space(32);
            GUILayout.Label("Click to update photon components in all scenes");
            
            if (GUILayout.Button("Open all Scenes"))
            {
                OpenScenes();
            }

            if (GUILayout.Button("Save and Close Open Scenes"))
            {
                SaveScenes();
            }
        }

        private void AddScene()
        {
            AppSetting appSetting = serializedObject.targetObject as AppSetting;
            if (appSetting == null)
            {
                return;
            }

            if (AppBuilder.SetScenes(appSetting.AppData.City,
                                     appSetting.AppData.Platform,
                                     appSetting.AppData.IsAdmin,
                                     out string[] scenes,
                                     false))
            {
                AppBuilder.AddScenesToBuildEditor(scenes);
            }
        }
        
        
        private void OpenScenes()
        {
            AppSetting appSetting = serializedObject.targetObject as AppSetting;
            if (appSetting == null)
            {
                return;
            }

            if (AppBuilder.SetScenes(appSetting.AppData.City,
                                     appSetting.AppData.Platform,
                                     appSetting.AppData.IsAdmin,
                                     out string[] scenes,
                                     false))
            {
                bool[] openScenes = new bool [scenes.Length];

                for (var i = 0; i < scenes.Length; i++)
                {
                    string scenePath = scenes[i];
                    Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
                    openScenes[i] = EditorSceneManager.IsPreviewScene(scene);
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
            }
        }

        private void SaveScenes()
        {
            AppSetting appSetting = serializedObject.targetObject as AppSetting;
            if (appSetting == null)
            {
                return;
            }

            if (AppBuilder.SetScenes(appSetting.AppData.City,
                                     appSetting.AppData.Platform,
                                     appSetting.AppData.IsAdmin,
                                     out string[] scenes,
                                     false))
            {
                for (var i = 0; i < scenes.Length; i++)
                {
                    string scenePath = scenes[i];
                    Scene scene = EditorSceneManager.GetSceneByPath(scenePath);

                    EditorSceneManager.SaveScene(scene);

                    if (i > 0)
                    {
                        EditorSceneManager.CloseScene(scene, false);
                    }
                }

                AssetDatabase.SaveAssets();
            }
        }
        
        private void UpdatePhoton(bool isAutoSave)
        {
            AppSetting appSetting = serializedObject.targetObject as AppSetting;
            if (appSetting == null)
            {
                return;
            }

            if (AppBuilder.SetScenes(appSetting.AppData.City,
                                     appSetting.AppData.Platform,
                                     appSetting.AppData.IsAdmin,
                                     out string[] scenes,
                                     false))
            {
                bool[] openScenes = new bool [scenes.Length];

                for (var i = 0; i < scenes.Length; i++)
                {
                    string scenePath = scenes[i];
                    Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
                    openScenes[i] = EditorSceneManager.IsPreviewScene(scene);
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }

                if (isAutoSave)
                {
                    for (var i = 0; i < scenes.Length; i++)
                    {
                        string scenePath = scenes[i];
                        Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
                        EditorSceneManager.SaveScene(scene);
                    }

                    bool isSuccess = EditorSceneManager.SaveOpenScenes();
                    AssetDatabase.SaveAssets();

                    for (var i = 0; i < scenes.Length; i++)
                    {
                        string scenePath = scenes[i];
                        Scene scene = EditorSceneManager.GetSceneByPath(scenePath);

                        if (!openScenes[i])
                        {
                            EditorSceneManager.CloseScene(scene, false);
                        }
                    }

                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}