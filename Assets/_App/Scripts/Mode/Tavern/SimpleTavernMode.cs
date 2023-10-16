using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.Tavern
{
    public class SimpleTavernMode : MonoBehaviour
    {
        [SerializeField] protected PlayerStateSO m_PlayerState;

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PlayerVR[] players = FindObjectsOfType<PlayerVR>();
                foreach (PlayerVR playerVR in players)
                {
                    playerVR.SetState(m_PlayerState);
                    playerVR.WizardPlayer.Reborn();
                }
            }
        }
    }
}