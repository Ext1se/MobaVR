using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVP
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Classic Mode State/Start Mode State")]
    public class StartModeState : PvpClassicModeState
    {
        protected override void UpdatePlayer(PlayerVR player)
        {
            base.UpdatePlayer(player);
            //player.WizardPlayer.RestoreHp();
            player.WizardPlayer.Reborn();
        }

        public override void Enter()
        {
            m_Content.ModeView.InfoView.Show();
            
            m_Content.ZoneManager.Hide();
            m_Content.KillZoneManager.Hide();
            
            m_Content.ModeView.RedTeamScoreView.Hide();
            m_Content.ModeView.RedTeamKillScoreView.Hide();
            
            m_Content.ModeView.BlueTeamScoreView.Hide();
            m_Content.ModeView.BlueTeamKillScoreView.Hide();
            
            m_Mode.BlueTeam.Score = 0;
            m_Content.ModeView.BlueTeamScoreView.SetScore(0); //
            m_Content.ModeView.BlueTeamScoreView.Show();
            
            m_Mode.RedTeam.Score = 0;
            m_Content.ModeView.RedTeamScoreView.SetScore(0); //
            m_Content.ModeView.RedTeamScoreView.Show();

            if (PhotonNetwork.IsMasterClient)
            {
                UpdatePlayers();
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