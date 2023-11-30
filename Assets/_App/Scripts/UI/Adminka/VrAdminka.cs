using System;
using BNG;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class VrAdminka : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Menu m_Menu;
        [SerializeField] private GameObject m_XrInputPlayer;
        [SerializeField] private GameObject m_VrKeyboard;
        [SerializeField] private GameObject m_MainPanel;
        [SerializeField] private GameObject m_LoadingPanel;

        private bool m_IsLoading = false;
        
        private void Awake()
        {
            m_MainPanel.SetActive(true);
            m_LoadingPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftAlt))
            {
                ConnectToOnlineMode();
            }

            if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftAlt))
            {
                ConnectToLocalMode();
            }
        }

        private void SetLoadingView()
        {
            m_VrKeyboard.SetActive(false);
            m_MainPanel.SetActive(false);
            m_LoadingPanel.SetActive(true);
            
            ScreenFader screenFader = FindObjectOfType<ScreenFader>();
            if (screenFader != null)
            {
                screenFader.DoFadeIn();
            }
        }

        public void ConnectToLocalMode()
        {
            if (m_IsLoading)
            {
                return;
            }
            
            m_IsLoading = true;
            
            SetLoadingView();
            m_Menu.ConnectToLocalMode();
        }

        public void ConnectToOnlineMode()
        {
            if (m_IsLoading)
            {
                return;
            }
            
            m_IsLoading = true;
            
            SetLoadingView();
            m_Menu.ConnectToOnlineMode();
        }

        public override void OnConnected()
        {
            base.OnConnected();
            StopAllCoroutines();
            Destroy(m_XrInputPlayer.gameObject);
        }
    }
}