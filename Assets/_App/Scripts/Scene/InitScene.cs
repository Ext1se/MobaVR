using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class InitScene : MonoBehaviour
    {
        [SerializeField] private AppSetting m_AppSetting;
        [SerializeField] private string m_NextClientScene = "Menu";
        [SerializeField] private string m_NextAdminScene = "Login";

        [SerializeField] private float m_Delay = 2f;
        
        private void Start()
        {
            Invoke(nameof(LoadNextScene), m_Delay);
        }

        private void LoadNextScene()
        {
            string nextScene = m_AppSetting.AppData.IsAdmin ? m_NextAdminScene : m_NextClientScene;
            SceneManager.LoadScene(nextScene);
        }
    }
}