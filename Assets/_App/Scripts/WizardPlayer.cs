#define UNITY_EDITOR
using System;
using System.Collections.Generic;
using BNG;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobaVR
{
    /// <summary>
    /// Отвечает за существование игрока, ХП, каст заклинаний, обработку ввода контроллера
    /// </summary>
    public class WizardPlayer : MonoBehaviourPunCallbacks
    {
        protected const string TAG = nameof(WizardPlayer);

        [Header("Big Fireball")]
        [SerializeField] private BigFireBall m_BigFireballPrefab;
        [SerializeField] private GravityType m_BigFireballGravityType = GravityType.REAL_GRAVITY;

        [Header("Small Fireball")]
        [SerializeField] private SmallFireBall m_SmallFireballPrefab;

        [Header("Test FireBreath")]
        [SerializeField] private bool m_UseFireBreath = true;
        [SerializeField] private FireBreath m_LeftFireBreath;
        [SerializeField] private FireBreath m_RightFireBreath;

        [Header("Shields")]
        [SerializeField] private Shield m_LeftShield;
        [SerializeField] private Shield m_RightShield;

        [Header("Player")]
        [SerializeField] private PlayerVR m_PlayerVR;
        [SerializeField] private Teammate m_Teammate;
        [SerializeField] private PlayerView m_PlayerView;
        [SerializeField] private InputBridge inputBridge;
        [SerializeField] private PlayerMode m_State;
        [SerializeField] private ClassStatsSO m_StatsSO;
        [SerializeField] private Collider m_Collider;
        [SerializeField] [ReadOnly] private List<HitCollider> m_Colliders = new();
        [SerializeField] private Transform m_PlayerPoint;
        [SerializeField] private bool m_SetDisableOnDie = false;

        /// <summary>
        /// TODO: input for Oculus and Pico
        /// </summary>
        [Header("Input Platform")]
        [SerializeField] private InputActionSO m_ActiveInput;
        [SerializeField] private bool m_IsAutoDetect = false;
        [SerializeField] private InputActionSO m_OculusInput;
        [SerializeField] private InputActionSO m_PicoInput;

        [Header("Input")]
        [SerializeField] private InputActionReference m_HealthInput;
        [SerializeField] private InputActionReference m_SwitchModeLeftHandInput;
        [SerializeField] private InputActionReference m_SwitchModeRightHandInput;

        /// <summary>
        /// TODO: make new class for left and right hand with this dependencies
        /// </summary>
        [Header("Right Hand")]
        [SerializeField] private Grabber m_RightGrabber;
        [SerializeField] private Transform m_RightBigFireballPoint;
        [SerializeField] private Transform m_RightSmallFireballPoint;
        [SerializeField] private InputActionReference m_RightGrabInput;
        [SerializeField] private InputActionReference m_RightActivateInput;
        [SerializeField] private InputActionReference m_RightGrabActivateInput;
        [SerializeField] private InputActionReference m_RightGrabDoubleTapInput;

        [Header("Left Hand")]
        [SerializeField] private Grabber m_LeftGrabber;
        [SerializeField] private Transform m_LeftBigFireballPoint;
        [SerializeField] private Transform m_LeftSmallFireballPoint;
        [SerializeField] private InputActionReference m_LeftGrabInput;
        [SerializeField] private InputActionReference m_LeftActivateInput;
        [SerializeField] private InputActionReference m_LeftGrabActivateInput;
        [SerializeField] private InputActionReference m_LeftGrabDoubleTapInput;

        private BigFireBall m_RightBigFireBall;
        private BigFireBall m_LeftBigFireBall;

        private SmallFireBall m_RightSmallFireBall;
        private SmallFireBall m_LeftSmallFireBall;

        private IDamageIndicator m_DamageIndicator;
        private TeamType m_TeamType = TeamType.RED;
        private float m_MaxHp = 100f;

        [SerializeField] private bool m_CanInit = true;

        [SerializeField] private float m_ThrowForce = 10f;
        [SerializeField] private bool m_UseAim = false;
        [SerializeField] private float m_CurrentHealth = 100f;
        //[SerializeField] [ReadOnly] private float m_CurrentHealth = 100f;

        /// <summary>
        /// TODO: move this logic to new class
        /// </summary>
        private bool m_IsAttackLeftHand = true;
        private bool m_IsAttackRightHand = true;

        private bool m_UseLeftShield = false;
        private bool m_UseRightShield = false;

        private bool m_UseLeftFireBreath = false;
        private bool m_UseRightFireBreath = false;

        [Space]
        [Header("Utils")]
        [SerializeField] private bool m_CanOverrideThrowableSettings = false;

        public float MaxHp => m_MaxHp;
        public float CurrentHealth => m_CurrentHealth;
        public GravityType GravityFireballType
        {
            get => m_BigFireballGravityType;
            set => m_BigFireballGravityType = value;
        }
        public float ThrowForce
        {
            get => m_ThrowForce;
            set => m_ThrowForce = value;
        }
        public bool UseAim
        {
            get => m_UseAim;
            set => m_UseAim = value;
        }
        public bool CanOverrideThrowableSettings
        {
            get => m_CanOverrideThrowableSettings;
            set => m_CanOverrideThrowableSettings = value;
        }
        public PlayerMode PlayerState => m_State;
        public PlayerStateSO CurrentPlayerState => m_State.StateSo;
        public PlayerVR PlayerVR => m_PlayerVR;
        public InputVR InputVR => m_PlayerVR.InputVR;
        public bool IsLife => m_CurrentHealth > 0;
        public TeamType TeamType
        {
            get => m_TeamType;
            set => m_TeamType = value;
        }
        public IDamageIndicator DamageIndicator
        {
            get => m_DamageIndicator;
            set => m_DamageIndicator = value;
        }
        public Transform PointPlayer
        {
            get
            {
                if (m_PlayerPoint != null)
                {
                    return m_PlayerPoint;
                }

                return transform;
            }
        }
        public ClassStatsSO Stats
        {
            get => m_StatsSO;
            set
            {
                m_StatsSO = value;
                m_MaxHp = m_StatsSO.MaxHp;
                m_CurrentHealth = m_MaxHp;
                //m_PlayerView.SetHealth(m_CurrentHealth);
                OnHealthChange?.Invoke(m_CurrentHealth);
                m_PlayerView.SetMaxHealth(m_CurrentHealth);
            }
        }

        //TODO: HitData
        public Action<HitData> OnPlayerHit;
        public Action<HitData> OnPlayerDie;
        public Action<float> OnHealthChange;
        
        public Action OnInit;
        public Action<float> OnHit;
        public Action<float> OnHeal;
        public Action OnDie;
        public Action OnReborn;

        private void Start()
        {
            m_Colliders.AddRange(GetComponentsInChildren<HitCollider>(true));

            if (!photonView.IsMine)
            {
                enabled = false; //TODO: merge
                return;
            }

            if (!m_CanInit)
            {
                return;
            }

            //m_Collider.enabled = true;

            InputActionSO inputActionSO = m_ActiveInput;
            InputBridge inputBridge = FindObjectOfType<InputBridge>();
            if (inputBridge != null && m_IsAutoDetect)
            {
                inputActionSO = inputBridge.InputSource == XRInputSource.Pico ? m_PicoInput : m_OculusInput;
            }

            m_HealthInput = inputActionSO.HealthInput;
            m_SwitchModeLeftHandInput = inputActionSO.SwitchModeLeftHandInput;
            m_SwitchModeRightHandInput = inputActionSO.SwitchModeRightHandInput;
            m_LeftGrabInput = inputActionSO.LeftGrabInput;
            m_LeftActivateInput = inputActionSO.LeftActivateInput;
            m_RightGrabInput = inputActionSO.RightGrabInput;
            m_RightActivateInput = inputActionSO.RightActivateInput;

            ShowShield(m_LeftShield, false);
            ShowShield(m_RightShield, false);

            #region Switch Mode

            /*
            m_SwitchModeLeftHandInput.action.performed += context =>
            {
                if (m_IsAttackLeftHand)
                {
                    ThrowBigFireBall(m_LeftBigFireBall);
                }
                else
                {
                    m_LeftShield.Show(false);
                }

                m_IsAttackLeftHand = !m_IsAttackLeftHand;
            };

            m_SwitchModeRightHandInput.action.performed += context =>
            {
                if (m_IsAttackRightHand)
                {
                    ThrowBigFireBall(m_RightBigFireBall);
                }
                else
                {
                    m_RightShield.Show(false);
                }

                m_IsAttackRightHand = !m_IsAttackRightHand;
            };
            */

            #endregion

            #region Attack big fireballs

            m_LeftGrabInput.action.started += context => { AppDebug.Log($"{TAG}: LeftGrab: started"); };
            m_RightGrabInput.action.started += context => { AppDebug.Log($"{TAG}: RightGrab: started"); };

            m_LeftGrabInput.action.performed += context =>
            {
                AppDebug.Log($"{TAG}: LeftGrab: performed");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    if (!m_UseLeftShield && !m_UseLeftFireBreath)
                    {
                        CreateBigFireBall(out m_LeftBigFireBall, m_LeftGrabber);
                    }
                    else
                    {
                        if (m_LeftShield is AttackMagicShield attackMagicShield)
                        {
                            //attackMagicShield.Throw(m_LeftGrabber.transform.forward);
                        }
                    }
                }
            };

            m_RightGrabInput.action.performed += context =>
            {
                AppDebug.Log($"{TAG}: RightGrab: performed");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    if (!m_UseRightShield && !m_UseRightFireBreath)
                    {
                        CreateBigFireBall(out m_RightBigFireBall, m_RightGrabber);
                    }
                    else
                    {
                        if (m_RightShield is AttackMagicShield attackMagicShield)
                        {
                            //attackMagicShield.Throw(m_RightGrabber.transform.forward);
                        }
                    }
                }
            };


            m_RightGrabInput.action.canceled += context =>
            {
                AppDebug.Log($"{TAG}: RightGrab: canceled");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    ThrowBigFireBall(m_RightBigFireBall);
                }
            };

            m_LeftGrabInput.action.canceled += context =>
            {
                AppDebug.Log($"{TAG}: LeftGrab: canceled");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    ThrowBigFireBall(m_LeftBigFireBall);
                }
            };

            #endregion

            #region Firebreath

            /*
            
            m_LeftGrabDoubleTapInput.action.performed += context =>
            {
                AppDebug.Log($"{TAG}: LeftGrabDoubleTapInput: performed");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    if (!m_UseLeftShield)
                    {
                        m_UseLeftFireBreath = true;
                        m_LeftFireBreath.gameObject.SetActive(true);
                    }
                    else
                    {
                        ThrowBigFireBall(m_LeftBigFireBall);
                    }
                }
            };

            m_RightGrabDoubleTapInput.action.performed += context =>
            {
                AppDebug.Log($"{TAG}: RightGrabDoubleTapInput: performed");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    if (!m_UseRightShield)
                    {
                        m_UseRightFireBreath = true;
                        m_RightFireBreath.gameObject.SetActive(true);
                    }
                    else
                    {
                        ThrowBigFireBall(m_RightBigFireBall);
                    }
                }
            };

            m_LeftGrabDoubleTapInput.action.canceled += context =>
            {
                AppDebug.Log($"{TAG}: LeftGrabDoubleTapInput: canceled");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    m_UseLeftFireBreath = false;
                    m_LeftFireBreath.gameObject.SetActive(false);
                }
            };

            m_RightGrabDoubleTapInput.action.canceled += context =>
            {
                AppDebug.Log($"{TAG}: RightGrabDoubleTapInput: canceled");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    m_UseRightFireBreath = false;
                    m_RightFireBreath.gameObject.SetActive(false);
                }
            };
            
            */

            #endregion

            #region Attack small fireballs or use shield

            m_RightActivateInput.action.started += context => { AppDebug.Log($"{TAG}: RightActivate: started"); };
            m_LeftActivateInput.action.started += context => { AppDebug.Log($"{TAG}: LeftActivate: started"); };

            m_LeftActivateInput.action.performed += context =>
            {
                AppDebug.Log($"{TAG}: LeftActivate: performed");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    if (!m_UseLeftShield && !m_UseLeftFireBreath)
                    {
                        ShootFireBall(m_LeftBigFireBall,
                                      out m_LeftSmallFireBall,
                                      m_LeftGrabber,
                                      m_LeftBigFireballPoint,
                                      m_LeftSmallFireballPoint,
                                      m_LeftGrabber.transform.right);
                    }
                    else
                    {
                        ThrowBigFireBall(m_LeftBigFireBall);
                    }

                    /*
                    if (m_IsAttackLeftHand)
                    {
                        ShootFireBall(m_LeftBigFireBall,
                                      out m_LeftSmallFireBall,
                                      m_LeftGrabber,
                                      m_LeftBigFireballPoint,
                                      m_LeftSmallFireballPoint,
                                      m_LeftGrabber.transform.right);
                    }
                    else
                    {
                        ThrowBigFireBall(m_LeftBigFireBall);

                        m_UseLeftShield = true;
                        if (!m_UseFireBreath)
                        {
                            ShowShield(m_LeftShield, true);
                        }
                        else
                        {
                            m_LeftFireBreath.gameObject.SetActive(true);
                        }
                    }
                    */
                }
            };

            m_RightActivateInput.action.performed += context =>
            {
                AppDebug.Log($"{TAG}: RightActivate: performed");

                if (!m_State.StateSo.CanCast)
                {
                    return;
                }

                if (IsLife)
                {
                    if (!m_UseRightShield && !m_UseRightShield)
                    {
                        ShootFireBall(m_RightBigFireBall,
                                      out m_RightSmallFireBall,
                                      m_RightGrabber,
                                      m_RightBigFireballPoint,
                                      m_RightSmallFireballPoint,
                                      -m_RightGrabber.transform.right);
                    }
                    else
                    {
                        ThrowBigFireBall(m_RightBigFireBall);
                    }

                    //if (!m_RightGrabInput.action.inProgress)
                    /*
                    if (m_IsAttackRightHand)
                    {
                        ShootFireBall(m_RightBigFireBall,
                                      out m_RightSmallFireBall,
                                      m_RightGrabber,
                                      m_RightBigFireballPoint,
                                      m_RightSmallFireballPoint,
                                      -m_RightGrabber.transform.right);
                    }
                    else
                    {
                        ThrowBigFireBall(m_RightBigFireBall);

                        m_UseRightShield = true;
                        if (!m_UseFireBreath)
                        {
                            ShowShield(m_RightShield, true);
                        }
                        else
                        {
                            m_RightFireBreath.gameObject.SetActive(true);
                        }
                    }
                    */
                }
            };

            m_LeftActivateInput.action.canceled += context =>
            {
                AppDebug.Log($"{TAG}: LeftActivate: canceled");

                /*
                if (m_UseLeftShield)
                {
                    m_UseLeftShield = false;
                    if (!m_UseFireBreath)
                    {
                        ShowShield(m_LeftShield, false);
                    }
                    else
                    {
                        m_LeftFireBreath.gameObject.SetActive(false);
                    }
                }
                */
            };


            m_RightActivateInput.action.canceled += context =>
            {
                AppDebug.Log($"{TAG}: RightActivate: canceled");

                /*
                if (m_UseRightShield)
                {
                    m_UseRightShield = false;
                    if (!m_UseFireBreath)
                    {
                        ShowShield(m_RightShield, false);
                    }
                    else
                    {
                        m_RightFireBreath.gameObject.SetActive(false);
                    }
                }
                */
            };

            #endregion

            #region Combo

            m_LeftGrabActivateInput.action.started += context =>
            {
                if (!m_State.StateSo.CanCast || !IsLife)
                {
                    return;
                }

                m_UseLeftShield = true;
                ShowShield(m_LeftShield, true);

                m_UseLeftFireBreath = false;
                m_LeftFireBreath.gameObject.SetActive(false);

                ThrowBigFireBall(m_LeftBigFireBall);
            };

            m_RightGrabActivateInput.action.started += context =>
            {
                if (!m_State.StateSo.CanCast || !IsLife)
                {
                    return;
                }

                m_UseRightShield = true;
                ShowShield(m_RightShield, true);

                m_UseRightFireBreath = false;
                m_RightFireBreath.gameObject.SetActive(false);

                ThrowBigFireBall(m_RightBigFireBall);
            };

            m_LeftGrabActivateInput.action.canceled += context =>
            {
                m_UseLeftShield = false;
                ShowShield(m_LeftShield, false);
            };

            m_RightGrabActivateInput.action.canceled += context =>
            {
                m_UseRightShield = false;
                ShowShield(m_RightShield, false);
            };

            #endregion

            #region Other Inputs

            //TODO
            m_HealthInput.action.performed += context =>
            {
                AppDebug.Log($"{TAG}: HealthButton: performed");
                RestoreHp();
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            };

            #endregion

            OnInit?.Invoke();
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (m_CanInit && m_State.StateSo.CanCast && IsLife)
            {
                CastSpells();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                Reborn();
                //RestoreHp();
            }
        }

        private void ResetSpells()
        {
            ThrowBigFireBall(m_LeftBigFireBall);
            ThrowBigFireBall(m_RightBigFireBall);

            m_UseLeftShield = false;
            ShowShield(m_LeftShield, false);

            m_UseRightShield = false;
            ShowShield(m_RightShield, false);

            m_UseRightFireBreath = false;
            m_RightFireBreath.gameObject.SetActive(false);

            m_UseLeftFireBreath = false;
            m_LeftFireBreath.gameObject.SetActive(false);
        }

        private void CastSpells()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CreateBigFireBall(out m_LeftBigFireBall, m_LeftGrabber);
                if (m_LeftBigFireBall != null)
                {
                    m_LeftBigFireBall.transform.parent = null;
                    m_LeftBigFireBall.ThrowByDirection(m_LeftGrabber.transform.forward);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CreateBigFireBall(out m_RightBigFireBall, m_RightGrabber);
                if (m_RightBigFireBall != null)
                {
                    m_RightBigFireBall.transform.parent = null;
                    m_RightBigFireBall.ThrowByDirection(m_RightGrabber.transform.forward);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CreateSmallFireBall(out m_LeftSmallFireBall, m_LeftGrabber);
                if (m_LeftSmallFireBall != null)
                {
                    m_LeftSmallFireBall.transform.parent = null;
                    m_LeftSmallFireBall.Shoot(m_LeftGrabber.transform.forward);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CreateSmallFireBall(out m_RightSmallFireBall, m_RightGrabber);
                if (m_RightSmallFireBall != null)
                {
                    m_RightSmallFireBall.transform.parent = null;
                    m_RightSmallFireBall.Shoot(m_RightGrabber.transform.forward);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ShowShield(m_LeftShield, true);
            }

            if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                ShowShield(m_LeftShield, false);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ShowShield(m_RightShield, true);
            }

            if (Input.GetKeyUp(KeyCode.Alpha8))
            {
                ShowShield(m_RightShield, false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                CreateBigFireBall(out m_RightBigFireBall, m_RightGrabber);
                if (m_RightBigFireBall != null)
                {
                    m_RightBigFireBall.transform.parent = null;
                    //m_RightBigFireBall.ThrowForce(m_RightGrabber.transform.forward);
                    m_RightBigFireBall.ThrowByDirection(m_RightGrabber.transform.forward);
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                CreateBigFireBall(out m_LeftBigFireBall, m_LeftGrabber);
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                ShootFireBall(m_RightBigFireBall,
                              out m_RightSmallFireBall,
                              m_RightGrabber,
                              m_RightBigFireballPoint,
                              m_RightSmallFireballPoint,
                              -m_RightGrabber.transform.right);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                CreateBigFireBall(out m_RightBigFireBall, m_RightGrabber);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                ShootFireBall(m_RightBigFireBall,
                              out m_RightSmallFireBall,
                              m_RightGrabber,
                              m_RightBigFireballPoint,
                              m_RightSmallFireballPoint,
                              -m_RightGrabber.transform.right);
            }
        }

        #region Shields

        private void ShowShield(Shield shield, bool isShow)
        {
            //shield.SetActive(isShow);
            shield.Show(isShow);
        }

        #endregion

        #region Throw Fireballs

        private void ThrowBigFireBall(BigFireBall fireBall)
        {
            if (fireBall != null)
            {
                //fireBall.Throw();
                fireBall.Throw();
                fireBall = null;
            }
        }

        private void ThrowSmallFireBall(SmallFireBall fireBall, Grabber grabber, Vector3 direction)
        {
            if (fireBall != null)
            {
                //fireBall.ThrowForce(grabber.transform.forward);
                fireBall.Shoot(grabber.transform.forward);
                fireBall = null;
            }
        }

        /// <summary>
        /// TODO: need refactoring
        /// У большого шара возможно два варианта полета: от руки / от плеча и по нажатию курка.
        /// Этот метод активируется при нажатии курка и проверяет, есть ли у игрока сейчас шар,
        /// Если есть, то мы управляем шаром в воздухе = меняем направление его движения.
        /// Если нет, то мы стреляем маленькими снарядами. 
        private void ShootFireBall(BigFireBall bigFireBall,
                                   out SmallFireBall smallFireBall,
                                   Grabber grabber,
                                   Transform bigFireballPoint,
                                   Transform smallFireballPoint,
                                   Vector3 direction)
        {
            if (bigFireBall != null)
            {
                bigFireBall.ThrowByDirection(direction);
            }
            else
            {
                CreateSmallFireBall(out smallFireBall, smallFireballPoint);
                ThrowSmallFireBall(smallFireBall, grabber, direction);
            }

            bigFireBall = null;
            smallFireBall = null;
        }

        #endregion

        #region Create Big Fireball

        private void CreateBigFireBall(out BigFireBall fireBall, Grabber grabber)
        {
            CreateBigFireBall(out fireBall, grabber.transform);
        }

        private void CreateBigFireBall(out BigFireBall fireBall, Transform point)
        {
            GameObject networkFireball = PhotonNetwork.Instantiate($"Spells/{m_BigFireballPrefab.name}",
                                                                   point.transform.position,
                                                                   point.transform.rotation);
            if (networkFireball.TryGetComponent(out fireBall))
            {
                fireBall.Init(this, m_TeamType);

                Transform fireBallTransform = fireBall.transform;
                fireBallTransform.parent = point.transform;
                fireBallTransform.localPosition = Vector3.zero;
                fireBallTransform.localRotation = Quaternion.identity;
                //fireBall.Owner = this;
            }
        }

        #endregion

        #region Create Small Fireball

        private void CreateSmallFireBall(out SmallFireBall fireBall, Grabber grabber)
        {
            CreateSmallFireBall(out fireBall, grabber.transform);
        }

        private void CreateSmallFireBall(out SmallFireBall fireBall, Transform point)
        {
            GameObject networkFireball = PhotonNetwork.Instantiate($"Spells/{m_SmallFireballPrefab.name}",
                                                                   point.transform.position,
                                                                   point.transform.rotation);
            if (networkFireball.TryGetComponent(out fireBall))
            {
                fireBall.Init(this, m_TeamType);

                Transform fireBallTransform = fireBall.transform;
                fireBallTransform.parent = null;
                fireBallTransform.position = point.transform.position;
                fireBallTransform.rotation = Quaternion.identity;
                //fireBall.Owner = this;
            }
        }

        #endregion

        #region HP

        [ContextMenu("Heal")]
        public void Heal(float amount)
        {
            //photonView.RPC(nameof(RpcHeal_Player), RpcTarget.AllBuffered, amount);
            photonView.RPC(nameof(RpcHeal_Player), RpcTarget.All, amount);
        }

        [PunRPC]
        private void RpcHeal_Player(float amount)
        {
            if (!IsLife)
            {
                return;
            }
            
            m_CurrentHealth += amount;
            if (m_CurrentHealth >= m_MaxHp)
            {
                m_CurrentHealth = m_MaxHp;
            }
            
            OnHeal?.Invoke(amount);
            m_PlayerView.RpcSetHealth(m_CurrentHealth);
            OnHealthChange?.Invoke(m_CurrentHealth);
        }

        [ContextMenu("Reborn")]
        public void Reborn()
        {
            //photonView.RPC(nameof(RpcReborn_Player), RpcTarget.AllBuffered);
            photonView.RPC(nameof(RpcReborn_Player), RpcTarget.All);
        }

        [PunRPC]
        private void RpcReborn_Player()
        {
            m_CurrentHealth = m_MaxHp;
            //m_Collider.enabled = true;

            foreach (HitCollider damagePlayer in m_Colliders)
            {
                damagePlayer.GetComponent<Collider>().enabled = true;
            }

            //OnReborn?.Invoke();


            //TODO: MERGE
            //m_PlayerVR.PlayerReborn();
            m_PlayerVR.RpcPlayerReborn();
            //m_PlayerView.SetHealth(m_CurrentHealth);
            m_PlayerView.RpcSetHealth(m_CurrentHealth);
            OnHealthChange?.Invoke(m_CurrentHealth);
            OnReborn?.Invoke();
        }

        public void RestoreHp()
        {
            //photonView.RPC(nameof(RpcRestoreHp_Player), RpcTarget.AllBuffered);
            photonView.RPC(nameof(RpcRestoreHp_Player), RpcTarget.All);
        }

        [PunRPC]
        private void RpcRestoreHp_Player()
        {
            m_CurrentHealth = m_MaxHp;

            if (photonView.IsMine)
            {
                m_PlayerView.RpcSetHealth(m_CurrentHealth);
                OnHealthChange?.Invoke(m_CurrentHealth);
            }
        }

        [ContextMenu("Hit")]
        private void Hit_Debug()
        {
            Hit(25f);
        }


        [ContextMenu("Kill")]
        private void Hit_Debug_Kill()
        {
            Hit(250f);
        }

        //public void Hit(HitData hitData)
        protected void Hit(HitData hitData)
        {
            if (!hitData.CanApplyBySelf)
            {
                if (hitData.PlayerVR != null && hitData.PlayerVR.WizardPlayer == this)
                {
                    return;
                }
                
                if (hitData.Player != null && hitData.Player.ActorNumber == photonView.Owner.ActorNumber)
                {
                    return;
                }
            }

            if (!hitData.CanApplyForTeammates)
            {
                if (hitData.TeamType == m_Teammate.TeamType)
                {
                    return;
                }
                
                if ((hitData.TeamType == TeamType.RED && m_Teammate.IsRed)
                    || (hitData.TeamType == TeamType.BLUE && !m_Teammate.IsRed))
                {
                    return;
                }
            }

            // For PVE mode
            if (hitData.TeamType != TeamType.OTHER && !m_State.StateSo.CanGetDamageFromEnemyPlayers)
            {
                return;
            }

            //RpcHit_Player(hitData.Amount);
            RpcHit_Player(hitData);
        }

        //public void Hit(float damage)
        private void Hit(float damage)
        {
            photonView.RPC(nameof(RpcHit_Player), RpcTarget.All, damage);
        }

        //public void Hit(ThrowableSpell fireball, float damage)
        private void Hit(ThrowableSpell fireball, float damage)
        {
            if ((fireball.Team.IsRed && m_Teammate.IsRed)
                || (!fireball.Team.IsRed && !m_Teammate.IsRed))
            {
                return;
            }

            if (!m_State.StateSo.CanGetDamageFromEnemyPlayers)
            {
                return;
            }

            photonView.RPC(nameof(RpcHit_Player), RpcTarget.All, damage);
        }

        [PunRPC]
        private void RpcHit_Player(HitData hitData)
        {
            if (!m_State.StateSo.CanGetDamage)
            {
                return;
            }

            if (!IsLife)
            {
                return;
            }

            OnHit?.Invoke(hitData.Amount);
            OnPlayerHit?.Invoke(hitData);
            
            m_CurrentHealth -= hitData.Amount;

            if (m_CurrentHealth <= 0)
            {
                //m_Collider.enabled = false;

                foreach (HitCollider damagePlayer in m_Colliders)
                {
                    if (m_SetDisableOnDie)
                    {
                        damagePlayer.GetComponent<Collider>().enabled = false;
                    }
                }

                ResetSpells();
                //OnDie?.Invoke();


                //TODO: MERGE
                //m_PlayerVR.PlayerDie();
                m_PlayerVR.RpcPlayerDie();
                
                OnPlayerDie?.Invoke(hitData);
                OnDie?.Invoke();
            }

            if (photonView.IsMine)
            {
                //TODO: merge
                //m_CurrentHealth -= damage;
                
                m_PlayerView.RpcSetHealth(m_CurrentHealth);
                OnHealthChange?.Invoke(m_CurrentHealth);
                m_DamageIndicator.Show();
            }
            
        }

        #endregion

        #region Init Transforms

        public void Init(PlayerVR playerVR)
        {
        }

        public void SetLeftGrabber(Grabber grabber, Transform smallFireballPoint, Transform bigFireballPoint)
        {
            m_LeftGrabber = grabber;
            m_LeftSmallFireballPoint = smallFireballPoint;
            m_LeftBigFireballPoint = bigFireballPoint;
        }

        public void SetRightGrabber(Grabber grabber, Transform smallFireballPoint, Transform bigFireballPoint)
        {
            m_RightGrabber = grabber;
            m_RightSmallFireballPoint = smallFireballPoint;
            m_RightBigFireballPoint = bigFireballPoint;
        }

        #endregion
    }
}