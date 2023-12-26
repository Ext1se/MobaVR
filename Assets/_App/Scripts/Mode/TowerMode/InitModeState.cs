using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.Tower
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Tower Mode State/Init Mode State")]
    public class InitModeState : TowerModeState
    {
        protected override void UpdatePlayer(PlayerVR player)
        {
            base.UpdatePlayer(player);
            player.WizardPlayer.Reborn();
        }
        
        public override void Enter()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                UpdatePlayers();
                
                foreach (MonsterPointSpawner pointSpawner in m_Content.Spawners)
                {
                    pointSpawner.ClearMonsters();
                }
            }

            /*
            foreach (SimpleTrap trap in m_Content.Traps)
            {
                trap.enabled = false;
            }
            */
            
            m_Content.ModeView.InfoView.Show();
            m_Content.ModeView.BlueTeamScoreView.Hide();
            m_Content.ModeView.BlueTeamKillScoreView.Hide();
            m_Content.ModeView.RedTeamScoreView.Hide();
            m_Content.ModeView.RedTeamKillScoreView.Hide();
            m_Content.ModeView.RoundTimeView.Hide();
            m_Content.ModeView.PreRoundTimeView.Hide();
            m_Content.ModeView.VictoryView.Hide();
            m_Content.ModeView.LoseView.Hide();
            m_Content.ModeView.MonsterCountView.Hide();
            
            //m_Content.Lich.Deactivate();
            m_Content.Lich.RpcPause_Monster();
            
            m_Content.IsVictory = false;

            m_Content.CurrentWave = 0;
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }
    }
}