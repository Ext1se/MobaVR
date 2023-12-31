﻿using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MobaVR
{
    [Serializable]
    public class HitData
    {
        #region Hit

        public HitActionType Action = HitActionType.Damage;
        public float Amount = 0;
        public Vector3 Position = Vector3.zero;
        public bool CanApplyBySelf = false;
        public bool CanApplyForTeammates = false;

        #endregion

        #region Team

        public TeamType TeamType = TeamType.OTHER;

        #endregion

        #region Owner Source

        public PlayerVR PlayerVR;
        public Player Player;
        public bool IsMine;
        public PhotonView PhotonOwner;
        public PhotonView PhotonView;
        //public Transform Source;
        //public Transform Owner;
        //public PlayerRef InstigatorRef;

        #endregion

        #region Time

        public long HitDate = 0;
        public DateTime DateTime;

        #endregion

    }
}