using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class DontDestroyMode : MonoBehaviourPunCallbacks
    {
        [SerializeField] private bool m_IsCreateOnAwake = false;
        [SerializeField] private GameObject m_DontDestroyGroup;
        [SerializeField] private string m_DestroySceneName = "Menu";

        [Header("DefaultScene")]
        [SerializeField] private bool m_IsLoadDefaultScene = false;
        [SerializeField] private string m_DefaultSceneName = "SkyArea";

        //[SerializeField] private GameObject m_DontDestroyObject;

        private void Awake() 
        {
            if (m_IsCreateOnAwake)
            {
                CreateGameSession();
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            //if (scene.buildIndex == 0 && gameObject != null)
            if (scene.name.Equals(m_DestroySceneName) && gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        public void CreateGameSession()
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

        public override void OnJoinedRoom()
        {
            CreateGameSession();
            base.OnJoinedRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Destroy(gameObject);
            base.OnDisconnected(cause);
        }
    }
}