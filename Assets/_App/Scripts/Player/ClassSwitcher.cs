using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class ClassSwitcher : MonoBehaviourPun
    {
        [Space]
        [Header("Classes")]
        [SerializeField] private WizardPlayer m_WizardPlayer;
        [SerializeField] private SkinCollection m_SkinCollection;
        [SerializeField] private List<ClassStats> m_Roles = new();

        [Space]
        [Header("Init role")]
        [SerializeField] private bool m_IsInitRoleOnStart;
        [SerializeField] private ClassStats m_StartRole;

        [Space]
        [Header("Spells")]
        [SerializeField] private SpellsHandler m_WizardSpellsHandler;
        [SerializeField] private SpellsHandler m_ArcherSpellsHandler;
        [SerializeField] private SpellsHandler m_DefenderSpellsHandler;

        private string m_CurrentIdClass;
        
        public Action<string> OnClassChange;

        public string CurrentIdClass => m_CurrentIdClass;

        private void Start()
        {
            if (photonView.IsMine && m_IsInitRoleOnStart && m_StartRole != null)
            {
                SetRole(m_StartRole.ClassId);
            }
        }

        private void Clear()
        {
            foreach (ClassStats role in m_Roles)
            {
                role.gameObject.SetActive(false);
            }
            
            m_WizardSpellsHandler.gameObject.SetActive(false);
            m_ArcherSpellsHandler.gameObject.SetActive(false);
            m_DefenderSpellsHandler.gameObject.SetActive(false);
        }

        public void SelectWizard()
        {
            photonView.RPC(nameof(RpcSelectWizard), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void RpcSelectWizard()
        {
            Clear();
            m_WizardSpellsHandler.gameObject.SetActive(true);
        }

        public void SelectArcher()
        {
            photonView.RPC(nameof(RpcSelectArcher), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void RpcSelectArcher()
        {
            Clear();
            m_ArcherSpellsHandler.gameObject.SetActive(true);
        }

        public void SelectDefender()
        {
            photonView.RPC(nameof(RpcSelectDefender), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void RpcSelectDefender()
        {
            Clear();
            m_DefenderSpellsHandler.gameObject.SetActive(true);
        }

        public void SetRole(string idClass)
        {
            photonView.RPC(nameof(RpcSetRole), RpcTarget.AllBuffered, idClass, true);
        }

        public void SetRole(string idClass, bool isMale)
        {
            photonView.RPC(nameof(RpcSetRole), RpcTarget.AllBuffered, idClass, isMale);
        }

        [PunRPC]
        private void RpcSetRole(string idClass, bool isMale)
        {
            ClassStats role = m_Roles.Find(role => role.ClassId.Equals(idClass));
            if (role != null)
            {
                m_CurrentIdClass = idClass;

                m_WizardPlayer.PlayerVR.PlayerData.IdRole = idClass;
                m_WizardPlayer.PlayerVR.PlayerData.IsMale = isMale;
                
                m_WizardPlayer.Stats = role.ClassStatsSo;
                //m_SkinCollection.SetAliveSkin(role.Skin);
                m_SkinCollection.SetAliveSkin(role.GetGenderSkin(isMale));
                
                Clear();
                role.gameObject.SetActive(true);
                
                OnClassChange?.Invoke(role.ClassId);
            }
        }
    }
}