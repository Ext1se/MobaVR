using UnityEngine;

namespace MobaVR
{
    public class ClassicModeView : BaseModeView
    {
        [SerializeField] protected PvpVictoryView m_PvpVictoryView;

        public PvpVictoryView PvpVictoryView => m_PvpVictoryView;
    }
}