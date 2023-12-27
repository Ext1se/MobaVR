using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVE
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/PVE Mode State/Ready Round State")]
    public class ReadyRoundState : PveModeState
    {
        [SerializeField] private float m_Time = 10f;
        private float m_CurrentTime;
        private bool m_IsWaiting = false;
        
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
                //m_Mode.PlayRound();
            }

            if (m_Time > 0)
            {
                m_Content.ModeView.InfoView.Show();
                m_Content.ModeView.VictoryView.Hide();
                m_Content.ModeView.LoseView.Hide();
                
                m_Content.ModeView.RoundTimeView.Show();
                m_Content.ModeView.RoundTimeView.UpdateTime(m_Time);

                m_IsWaiting = true;
                m_CurrentTime = m_Time;
            }
            else
            {
                m_IsWaiting = false;
                m_Content.ModeView.InfoView.Hide();
                m_Content.ModeView.RoundTimeView.Hide();
                
                m_Mode.PlayRound();
            }
        }

        public override void Update()
        {
            if (m_IsWaiting)
            {
                m_CurrentTime -= Time.deltaTime;
                m_Content.ModeView.RoundTimeView.UpdateTime(m_CurrentTime);
                
                if (m_CurrentTime <= 0)
                {
                    m_CurrentTime = 0f;
                    m_IsWaiting = false;
                    m_Mode.PlayRound();
                }
            }
        }

        public override void Exit()
        {
            m_Content.ModeView.InfoView.Hide();
            m_Content.ModeView.RoundTimeView.Hide();
        }
    }
}