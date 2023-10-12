using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class AdminStatPlayerView : BaseAdminPlayerView
    {
        [SerializeField] private TextMeshProUGUI m_NickNameText;
        [SerializeField] private TextMeshProUGUI m_TeamText;
        [SerializeField] private TextMeshProUGUI m_RoleText;
        [SerializeField] private TextMeshProUGUI m_KillsCountText;
        [SerializeField] private TextMeshProUGUI m_DeathsCountText;
        [SerializeField] private TextMeshProUGUI m_AssistsCountText;
        [SerializeField] private TextMeshProUGUI m_MonsterCountText;
        [SerializeField] private TextMeshProUGUI m_CaloriesCountText;

        [Header("User Settings")]
        [SerializeField] private Color m_LocalUserColor = Color.white;
        [SerializeField] private Color m_RemoteUserColor = Color.white;
        
        public Action<TeamType> OnUpdateTeamView;
        
        #region OnUpdate

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_PlayerVR != null)
            {
                m_PlayerVR.PlayerScore.OnUpdateScore -= OnUpdateScore;
            }
        }

        public override void SetPlayerData()
        {
            base.SetPlayerData();

            UpdateScore(m_PlayerVR.PlayerScore.ScoreData);
            m_PlayerVR.PlayerScore.OnUpdateScore += OnUpdateScore;
        }

        private void UpdateScore(PlayerScoreData scoreData)
        {
            m_KillsCountText.text = scoreData.KillsCount.ToString();
            m_DeathsCountText.text = scoreData.DeathsCount.ToString();
            m_AssistsCountText.text = scoreData.AssistsCount.ToString();
            m_MonsterCountText.text = scoreData.MonsterCount.ToString();
            m_CaloriesCountText.text = scoreData.CaloriesCount.ToString();
        }

        protected override void OnUpdateRole(string idRole)
        {
            m_RoleText.text = idRole;
        }
        
        private void OnUpdateScore()
        {
            PlayerScoreData scoreData = m_PlayerVR.PlayerScore.ScoreData;
            UpdateScore(scoreData);
        }
        
        protected override void OnUpdateNickName(string nickName)
        {
            if (m_PlayerVR != null && m_PlayerVR.photonView.IsMine)
            {
                m_NickNameText.color = m_LocalUserColor;
                m_NickNameText.text = $"<b>{nickName}</b>";
            }
            else
            {
                m_NickNameText.color = m_RemoteUserColor;
                m_NickNameText.text = nickName;
            }
        }

        protected override void OnUpdateTeam(TeamType teamType)
        {
            m_TeamText.text = teamType switch
            {
                TeamType.RED => "<color=red>RED</color>",
                TeamType.BLUE => "<color=blue>BLUE</color>",
                _ => m_TeamText.text
            };
            
            OnUpdateTeamView?.Invoke(teamType);
        }

        #endregion
    }
}