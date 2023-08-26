﻿using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

namespace MobaVR
{
    public class SkinRagdoll : MonoBehaviour
    {
        [SerializeField] private Transform m_Root;
        [SerializeField] private bool m_IsDisableOnEnable = true;
        [SerializeField] private float m_HideTimeout = 10f;

        private WizardPlayer m_Wizard;
        private Animator m_Animator;
        private VRIK m_Vrik;
        private List<Rigidbody> m_ChildRigidbodies = new();
        private List<Collider> m_ChildColliders = new();


        private bool m_IsDie = false;
        
        private void OnEnable()
        {
            if (m_Wizard != null)
            {
                m_Wizard.OnDie += OnDie;
                m_Wizard.OnReborn += OnReborn;
            }
            
            //TODO: Костыль
            if (m_IsDisableOnEnable && m_Wizard.IsLife)
            {
                OnReborn();
            }
        }

        private void OnDisable()
        {
            if (m_Wizard != null)
            {
                m_Wizard.OnDie -= OnDie;
                m_Wizard.OnReborn -= OnReborn;
            }
        }

        private void Awake()
        {
            m_Wizard = GetComponentInParent<WizardPlayer>();
            m_Vrik = GetComponent<VRIK>();
            m_Animator = GetComponent<Animator>();
            m_ChildRigidbodies.AddRange(m_Root.GetComponentsInChildren<Rigidbody>());
            m_ChildColliders.AddRange(m_Root.GetComponentsInChildren<Collider>());
        }

        private void Hide()
        {
            if (!m_Wizard.IsLife)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDie()
        {
            m_IsDie = true;
            gameObject.SetActive(true);
            
            Invoke(nameof(Hide), m_HideTimeout);
            SetRagDoll(true);
        }

        private void OnReborn()
        {
            CancelInvoke(nameof(Hide));
            m_IsDie = false;
            gameObject.SetActive(true);
            SetRagDoll(false);
        }

        private void SetRagDoll(bool useRagDoll)
        {
            //m_BodyCollider.isTrigger = useRagDoll;
            gameObject.SetActive(true);
            
            m_Animator.enabled = !useRagDoll;
            m_Vrik.enabled = !useRagDoll;

            foreach (Rigidbody childRigidbody in m_ChildRigidbodies)
            {
                childRigidbody.isKinematic = !useRagDoll;
                childRigidbody.useGravity = useRagDoll;
            }

            foreach (Collider childCollider in m_ChildColliders)
            {
                //childCollider.enabled = useRagDoll;
                childCollider.isTrigger = !useRagDoll;
            }
        }
    }
}