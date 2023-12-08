using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MobaVR
{
    public class GameStatistics : MonoBehaviourPunCallbacks
    {
        [SerializeField] private AppSetting m_AppSetting;
        [SerializeField] private bool m_IsBackupData = false;
        [SerializeField] private bool m_IsAutoStartSession = true;

        private ClassicGameSession m_GameSession;
        private StatSessionView m_StatSessionView;

        private LocalRepository m_LocalRepository;
        private List<PlayerVR> m_Players = new();

        private int m_MaxCountPlayers = 0;
        private DateTime m_StartDateTime;
        private DateTime m_EndDateTime;

        public int MaxCountPlayers => m_MaxCountPlayers;

        public override void OnEnable()
        {
            base.OnEnable();
            if (m_GameSession != null)
            {
                m_GameSession.OnAddPlayer += AddPlayer;
                m_GameSession.OnRemovePlayer += RemovePlayer;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (m_GameSession != null)
            {
                m_GameSession.OnAddPlayer -= AddPlayer;
                m_GameSession.OnRemovePlayer -= RemovePlayer;
            }
        }

        private void Awake()
        {
            m_GameSession = FindObjectOfType<ClassicGameSession>();
            m_StatSessionView = FindObjectOfType<StatSessionView>();
            m_LocalRepository = new LocalRepository();
        }

        private void Start()
        {
            if (m_IsAutoStartSession)
            {
                StartSession();
            }
        }

        #region Player Score

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

        public void SendCalories(PlayerVR playerVR, float calories)
        {
            if (playerVR == null)
            {
                return;
            }

            playerVR.PlayerScore.ScoreData.CaloriesCount = (int)calories;
            playerVR.PlayerScore.UpdateScore();
        }

        #endregion

        #region Session

        public GameSessionStat GetSessionStat()
        {
            //string format = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
            string format = "yyyy-MM-dd'T'HH:mm:ss.fff";
            string startDate = m_StartDateTime.ToString(format);
            string endDate = m_EndDateTime.ToString(format);

            GameSessionStat gameSessionStat = new GameSessionStat()
            {
                ClubId = m_AppSetting.IdClub,
                GameId = m_AppSetting.IdGame,
                GameVersion = m_AppSetting.GameVersion,
                CountPlayers = MaxCountPlayers,
                EndTime = endDate,
                StartTime = startDate,
            };

            return gameSessionStat;
        }

        public void StartSession()
        {
            m_StartDateTime = DateTime.Now;
            m_LocalRepository.SetStartTime(m_StartDateTime);
        }

        public void CompleteSession()
        {
            m_EndDateTime = DateTime.Now;
            m_LocalRepository.SetEndTime(m_EndDateTime);
        }
        
        #endregion

        #region Listeners

        public virtual void RemovePlayer(PlayerVR playerVR)
        {
            m_Players.Remove(playerVR);
        }

        public virtual void AddPlayer(PlayerVR playerVR)
        {
            m_Players.Add(playerVR);
            m_MaxCountPlayers = m_Players.Count > m_MaxCountPlayers ? m_Players.Count : m_MaxCountPlayers;
        }

        #endregion


        #region Photon

        private void SavePlayerData()
        {
            if (!m_IsBackupData)
            {
                return;
            }

            if (m_GameSession == null)
            {
                return;
            }

            if (m_GameSession.LocalPlayer == null)
            {
                return;
            }

            BackupPlayerData backupPlayerData = new BackupPlayerData
            {
                PlayerData = m_GameSession.LocalPlayer.PlayerData,
                PlayerScoreData = m_GameSession.LocalPlayer.PlayerScore.ScoreData,
                BackupDate = DateTime.Now.Ticks
            };

            m_LocalRepository.SavePlayerData(backupPlayerData);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SavePlayerData();
            base.OnDisconnected(cause);
        }

        public override void OnConnected()
        {
            base.OnConnected();
            StartSession();
        }

        public override void OnLeftRoom()
        {
            SavePlayerData();
            base.OnLeftRoom();
        }

        #endregion
    }
}