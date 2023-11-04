using System;
using UnityEngine;

namespace MobaVR
{
    public class CustomPhotonDataRegister : MonoBehaviour
    {
        private void Awake()
        {
            PhotonCustomHitData.Register();
        }
    }
}