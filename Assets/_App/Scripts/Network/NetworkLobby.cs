using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class NetworkLobby : MonoBehaviourPunCallbacks
    {
        [Header("Photon")]
        [SerializeField] private string m_SceneName = "Room";
        [SerializeField] private byte m_MaxPlayersPerRoom = 12;
        [SerializeField] private string m_RoomName = "MobaVR";
        [SerializeField] private string m_GameVersion = "1";
        [SerializeField] private bool m_IsGetOnlineFromPlayerPrefs = true;
        [SerializeField] private bool m_GameOnline;
        [SerializeField] private string ipServ;
        [SerializeField] private bool isAutoConnect = true;
        
        [Header("Settings")]
        [SerializeField] private AppPhotonSettingsSO m_Settings;
        [SerializeField] public AppSetting appSettings;
        [SerializeField] public string m_MenuScene = "Menu";
        
        private bool m_IsConnecting = false;

        private LocalRepository localRepository;
        private RoomOptions roomOptions;

        public UnityEvent OnRoomJoined;
        public UnityEvent OnRoomDisconnected;

        #region Photon

        private void Awake()
        {
            localRepository = new LocalRepository();
            if (m_IsGetOnlineFromPlayerPrefs)
            {
                m_GameOnline = !localRepository.IsLocalServer;
                if (!m_GameOnline)
                {
                    string savedIPAddress = localRepository.LastIPAddress;
                    if (!string.IsNullOrEmpty(savedIPAddress))
                    {
                        ipServ = savedIPAddress;
                    }
                    else
                    {
                        BackToMenu();
                        return;
                    }
                }
            }

            PhotonNetwork.NetworkingClient.SerializationProtocol = SerializationProtocol.GpBinaryV16;
            // PhotonNetwork.OfflineMode = true;

            roomOptions = new RoomOptions()
            {
                MaxPlayers = m_MaxPlayersPerRoom,
            };
        }

        private void Start()
        {
            if (isAutoConnect)
            {
                Connect();
            }
        }

        private void BackToMenu()
        {
            //SceneManager.LoadScene(0);
            SceneManager.LoadScene(m_MenuScene);
        }

        private void JoinOrCreateRoom()
        {
            if (appSettings.AppData.IsDevelopmentBuild)
            {
                m_RoomName += "_Dev";
                PhotonNetwork.JoinOrCreateRoom(m_RoomName,
                                               roomOptions,
                                               TypedLobby.Default);
            }
            else
            {
                if (appSettings.AppData.IsAdmin)
                {
                    //PhotonNetwork.CreateRoom(m_RoomName,
                    PhotonNetwork.JoinOrCreateRoom(m_RoomName,
                                                   roomOptions,
                                                   TypedLobby.Default);
                }
                else
                {
                    PhotonNetwork.JoinRoom(m_RoomName);
                }
            }
        }

        public void Connect(string username = null)
        {
            m_IsConnecting = true;

            if (PhotonNetwork.IsConnected)
            {
                JoinOrCreateRoom();
            }
            else
            {
                if (username != null)
                {
                    PhotonNetwork.NickName = username;
                }

                if (m_GameOnline)
                {
                    Debug.Log("Запускаем онлайн");
                    PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = m_Settings.OnlineKey;
                    PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
                    PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
                    PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    Debug.Log("Запускаем локальный, через IP " + ipServ);
                    //PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "1234567890-1234567890-1234567890";
                    PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = m_Settings.OfflineKey;
                    PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
                    // PhotonNetwork.PhotonServerSettings.AppSettings.Server = "LocalServer";
                    PhotonNetwork.PhotonServerSettings.AppSettings.Server = ipServ;
                    PhotonNetwork.ConnectUsingSettings();
                    //PhotonNetwork.ConnectToMaster("192.168.0.182", 5055, "1");
                }

                // PhotonNetwork.ConnectUsingSettings();

                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.OfflineMode = false;
                PhotonNetwork.GameVersion = m_GameVersion;
                //PhotonNetwork.UseRpcMonoBehaviourCache = true;
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            if (m_IsConnecting)
            {
                Debug.Log(
                    $"{name}: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
                JoinOrCreateRoom();
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log($"{name}: Launcher:OnJoinRoomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            BackToMenu();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            Debug.Log($"{name}: Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            /*
            PhotonNetwork.CreateRoom(m_RoomName,
                                     new RoomOptions()
                                     roomOptions);
            */
            
            BackToMenu();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.LogError($"{name}: Launcher:Disconnected");

            m_IsConnecting = false;
            OnRoomDisconnected?.Invoke();
            BackToMenu();
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

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log(
                $"{name}: Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

            OnRoomJoined?.Invoke();
            LoadCityScene(m_SceneName); // Здесь мы вызываем новый метод
        }

        private void LoadCityScene(string baseSceneName)
        {
            string sceneName = $"{baseSceneName}_{appSettings.AppData.City}";
            PhotonNetwork.LoadLevel(sceneName);
        }

        #endregion
    }
}