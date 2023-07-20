using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace MobaVR
{
    public class SkinCollection : TeamItem
    {
        [Header("Wizard")]
        [SerializeField] private PlayerVR m_PlayerVR;
        [SerializeField] private PhotonView m_PhotonView;

        [FormerlySerializedAs("m_Skins")]
        [Header("Skins")]
        [SerializeField] private List<Skin> m_AliveSkins = new();
        [SerializeField] private List<Skin> m_DeadSkins = new();

        private Skin m_AliveActiveSkin = null;
        private int m_AliveSkinPosition = 0;

        private Skin m_DeadActiveSkin = null;
        private int m_DeadSkinPosition = 0;

        public List<Skin> AliveSkins => m_AliveSkins;

        #region MonoBehaviour

        private void OnValidate()
        {
            if (m_PlayerVR == null)
            {
                m_PlayerVR = GetComponentInParent<PlayerVR>();
            }

            if (m_AliveSkins.Count == 0)
            {
                m_AliveSkins.AddRange(GetComponentsInChildren<Skin>(true));
            }

            if (m_PhotonView == null)
            {
                m_PhotonView = GetComponent<PhotonView>();
            }
        }

        private void Awake()
        {
            Clear();
            //SetSkin(0);
            RpcSetAliveSkin(0);
        }

        #endregion

        #region Visibility

        private void Clear()
        {
            foreach (Skin skin in m_AliveSkins)
            {
                skin.DeactivateSkin();
            }

            foreach (Skin skin in m_DeadSkins)
            {
                skin.DeactivateSkin();
            }
        }

        [ContextMenu("SetVisibilityLegs")]
        public void SetVisibilityLegs(bool isVisible = false)
        {
            foreach (Skin skin in m_AliveSkins)
            {
                skin.SetVisibilityLegs(isVisible);
            }
            
            foreach (Skin skin in m_DeadSkins)
            {
                skin.SetVisibilityLegs(isVisible);
            }
        }

        [ContextMenu("SetVisibilityFace")]
        public void SetVisibilityFace(bool isVisible = false)
        {
            foreach (Skin skin in m_AliveSkins)
            {
                skin.SetVisibilityFace(isVisible);
            }
            
            foreach (Skin skin in m_DeadSkins)
            {
                skin.SetVisibilityFace(isVisible);
            }
        }

        [ContextMenu("SetVisibilityVR")]
        public void SetVisibilityVR(bool isVisible = false)
        {
            foreach (Skin skin in m_AliveSkins)
            {
                skin.SetVisibilityVR(isVisible);
            }
            
            foreach (Skin skin in m_DeadSkins)
            {
                skin.SetVisibilityVR(isVisible);
            }
        }

        [ContextMenu("SetVisibilityBody")]
        public void SetVisibilityBody(bool isVisible = false)
        {
            foreach (Skin skin in m_AliveSkins)
            {
                skin.SetVisibilityBody(isVisible);
            }
            
            foreach (Skin skin in m_DeadSkins)
            {
                skin.SetVisibilityBody(isVisible);
            }
        }

        #endregion

        #region Set Skin

        [ContextMenu("SetNextSkin")]
        public void SetNextSkin()
        {
            m_AliveSkinPosition++;
            m_AliveSkinPosition %= m_AliveSkins.Count;
            SetAliveSkin(m_AliveSkinPosition);
        }

        [ContextMenu("SetPrevSkin")]
        public void SetPrevSkin()
        {
            m_AliveSkinPosition--;
            m_AliveSkinPosition %= m_AliveSkins.Count;
            SetAliveSkin(m_AliveSkinPosition);
        }

        [ContextMenu("SetAliveDefaultSkin")]
        public void SetAliveDefaultSkin()
        {
            SetAliveSkin(0);
        }

        [ContextMenu("SetAliveSkin")]
        public void SetAliveSkin(int position = 0)
        {
            if (m_PhotonView != null)
            {
                m_PhotonView.RPC(nameof(RpcSetAliveSkin), RpcTarget.AllBuffered, position);
            }
        }

        [PunRPC]
        private void RpcSetAliveSkin(int position)
        {
            if (m_DeadActiveSkin != null)
            {
                m_DeadActiveSkin.DeactivateSkin();
            }

            if (m_AliveActiveSkin != null)
            {
                m_AliveActiveSkin.DeactivateSkin();
            }

            m_AliveSkinPosition = Math.Clamp(position, 0, m_AliveSkins.Count - 1);
            m_AliveActiveSkin = m_AliveSkins[m_AliveSkinPosition];

            TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
            m_AliveActiveSkin.ActivateSkin(teamType);
        }

        [ContextMenu("SetDeadDefaultSkin")]
        public void SetDeadDefaultSkin()
        {
            SetDeadSkin(0);
        }

        [ContextMenu("SetDeadSkin")]
        public void SetDeadSkin(int position = 0)
        {
            if (m_PhotonView != null)
            {
                m_PhotonView.RPC(nameof(RpcSetDeadSkin), RpcTarget.AllBuffered, position);
            }
        }

        [PunRPC]
        private void RpcSetDeadSkin(int position = 0)
        {
            if (m_AliveActiveSkin != null)
            {
                m_AliveActiveSkin.DeactivateSkin();
            }

            if (m_DeadActiveSkin != null)
            {
                m_DeadActiveSkin.DeactivateSkin();
            }

            m_DeadSkinPosition = Math.Clamp(position, 0, m_DeadSkins.Count - 1);
            m_DeadActiveSkin = m_DeadSkins[m_DeadSkinPosition];

            TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
            m_DeadActiveSkin.ActivateSkin(teamType);
        }

        [ContextMenu("RestoreSkin")]
        public void RestoreSkin()
        {
            if (m_PhotonView != null)
            {
                m_PhotonView.RPC(nameof(RpcRestoreSkin), RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void RpcRestoreSkin()
        {
            if (m_AliveActiveSkin == null)
            {
                return;
            }

            if (m_DeadActiveSkin != null)
            {
                m_DeadActiveSkin.DeactivateSkin();
            }

            TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
            m_AliveActiveSkin.ActivateSkin(teamType);
        }

        public override void SetTeam(TeamType teamType)
        {
            base.SetTeam(teamType);
            if (m_AliveActiveSkin != null)
            {
                m_AliveActiveSkin.SetTeam(teamType);
            }
        }

        #endregion
    }
}