using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class ScenesEnvironment : MonoBehaviourPun
    {
        [Header("Maps")]
        [SerializeField] private string m_SkyLandMap;
        [SerializeField] private string m_SkyLandProps;
        [SerializeField] private string m_MobaMap;
        [SerializeField] private string m_LichMap;
        [SerializeField] private string m_NecropolisMap;
        [SerializeField] private string m_TavernMap;
        [SerializeField] private string m_TowerDemoMap = "Tower_Demo";
        [SerializeField] private string m_TowerMap = "Tower";
        [SerializeField] private string m_DungeonMap = "Dungeon";

        [Header("Default Map")]
        [SerializeField] private bool m_IsLoadDefaultMap;
        [SerializeField] private string m_DefaultMap;
        /*
        [SerializeField] private SceneAsset m_SkyLandMap;
        [SerializeField] private SceneAsset m_SkyLandProps;
        [SerializeField] private SceneAsset m_MobaMap;
        [SerializeField] private SceneAsset m_LichMap;
        [SerializeField] private SceneAsset m_TavernMap;
        */
        private string m_CurrentMap;
        private bool m_CanLoad = true;
        
        public UnityEvent OnShow;
        public UnityEvent OnHide;

        private void Awake()
        {
            ClearScenes();
        }

        //TODO: Remove this
        private void OnEnable()
        {
            //SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            //SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
        }

        //TODO: Remove this
        private void OnDisable()
        {
            //SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
            //SceneManager.sceneUnloaded -= SceneManagerOnSceneUnloaded;
        }

        private void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            if (!string.IsNullOrEmpty(m_CurrentMap))
            {
                SceneManager.LoadSceneAsync(m_CurrentMap, LoadSceneMode.Additive);
            }
        }

        private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name.Equals(m_DefaultMap))
            {
                m_CanLoad = true;
                SceneManager.SetActiveScene(scene);
            }
        }

        private void ClearScenes()
        {
            if (SceneManager.sceneCount > 0)
            {
                for (int i = SceneManager.sceneCount - 1; i >= 1; i--)
                {
                    //SceneManager.UnloadSceneAsync(i);
                    //SceneManager.UnloadSceneAsync(1);
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                }
            }
        }

        private void Start()
        {
            if (m_IsLoadDefaultMap)
            {
                LoadSceneAsync(m_DefaultMap);
            }
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            /*
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!PhotonNetwork.AutomaticallySyncScene)
                {
                    photonView.RPC(nameof(RpcShowTavernMap), RpcTarget.All);
                }

                else
                {
                    RpcShowTavernMap();
                }
                //LoadScene(m_SkyLandMap);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (!PhotonNetwork.AutomaticallySyncScene)
                {
                    photonView.RPC(nameof(RpcShowSkyLandMap), RpcTarget.All);
                }

                else
                {
                    RpcShowSkyLandMap();
                }

                //LoadScene(m_TavernMap);
            }
            */
        }

        private void LoadScene(string sceneName)
        {
            if (!m_CanLoad)
            {
                return;
            }

            /*
            if (!string.IsNullOrEmpty(m_CurrentMap))
            {
                SceneManager.UnloadSceneAsync(m_CurrentMap);
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName);
            }
            */
            
            //PhotonNetwork.scene

            SceneManager.LoadSceneAsync(sceneName);

            m_CurrentMap = sceneName;
        }

        private void LoadSceneAsync(string sceneName)
        {
            PhotonNetwork.LoadLevel(sceneName);
            m_CurrentMap = sceneName;
        }
        
        private void LoadSceneAsync1(string sceneName)
        {
            if (!m_CanLoad)
            {
                return;
            }

            m_CanLoad = false;

            if (!string.IsNullOrEmpty(m_CurrentMap))
            {
                SceneManager.UnloadSceneAsync(m_CurrentMap);
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            m_CurrentMap = sceneName;
        }
        
        public void ShowMap(string mapName)
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowMap), RpcTarget.All, mapName);
            }
            else
            {
                RpcShowMap(mapName);
            }
        }
        
        [PunRPC]
        public void RpcShowMap(string mapName)
        {
            LoadSceneAsync(mapName);
        }
        
        [ContextMenu("Show Tavern")]
        public void ShowTavernMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowTavernMap), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(SetProps), RpcTarget.All, false, false);
                RpcShowTavernMap();
            }
        }
        
        [PunRPC]
        public void RpcShowTavernMap()
        {
            //LoadSceneAsync(m_TavernMap.name);
            LoadSceneAsync(m_TavernMap);
            SetProps(false, false);
        }

        [ContextMenu("Show Sky Land")]
        public void ShowSkyLandMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowSkyLandMap), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(SetProps), RpcTarget.All, true, false);
                RpcShowSkyLandMap();
            }
        }
        
        [PunRPC]
        public void RpcShowSkyLandMap()
        {
            //LoadSceneAsync(m_SkyLandMap.name);
            LoadSceneAsync(m_SkyLandMap);
            SetProps(true, false);

        }

        [ContextMenu("Show Sky Land Props")]
        public void ShowSkyLandWithPropMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowSkyLandWithPropMap), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(SetProps), RpcTarget.All, true, false);
                RpcShowSkyLandWithPropMap();
            }
        }
        
        [PunRPC]
        public void RpcShowSkyLandWithPropMap()
        {
            //LoadSceneAsync(m_SkyLandProps.name);
            LoadSceneAsync(m_SkyLandProps);
            SetProps(true, false);
        }

        [ContextMenu("Show Moba")]
        public void ShowMobaMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowMobaMap), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(SetProps), RpcTarget.All, true, false);
                RpcShowMobaMap();
            }
        }
        
        [PunRPC]
        public void SetProps(bool isPvp, bool isPve)
        {
            //m_PvpProps.SetActive(isPvp);
            //m_PveProps.SetActive(isPve);
        }
        
        [PunRPC]
        public void RpcShowMobaMap()
        {
            //LoadSceneAsync(m_MobaMap.name);
            LoadSceneAsync(m_MobaMap);
            SetProps(true, false);
        }
        
        [ContextMenu("Show Lich")]
        public void ShowLichMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowLichMap), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(SetProps), RpcTarget.All, false, true);
                RpcShowLichMap();
            }
        }
        
        [PunRPC]
        public void RpcShowLichMap()
        {
            SetProps(false, true);
            //m_Settings.SetNight();
            //LoadSceneAsync(m_LichMap.name);
            LoadSceneAsync(m_LichMap);
        }
        
        [ContextMenu("Show Necropolis")]
        public void ShowNecropolisMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowLichMap), RpcTarget.All);
            }
            else
            {
                RpcShowNecropolisMap();
            }
        }
        
        [PunRPC]
        public void RpcShowNecropolisMap()
        {
            LoadSceneAsync(m_NecropolisMap);
        }
        
        [ContextMenu("Show Tower")]
        public void ShowTowerMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowTower), RpcTarget.All);
            }
            else
            {
                RpcShowTower();
            }
        }
        
        [PunRPC]
        public void RpcShowTower()
        {
            LoadSceneAsync(m_TowerMap);
        }

        [ContextMenu("Show Demo Tower")]
        public void ShowTowerDemoMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowDemoTower), RpcTarget.All);
            }
            else
            {
                RpcShowDemoTower();
            }
        }
        
        [PunRPC]
        public void RpcShowDemoTower()
        {
            LoadSceneAsync(m_TowerDemoMap);
        }
        
        [ContextMenu("Show Dungeon")]
        public void ShowDungeonMap()
        {
            if (!PhotonNetwork.AutomaticallySyncScene)
            {
                photonView.RPC(nameof(RpcShowDungeonMap), RpcTarget.All);
            }
            else
            {
                RpcShowDungeonMap();
            }
        }
        
        [PunRPC]
        public void RpcShowDungeonMap()
        {
            LoadSceneAsync(m_DungeonMap);
        }
        
        public void ShowDefaultPvPMap()
        {
            ShowSkyLandMap();
        }
        
        [PunRPC]
        public void RpcShowDefaultPvPMap()
        {
            ShowSkyLandMap();
        }

        public void ShowDefaultPvEMap()
        {
            ShowLichMap();
        }
        
        [PunRPC]
        public void RpcShowDefaultPvEMap()
        {
            ShowLichMap();
        }
    }
}