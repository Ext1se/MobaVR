using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class DontDestroyIfOnline : MonoBehaviour
    {
        [SerializeField] private string m_DestroySceneName = "Menu";

        private void Awake()
        {
            //m_ParentSceneName = SceneManager.GetActiveScene().name;
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
            /*
            if (scene.buildIndex == 0 && gameObject != null)
            {
                Destroy(gameObject);
            }
            */

            if (scene.name.Equals(m_DestroySceneName) && gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}