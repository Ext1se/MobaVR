using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class DontDestroyGameSession : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject m_GameSession;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CreateGameSession()
        {
            if (PhotonNetwork.IsConnected)
            {
                m_GameSession.SetActive(true);
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
            Destroy(gameObject);
            base.OnDisconnected(cause);
        }
    }
}