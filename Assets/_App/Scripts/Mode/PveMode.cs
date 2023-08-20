﻿using MobaVR.Content;
using UnityEngine;

namespace MobaVR
{
    public class PveMode : GameMode
    {
        [SerializeField] private PveModeContent m_Content;

        public PveModeContent Content => m_Content;

        private void Awake()
        {
            InitStateMachine();
        }

        public override void InitStateMachine()
        {
            m_StateMachine.Init(this);
        }
    }
}