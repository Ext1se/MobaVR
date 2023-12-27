using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class ZoneManager : MonoBehaviourPun
    {
        [SerializeField] private List<RespawnZone> m_RespawnZones;

        public void Show()
        {
            foreach (RespawnZone respawnZone in m_RespawnZones)
            {
                if (respawnZone != null)
                {
                    respawnZone.gameObject.SetActive(true);
                }
            }
        }

        public void Hide()
        {
            foreach (RespawnZone respawnZone in m_RespawnZones)
            {
                if (respawnZone != null)
                {
                    respawnZone.gameObject.SetActive(false);
                }
            }
        }
    }
}