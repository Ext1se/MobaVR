using UnityEngine;

namespace MobaVR
{
    public class PveModeView : MonoBehaviour
    {
        [SerializeField] protected BaseView m_InfoView;
        [SerializeField] protected BaseTimeView m_RoundTimeView;
        [SerializeField] protected BaseView m_VictoryView;
        [SerializeField] protected BaseView m_LoseView;

        public BaseTimeView RoundTimeView => m_RoundTimeView;
        public BaseView VictoryView => m_VictoryView;
        public BaseView LoseView => m_LoseView;
        public BaseView InfoView => m_InfoView;
    }
}