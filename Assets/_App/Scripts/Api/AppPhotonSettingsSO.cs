using System;
using UnityEngine;

namespace MobaVR
{
    [Serializable]
    [CreateAssetMenu(fileName = "ClassStats", menuName = "MobaVR API/Create photon settings")]
    public class AppPhotonSettingsSO : ScriptableObject
    {
        [SerializeField] private string m_DefaultRoom = "Arena";
        [SerializeField] private string m_OnlineKey = "2a8deb94-f484-4b03-b45a-b37ae3a077cc";
        [SerializeField] private string m_OfflineKey = "1234567890-1234567890-1234567890";

        public string DefaultRoom => m_DefaultRoom;
        public string OnlineKey => m_OnlineKey;
        public string OfflineKey => m_OfflineKey;
    }
}