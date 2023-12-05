using System;
using Michsky.MUIP;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobaVR
{
    public class StatSessionView : MonoBehaviour
    {
        [SerializeField] private GameObject m_MainPanel;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private Button m_SendButton;
        [SerializeField] private GameObject m_ProgressBar;
        [SerializeField] private SwitchManager m_SwitchManager;
        [SerializeField] private NotificationManager m_Notification;
        [SerializeField] private TextMeshProUGUI m_IdTextView;
        [SerializeField] private TextMeshProUGUI m_CountPlayerTextView;
        [SerializeField] private TextMeshProUGUI m_StartDateTextView;
        [SerializeField] private TextMeshProUGUI m_EndDateTextView;

        private BaseApiProvider m_ApiProvider;
        private CompanyHandler m_CompanyHandler;
        private GameStatistics m_GameStatistics;
        private ClassicGameSession m_GameSession;

        private GameSessionStat m_GameSessionStat;

        private void Awake()
        {
            m_GameSession = FindObjectOfType<ClassicGameSession>();
            m_GameStatistics = FindObjectOfType<GameStatistics>();
            
            m_ApiProvider = BaseApiProvider.Instance;
            m_CompanyHandler = CompanyHandler.Instance;
            m_ProgressBar.SetActive(false);
            m_MainPanel.SetActive(false);
        }

        public void StartSession()
        {
            m_GameStatistics.StartSession();
        }

        public void CompleteSession()
        {
            m_GameStatistics.CompleteSession();
            SetInfo(m_GameStatistics.GetSessionStat());
            
            m_MainPanel.SetActive(true);
        }

        private void ShowNotification(string title, string description)
        {
            m_Notification.title = title;
            m_Notification.description = description;
            m_Notification.UpdateUI();
            m_Notification.OpenNotification();
        }

        public void SetInfo(GameSessionStat gameSessionStat)
        {
            m_GameSessionStat = gameSessionStat;
            string format = "dd MMMM yyyy HH:mm:ss";

            m_IdTextView.text = gameSessionStat.GameId.ToString();
            m_CountPlayerTextView.text = gameSessionStat.CountPlayers.ToString();
            m_StartDateTextView.text = DateTime.Parse(gameSessionStat.StartTime).ToString(format);
            m_EndDateTextView.text = DateTime.Parse(gameSessionStat.EndTime).ToString(format);
        }

        private void HideViews()
        {
            m_ProgressBar.SetActive(true);
            m_CancelButton.interactable = false;
            m_SendButton.interactable = false;
        }
        
        private void ShowViews()
        {
            m_ProgressBar.SetActive(false);
            m_CancelButton.interactable = true;
            m_SendButton.interactable = true;
        }

        private void CloseGame()
        {
            m_GameSession.CloseGame();
        }

        public void Send()
        {
            if (m_ApiProvider == null)
            {
                return;
            }

            HideViews();

            if (m_SwitchManager.isOn)
            {
                m_ApiProvider.SendGameSession(
                    m_GameSessionStat,
                    new RequestResultCallback<GameSessionStat>()
                    {
                        OnSuccess = response =>
                        {
                            m_GameSession.CloseGame();
                        },

                        OnError = message =>
                        {
                            ShowNotification("Error", message);
                            ShowViews();
                        },

                        OnFinish = () =>
                        {
                        }
                    });
            }
            else
            {
                m_GameSession.CloseGame();
            }
        }
    }
}