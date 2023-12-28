using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVE
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/PVE Mode State/Play Round State")]
    public class PlayRoundState : PveModeState
    {
        public override void Enter()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                UpdatePlayers();

                foreach (MonsterPointSpawner pointSpawner in m_Content.Spawners)
                {
                    pointSpawner.GenerateMonsters();
                }
            }

            m_Content.ModeView.InfoView.Hide();
            m_Content.ModeView.RoundTimeView.Hide();
            m_Content.ModeView.VictoryView.Hide();
            m_Content.ModeView.LoseView.Hide();
            
            m_Content.Lich.RpcRelease_Monster();
        }

        public override void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!m_Content.Lich.IsLife)
            {
                m_Mode.CompleteRound();
                if (m_Content.Sound != null)
                {
                    m_Content.Sound.PlayVictory();
                }
                
                return;
            }

            if (m_Mode.Players.Count(player => player.WizardPlayer.IsLife) <= 0)
            {
                m_Mode.CompleteRound();
                if (m_Content.Sound != null)
                {
                    m_Content.Sound.PlayLose();
                }
                
                return;
            }
        }

        public override void Exit()
        {
        }
    }
}