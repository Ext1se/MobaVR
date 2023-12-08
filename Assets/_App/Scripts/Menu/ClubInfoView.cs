using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobaVR
{
    public class ClubInfoView : MonoBehaviour
    {
        [SerializeField] private ButtonManager m_RefreshButton;
        [SerializeField] private GameObject m_ProgressBar;
        [SerializeField] private GameObject m_InfoPanel;
        [SerializeField] private NotificationManager m_Notification;
        [SerializeField] private TextMeshProUGUI m_IdTextView;
        [SerializeField] private TextMeshProUGUI m_TitleTextView;
        [SerializeField] private TextMeshProUGUI m_ShortTitleTextView;
        [SerializeField] private TextMeshProUGUI m_AddressTextView;

        private BaseApiProvider m_ApiProvider;
        private CompanyHandler m_CompanyHandler;

        private void Awake()
        {
            m_ApiProvider = BaseApiProvider.Instance;
            m_CompanyHandler = CompanyHandler.Instance;
            m_ProgressBar.SetActive(false);
            m_InfoPanel.SetActive(false);
        }

        private void ShowNotification(string title, string description)
        {
            m_Notification.title = title;
            m_Notification.description = description;
            m_Notification.UpdateUI();
            m_Notification.OpenNotification();
        }

        public void GetInfo()
        {
            if (m_ApiProvider == null)
            {
                return;
            }

            m_ProgressBar.SetActive(true);
            m_InfoPanel.SetActive(false);
            m_RefreshButton.isInteractable = false;

            m_ApiProvider.GetClubInfo(
                m_CompanyHandler.AppSetting.IdClub,
                new RequestResultCallback<Club>()
                {
                    OnSuccess = response =>
                    {
                        m_InfoPanel.SetActive(true);
                        m_TitleTextView.text = response.Title;
                        m_ShortTitleTextView.text = response.ShortTitle;
                        m_AddressTextView.text = response.Address;
                        m_IdTextView.text = response.Id.ToString();
                    },

                    OnError = message =>
                    {
                        ShowNotification("Error", message);
                        m_InfoPanel.SetActive(false);
                    },

                    OnFinish = () =>
                    {
                        m_ProgressBar.SetActive(false);
                        m_RefreshButton.isInteractable = true;
                    }
                });
        }
    }
}