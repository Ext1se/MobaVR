using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MobaVR
{
    public class TeamScoreContentView : MonoBehaviour
    {
        [SerializeField] protected AdminStatPlayerView m_PlayerView;
        [SerializeField] protected RectTransform m_MainPanel;
        [SerializeField] protected Transform m_RedTeamContentPoint;
        [SerializeField] protected Transform m_BlueTeamContentPoint;

        [SerializeField] [ReadOnly] protected List<AdminStatPlayerView> m_PlayerViews = new();

        public virtual void RemovePlayer(PlayerVR playerVR)
        {
            AdminStatPlayerView playerInfoView = m_PlayerViews.Find(view => view.PlayerVR == playerVR);
            RemovePlayerView(playerInfoView);
        }

        public virtual void AddPlayer(PlayerVR playerVR)
        {
            Transform contentTransform =
                playerVR.TeamType == TeamType.RED ? m_RedTeamContentPoint : m_BlueTeamContentPoint;

            AdminStatPlayerView playerInfo = Instantiate(m_PlayerView, contentTransform);
            playerInfo.OnUpdateTeamView += (teamType) => OnUpdateTeamView(playerInfo, teamType);
            playerInfo.PlayerVR = playerVR;
            m_PlayerViews.Add(playerInfo);
            
            UpdateLayout();
        }

        private void RemovePlayerView(AdminStatPlayerView playerInfoView)
        {
            if (playerInfoView != null)
            {
                playerInfoView.OnUpdateTeamView -= (teamType) => OnUpdateTeamView(playerInfoView, teamType);
                m_PlayerViews.Remove(playerInfoView);
                Destroy(playerInfoView.gameObject);
            }
        }
        
        private void OnUpdateTeamView(AdminStatPlayerView playerInfo, TeamType teamType)
        {
            if (playerInfo == null)
            {
                //TODO
                return;
            }
            
            playerInfo.transform.parent = teamType == TeamType.RED ? m_RedTeamContentPoint : m_BlueTeamContentPoint;
            UpdateLayout();
        }

        [ContextMenu("UpdateLayout")]
        private void UpdateLayout()
        {
            StopAllCoroutines();
            StartCoroutine(WaitAndUpdateLayout());
        }

        private IEnumerator WaitAndUpdateLayout()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_MainPanel);
            m_MainPanel.ForceUpdateRectTransforms();
        }

        public void UpdatePlayers()
        {
            for (int i = m_PlayerViews.Count - 1; i >= 0; i--)
            {
                //Destroy(m_PlayerViews[i].gameObject);
                RemovePlayerView(m_PlayerViews[i]);
            }

            m_PlayerViews.Clear();

            PlayerVR[] players = FindObjectsOfType<PlayerVR>();
            foreach (PlayerVR playerVR in players)
            {
                AddPlayer(playerVR);
            }
            
            UpdateLayout();
        }
    }
}