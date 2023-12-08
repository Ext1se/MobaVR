using System;
using System.Collections.Generic;
using MetaConference;
using MobaVR.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace MobaVR
{
    public class ClassicGameSession : BaseGameSession
    {
        //[Header("Network")]
        //[SerializeField] private NetworkSession m_NetworkSession;

        [Header("Modes")]
        [FormerlySerializedAs("m_Mode")]
        //[SerializeField] private GameMode m_ClassicMode;
        [SerializeField] private GameMode m_Mode;
        //[SerializeField] private PveMode m_LichMode;
        //[SerializeField] private LichGame m_LichMode;
        //[SerializeField] private Environment m_Environment;
        public ManagerDevice managerDevice; // Ссылка на объект ManagerDevice

        [Header("Player")]
        [SerializeField] private BasePlayerSpawner<PlayerVR> m_PlayerSpawner;
        [SerializeField] private Team m_RedTeam;
        [SerializeField] private Team m_BlueTeam;

        private List<PlayerVR> m_Players = new();
        private bool m_IsPvPMode = true;
        //private PlayerVR m_LocalPlayer;

        //public PlayerVR LocalPlayer => m_LocalPlayer;
        public Team RedTeam => m_RedTeam;
        public Team BlueTeam => m_BlueTeam;
        public bool IsPvPMode => m_IsPvPMode;
        public GameMode Mode
        {
            get => m_Mode;
            set => m_Mode = value;
        }
        public List<PlayerVR> Players => m_Players;
        public List<PlayerVR> TeamPlayers
        {
            get
            {
                List<PlayerVR> players = new List<PlayerVR>();
                players.AddRange(RedTeam.Players);
                players.AddRange(BlueTeam.Players);

                return players;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }


        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void Awake()
        {
            //managerDevice = FindObjectOfType<ManagerDevice>();
            //TODO: 
            /*
            PhotonCustomHitData.Register();
            #if !UNITY_EDITOR
                CustomComposites.Init();
            #endif
            */
        }

        private void Start()
        {
            if (managerDevice == null)
            {
                managerDevice = FindObjectOfType<ManagerDevice>();
            }
            /*
            if (managerDevice != null && managerDevice.CanCreatePlayer) // Проверяем, нужно ли создавать игрока
            {
                Invoke(nameof(InitPlayer), 2f);
            }
            */
        }

        #region Scenes

        private void OnSceneUnloaded(Scene arg0)
        {
            if (m_Mode != null)
            {
                m_Mode.CompleteMode();
                m_Mode = null;
            }
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            m_Mode = FindObjectOfType<GameMode>();
        }

        #endregion

        #region Connection
        
        public void CloseGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //PhotonNetwork.EnableCloseConnection = true;
                //PhotonNetwork.CurrentRoom.IsOpen = false;
                //PhotonNetwork.CurrentRoom.PlayerTtl = 0;
                //PhotonNetwork.CurrentRoom.EmptyRoomTtl = 0;

                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (Equals(player, PhotonNetwork.LocalPlayer))
                    {
                        continue;
                    }
                    
                    //PhotonNetwork.CloseConnection(player);
                }

                //PhotonNetwork.CloseConnection(PhotonNetwork.LocalPlayer);
                //PhotonNetwork.Disconnect();
            }
            
            photonView.RPC(nameof(RpcDisconnect), RpcTarget.All);
        }

        [PunRPC]
        private void RpcDisconnect()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Invoke(nameof(WaitAndDisconnect), 5f);
            }
            else
            {
                Invoke(nameof(WaitAndDisconnect), 0f);
            }
        }

        private void WaitAndDisconnect()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            //SceneManager.LoadScene("Menu");
        }

        #endregion

        #region Player and Team

        [PunRPC]
        private void RpcAddPlayer(int idPhotonView)
        {
            if (PhotonViewExtension.TryGetComponent(idPhotonView, out PlayerVR player))
            {
                if (!m_Players.Contains(player))
                {
                    OnAddPlayer?.Invoke(player);
                    m_Players.Add(player);
                }
            }
        }

        [PunRPC]
        private void RpcRemovePlayer(int idPhotonView)
        {
            if (PhotonViewExtension.TryGetComponent(idPhotonView, out PlayerVR player))
            {
                OnRemovePlayer?.Invoke(player);
                m_Players.Remove(player);
            }
        }

        private void InitPlayer()
        {
            //if (!photonView.IsMine)
            if (m_LocalPlayer != null)
            {
                return;
            }

            Team team = m_BlueTeam.Players.Count > m_RedTeam.Players.Count ? m_RedTeam : m_BlueTeam;
            m_LocalPlayer = m_PlayerSpawner.SpawnPlayer(team);
            m_Player = m_LocalPlayer.gameObject;
            team.AddPlayer(m_LocalPlayer);

            photonView.RPC(nameof(RpcAddPlayer), RpcTarget.AllBuffered, m_LocalPlayer.photonView.ViewID);
        }

        public void SwitchTeam()
        {
            if (m_LocalPlayer == null)
            {
                return;
            }

            if (m_LocalPlayer.Team.TeamType == TeamType.RED)
            {
                m_RedTeam.RemovePlayer(m_LocalPlayer);
                m_BlueTeam.AddPlayer(m_LocalPlayer);

                //m_LocalPlayer.SetTeamAndSync(m_BlueTeam);
            }
            else
            {
                m_BlueTeam.RemovePlayer(m_LocalPlayer);
                m_RedTeam.AddPlayer(m_LocalPlayer);

                //m_LocalPlayer.SetTeamAndSync(m_RedTeam);
            }

            //m_LocalPlayer.ChangeTeamOnClick();
        }

        public void SetTeam(TeamType teamType)
        {
            SetTeam(teamType, m_LocalPlayer);
        }

        public void SetTeam(TeamType teamType, PlayerVR playerVR)
        {
            if (playerVR == null)
            {
                return;
            }

            if (teamType == TeamType.RED)
            {
                m_BlueTeam.RemovePlayer(playerVR);
                m_RedTeam.AddPlayer(playerVR);

                playerVR.SetTeamAndSync(m_RedTeam);
            }
            else
            {
                m_RedTeam.RemovePlayer(playerVR);
                m_BlueTeam.AddPlayer(playerVR);

                playerVR.SetTeamAndSync(m_BlueTeam);
            }
        }

        [ContextMenu("Set Red Team")]
        public void SetRedTeam()
        {
            SetTeam(TeamType.RED);
        }


        [ContextMenu("Set Blue Team")]
        public void SetBlueTeam()
        {
            SetTeam(TeamType.BLUE);
        }

        public void SetRole(string idClass)
        {
            if (m_LocalPlayer != null)
            {
                m_LocalPlayer.PlayerData.IdRole = idClass;
                SwitchRoleAndSkin();
            }
        }
        
        public void SetGender(bool isMale)
        {
            if (m_LocalPlayer != null)
            {
                m_LocalPlayer.PlayerData.IsMale = isMale;
                SwitchRoleAndSkin();
            }
        }

        public void SwitchRole(string idClass)
        {
            SwitchRole(idClass, true);
        }

        public void SwitchRole(string idClass, bool isMale)
        {
            if (m_LocalPlayer.TryGetComponent(out ClassSwitcher classSwitcher))
            {
                m_LocalPlayer.PlayerData.IdRole = idClass;
                m_LocalPlayer.PlayerData.IsMale = isMale;
                
                classSwitcher.SetRole(idClass, isMale);
            }
        }

        public void SwitchRoleAndSkin()
        {
            if (m_LocalPlayer.TryGetComponent(out ClassSwitcher classSwitcher))
            {
                classSwitcher.SetRole(
                    m_LocalPlayer.PlayerData.IdRole,
                    m_LocalPlayer.PlayerData.IsMale);
            }
        }

        #endregion

        #region Game Mode

        private void InitMode()
        {
            if (m_Mode != null)
            {
                m_Mode.InitMode();
            }
        }

        public void DeactivateMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (m_Mode != null)
            {
                m_Mode.DeactivateMode();
            }
        }

        public void StartMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (m_Mode != null)
            {
                m_Mode.StartMode();
            }
        }

        public void CompleteMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (m_Mode != null)
            {
                m_Mode.CompleteMode();
            }
        }

        #endregion

        #region PvP Mode

        public void StartPvPMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!m_IsPvPMode)
            {
                //m_LichMode.CompleteMode();
                //m_Environment.ShowDefaultPvPMap();
            }

            m_IsPvPMode = true;
            m_Mode.StartMode();
        }

        public void CompletePvPMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            m_Mode.CompleteMode();
        }

        public void DeactivatePvPMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            m_Mode.DeactivateMode();
        }

        #endregion

        #region PvE Mode

        public void StartPvEMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (m_IsPvPMode)
            {
                m_Mode.DeactivateMode();
                //m_Environment.ShowDefaultPvEMap();
            }

            m_IsPvPMode = false;
            //m_LichMode.StartMode();
        }

        public void CompletePvEMode()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            //m_LichMode.CompleteMode();
        }

        private void ResetModes()
        {
            //m_LichMode.CompleteMode();
            if (m_Mode != null)
            {
                m_Mode.DeactivateMode();
            }

            //m_Environment.ShowDefaultPvPMap();
            m_IsPvPMode = true;
        }

        #endregion

        #region Photon

        public void SetMaster()
        {
            //ResetModes();
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
        }


        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            if (PhotonNetwork.IsMasterClient)
            {
                ResetModes();
            }

            /*
            if (m_LocalPlayer.Team.TeamType == TeamType.RED)
            {
                m_RedTeam.RemovePlayer(m_LocalPlayer);
            }
            else
            {
                m_BlueTeam.RemovePlayer(m_LocalPlayer);
            }
            */
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //foreach (PlayerVR playerVR in m_Players)
                try
                {
                    foreach (PlayerVR playerVR in m_Players)
                    {
                        if (playerVR.PlayerData.ActorNumber == otherPlayer.ActorNumber)
                        {
                            photonView.RPC(nameof(RpcRemovePlayer), RpcTarget.AllBuffered, playerVR.photonView.ViewID);
                        }
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    //throw;
                }

                //ResetModes();
            }

            base.OnPlayerLeftRoom(otherPlayer);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            ResetModes();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
        }

        public override void OnJoinedRoom()
        {
            if (managerDevice != null && managerDevice.CanCreatePlayer) // Проверяем, нужно ли создавать игрока
            {
                Invoke(nameof(InitPlayer), 2f);
            }

            base.OnJoinedRoom();

            //InitPlayer();
            //InitMode();
        }


        public override void OnConnected()
        {
            base.OnConnected();
        }

        #endregion
    }
}