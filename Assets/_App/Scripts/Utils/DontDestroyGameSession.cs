using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class DontDestroyGameSession : MonoBehaviourPunCallbacks
    {
        private static DontDestroyGameSession Instance;
        
        [SerializeField] private GameObject m_GameSessionPrefab;
        private GameObject m_GameSession;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void CreateGameSession()
        {
            if (PhotonNetwork.IsConnected)
            {
                m_GameSession = Instantiate(m_GameSessionPrefab, transform);
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        }

        public override void OnJoinedRoom()
        {
            CreateGameSession();
            base.OnJoinedRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Destroy(m_GameSession.gameObject);
            base.OnDisconnected(cause);
        }
    }
}