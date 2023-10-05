using System;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public class ScorePlayersView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private AdminStatContentView m_StatContentView;
        [SerializeField] [ReadOnly] private ClassicGameSession m_GameSession;

        private void OnDestroy()
        {
            if (!photonView.IsMine &&  m_GameSession != null)
            {
                m_GameSession.OnAddPlayer -= OnAddPlayer;
                m_GameSession.OnRemovePlayer -= OnRemovePlayer;
            }
        }

        private void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (m_GameSession == null)
            {
                m_GameSession = FindObjectOfType<ClassicGameSession>(true);
            }

            if (m_GameSession != null)
            {
                foreach (PlayerVR playerVR in  m_GameSession.Players)
                {
                    OnAddPlayer(playerVR);
                }
                
                m_GameSession.OnAddPlayer += OnAddPlayer;
                m_GameSession.OnRemovePlayer += OnRemovePlayer;
            }
        }

        private void OnRemovePlayer(PlayerVR playerVR)
        {
            m_StatContentView.RemovePlayer(playerVR);
        }

        private void OnAddPlayer(PlayerVR playerVR)
        {
            m_StatContentView.AddPlayer(playerVR);
        }

        public void UpdatePlayers()
        {
            m_StatContentView.UpdatePlayers();
        }
    }
}