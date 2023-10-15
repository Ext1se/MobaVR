using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class DontDestroyMode : MonoBehaviour
    {
        [SerializeField] private GameObject m_DontDestroyGroup;
        
        [Header("DefaultScene")]
        [SerializeField] private bool m_IsLoadDefaultScene = false;
        [SerializeField] private string m_DefaultSceneName = "SkyArea";
        
        //[SerializeField] private GameObject m_DontDestroyObject;

        private void Awake()
        {
            GameObject oldGameObject = GameObject.Find(gameObject.name);
            if (oldGameObject != null && oldGameObject != gameObject)
            {
                Destroy(gameObject);
                return;
            }
            
            if (PhotonNetwork.IsConnected)
            {
                DontDestroyOnLoad(gameObject);
                m_DontDestroyGroup.SetActive(true);

                //PhotonNetwork.AutomaticallySyncScene = false;
                PhotonNetwork.AutomaticallySyncScene = true;
                if (PhotonNetwork.IsMasterClient && m_IsLoadDefaultScene)
                {
                    PhotonNetwork.LoadLevel(m_DefaultSceneName);
                }
                //SceneManager.LoadSceneAsync("SkyArena");
                
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
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