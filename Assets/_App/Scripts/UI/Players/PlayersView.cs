using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class PlayersView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ManagerDevice m_ManagerDevice;
        [SerializeField] private AdminInfoContentView m_InfoContentView;
        [SerializeField] private AdminStatContentView m_StatContentView;
        [SerializeField] [ReadOnly] private ClassicGameSession m_GameSession;

        private void OnDestroy()
        {
            if (m_ManagerDevice.IsAdmin && m_GameSession != null)
            {
                m_GameSession.OnAddPlayer -= OnAddPlayer;
                m_GameSession.OnRemovePlayer -= OnRemovePlayer;
            }
        }

        private void Awake()
        {
            if (m_ManagerDevice.IsAdmin)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                ShowInfoContentView();
            }
        }

        private void FindGameSession()
        {
            if (!m_ManagerDevice.IsAdmin || m_GameSession != null)
            {
                return;
            }

            if (m_GameSession == null)
            {
                m_GameSession = FindObjectOfType<ClassicGameSession>(true);
            }

            if (m_GameSession != null)
            {
                m_GameSession.OnAddPlayer += OnAddPlayer;
                m_GameSession.OnRemovePlayer += OnRemovePlayer;
            }
        }

        private void OnRemovePlayer(PlayerVR playerVR)
        {
            m_InfoContentView.RemovePlayer(playerVR);
            m_StatContentView.RemovePlayer(playerVR);
        }

        private void OnAddPlayer(PlayerVR playerVR)
        {
            m_InfoContentView.AddPlayer(playerVR);
            m_StatContentView.AddPlayer(playerVR);
        }

        public void UpdatePlayers()
        {
            m_InfoContentView.UpdatePlayers();
            m_StatContentView.UpdatePlayers();
        }

        public void ShowInfoContentView()
        {
            m_InfoContentView.gameObject.SetActive(true);
            m_StatContentView.gameObject.SetActive(false);
        }

        public void ShowStatContentView()
        {
            m_InfoContentView.gameObject.SetActive(false);
            m_StatContentView.gameObject.SetActive(true);
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            FindGameSession();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            FindGameSession();
        }
    }
}