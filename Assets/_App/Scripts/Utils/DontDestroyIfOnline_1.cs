using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class DontDestroyIfOnline_1 : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 0 && gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}