using UnityEngine;

namespace MobaVR
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private MenuLobby m_Lobby;

        private LocalRepository m_LocalRepository;
        private string m_IpAddress = "";

        private void Awake()
        {
            m_LocalRepository = new LocalRepository();
        }

        private void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftAlt))
            {
                ConnectToOnlineMode();
            }

            if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftAlt))
            {
                ConnectToLocalMode();
            }
            */
        }

        #region Language

        public void ChangeLanguage(LanguageManager.Language newLanguage)
        {
            if (LanguageManager.Instance == null)
            {
                return;
            }

            LanguageManager.Instance.ChangeLanguage(newLanguage);
        }

        public void SetRusLanguage()
        {
            ChangeLanguage(LanguageManager.Language.Rus);
        }

        public void SetEnglishLanguage()
        {
            ChangeLanguage(LanguageManager.Language.Eng);
        }

        public void SetChinaLanguage()
        {
            ChangeLanguage(LanguageManager.Language.Chn);
        }

        #endregion

        #region Server

        public void SetIpAddress(string ip)
        {
            m_IpAddress = ip;
        }

        public void ConnectToOnlineMode()
        {
            m_LocalRepository.SetLocalServer(false);
            m_Lobby.ConnectOnlineMode();
        }

        public void ConnectToLocalMode()
        {
            m_LocalRepository.SetLocalServer(true);
            if (!string.IsNullOrEmpty(m_IpAddress))
            {
                m_LocalRepository.SaveIpAddress(m_IpAddress);
                m_Lobby.ConnectLocalMode(m_IpAddress);
            }
        }

        #endregion
    }
}