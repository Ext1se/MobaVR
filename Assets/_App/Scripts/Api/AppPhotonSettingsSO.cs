using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MobaVR
{
    [Serializable]
    [CreateAssetMenu(fileName = "ClassStats", menuName = "MobaVR API/Create photon settings")]
    public class AppPhotonSettingsSO : ScriptableObject
    {
        [SerializeField] private string m_DefaultRoom = "Arena";
        [Header("Online")]
        [SerializeField] private string m_OnlineKey = "2a8deb94-f484-4b03-b45a-b37ae3a077cc";
        [SerializeField] private int m_OnlinePort = 0;
       
        [Header("Local")]
        [SerializeField] private string m_LocalKey = "1234567890-1234567890-1234567890";
        [SerializeField] private int m_LocalPort = 5055;

        public string DefaultRoom => m_DefaultRoom;
        public string OnlineKey => m_OnlineKey;
        public string LocalKey => m_LocalKey;
        public int OnlinePort => m_OnlinePort;
        public int LocalPort => m_LocalPort;
    }
}