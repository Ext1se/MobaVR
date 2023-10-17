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

        private BaseApiProvider m_ApiProvider;
        
        private void Awake()
        {
            m_ApiProvider = FindObjectOfType<BaseApiProvider>();
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
            
            m_ValidateButton.interactable = false;
            string key = m_InputField.text;
            
            m_ApiProvider.ValidateLicense(key, new RequestResultCallback<LicenseKeyResponse>()
            {
                OnSuccess = response =>
                {
                    SceneManager.LoadScene(m_NextScene);
                },
                
                OnError = message =>
                {
                    m_ValidateButton.interactable = true;
                    ShowNotification("Error", message);
                }
            });
        }
    }
}