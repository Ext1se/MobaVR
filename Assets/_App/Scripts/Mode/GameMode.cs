﻿using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public abstract class GameMode<T>: MonoBehaviourPun where T : GameMode<T>
    {
        [SerializeField] protected ClassicGameSession m_GameSession;
        //[SerializeField] protected StateMachine m_StateMachine;
        [SerializeField] protected BaseStateMachine<T> m_StateMachine;

        public Team RedTeam => m_GameSession != null ? m_GameSession.RedTeam : null;
        public Team BlueTeam => m_GameSession != null ? m_GameSession.BlueTeam : null;

        public List<PlayerVR> Players
        {
            get
            {
                if (m_GameSession == null)
                {
                    return new List<PlayerVR>();
                }

                List<PlayerVR> players = new List<PlayerVR>();
                players.AddRange(m_GameSession.RedTeam.Players);
                players.AddRange(m_GameSession.BlueTeam.Players);

                return players;
            }
        }

        public void InitMode()
        {
            //m_StateMachine.SetState(m_StateMachine.InitModeState);
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

        /*
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
        */


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

        public abstract void InitStateMachine();
    }
}