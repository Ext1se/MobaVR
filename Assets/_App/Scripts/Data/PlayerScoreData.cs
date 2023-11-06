using System;

namespace MobaVR
{
    [Serializable]
    public class PlayerScoreData
    {
        public int KillsCount;
        public int DeathsCount;
        public int AssistsCount;
        public int MonsterCount;
        public int CaloriesCount;

        public PlayerScoreData()
        {
        }
    }
}