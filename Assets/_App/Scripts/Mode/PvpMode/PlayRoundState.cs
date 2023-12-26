using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVP
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Classic Mode State/Play Round State")]
    public class PlayRoundState : PvpClassicModeState
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
                
                if (m_Content.PvpModeSound != null)
                {
                    m_Content.PvpModeSound.PlayStartRound();
                }
            }
            
            m_Content.ModeView.InfoView.Hide();
            m_Content.KillZoneManager.Show();
        }

        public override void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            //TODO: checkPlayer is null
            int redLifeCount = m_Mode.RedTeam.Players.Count(player => player.WizardPlayer.IsLife);
            if (redLifeCount == 0)
            {
                m_Mode.BlueTeam.Score++;
                m_Content.ModeView.BlueTeamScoreView.SetScore(m_Mode.BlueTeam.Score);
                m_Mode.CompleteRound();
                
                if (m_Content.PvpModeSound != null)
                {
                    m_Content.PvpModeSound.PlayBlueVictory();
                }
                
                return;
            }
            
            int blueLifeCount = m_Mode.BlueTeam.Players.Count(player => player.WizardPlayer.IsLife);
            if (blueLifeCount == 0)
            {
                m_Mode.RedTeam.Score++;
                m_Content.ModeView.RedTeamScoreView.SetScore(m_Mode.RedTeam.Score);
                m_Mode.CompleteRound();
                
                if (m_Content.PvpModeSound != null)
                {
                    m_Content.PvpModeSound.PlayRedVictory();
                }
                
                return;
            }
        }

        public override void Exit()
        {
            m_Content.KillZoneManager.Hide();
        }
    }
}