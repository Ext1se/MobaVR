using MobaVR.Content;
using UnityEngine;

namespace MobaVR
{
    public class TavernMode : GameMode
    {
        protected override void Awake()
        {
            base.Awake();
            InitStateMachine();
        }

        public override void InitStateMachine()
        {
            m_StateMachine.Init(this);
        }
    }
}