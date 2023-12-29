#define UNITY_EDITOR

using System;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public abstract class SpellBehaviour : MonoBehaviour, ISpellState
    {
        protected const string TAG = nameof(SpellBehaviour);

        [Header("Blocking spells")]
        [Tooltip("If blocking spell is performed, player can't use this spell.")]
        [SerializeField] protected List<SpellBehaviour> m_BlockingSpells = new();

        [Header("Spell Info")]
        [SerializeField] protected string m_SpellName;
        [SerializeField] protected bool m_CanInterrupted = true;
        [SerializeField] protected bool m_UseCooldown = false;
        [SerializeField] protected float m_CooldownTime = 0f;

        [SerializeField] [ReadOnly] protected bool m_IsPerformed = false;

        [Header("Network")]
        [SerializeField] protected PhotonView m_PhotonView;

        protected ClassicGameSession m_GameSession;
        protected SpellHandler m_SpellsHandler;
        [SerializeField] [ReadOnly] protected bool m_IsInCooldown = false;
        [SerializeField] [ReadOnly] protected float m_CurrentTime = 0f;
        protected PlayerVR m_PlayerVR;
        protected bool m_IsInit = false;
        [SerializeField] [ReadOnly] protected bool m_IsAvailable = true;

        public string SpellName => m_SpellName;
        public bool IsInit => m_IsInit;
        public bool IsAvailable => !m_UseCooldown || m_IsAvailable;
        public bool UseCooldown => m_UseCooldown;
        public float CooldownTime => m_CooldownTime;
        public bool IsInCooldown => m_IsInCooldown;
        public float CurrentTime => m_CurrentTime;

        public Action OnStarted;
        public Action OnPerformed;
        public Action OnCompleted;

        #region Spell

        protected virtual void OnValidate()
        {
            if (m_PhotonView == null)
            {
                m_PhotonView = GetComponentInParent<PhotonView>();
            }
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
            m_IsPerformed = false;
        }

        public virtual void Init(SpellHandler spellHandler, PlayerVR playerVR)
        {
            m_IsInit = true;

            m_SpellsHandler = spellHandler;
            m_PlayerVR = playerVR;

            m_GameSession = FindObjectOfType<ClassicGameSession>();
        }

        protected virtual bool CanCast()
        {
            //return m_PlayerVR.WizardPlayer.PlayerState.StateSo.CanCast && m_PlayerVR.WizardPlayer.IsLife;
            return IsInit &&
                   IsAvailable &&
                   m_PhotonView.IsMine &&
                   m_PlayerVR.WizardPlayer.PlayerState.StateSo.CanCast &&
                   m_PlayerVR.WizardPlayer.IsLife;
        }

        protected virtual bool HasBlockingSpells()
        {
            foreach (SpellBehaviour spellBehaviour in m_BlockingSpells)
            {
                if (spellBehaviour.IsPerformed())
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void WaitCooldown()
        {
            if (m_UseCooldown)
            {
                m_CurrentTime = 0f;
                m_IsAvailable = false;
                m_IsInCooldown = true;
                //Invoke(nameof(SetAvailable), m_CooldownTime);
            }
            else
            {
                m_IsAvailable = true;
            }
        }

        protected virtual void Update()
        {
            if (m_IsInCooldown)
            {
                m_CurrentTime += Time.deltaTime;
                if (m_CurrentTime >= m_CooldownTime)
                {
                    m_CurrentTime = m_CooldownTime;
                    //m_CurrentTime = 0;
                    m_IsInCooldown = false;
                    SetAvailable();
                }
            }
        }

        protected virtual void SetAvailable()
        {
            //m_CurrentTime = 0;
            m_IsInCooldown = false;
            m_IsAvailable = true;
        }

        #endregion

        #region Spell States

        public virtual bool IsPerformed() => m_IsPerformed;

        public virtual bool TryInterrupt()
        {
            if (!m_PhotonView.IsMine)
            {
                return false;
            }

            if (m_CanInterrupted)
            {
                Interrupt();
            }

            return m_CanInterrupted;
        }

        public void Reset()
        {
            if (!m_PhotonView.IsMine)
            {
                return;
            }

            Interrupt();
        }

        protected virtual void Interrupt()
        {
            AppDebug.Log($"{TAG}: {SpellName}: Interrupt");
        }

        #endregion
    }
}