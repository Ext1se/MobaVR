using UnityEngine;

namespace MobaVR
{
    public abstract class BaseModeView : MonoBehaviour
    {
        [SerializeField] protected BaseTeamScoreView m_RedTeamScoreView;
        [SerializeField] protected BaseTeamScoreView m_BlueTeamScoreView;
        [SerializeField] protected BaseTeamScoreView m_RedTeamKillScoreView;
        [SerializeField] protected BaseTeamScoreView m_BlueTeamKillScoreView;
        [SerializeField] protected BaseTimeView m_PreRoundTimeView;
        [SerializeField] protected BaseTimeView m_RoundTimeView;
        [SerializeField] protected BaseVictoryView m_VictoryView;
        [SerializeField] protected BaseVictoryView m_LoseView;
        [SerializeField] protected BaseView m_InfoView;
        //[SerializeField] protected BaseTimeView m_GameTimeView;

        public BaseTeamScoreView RedTeamScoreView => m_RedTeamScoreView;
        public BaseTeamScoreView BlueTeamScoreView => m_BlueTeamScoreView;
        public BaseTimeView RoundTimeView => m_RoundTimeView;
        public BaseTimeView PreRoundTimeView => m_PreRoundTimeView;
        public BaseVictoryView VictoryView => m_VictoryView;
        public BaseVictoryView LoseView => m_LoseView;
        public BaseTeamScoreView RedTeamKillScoreView => m_RedTeamKillScoreView;
        public BaseTeamScoreView BlueTeamKillScoreView => m_BlueTeamKillScoreView;

        public BaseView InfoView => m_InfoView;

        //public BaseTimeView GameTimeView => m_GameTimeView;
    }
}