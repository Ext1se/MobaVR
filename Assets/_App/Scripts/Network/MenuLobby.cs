using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class MenuLobby : MonoBehaviourPunCallbacks
    {
        private static string TAG = nameof(MenuLobby);

        [Header("Scenes")]
        [SerializeField] private string m_GameScene = "Tavern";
        [SerializeField] public string m_LobbyScene = "Menu";

        [Header("Photon Room")]
        [SerializeField] private byte m_MaxPlayersPerRoom = 12;
        [SerializeField] private string m_GameVersion = "1";

        [Header("Server Settings")]
        [SerializeField] private AppPhotonSettingsSO m_Settings;
        [SerializeField] public AppSetting appSettings;

        [Space]
        [SerializeField] [ReadOnly] private string m_RoomName = "MobaVR";
        [SerializeField] [ReadOnly] private bool m_IsGameOnline;
        [SerializeField] [ReadOnly] private string m_IpServer;

        private bool m_IsConnecting = false;
        private LocalRepository m_LocalRepository;
        private RoomOptions m_RoomOptions;

        [Space]
        public UnityEvent OnRoomJoined;
        public UnityEvent OnRoomDisconnected;

        private void Awake()
        {
            m_RoomName = appSettings.AppData.City;

            PhotonNetwork.NetworkingClient.SerializationProtocol = SerializationProtocol.GpBinaryV16;
            PhotonNetwork.EnableCloseConnection = true;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.GameVersion = m_GameVersion;

            m_RoomOptions = new RoomOptions()
            {
                MaxPlayers = m_MaxPlayersPerRoom,
            };
        }

        #region Scenes

        private void LoadGameScene(string baseSceneName)
        {
            string sceneName = $"{baseSceneName}_{appSettings.AppData.City}";
            PhotonNetwork.LoadLevel(sceneName);
        }

        private void WaitAndLoadMenuScene()
        {
            if (ExtensionSceneManager.Instance != null)
            {
                ExtensionSceneManager.Instance.LoadScene(m_LobbyScene, 4f);
                //ExtensionSceneManager.Instance.FadeAndLoadScene(m_LobbyScene, 4f);
            }
            else
            {
                Invoke(nameof(LoadMenuScene), 4f);
            }
        }

        private void LoadMenuScene()
        {
            SceneManager.LoadScene(m_LobbyScene);
        }
        
        #endregion

        private void JoinOrCreateRoom()
        {
            if (appSettings.AppData.IsDevBuild)
            {
                m_RoomName += "_Dev";
                PhotonNetwork.JoinOrCreateRoom(m_RoomName,
                                               m_RoomOptions,
                                               TypedLobby.Default);
            }
            else
            {
                if (appSettings.AppData.IsAdmin)
                {
                    PhotonNetwork.JoinOrCreateRoom(m_RoomName,
                                                   m_RoomOptions,
                                                   TypedLobby.Default);
                }
                else
                {
                    PhotonNetwork.JoinRoom(m_RoomName);
                }
            }
        }

        public void ConnectOnlineMode()
        {
            Debug.Log($"{TAG}: Online mode");

            m_IsConnecting = true;

            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = m_Settings.OnlineKey;
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
            PhotonNetwork.ConnectUsingSettings();
        }

        public void ConnectLocalMode(string ipAddress)
        {
            Debug.Log($"{TAG}: Local mode. IP = {m_IpServer}");

            m_IsConnecting = true;
            m_IpServer = ipAddress;

            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = m_Settings.OfflineKey;
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = m_IpServer;
            PhotonNetwork.ConnectUsingSettings();
        }

        #region Photon Callbacks

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            if (m_IsConnecting)
            {
                Debug.Log($"{TAG}: OnConnectedToMaster");
                JoinOrCreateRoom();
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log($"{TAG}: OnJoinRoomFailed");
            //BackToMenu();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            Debug.Log($"{TAG}: OnJoinRandomFailed");
            //BackToMenu();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.LogError($"{TAG}: OnDisconnected");

            m_IsConnecting = false;
            OnRoomDisconnected?.Invoke();
            
            WaitAndLoadMenuScene();
        }

        /*public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log(
                $"{name}: Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
            //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel(m_SceneName);
            }
        }*/

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log($"{TAG}: OnJoinedRoom");

            OnRoomJoined?.Invoke();
            LoadGameScene(m_GameScene);
        }

        #endregion
    }
}