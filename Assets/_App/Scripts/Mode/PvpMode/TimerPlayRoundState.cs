using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVP
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Classic Mode State/Play Timer Round State")]
    public class TimerPlayRoundState : PvpClassicModeState
    {
        [SerializeField] private float m_Time = 3f;
        [SerializeField] private bool m_IsSumKills = true;
        private float m_CurrentTime;
        private bool m_IsTimeout = false;

        private int m_RedScore = 0;
        private int m_RedRoundScore = 0;
        private int m_BlueScore = 0;
        private int m_BlueRoundScore = 0;

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

            m_IsTimeout = false;
            m_CurrentTime = m_Time;

            m_BlueScore = m_IsSumKills ? m_Mode.BlueTeam.Score : 0;
            m_RedScore = m_IsSumKills ? m_Mode.RedTeam.Score : 0;

            m_Content.ModeView.BlueTeamKillScoreView.SetScore(m_BlueScore);
            m_Content.ModeView.RedTeamKillScoreView.SetScore(m_RedScore);

            m_Content.ModeView.RoundTimeView.Show();
            m_Content.ModeView.RoundTimeView.UpdateTime(m_Time);

            m_Content.ModeView.BlueTeamKillScoreView.Show();
            m_Content.ModeView.RedTeamKillScoreView.Show();
            
            m_Content.ZoneManager.Show();
            m_Content.KillZoneManager.Show();
            
            m_Content.ModeView.InfoView.Hide();
            m_Content.ModeView.PvpVictoryView.Hide();

            if (m_Mode.Player != null)
            {
                m_Mode.Player.WizardPlayer.OnDie -= OnDieLocalPlayer;
                m_Mode.Player.WizardPlayer.OnReborn -= OnRebornLocalPlayer;

                m_Mode.Player.WizardPlayer.OnDie += OnDieLocalPlayer;
                m_Mode.Player.WizardPlayer.OnReborn += OnRebornLocalPlayer;
            }

            foreach (PlayerVR player in m_Mode.RedTeam.Players)
            {
                player.WizardPlayer.OnDie += OnDieRedPlayer;
            }

            foreach (PlayerVR player in m_Mode.BlueTeam.Players)
            {
                player.WizardPlayer.OnDie += OnDieBluePlayer;
            }
        }
        
        private void OnDieLocalPlayer()
        {
            m_Content.ZoneManager.Show();
            m_Content.ModeView.InfoView.Show();
        }

        //TODO: Need to remove listeners when user go to other skin from PVP mode
        private void OnRebornLocalPlayer()
        {
            if (m_Content != null)
            {
                if (m_Content.ZoneManager != null)
                {
                    m_Content.ZoneManager.Hide();
                }

                if (m_Content.ModeView != null && m_Content.ModeView.InfoView != null)
                {
                    m_Content.ModeView.InfoView.Hide();
                }
            }
        }

        private void OnDieBluePlayer()
        {
            m_RedScore++;
            m_Mode.RedTeam.Kills = m_RedScore;
            m_Content.ModeView.RedTeamKillScoreView.SetScore(m_RedScore);
        }

        private void OnDieRedPlayer()
        {
            m_BlueScore++;
            m_Mode.BlueTeam.Kills = m_BlueScore;
            m_Content.ModeView.BlueTeamKillScoreView.SetScore(m_BlueScore);
        }

        public override void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                //return;
            }

            if (m_IsTimeout)
            {
                return;
            }

            m_CurrentTime -= Time.deltaTime;
            m_Content.ModeView.RoundTimeView.UpdateTime(m_CurrentTime);

            if (m_CurrentTime <= 0)
            {
                m_CurrentTime = 0f;
                m_IsTimeout = true;

                if (m_RedScore > m_BlueScore)
                {
                    m_Mode.RedTeam.Score++;
                    m_Content.ModeView.RedTeamScoreView.SetScore(m_Mode.RedTeam.Score);
                    
                    if (m_Content.PvpModeSound != null)
                    {
                        m_Content.PvpModeSound.PlayRedVictory();
                    }
                    
                    m_Mode.CompleteRound();
                }

                if (m_RedScore < m_BlueScore)
                {
                    m_Mode.BlueTeam.Score++;
                    m_Content.ModeView.BlueTeamScoreView.SetScore(m_Mode.BlueTeam.Score);
                    
                    if (m_Content.PvpModeSound != null)
                    {
                        m_Content.PvpModeSound.PlayBlueVictory();
                    }
                    
                    m_Mode.CompleteRound();
                }

                if (m_RedScore == m_BlueScore)
                {
                    m_Mode.RedTeam.Score++;
                    m_Content.ModeView.RedTeamScoreView.SetScore(m_Mode.RedTeam.Score);

                    m_Mode.BlueTeam.Score++;
                    m_Content.ModeView.BlueTeamScoreView.SetScore(m_Mode.BlueTeam.Score);
                    
                    if (m_Content.PvpModeSound != null)
                    {
                        m_Content.PvpModeSound.PlayCompleteRound();
                    }
                    
                    m_Mode.CompleteRound();
                }
            }
        }

        public override void Exit()
        {
            //m_Mode.ModeView.BlueTeamKillScoreView.Hide();
            //m_Mode.ModeView.RedTeamKillScoreView.Hide();

            m_Content.ModeView.RoundTimeView.Hide();
            m_Content.ZoneManager.Hide();
            m_Content.KillZoneManager.Hide();
            m_Content.ModeView.InfoView.Hide();

            /*
            foreach (PlayerVR player in m_Mode.RedTeam.Players)
            {
                player.WizardPlayer.OnDie -= OnDieRedPlayer;
            }

            foreach (PlayerVR player in m_Mode.BlueTeam.Players)
            {
                player.WizardPlayer.OnDie -= OnDieBluePlayer;
            }
            */

            foreach (PlayerVR player in m_Mode.Players)
            {
                player.WizardPlayer.OnDie -= OnDieRedPlayer;
                player.WizardPlayer.OnDie -= OnDieBluePlayer;
            }

            if (m_Mode.Player != null)
            {
                m_Mode.Player.WizardPlayer.OnDie -= OnDieLocalPlayer;
                m_Mode.Player.WizardPlayer.OnReborn -= OnRebornLocalPlayer;
            }
        }
    }
}