using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MobaVR
{
    [Serializable]
    [CreateAssetMenu(fileName = "PlayerState", menuName = "MobaVR API/Create player state")]
    public class PlayerStateSO : ScriptableObject
    {
        [SerializeField] private PlayerState m_PlayerState = PlayerState.PLAY_PVP;
        [SerializeField] private bool m_IsInPlayMode = false;
        [SerializeField] private bool m_CanCast = false;
        [SerializeField] private bool m_CanGetDamage = false;
        [SerializeField] private bool m_CanGetDamageFromFriendPlayers = false;
        [SerializeField] private bool m_CanGetDamageFromEnemyPlayers = true;
        [SerializeField] private bool m_IsLife = false;

        public PlayerState State
        {
            get => m_PlayerState;
            set => m_PlayerState = value;
        }
        public bool IsInPlayMode
        {
            get => m_IsInPlayMode;
            set => m_IsInPlayMode = value;
        }
        public bool CanCast
        {
            get => m_CanCast;
            set => m_CanCast = value;
        }
        public bool CanGetDamage
        {
            get => m_CanGetDamage;
            set => m_CanGetDamage = value;
        }
        public bool CanGetDamageFromFriendPlayers
        {
            get => m_CanGetDamageFromFriendPlayers;
            set => m_CanGetDamageFromFriendPlayers = value;
        }
        public bool CanGetDamageFromEnemyPlayers
        {
            get => m_CanGetDamageFromEnemyPlayers;
            set => m_CanGetDamageFromEnemyPlayers = value;
        }
        public bool IsLife
        {
            get => m_IsLife;
            set => m_IsLife = value;
        }

        /*
        public PlayerState State => m_PlayerState;
        public bool IsInPlayMode => m_IsInPlayMode;
        public bool CanCast => m_CanCast;
        public bool CanGetDamage => m_CanGetDamage;
        public bool IsLife => m_IsLife;
        public bool CanGetDamageFromFriendPlayers => m_CanGetDamageFromFriendPlayers;
        public bool CanGetDamageFromEnemyPlayers => m_CanGetDamageFromEnemyPlayers;
        */
        
        public void PasteCopyValue(PlayerStateSO playerStateSo)
        {
            m_PlayerState = playerStateSo.State;
            m_IsInPlayMode = playerStateSo.IsInPlayMode;
            m_CanCast = playerStateSo.CanCast;
            m_CanGetDamage = playerStateSo.CanGetDamage;
            m_CanGetDamageFromFriendPlayers = playerStateSo.CanGetDamageFromFriendPlayers;
            m_CanGetDamageFromEnemyPlayers = playerStateSo.CanGetDamageFromEnemyPlayers;
            m_IsLife = playerStateSo.IsLife;
        }
    }
}