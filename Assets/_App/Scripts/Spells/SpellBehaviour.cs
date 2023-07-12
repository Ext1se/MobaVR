﻿using System;
using System.Collections.Generic;
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
        [SerializeField] [ReadOnly] protected bool m_IsPerformed = false;

        protected SpellHandler m_SpellsHandler;
        protected PlayerVR m_PlayerVR;

        public string SpellName => m_SpellName;

        public Action OnStarted;
        public Action OnPerformed;
        public Action OnCompleted;

        #region Spell

        public virtual void Init(SpellHandler spellHandler, PlayerVR playerVR)
        {
            m_SpellsHandler = spellHandler;
            m_PlayerVR = playerVR;
        }

        protected virtual bool CanCast()
        {
            return m_PlayerVR.WizardPlayer.PlayerState.StateSo.CanCast && m_PlayerVR.WizardPlayer.IsLife;
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

        #endregion


        #region Spell States

        public virtual bool IsPerformed() => m_IsPerformed;

        public virtual bool TryInterrupt()
        {
            if (m_CanInterrupted)
            {
                Interrupt();
            }

            return m_CanInterrupted;
        }

        protected virtual void Interrupt()
        {
            Debug.Log($"{SpellName}: Interrupt");
        }

        #endregion
    }
}