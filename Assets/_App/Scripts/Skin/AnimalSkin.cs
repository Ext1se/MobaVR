using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MobaVR
{
    public class AnimalSkin : TeamItem, ISkin
    {
        public string ID;

        [Header("Armature")]
        [SerializeField] private Transform m_Armature;
        [SerializeField] [ReadOnly] private float m_ArmatureScale = 0.54f;

        [Header("Hand")]
        [SerializeField] private bool m_UseSkinHands = true;
        [SerializeField] private GameObject m_LeftHandModel;
        [SerializeField] private GameObject m_RightHandModel;

        [Header("Team")]
        [SerializeField] private List<SkinItem> m_TeamRenderers = new();
        [SerializeField] private List<Renderer> m_HiddenVrRenderers = new();

        private PhotonView m_PhotonView;

        [Space]
        [Header("Events")]
        public UnityEvent OnActivated;
        public UnityEvent OnDeactivated;
        public UnityEvent OnDie;

        public Transform Armature => m_Armature;

        #region Find Renderers

        [ContextMenu("FindArmature")]
        private void FindArmature()
        {
            if (m_Armature == null)
            {
                m_Armature = transform.Find("Armature");
            }
        }

        [ContextMenu("FindTeamRenderers")]
        private void FindTeamRenderers()
        {
            if (m_TeamRenderers.Count == 0)
            {
                m_TeamRenderers.AddRange(GetComponentsInChildren<SkinItem>(true));
            }
        }

        [ContextMenu("SetVisibility")]
        public void SetVisibilityVR(bool isVisible = false)
        {
            foreach (Renderer meshRenderer in m_HiddenVrRenderers)
            {
                meshRenderer.gameObject.SetActive(isVisible);
            }
        }

        #endregion


        private void Awake()
        {
            m_PhotonView = GetComponentInParent<PhotonView>();
        }

        #region Skin

        private void SetEnableHands(bool isEnable)
        {
            if (m_PhotonView != null && !m_PhotonView.IsMine)
            {
                return;
            }

            if (m_UseSkinHands)
            {
                if (m_LeftHandModel)
                {
                    m_LeftHandModel.gameObject.SetActive(isEnable);
                }

                if (m_RightHandModel)
                {
                    m_RightHandModel.gameObject.SetActive(isEnable);
                }
            }
        }
        
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            position.y = transform.position.y;
            transform.position = position;
            //transform.rotation = rotation;
        }

        public override void SetTeam(TeamType teamType)
        {
            base.SetTeam(teamType);

            foreach (SkinItem skinItem in m_TeamRenderers)
            {
                skinItem.SetTeam(teamType);
            }
        }

        public void ActivateSkin(TeamType teamType)
        {
            gameObject.SetActive(true);
            SetEnableHands(true);

            SetTeam(teamType);

            OnActivated?.Invoke();
        }

        public void DeactivateSkin()
        {
            gameObject.SetActive(false);
            SetEnableHands(false);

            OnDeactivated?.Invoke();
        }

        public void SetDieSkin()
        {
            gameObject.SetActive(false);
            SetEnableHands(false);

            OnDie?.Invoke();
        }

        #endregion
    }
}