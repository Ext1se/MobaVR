using System;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MobaVR
{
    public class SkinCollection : TeamItem
    {
        [Header("Wizard")]
        [SerializeField] private PlayerVR m_PlayerVR;
        [SerializeField] private PhotonView m_PhotonView;
        [SerializeField] private bool m_IsHideVR = true;

        [FormerlySerializedAs("m_Skins")]
        [Header("Skins")]
        [SerializeField] [ReadOnly] private SkinType m_LastSkinType = SkinType.ALIVE;
        [SerializeField] private List<Skin> m_AliveSkins = new();
        [SerializeField] private List<Skin> m_DeadSkins = new();
        [Header("Animal Skins")]
        [SerializeField] private List<AnimalSkin> m_AnimalSkins = new();

        private Skin m_AliveActiveSkin = null;
        private int m_AliveSkinPosition = 0;

        private Skin m_DeadActiveSkin = null;
        private int m_DeadSkinPosition = 0;

        private AnimalSkin m_AnimalSkin = null;
        private int m_AnimalSkinPosition = 0;
        
        private Vector3 m_LastPosition = Vector3.zero;
        private Quaternion m_LastRotation = Quaternion.identity;

        public List<Skin> AliveSkins => m_AliveSkins;
        public Skin AliveActiveSkin => m_AliveActiveSkin;

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

        private void OnEnable()
        {
            m_PlayerVR.WizardPlayer.OnDie += OnDie;
            m_PlayerVR.WizardPlayer.OnReborn += OnReborn;
        }

        private void OnDisable()
        {
            m_PlayerVR.WizardPlayer.OnDie -= OnDie;
            m_PlayerVR.WizardPlayer.OnReborn -= OnReborn;
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

            foreach (AnimalSkin skin in m_AnimalSkins)
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

        [ContextMenu("SetVisibilityDie")]
        public void SetVisibilityDie(bool isVisible = false)
        {
            foreach (Skin skin in m_AliveSkins)
            {
                skin.SetVisibilityDieRenderers(isVisible);
            }
            /*
            foreach (Skin skin in m_DeadSkins)
            {
                skin.SetVisibilityDieRenderers(isVisible);
            }
            */
        }

        #endregion

        #region Set Skin

        #region Alive Skin

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

        public void SetAliveSkin(Skin skin)
        {
            int position = m_AliveSkins.FindIndex(matchSkin => matchSkin == skin);
            if (position >= 0)
            {
                SetAliveSkin(position);
            }
        }

        [ContextMenu("SetAliveSkin")]
        public void SetAliveSkin(int position = 0)
        {
            if (m_PhotonView != null)
            {
                if (m_PhotonView.IsMine && m_IsHideVR)
                {
                    SetVisibilityVR(false);
                    SetVisibilityFace(false);
                }

                //m_PhotonView.RPC(nameof(RpcSetAliveSkin), RpcTarget.AllBuffered, position);
                m_PhotonView.RPC(nameof(RpcSetAliveSkin), RpcTarget.All, position);
            }
        }

        [PunRPC]
        private void RpcSetAliveSkin(int position)
        {
            // TODO: Очищать все активные скины через отдельный метод, а то лишние разы вызывает методы
            /*
            if (m_AnimalSkin != null)
            {
                m_AnimalSkin.DeactivateSkin();
            }
            */
            SaveLastPositionAndRotation();
            DeactivateAnimalSkin();

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
            m_LastSkinType = SkinType.ALIVE;

            TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
            m_AliveActiveSkin.SetPositionAndRotation(m_LastPosition, m_LastRotation);
            m_AliveActiveSkin.ActivateSkin(teamType);
        }

        public void SetAliveSkin(string idSkin)
        {
            if (m_PhotonView != null)
            {
                //m_PhotonView.RPC(nameof(RpcSetAliveSkinById), RpcTarget.AllBuffered, idSkin);
                m_PhotonView.RPC(nameof(RpcSetAliveSkinById), RpcTarget.All, idSkin);
            }
        }

        [PunRPC]
        private void RpcSetAliveSkinById(string idSkin)
        {
            Skin skin = m_AliveSkins.Find(skin => skin.ID.Equals(idSkin));
            if (skin != null)
            {
                SaveLastPositionAndRotation();
                DeactivateAnimalSkin();

                if (m_DeadActiveSkin != null)
                {
                    m_DeadActiveSkin.DeactivateSkin();
                }

                if (m_AliveActiveSkin != null)
                {
                    m_AliveActiveSkin.DeactivateSkin();
                }

                m_AliveActiveSkin = skin;
                m_LastSkinType = SkinType.ALIVE;

                TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
                m_AliveActiveSkin.SetPositionAndRotation(m_LastPosition, m_LastRotation);
                m_AliveActiveSkin.ActivateSkin(teamType);
            }
        }

        #endregion

        #region Dead Skin

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
                //m_PhotonView.RPC(nameof(RpcSetDeadSkin), RpcTarget.AllBuffered, position);
                m_PhotonView.RPC(nameof(RpcSetDeadSkin), RpcTarget.All, position);
            }
        }

        [PunRPC]
        public void RpcSetDeadSkin(int position = 0)
        {
            SaveLastPositionAndRotation();
            DeactivateAnimalSkin();

            if (m_AliveActiveSkin != null)
            {
                //TODO:
                //m_AliveActiveSkin.DeactivateSkin();
                
                SetVisibilityDie(true);
                m_AliveActiveSkin.SetPositionAndRotation(m_LastPosition, m_LastRotation);
                m_AliveActiveSkin.SetDieSkin();
            }

            if (m_DeadActiveSkin != null)
            {
                m_DeadActiveSkin.DeactivateSkin();
            }
            
            m_LastSkinType = SkinType.DEAD;
            
            m_DeadSkinPosition = Math.Clamp(position, 0, m_DeadSkins.Count - 1);
            m_DeadActiveSkin = m_DeadSkins[m_DeadSkinPosition];

            TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
            
            // TODO: add last position and rotation for dead skin
            m_DeadActiveSkin.SetPositionAndRotation(m_LastPosition, m_LastRotation);
            m_DeadActiveSkin.ActivateSkin(teamType);
        }

        #endregion

        #region Animal Skin

        private void DeactivateAnimalSkin()
        {
            if (m_AnimalSkin != null)
            {
                m_AnimalSkin.DeactivateSkin();

                m_AnimalSkin = null;
                m_AnimalSkinPosition = -1;
            }
        }

        [ContextMenu("SetDefaultAnimalSkin")]
        public void SetAnimalDefaultSkin()
        {
            SetAnimalSkin(0);
        }

        [ContextMenu("SetAnimalSkin")]
        public void SetAnimalSkin(int position = 0)
        {
            if (m_PhotonView != null)
            {
                //m_PhotonView.RPC(nameof(RpcSetAnimalSkin), RpcTarget.AllBuffered, position);
                m_PhotonView.RPC(nameof(RpcSetAnimalSkin), RpcTarget.All, position);
            }
        }

        [PunRPC]
        public void RpcSetAnimalSkin(int position = 0)
        {
            SaveLastPositionAndRotation();
            
            if (m_AliveActiveSkin != null)
            {
                m_AliveActiveSkin.DeactivateSkin();
            }

            if (m_DeadActiveSkin != null)
            {
                m_DeadActiveSkin.DeactivateSkin();
            }

            DeactivateAnimalSkin();
            
            m_LastSkinType = SkinType.ANIMAL;

            m_AnimalSkinPosition = Math.Clamp(position, 0, m_DeadSkins.Count - 1);
            m_AnimalSkin = m_AnimalSkins[m_AnimalSkinPosition];
            
            // TODO: скрываем тело свиньи. Нужно делать в другом месте
            // + скрываем, но не раскрываем потом. Могут быть траблы.
            if (m_PhotonView.IsMine && m_IsHideVR)
            {
                m_AnimalSkin.SetVisibilityVR(false);
            }

            TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
            m_AnimalSkin.SetPositionAndRotation(m_LastPosition, m_LastRotation);
            m_AnimalSkin.ActivateSkin(teamType);
        }

        #endregion

        private void SaveLastPositionAndRotation()
        {
            switch (m_LastSkinType)
            {
                case SkinType.ALIVE:
                    if (m_AliveActiveSkin != null)
                    {
                        m_LastPosition = m_AliveActiveSkin.transform.position;
                        m_LastRotation = m_AliveActiveSkin.transform.rotation;
                    }
                    break;
                case SkinType.DEAD:
                    if (m_DeadActiveSkin != null)
                    {
                        m_LastPosition = m_DeadActiveSkin.transform.position;
                        m_LastRotation = m_DeadActiveSkin.transform.rotation;
                    }
                    break;
                case SkinType.ANIMAL:
                    if (m_AnimalSkin != null)
                    {
                        m_LastPosition = m_AnimalSkin.transform.position;
                        m_LastRotation = m_AnimalSkin.transform.rotation;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ContextMenu("RestoreSkin")]
        public void RestoreSkin()
        {
            if (m_PhotonView != null)
            {
                //m_PhotonView.RPC(nameof(RpcRestoreSkin), RpcTarget.AllBuffered);
                m_PhotonView.RPC(nameof(RpcRestoreSkin), RpcTarget.All);
            }
        }

        [PunRPC]
        public void RpcRestoreSkin()
        {
            SaveLastPositionAndRotation();
            
            if (m_AliveActiveSkin == null)
            {
                return;
            }

            if (m_DeadActiveSkin != null)
            {
                m_DeadActiveSkin.DeactivateSkin();
            }

            DeactivateAnimalSkin();
            m_LastSkinType = SkinType.ALIVE;

            TeamType teamType = m_PlayerVR != null ? m_PlayerVR.TeamType : TeamType.RED;
            SetVisibilityDie(false);
            m_AliveActiveSkin.SetPositionAndRotation(m_LastPosition, m_LastRotation);
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

        private void OnDie()
        {
            if (m_PhotonView.IsMine && m_IsHideVR)
            {
                SetVisibilityVR(true);
                SetVisibilityFace(true);
            }
        }

        private void OnReborn()
        {
            if (m_PhotonView.IsMine && m_IsHideVR)
            {
                SetVisibilityVR(false);
                SetVisibilityFace(false);
            }
        }

        #endregion
    }
}