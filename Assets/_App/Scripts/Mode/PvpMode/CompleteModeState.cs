using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVP
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Classic Mode State/Complete Mode State")]
    public class CompleteModeState : PvpClassicModeState
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
                
                if (m_Content.Environment != null)
                {
                    m_Content.Environment.ResetEnvironment();
                }

                if (m_Mode.RedTeam.Score == m_Mode.BlueTeam.Score)
                {
                    m_Content.ModeView.PvpVictoryView.SetVictory(PvpVictoryView.PvpVictoryType.DRAW);
                }
                else
                {
                    if (m_Mode.RedTeam.Score > m_Mode.BlueTeam.Score)
                    {
                        m_Content.ModeView.PvpVictoryView.SetVictory(PvpVictoryView.PvpVictoryType.WIN_RED);
                    }
                    else
                    {
                        m_Content.ModeView.PvpVictoryView.SetVictory(PvpVictoryView.PvpVictoryType.WIN_BLUE);
                    }
                }
            }
            
            m_Content.ModeView.PvpVictoryView.Show();
            //m_Content.ModeView.VictoryView.Show();
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            //m_Content.ModeView.VictoryView.Hide();
            m_Content.ModeView.PvpVictoryView.Hide();
        }
    }
}