﻿using UnityEngine;

namespace MobaVR.Content
{
    public class ClassicModeContent : MonoBehaviour
    {
        [SerializeField] private BaseModeView m_ModeView;
        [SerializeField] private BaseEnvironmentMode m_Environment;
        [SerializeField] private ZoneManager m_ZoneManager;

        public BaseModeView ModeView => m_ModeView;
        public BaseEnvironmentMode Environment => m_Environment;
        public ZoneManager ZoneManager => m_ZoneManager;
    }
}