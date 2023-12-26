using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class PingView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        
        private void Update()
        {
            m_Text.text = $"ip: {PhotonNetwork.ServerAddress}\n" +
                          $"ping: {PhotonNetwork.GetPing()}";
        }
    }
}