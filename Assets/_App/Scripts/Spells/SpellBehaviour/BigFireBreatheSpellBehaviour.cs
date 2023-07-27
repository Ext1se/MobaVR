﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobaVR
{
    public class BigFireBreatheSpellBehaviour : InputSpellBehaviour
    {
        [SerializeField] private FireBreath m_FireBreath;
        [SerializeField] private float m_Duration = 8f;
        [SerializeField] private bool m_CheckDistanceAngle = false;
        [SerializeField] private float m_MaxDistance = 2f;
        [SerializeField] private float m_MaxAngle = 120f;

        private float Distance => Vector3.Distance(m_LeftHand.InsideHandPoint.transform.position, 
                                                   m_RightHand.InsideHandPoint.transform.position);
        private float Angle => Vector3.Angle(m_LeftHand.InsideHandPoint.transform.forward, 
                                             m_RightHand.InsideHandPoint.transform.forward);

        protected override void OnEnable()
        {
            base.OnEnable();
            m_FireBreath.gameObject.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_FireBreath.gameObject.SetActive(false);
        }

        protected override void OnStartCast(InputAction.CallbackContext context)
        {
            base.OnStartCast(context);
            OnStarted?.Invoke();
        }

        protected override void OnPerformedCast(InputAction.CallbackContext context)
        {
            base.OnPerformedCast(context);

            if (!CanCast() || HasBlockingSpells())
            {
                return;
            }

            if (m_CheckDistanceAngle)
            {
                if (Distance > m_MaxDistance || Angle > m_MaxAngle)
                {
                    return;
                }
            }

            OnPerformed?.Invoke();
            
            m_IsPerformed = true;
            m_FireBreath.Show(true);
            Invoke(nameof(Stop), m_Duration);
        }

        protected override void OnCanceledCast(InputAction.CallbackContext context)
        {
            base.OnCanceledCast(context);
            if (m_IsPerformed)
            {
                m_IsPerformed = false;
                WaitCooldown();
                Interrupt();
            }
        }

        protected override void Interrupt()
        {
            base.Interrupt();
            
            CancelInvoke(nameof(Stop));
            OnCompleted?.Invoke();
            m_IsPerformed = false;
            m_FireBreath.Show(false);
        }

        private void Stop()
        {
            WaitCooldown();
            Interrupt();
        }
        
        private void Update()
        {
            if (!m_IsPerformed)
            {
                return;
            }
            
            if (Distance > m_MaxDistance || Angle > m_MaxAngle)
            {
                WaitCooldown();
                Interrupt();
            }
        }
    }
}