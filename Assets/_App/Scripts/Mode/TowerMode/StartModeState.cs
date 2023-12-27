using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.Tower
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Tower Mode State/Start Mode State")]
    public class StartModeState : TowerModeState
    {
        protected override void UpdatePlayer(PlayerVR player)
        {
            base.UpdatePlayer(player);
            player.WizardPlayer.Reborn();
        }

        public override void Enter()
        {
            m_Content.Tower.Restore();
            
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
            
            if (PhotonNetwork.IsMasterClient)
            {
                UpdatePlayers();
                
                if (!m_Content.Lich.IsLife)
                {
                    m_Content.Lich.Init();
                }
                else
                {
                    m_Content.Lich.Activate();
                }
                
                m_Mode.ReadyRound();
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