﻿using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobaVR
{
    public class DontDestroyGameSession_2 : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject m_GameSession;
        [SerializeField] private string m_DestroySceneName = "Menu";

        private void Awake()
        {
            DontDestroyGameSession[] dontDestroyGameSessions = FindObjectsOfType<DontDestroyGameSession>();
            foreach (DontDestroyGameSession dontDestroyGameSession in dontDestroyGameSessions)
            {
                if (dontDestroyGameSession != this)
                {
                    Destroy(dontDestroyGameSession.gameObject);
                }
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name.Equals(m_DestroySceneName))
            {
                DestroyObjects();
            }
        }

        private void DestroyObjects()
        {
            if (m_GameSession.gameObject != null)
            {
                Destroy(m_GameSession.gameObject);
            }

            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        public void CreateGameSession()
        {
            if (PhotonNetwork.IsConnected)
            {
                m_GameSession.SetActive(true);
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        }

        public override void OnLeftLobby()
        {
            DestroyObjects();
            base.OnLeftLobby();
        }

        public override void OnLeftRoom()
        {
            DestroyObjects();
            base.OnLeftRoom();
        }

        public override void OnJoinedRoom()
        {
            CreateGameSession();
            base.OnJoinedRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            DestroyObjects();
            base.OnDisconnected(cause);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            DestroyObjects();
            base.OnJoinRoomFailed(returnCode, message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            DestroyObjects();
            base.OnJoinRandomFailed(returnCode, message);
        }
    }
}