using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.PVE
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/PVE Mode State/Complete Mode State")]
    public class CompleteModeState : PveModeState
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
                
                if (m_Content.Lich.IsLife)
                {
                    m_Content.Lich.Deactivate();
                }
                
                foreach (MonsterPointSpawner pointSpawner in m_Content.Spawners)
                {
                    pointSpawner.ClearMonsters();
                }
            }
            
            m_Content.ModeView.InfoView.Show();
            m_Content.ModeView.RoundTimeView.Hide();
            m_Content.ModeView.VictoryView.Hide();
            m_Content.ModeView.LoseView.Hide();

            if (m_Content.Lich.IsLife)
            {
                //TODO: WIN
                m_Content.ModeView.InfoView.Show();
                m_Content.ModeView.LoseView.Show();
            }
            else
            {
                //TODO: LOSE
                m_Content.ModeView.InfoView.Hide();
                m_Content.ModeView.VictoryView.Show();
            }
            
            m_Content.Lich.RpcPause_Monster();
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }
    }
}