﻿using System;
using Photon.Realtime;

namespace MobaVR
{
    [Serializable]
    public class PlayerData
    {
        public int ActorNumber;
        public string IdUser;
        public string NickName;
        public string IdRole;
        public bool IsMale = true;

        public Player Player;

        public PlayerData()
        {
        }
    }
}