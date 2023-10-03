using System;
using Michsky.MUIP;
using MobaVR.Utils;
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

        #region OnUpdate

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_PlayerVR != null)
            {
                m_PlayerVR.PlayerScore.OnUpdateScore -= OnUpdateScore;
            }
        }

        protected override void SetPlayerData()
        {
            base.SetPlayerData();

            m_PlayerVR.PlayerScore.OnUpdateScore += OnUpdateScore;
        }

        protected override void OnUpdateRole(string idRole)
        {
            m_RoleText.text = idRole;
        }
        
        private void OnUpdateScore()
        {
            PlayerScoreData scoreData = m_PlayerVR.PlayerScore.ScoreData;
            
            m_KillsCountText.text = scoreData.KillsCount.ToString();
            m_DeathsCountText.text = scoreData.DeathsCount.ToString();
            m_AssistsCountText.text = scoreData.AssistsCount.ToString();
            m_MonsterCountText.text = scoreData.MonsterCount.ToString();
            m_CaloriesCountText.text = scoreData.CaloriesCount.ToString();
        }

        protected override void OnUpdateNickName(string nickName)
        {
            m_NickNameText.text = nickName;
        }

        protected override void OnUpdateTeam(TeamType teamType)
        {
            m_TeamText.text = teamType switch
            {
                TeamType.RED => "<color=red>RED</color>",
                TeamType.BLUE => "<color=blue>BLUE</color>",
                _ => m_TeamText.text
            };
        }

        #endregion
    }
}