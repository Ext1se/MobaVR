using System;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public class CaloriesScore : MonoBehaviourPun
    {
        [SerializeField] private float m_Delay = 5f;

        [Header("Transforms")]
        [SerializeField] private DistanceMonitor m_LeftHand;
        [SerializeField] private DistanceMonitor m_RightHand;
        [SerializeField] private DistanceMonitor m_Head;
        
        [Header("Settings")]
        [SerializeField] private float m_Mass = 60;
        [SerializeField] private float m_KHand = 0.5f;
        [SerializeField] private float m_KHead = 1f;

        [SerializeField] [ReadOnly] private float m_Sum = 0;

        private GameStatistics m_GameStatistics;
        private PlayerVR m_PlayerVR;
        
        public float Sum => m_Sum;
        
        private void OnEnable()
        {
            if (photonView.IsMine)
            {
                InvokeRepeating(nameof(SumCalories), 0, m_Delay);
            }
        }

        private void Awake()
        {
            m_PlayerVR = GetComponent<PlayerVR>();
            m_GameStatistics = FindObjectOfType<GameStatistics>();
        }

        private void OnDisable()
        {
            if (photonView.IsMine)
            {
                CancelInvoke(nameof(SumCalories));
            }
        }
        
        private void SumCalories()
        {
            float caloriesLeftHand = m_KHand * m_Mass * m_LeftHand.Sum;
            float caloriesRightHand = m_KHand * m_Mass * m_RightHand.Sum;
            float caloriesHead = m_KHead * m_Mass * m_Head.Sum;

            m_Sum = (caloriesHead + caloriesLeftHand + caloriesRightHand) / 1000f;
            photonView.RPC(nameof(RpcSumCalories), RpcTarget.All, m_Sum);
        }

        [PunRPC]
        private void RpcSumCalories(float calories)
        {
            m_GameStatistics.SendCalories(m_PlayerVR, calories);
        }
    }
}