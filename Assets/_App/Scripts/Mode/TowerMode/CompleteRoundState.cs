﻿using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.Tower
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Tower Mode State/Complete Round State")]
    public class CompleteRoundState : TowerModeState
    {
        public override void Enter()
        {
            m_Content.Lich.RpcPause_Monster();
            
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (MonsterPointSpawner pointSpawner in m_Content.Spawners)
                {
                    pointSpawner.ClearMonsters();
                }
                
                if (!m_Content.IsVictory)
                {
                    m_Mode.CompleteMode();
                    return;
                }
                
                m_Content.Lich.Pause();
                
                m_Content.CurrentWave++;
                if (m_Content.CurrentWave < m_Content.Waves.Count)
                {
                    if (m_Content.Sound != null)
                    {
                        m_Content.Sound.PlayWaveVictory();
                    }
                    
                    m_Mode.ReadyRound();
                }
                else
                {
                    if (m_Content.Sound != null)
                    {
                        m_Content.Sound.PlayCompleteMode();
                    }
                    
                    m_Mode.CompleteMode();
                }
            }
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }
    }
}