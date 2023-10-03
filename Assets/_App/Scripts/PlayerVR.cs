using System;
using System.Collections.Generic;
using BNG;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    /// <summary>
    /// Основной класс, который содержит в себе все зависимости игрока.
    /// Этот класс используется для инициализации игрока, передачи ему InputVR, выбор команды
    /// ИК: сейчас не работает.
    /// </summary>
    public class PlayerVR : MonoBehaviourPunCallbacks
    {
        [SerializeField] private InputVR m_InputVR;
        //[SerializeField] private SpellsHandler m_SpellsHandler;
        [SerializeField] private GameObject m_Spells;

        [SerializeField] private WizardPlayer m_WizardPlayer;
        [SerializeField] private PlayerMode m_PlayerMode;
        [SerializeField] private PlayerScore m_PlayerScore;
        [SerializeField] private ClassSwitcher m_ClassSwitcher;
        [SerializeField] private Teammate m_Teammate;
        [SerializeField] private DieView m_DieView;
        [SerializeField] private Damageable m_Damageable;
        [SerializeField] private CharacterIK m_CharacterIK;
        [SerializeField] private SkinCollection m_SkinCollection;
        [SerializeField] private CalibrationPol m_Calibration;

        [Header("IK")]
        [SerializeField] private Transform m_BodyTarget;
        [SerializeField] private Transform m_HeadTarget;
        [SerializeField] private Transform m_LeftHandTarget;
        [SerializeField] private Transform m_RightHandTarget;

        [Header("Hands")]
        [SerializeField] private HandController m_LeftHand;
        [SerializeField] private HandController m_RightHand;

        [Header("Renderers")]
        [SerializeField] private bool m_IsRender = false;
        [SerializeField] private List<Renderer> m_Renderers;

        private ChangeTeam m_ChangeTeam;

        [SerializeField] [ReadOnly] private PlayerData m_PlayerData;
        private bool m_IsLocalPlayer = false;
        private bool m_IsInit = false;
        [SerializeField] [ReadOnly] private TeamType m_TeamType = TeamType.RED;
        private Team m_Team = null;

        public bool IsMine => photonView.IsMine;
        public bool IsLocalPlayer => m_IsLocalPlayer;
        public bool IsInit => m_IsInit;
        public TeamType TeamType
        {
            get
            {
                if (m_Team != null)
                {
                    return m_Team.TeamType;
                }

                return m_TeamType;
            }
        }
        public PlayerData PlayerData => m_PlayerData;
        public InputVR InputVR => m_InputVR;
        public ClassSwitcher ClassSwitcher => m_ClassSwitcher;
        public Team Team => m_Team;
        public PlayerMode PlayerMode => m_PlayerMode;
        public Damageable Damageable => m_Damageable;
        public WizardPlayer WizardPlayer => m_WizardPlayer;
        public PlayerScore PlayerScore => m_PlayerScore;
        public DieView DieView => m_DieView;
        public SkinCollection SkinCollection => m_SkinCollection;
        public CalibrationPol Calibration => m_Calibration;

        public Action<PlayerVR> OnDestroyPlayer;
        public Action<PlayerVR> OnInitPlayer;
        public Action<TeamType> OnChangeTeam;
        public Action<TeamType> OnRpcChangeTeam;
        public Action<string> OnRoleChange;
        public Action<string> OnNickNameChange;

        public Transform BodyTarget => m_BodyTarget;
        public Transform HeadTarget => m_HeadTarget;
        public Transform LeftHandTarget => m_LeftHandTarget;
        public Transform RightHandTarget => m_RightHandTarget;

        private void OnValidate()
        {
            if (m_WizardPlayer == null)
            {
                TryGetComponent(out m_WizardPlayer);
            }

            if (m_CharacterIK == null)
            {
                TryGetComponent(out m_CharacterIK);
            }

            if (m_Teammate == null)
            {
                TryGetComponent(out m_Teammate);
            }

            if (m_PlayerMode == null)
            {
                TryGetComponent(out m_PlayerMode);
            }

            if (m_Damageable == null)
            {
                TryGetComponent(out m_Damageable);
            }

            if (m_ClassSwitcher == null)
            {
                TryGetComponent(out m_ClassSwitcher);
            }

            if (m_Calibration == null)
            {
                m_Calibration = GetComponentInChildren<CalibrationPol>();
            }
            
            if (m_PlayerScore == null)
            {
                m_PlayerScore = GetComponentInChildren<PlayerScore>();
            }

            /*
            if (m_SpellsHandler == null)
            {
                m_SpellsHandler = GetComponentInChildren<SpellsHandler>();
            }
            */
        }

        public override void OnDisable()
        {
            base.OnDisable();
            OnDestroyPlayer?.Invoke(this);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (m_InputVR != null)
            {
                m_InputVR.gameObject.SetActive(photonView.IsMine);
            }

            /*
            if (m_SpellsHandler != null)
            {
                m_SpellsHandler.gameObject.SetActive(photonView.IsMine);
            }
            */
            if (m_Spells != null)
            {
                //m_Spells.SetActive(photonView.IsMine);
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (m_IsLocalPlayer && m_IsInit)
            {
                //Vector3 customPosition =  m_InputVR.BngPlayerController.transform.position;
                //customPosition.y = 0;
                //transform.position = customPosition;

                /*
                transform.position = m_InputVR.BngPlayerController.transform.position;
                transform.rotation = m_InputVR.BngPlayerController.transform.rotation;

                if (m_LeftHand != null)
                {
                    m_LeftHand.transform.position = m_InputVR.IKLeftHand.transform.position;
                    m_LeftHand.transform.rotation = m_InputVR.IKLeftHand.transform.rotation;
                }

                if (m_RightHand != null)
                {
                    m_RightHand.transform.position = m_InputVR.IKRightHand.transform.position;
                    m_RightHand.transform.rotation = m_InputVR.IKRightHand.transform.rotation;
                }
                */

                if (m_BodyTarget != null)
                {
                    m_BodyTarget.position = m_InputVR.BngPlayerController.transform.position;
                    m_BodyTarget.rotation = m_InputVR.BngPlayerController.transform.rotation;
                }

                if (m_HeadTarget != null)
                {
                    m_HeadTarget.position = m_InputVR.HeadTarget.transform.position;
                    m_HeadTarget.rotation = m_InputVR.HeadTarget.transform.rotation;
                }

                if (m_LeftHandTarget != null)
                {
                    m_LeftHandTarget.position = m_InputVR.LeftHandTarget.transform.position;
                    m_LeftHandTarget.rotation = m_InputVR.LeftHandTarget.transform.rotation;
                }

                if (m_RightHandTarget != null)
                {
                    m_RightHandTarget.position = m_InputVR.RightHandTarget.transform.position;
                    m_RightHandTarget.rotation = m_InputVR.RightHandTarget.transform.rotation;
                }
            }
        }

        /*
        public void SetLocalPlayer()
        {
            photonView.RPC(nameof(SetLocalPlayer), RpcTarget.All);
        }
        */

        public void SetNickName(string nickName)
        {
            PhotonNetwork.LocalPlayer.NickName = nickName;
            photonView.RPC(nameof(RpcSetNickName), RpcTarget.AllBuffered, nickName);
        }

        [PunRPC]
        private void RpcSetNickName(string nickName)
        {
            m_PlayerData.NickName = nickName;
            OnNickNameChange?.Invoke(nickName);
        }

        public void SetState(PlayerState playerState)
        {
            photonView.RPC(nameof(RpcSetState), RpcTarget.AllBuffered, playerState);
        }

        public void SetState(PlayerStateSO playerStateSo)
        {
            //m_PlayerMode.SetState(playerStateSo);
            //photonView.RPC(nameof(RpcSetStateSo), RpcTarget.AllBuffered, playerStateSo);

            //photonView.RPC(nameof(RpcSetState), RpcTarget.AllBuffered, playerStateSo.State);
            photonView.RPC(nameof(RpcSetState), RpcTarget.AllBuffered, playerStateSo.State);
        }


        [PunRPC]
        public void RpcSetState(PlayerState playerState)
        {
            m_PlayerMode.SetState(playerState);
        }

        [PunRPC]
        public void RpcSetStateSo(PlayerStateSO playerStateSo)
        {
            m_PlayerMode.SetState(playerStateSo);
        }


        //смена команды
        public void ChangeTeamOnClick()
        {
            // Переключаем команду на противоположную
            m_TeamType = (m_TeamType == TeamType.RED) ? TeamType.BLUE : TeamType.RED;

            // Применяем новую команду для всех объектов
            //эта функция меняет у игрока команду в WizardPlayer м Teammate
            SetTeamAndSync(m_TeamType);
        }


        private void ChangeTeamColor(TeamType teamType)
        {
            // Находим объект InputVR
            //_InputVR = GameObject.Find("InputVR");
            if (m_InputVR != null)
            {
                // Получаем компонент ChangeTeam для смены цвета скина
                m_ChangeTeam = m_InputVR.GetComponent<ChangeTeam>();
                //m_SkinCollection.SetTeam(teamType);

                if (m_ChangeTeam != null)
                {
                    // Выполняем функцию ChangeAllTeams
                    m_ChangeTeam.ChangeAllTeams(teamType);
                }
            }
        }

        /*
        public void SetRedTeam()
        {
            SetTeam(TeamType.RED);
        }

        public void SetBlueTeam()
        {
            SetTeam(TeamType.BLUE);
        }
        */

        public void InitPlayer()
        {
            photonView.RPC(nameof(RpcInitPlayer), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }

        [PunRPC]
        private void RpcInitPlayer(Player player)
        {
            m_PlayerData = new PlayerData()
            {
                Player = player,
                NickName = player.NickName,
                IdUser = player.UserId,
                ActorNumber = player.ActorNumber,
            };

            OnInitPlayer?.Invoke(this);
        }

        #region Team

        public void SetTeamAndSync(TeamType teamType)
        {
            //ChangeTeamColor(teamType);

            m_TeamType = teamType;
            OnChangeTeam?.Invoke(m_TeamType);
            photonView.RPC(nameof(RpcSetTeamType), RpcTarget.AllBuffered, teamType);
        }

        public void SetTeamAndSync(Team team)
        {
            //ChangeTeamColor(m_TeamType);

            m_Team = team;
            m_TeamType = m_Team.TeamType;
            OnChangeTeam?.Invoke(m_TeamType);
            photonView.RPC(nameof(RpcSetTeamType), RpcTarget.AllBuffered, m_TeamType);
        }

        private void SetTeam()
        {
            OnRpcChangeTeam?.Invoke(m_TeamType);
            m_SkinCollection.SetTeam(m_TeamType);

            if (m_WizardPlayer != null)
            {
                m_WizardPlayer.TeamType = m_TeamType;
            }

            if (m_Teammate != null)
            {
                m_Teammate.SetTeam(m_TeamType);

                if (photonView.IsMine)
                {
                    TeamItem leftHandTheme = m_InputVR.LeftController.GetComponentInChildren<TeamItem>();
                    if (leftHandTheme != null)
                    {
                        leftHandTheme.SetTeam(m_TeamType);
                    }

                    TeamItem rightHandTheme = m_InputVR.RightController.GetComponentInChildren<TeamItem>();
                    if (rightHandTheme != null)
                    {
                        rightHandTheme.SetTeam(m_TeamType);
                    }
                }
            }
        }

        /// <summary>
        /// TODO: костыльный метод
        /// teamType синхронится легко, а команда - нет, поэтому сейчас команда ищется через FindObject
        /// </summary>
        /// <param name="teamType"></param>
        [PunRPC]
        public void RpcSetTeamType(TeamType teamType)
        {
            m_TeamType = teamType;
            // TODO: костыль
            ClassicGameSession classicGameSession = FindObjectOfType<ClassicGameSession>();
            if (classicGameSession != null)
            {
                if (m_TeamType == TeamType.RED)
                {
                    m_Team = classicGameSession.RedTeam;
                }
                else if (m_TeamType == TeamType.BLUE)
                {
                    m_Team = classicGameSession.BlueTeam;
                }
            }

            SetTeam();
        }

        public void SetTeam(Team team)
        {
            m_TeamType = team.TeamType;
            m_Team = team;

            SetTeam();
        }

        #endregion


        public void SetLocalPlayer()
        {
            if (m_InputVR != null)
            {
                SetLocalPlayer(m_InputVR);
            }
        }

        public void SetLocalPlayer(InputVR inputVR)
        {
            if (inputVR != null)
            {
                m_InputVR = inputVR;
            }

            if (m_InputVR == null)
            {
                m_InputVR = FindObjectOfType<InputVR>();
            }

            if (m_InputVR == null)
            {
                return;
            }

            m_InputVR.gameObject.SetActive(true);

            m_IsLocalPlayer = true;

            if (m_CharacterIK != null)
            {
                m_CharacterIK.FollowHead = m_InputVR.IKHead;
                m_CharacterIK.FollowLeftController = m_InputVR.IKLeftHand;
                m_CharacterIK.FollowRightController = m_InputVR.IKRightHand;
                m_CharacterIK.FollowPlayer = m_InputVR.CharacterController;
                m_CharacterIK.IKActive = true;
            }

            if (m_WizardPlayer != null)
            {
                m_WizardPlayer.SetLeftGrabber(m_InputVR.LeftGrabber,
                                              m_InputVR.LeftSmallFireballPoint,
                                              m_InputVR.LeftBigFireballPoint);

                m_WizardPlayer.SetRightGrabber(m_InputVR.RightGrabber,
                                               m_InputVR.RightSmallFireballPoint,
                                               m_InputVR.RightBigFireballPoint);

                m_WizardPlayer.DamageIndicator = m_InputVR.DamageIndicator;
            }

            if (m_LeftHand != null)
            {
                //m_LeftHand = m_InputVR.LeftController;
                m_LeftHand.handPoser = m_InputVR.LeftController.handPoser;
                m_LeftHand.grabber = m_InputVR.LeftController.grabber;
            }

            if (m_RightHand != null)
            {
                //m_RightHand = m_InputVR.RightController;
                m_RightHand.handPoser = m_InputVR.RightController.handPoser;
                m_RightHand.grabber = m_InputVR.RightController.grabber;
            }


            //TODO: ??
            if (m_Teammate != null)
            {
                TeamItem leftHandTheme = m_InputVR.LeftController.GetComponentInChildren<TeamItem>();
                if (leftHandTheme != null)
                {
                    leftHandTheme.SetTeam(m_TeamType);
                }

                TeamItem rightHandTheme = m_InputVR.RightController.GetComponentInChildren<TeamItem>();
                if (rightHandTheme != null)
                {
                    rightHandTheme.SetTeam(m_TeamType);
                }
            }


            if (!m_IsRender)
            {
                foreach (Renderer meshRenderer in m_Renderers)
                {
                    meshRenderer.enabled = false;
                }
            }

            m_IsInit = true;
        }

        [ContextMenu("Die")]
        public void PlayerDie()
        {
            photonView.RPC(nameof(RpcPlayerDie), RpcTarget.All);
        }

        [PunRPC]
        public void RpcPlayerDie()
        {
            if (m_SkinCollection != null)
            {
                //m_SkinCollection.SetDeadSkin();
                m_SkinCollection.RpcSetDeadSkin();
            }
        }

        [ContextMenu("Reborn")]
        public void PlayerReborn()
        {
            photonView.RPC(nameof(RpcPlayerReborn), RpcTarget.All);
        }

        [PunRPC]
        public void RpcPlayerReborn()
        {
            if (m_SkinCollection != null)
            {
                //m_SkinCollection.RestoreSkin();
                m_SkinCollection.RpcRestoreSkin();
            }
        }
    }
}