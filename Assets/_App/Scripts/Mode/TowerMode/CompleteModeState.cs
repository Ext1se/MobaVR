using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.ClassicModeStateMachine.Tower
{
    [Serializable]
    [CreateAssetMenu(menuName = "API/Tower Mode State/Complete Mode State")]
    public class CompleteModeState : TowerModeState
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

            if (m_Content.IsVictory)
            {
                m_Content.ModeView.VictoryView.Show();
            }
            else
            {
                m_Content.ModeView.InfoView.Show();
                m_Content.ModeView.LoseView.Show();
            }

            //m_Content.CurrentWave = 0;
            m_Content.ResetWave();
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