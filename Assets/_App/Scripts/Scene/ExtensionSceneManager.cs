using System.Collections;
using BNG;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class ExtensionSceneManager : MonoBehaviour
    {
        public static ExtensionSceneManager Instance;

        [SerializeField] private float m_ScreenFadeTime = 0.5f;
        [SerializeField] private float m_DefaultDelay = 4f;

        private ScreenFader m_ScreenFader;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName, float delay = 0f)
        {
            if (delay > 0)
            {
                StartCoroutine(WaitAndLoadScene(sceneName, delay));
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void FadeAndLoadScene(string sceneName, float delay = 0f)
        {
            m_ScreenFader = FindObjectOfType<ScreenFader>();
            if (m_ScreenFader != null)
            {
                StartCoroutine(FadeAndLoadSceneCoroutine(sceneName, delay));
            }
            else
            {
                LoadScene(sceneName, delay);
            }
        }

        private IEnumerator FadeAndLoadSceneCoroutine(string sceneName, float delay)
        {
            if (m_ScreenFader != null)
            {
                m_ScreenFader.DoFadeIn();
            }

            yield return new WaitForSeconds(delay);
            yield return new WaitForSeconds(m_ScreenFadeTime);

            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator WaitAndLoadScene(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(sceneName);
        }
    }
}