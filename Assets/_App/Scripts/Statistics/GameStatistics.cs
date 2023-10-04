using System.Collections.Generic;
using Photon.Pun;

namespace MobaVR
{
    public class GameStatistics : MonoBehaviourPun
    {
        private ClassicGameSession m_GameSession;

        private List<PlayerVR> m_Players = new();

        private void OnEnable()
        {
            if (m_GameSession != null)
            {
                m_GameSession.OnAddPlayer += AddPlayer;
                m_GameSession.OnRemovePlayer += RemovePlayer;
            }
        }
        
        private void OnDisable()
        {
            if (m_GameSession != null)
            {
                m_GameSession.OnAddPlayer -= AddPlayer;
                m_GameSession.OnRemovePlayer -= RemovePlayer;
            }
        }

        private void Awake()
        {
            m_GameSession = FindObjectOfType<ClassicGameSession>();
        }

        public void SendDeathData(DeathPlayerData deathPlayerData)
        {
            if (deathPlayerData.KillPlayer != null)
            {
                deathPlayerData.KillPlayer.PlayerScore.ScoreData.KillsCount += 1;
                deathPlayerData.KillPlayer.PlayerScore.UpdateScore();
            }
            
            if (deathPlayerData.DeadPlayer != null)
            {
                deathPlayerData.DeadPlayer.PlayerScore.ScoreData.DeathsCount += 1;
                deathPlayerData.DeadPlayer.PlayerScore.UpdateScore();
            }

            foreach (PlayerVR assistPlayer in deathPlayerData.AssistPlayers)
            {
                assistPlayer.PlayerScore.ScoreData.AssistsCount += 1;
                assistPlayer.PlayerScore.UpdateScore();
            }
        }

        public void SendMonsterDeathData(PlayerVR playerVR, int score)
        {
            if (playerVR == null)
            {
                return;
            }

            playerVR.PlayerScore.ScoreData.MonsterCount += score;
            playerVR.PlayerScore.UpdateScore();
        }

        public virtual void RemovePlayer(PlayerVR playerVR)
        {
            m_Players.Add(playerVR);
        }

        public virtual void AddPlayer(PlayerVR playerVR)
        {
            m_Players.Remove(playerVR);
        }
    }
}