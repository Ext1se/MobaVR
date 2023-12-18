using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVP
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Classic Mode State/Inactive Mode State")]
    public class InactiveModeState : PvpClassicModeState
    {
        public override void Enter()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                UpdatePlayers();
            }

            m_Content.ModeView.InfoView.Show();
            m_Content.ModeView.BlueTeamScoreView.Hide();
            m_Content.ModeView.BlueTeamKillScoreView.Hide();
            m_Content.ModeView.RedTeamScoreView.Hide();
            m_Content.ModeView.RedTeamKillScoreView.Hide();
            m_Content.ModeView.RoundTimeView.Hide();
            m_Content.ModeView.PreRoundTimeView.Hide();
            m_Content.ModeView.VictoryView.Hide();
            //m_Content.ModeView.LoseView.Hide();
            m_Content.ZoneManager.Hide();
            m_Content.KillZoneManager.Hide();
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}