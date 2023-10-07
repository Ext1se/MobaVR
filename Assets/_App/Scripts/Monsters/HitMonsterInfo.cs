using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace MobaVR
{
    [RequireComponent(typeof(PhotonView))]
    public class HitMonsterInfo : MonoBehaviourPun
    {
        [SerializeField] private int m_MonsterScore = 1;
        private Monster m_Monster;

        private GameStatistics m_GameStatistics;
        private PlayerVR m_Killer;

        private void OnEnable()
        {
            if (m_Monster != null)
            {
                m_Monster.OnLastHit += OnMonsterDie;
            }
        }

        private void OnDisable()
        {
            if (m_Monster != null)
            {
                m_Monster.OnLastHit -= OnMonsterDie;
            }
        }

        private void Awake()
        {
            m_Monster = GetComponent<Monster>();
            m_GameStatistics = FindObjectOfType<GameStatistics>();
            Reset();
        }
        
        private void OnMonsterDie(HitData hitData)
        {
            if (hitData == null || hitData.PlayerVR == null)
            {
                return;
            }

            m_Killer = hitData.PlayerVR;
            SendHitData();
        }

        private void SendHitData()
        {
            if (m_GameStatistics != null)
            {
                m_GameStatistics.SendMonsterDeathData(m_Killer, m_MonsterScore);
            }
        }

        private void Reset()
        {
            m_Killer = null;
        }
    }
}