using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobaVR
{
    public class VoiceInputSpellBehaviour : InputSpellBehaviour
    {
        [Header("Microphone")]
        [SerializeField] protected bool m_UseActivateButton = true;
        [SerializeField] protected float m_Duration = 10f;
        [SerializeField] protected MicrophoneInput m_MicrophoneInput;
        [SerializeField] protected float m_LoudnessSensibility = 100f;
        [SerializeField] protected float m_Threshold = 0.1f;
        [SerializeField] protected float m_MinVoiceValue = 10f;
        [SerializeField] [ReadOnly] protected float m_CurrentLoudness;

        protected bool m_CanUseMicrophone = false;

        public Action OnVoiced;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!m_UseActivateButton)
            {
                m_MicrophoneInput.Start();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_MicrophoneInput.IsRecorded)
            {
                m_MicrophoneInput.Stop();
            }
        }
        
        protected override void OnStartCast(InputAction.CallbackContext context)
        {
            base.OnStartCast(context);
            OnStarted?.Invoke();
        }

        protected override void OnPerformedCast(InputAction.CallbackContext context)
        {
            base.OnPerformedCast(context);
            Perform();
        }

        protected override void OnCanceledCast(InputAction.CallbackContext context)
        {
            base.OnCanceledCast(context);
            /*
            if (m_IsPerformed)
            {
                m_IsPerformed = false;
                WaitCooldown();
                Interrupt();
            }
            */
        }

        protected void Perform()
        {
            if (!CanCast() 
                || HasBlockingSpells()
                || !m_IsAvailable)
            {
                return;
            }
            
            //TODO:
            if (m_IsPerformed)
            {
                return;
            }
            
            m_CanUseMicrophone = true;
            StartRecord();
            
            OnPerformed?.Invoke();
            m_IsPerformed = true;
            Invoke(nameof(Stop), m_Duration);
        }

        protected override void Interrupt()
        {
            base.Interrupt();

            StopRecord();
            
            if (m_IsPerformed)
            {
                WaitCooldown();
            }
            
            CancelInvoke(nameof(Stop));
            OnCompleted?.Invoke();
            m_IsPerformed = false;
            m_CanUseMicrophone = false;
        }

        private void Stop()
        {
            WaitCooldown();
            Interrupt();
        }

        protected override void Update()
        {
            base.Update();

            if (m_UseActivateButton)
            {
                UpdateWithButton();
            }
            else
            {
                UpdateWithoutButton();
            }
        }

        private void UpdateWithButton()
        {
            if (!m_CanUseMicrophone || !m_IsPerformed)
            {
                return;
            }
            
            if (!m_MicrophoneInput.IsInitialized || !m_MicrophoneInput.IsRecorded)
            {
                return;
            }

            m_CurrentLoudness = m_MicrophoneInput.GetLoundessFromMicrophone() * m_LoudnessSensibility;
            if (m_CurrentLoudness < m_Threshold)
            {
                m_CurrentLoudness = 0;
            }

            if (m_CurrentLoudness >= m_MinVoiceValue)
            {
                ExecuteVoice();
            }
        }

        private void UpdateWithoutButton()
        {
            if (!m_IsPerformed)
            {
                m_CurrentLoudness = m_MicrophoneInput.GetLoundessFromMicrophone() * m_LoudnessSensibility;
                if (m_CurrentLoudness < m_Threshold)
                {
                    m_CurrentLoudness = 0;
                }

                if (m_CurrentLoudness >= m_MinVoiceValue)
                {
                    Perform();
                }
            }
            else
            {
                UpdateWithButton();
            }
        }

        protected virtual void ExecuteVoice()
        {
            OnVoiced?.Invoke();
        }

        [ContextMenu("Start Record")]
        private void StartRecord()
        {
            m_MicrophoneInput.Start();
        }

        [ContextMenu("Stop Record")]
        private void StopRecord()
        {
            m_MicrophoneInput.Stop();
        }
    }
}