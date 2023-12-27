using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.Tower
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Tower Mode State/Ready Round State")]
    public class ReadyRoundState : TowerModeState
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
            }

            if (m_Content.Lich.IsLife)
            {
                m_Content.Lich.RpcPause_Monster();
            }

            m_Content.ModeView.InfoView.Show();
            m_Content.ModeView.PreRoundTimeView.Show();
            m_Content.ModeView.PreRoundTimeView.UpdateTime(m_Time);
            
            m_IsWaiting = true;
            m_CurrentTime = m_Time;
        }

        public override void Update()
        {
            if (m_IsWaiting)
            {
                m_CurrentTime -= Time.deltaTime;
                m_Content.ModeView.PreRoundTimeView.UpdateTime(m_CurrentTime);
                
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
            m_Content.ModeView.PreRoundTimeView.Hide();
            m_Content.ModeView.InfoView.Hide();
        }
    }
}