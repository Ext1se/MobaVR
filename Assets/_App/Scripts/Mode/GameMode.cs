using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public abstract class GameMode : MonoBehaviourPun
    {
        [SerializeField] private GameModeType m_GameModeType = GameModeType.PVP;
        [SerializeField] protected ClassicGameSession m_GameSession;
        //[SerializeField] protected StateMachine m_StateMachine;
        [SerializeField] protected BaseStateMachine m_StateMachine;

        public GameModeType GameModeType => m_GameModeType;
        public Team RedTeam => m_GameSession != null ? m_GameSession.RedTeam : null;
        public Team BlueTeam => m_GameSession != null ? m_GameSession.BlueTeam : null;
        public BaseStateMachine StateMachine => m_StateMachine;
        public PlayerVR Player => m_GameSession.LocalPlayer;
        public List<PlayerVR> Players
        {
            get
            {
                if (m_GameSession == null)
                {
                    return new List<PlayerVR>();
                }

                return m_GameSession.Players;

                /*
                if (m_GameSession == null)
                {
                    return new List<PlayerVR>();
                }

                List<PlayerVR> players = new List<PlayerVR>();
                players.AddRange(m_GameSession.RedTeam.Players);
                players.AddRange(m_GameSession.BlueTeam.Players);

                return players;
                */
            }
        }

        protected virtual void Awake()
        {
            if (m_GameSession == null)
            {
                m_GameSession = FindObjectOfType<ClassicGameSession>();
            }

            /*
            if (m_GameSession != null)
            {
                m_GameSession.Mode = this;
            }
            */
        }

        public void InitMode()
        {
            m_StateMachine.InitMode();
        }

        public void DeactivateMode()
        {
            m_StateMachine.DeactivateMode();
        }

        public void StartMode()
        {
            m_StateMachine.StartMode();
        }

        public void ReadyRound()
        {
            m_StateMachine.ReadyRound();
        }

        public void PlayRound()
        {
            m_StateMachine.PlayRound();
        }

        public void CompleteRound()
        {
            m_StateMachine.CompleteRound();
        }

        public void CompleteMode()
        {
            m_StateMachine.CompleteMode();
        }

        public void SetStateMachine(StateMachine stateMachine)
        {
            if (m_StateMachine != null)
            {
                m_StateMachine.DeactivateMode();
            }

            m_StateMachine = stateMachine;
            //m_StateMachine.Init(this);

            InitStateMachine();
        }

        /*
        public void SetStateMachine(BaseStateMachine<T> stateMachine)
        {
            if (m_StateMachine != null)
            {
                m_StateMachine.DeactivateMode();
            }

            m_StateMachine = stateMachine;
            //m_StateMachine.Init(this);

            InitStateMachine();
        }
        */

        public abstract void InitStateMachine();
    }
}