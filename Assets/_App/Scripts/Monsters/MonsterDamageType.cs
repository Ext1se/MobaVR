namespace MobaVR
{
    public partial class Monster
    {
        public enum MonsterDamageType
        {
            HP,
            IMMORTAL,
            HEAL,
            CRIT
        }

        public enum MonsterState
        {
            NOT_ACTIVE = 0,
            START_ACTIVATION = 100,
            COMPLETE_ACTIVATION = 101,

            IDLE = 300,
            MOVE = 400,

            ATTACK = 500,
            START_ATTACK = 501,
            COMPLETE_ATTACK = 502,

            DEATH = 900
        }

        public enum MonsterAttackType
        {
            MELEE_ATTACK = 0,
            RANGE_ATTACK = 1,
            SUPER_ATTACK = 2,
        }
    }
}