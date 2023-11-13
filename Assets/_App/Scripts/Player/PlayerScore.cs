using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public class PlayerScore : MonoBehaviour
    {
        private PlayerVR m_PlayerVR;
        [SerializeField] [ReadOnly] private PlayerScoreData m_ScoreData = new PlayerScoreData();

        public Action OnUpdateScore;
        
        public PlayerScoreData ScoreData => m_ScoreData;
        public PlayerVR PlayerVR => m_PlayerVR;

        private void Awake()
        {
            m_PlayerVR = GetComponent<PlayerVR>();
        }

        public void UpdateScore()
        {
            OnUpdateScore?.Invoke();
        }
    }
}