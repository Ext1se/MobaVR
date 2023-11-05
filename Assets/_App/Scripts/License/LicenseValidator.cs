using System;
using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MobaVR
{
    public class LicenseValidator : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private string m_NextScene = "Menu";
        
        [Header("View")]
        [SerializeField] private Button m_ValidateButton;
        [SerializeField] private TMP_InputField m_InputField;
        [SerializeField] private NotificationManager m_Notification;
        [SerializeField] private GameObject m_ProgressBar;

        private BaseApiProvider m_ApiProvider;
        private CompanyHandler m_CompanyHandler;
        
        private void Awake()
        {
            m_ApiProvider = BaseApiProvider.Instance;
            m_CompanyHandler = CompanyHandler.Instance;
            m_ProgressBar.SetActive(false);
        }

        private void ShowNotification(string title, string description)
        {
            m_Notification.title = title;
            m_Notification.description = description;
            m_Notification.UpdateUI();
            m_Notification.OpenNotification();
        }

        public void Validate()
        {
            if (m_ApiProvider == null)
            {
                return;
            }
            
            m_ProgressBar.SetActive(true);
            m_ValidateButton.interactable = false;
            string key = m_InputField.text;
            
            m_ApiProvider.ValidateLicense(key, 
                                          m_CompanyHandler.AppSetting.IdGame,
                                          m_CompanyHandler.AppSetting.IdClub,
                                          new RequestResultCallback<LicenseKeyResponse>()
                                          {
                                              OnSuccess = response =>
                                              {
                                                  m_CompanyHandler.LicenseKeyResponse = response;
                                                  m_CompanyHandler.LicenseKey = response.Key;
                                                  SceneManager.LoadSceneAsync(m_NextScene);
                                              },
                
                                              OnError = message =>
                                              {
                                                  ShowNotification("Error", message);
                    
                                                  m_ProgressBar.SetActive(false);
                                                  m_ValidateButton.interactable = true;
                                              },
                
                                              OnFinish = () =>
                                              {
                                                  //m_ProgressBar.SetActive(false);
                                                  //m_ValidateButton.interactable = true;
                                              }
                                          });
        }
    }
}