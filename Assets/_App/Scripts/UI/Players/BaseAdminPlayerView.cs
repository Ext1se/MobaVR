using System;
using Michsky.MUIP;
using MobaVR.Utils;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public abstract class BaseAdminPlayerView : MonoBehaviour
    {
        protected ClassicGameSession m_GameSession;
        protected PlayerVR m_PlayerVR;

        public PlayerVR PlayerVR
        {
            get => m_PlayerVR;
            set
            {
                m_PlayerVR = value;
                SetPlayerData();
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_PlayerVR != null)
            {
                m_PlayerVR.OnNickNameChange -= OnUpdateNickName;
                m_PlayerVR.OnRpcChangeTeam -= OnUpdateTeam;
                m_PlayerVR.ClassSwitcher.OnClassChange -= OnUpdateRole;
            }
        }

        protected virtual void Awake()
        {
            FindGameSession();
        }

        protected virtual void Start()
        {
            /*
            if (m_PlayerVR != null)
            {
                SetPlayerData();
            }
            */
        }

        private void FindGameSession()
        {
            if (m_GameSession == null)
            {
                m_GameSession = FindObjectOfType<ClassicGameSession>();
            }
        }
        

        public virtual void SetPlayerData()
        {
            FindGameSession();
            
            OnUpdateNickName(m_PlayerVR.PlayerData.NickName);
            m_PlayerVR.OnNickNameChange += OnUpdateNickName;

            OnUpdateTeam(m_PlayerVR.TeamType);
            m_PlayerVR.OnRpcChangeTeam += OnUpdateTeam;

            OnUpdateRole(m_PlayerVR.ClassSwitcher.CurrentIdClass);
            m_PlayerVR.ClassSwitcher.OnClassChange += OnUpdateRole;
        }

        #region OnUpdate

        protected abstract void OnUpdateRole(string idRole);
        protected abstract void OnUpdateNickName(string nickName);
        protected abstract void OnUpdateTeam(TeamType teamType);

        #endregion
    }
}