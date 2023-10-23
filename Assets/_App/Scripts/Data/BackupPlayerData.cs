using System;
using Photon.Realtime;

namespace MobaVR
{
    [Serializable]
    public class BackupPlayerData
    {
        public PlayerData PlayerData = new PlayerData();
        public PlayerScoreData PlayerScoreData = new PlayerScoreData();
        public long BackupDate = -1;

        public BackupPlayerData()
        {
        }
    }
}