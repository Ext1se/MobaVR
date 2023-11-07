using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class ExtensionSceneManager : MonoBehaviour
    {
        public static ExtensionSceneManager Instance;
        
        [SerializeField] private float m_DefaultDelay = 4f;
        
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
        
        private IEnumerator WaitAndLoadScene(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(sceneName);
        }
    }
}