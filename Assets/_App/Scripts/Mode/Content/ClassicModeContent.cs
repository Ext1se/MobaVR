using MobaVR.Sound;
using UnityEngine;

namespace MobaVR.Content
{
    public class ClassicModeContent : MonoBehaviour
    {
        [SerializeField] private ClassicModeView m_ModeView;
        //[SerializeField] private BaseModeView m_ModeView;
        [SerializeField] private BaseEnvironmentMode m_Environment;
        [SerializeField] private ZoneManager m_ZoneManager;
        [SerializeField] private KillZoneManager m_KillZoneManager;
        [SerializeField] private PvpModeSound m_PvpModeSound;

        //public BaseModeView ModeView => m_ModeView;
        public ClassicModeView ModeView => m_ModeView;
        public BaseEnvironmentMode Environment => m_Environment;
        public ZoneManager ZoneManager => m_ZoneManager;
        public KillZoneManager KillZoneManager => m_KillZoneManager;
        public PvpModeSound PvpModeSound => m_PvpModeSound;
    }
}