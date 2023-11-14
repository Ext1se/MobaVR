using System;
using MobaVR.ClassicModeStateMachine;
using MobaVR.ClassicModeStateMachine.PVP;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    [Serializable]
    public class BaseStateMachine : MonoBehaviourPun, IClassicModeState
    {
        [Header("States")]
        [SerializeField] protected ModeState m_InitModeState;
        [SerializeField] protected ModeState m_InactiveModeState;
        [SerializeField] protected ModeState m_StartModeState;
        [SerializeField] protected ModeState m_ReadyRoundState;
        [SerializeField] protected ModeState m_PlayRoundState;
        [SerializeField] protected ModeState m_CompleteRoundState;
        [SerializeField] protected ModeState m_CompleteModeState;

        [Header("Current State")]
        [SerializeField] protected bool m_IsInitOnAwake = true;
        [SerializeField] protected ClassicModeState m_ModeState = ClassicModeState.MODE_INACTIVE;
        [SerializeField] protected ModeState m_CurrentState;

        protected ClassicMode m_Mode;

        public event Action<ModeState> OnStateChanged;

        public ModeState CurrentState => m_CurrentState;
        public ModeState InactiveModeState => m_InactiveModeState;
        public ModeState InitModeState => m_InitModeState;
        public ModeState StartModeState => m_StartModeState;
        public ModeState ReadyRoundState => m_ReadyRoundState;
        public ModeState PlayRoundState => m_PlayRoundState;
        public ModeState CompleteRoundState => m_CompleteRoundState;
        public ModeState CompleteModeState => m_CompleteModeState;
        public ClassicModeState ModeState => m_ModeState;
        public ClassicMode Mode => m_Mode;

        /*
        public StateMachine(ClassicMode mode)
        {
            Init(mode);
        }
        */

        private void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        public void Init(GameMode mode)
        {
            m_InitModeState.Init(mode);
            m_InactiveModeState.Init(mode);
            m_StartModeState.Init(mode);
            m_ReadyRoundState.Init(mode);
            m_PlayRoundState.Init(mode);
            m_CompleteRoundState.Init(mode);
            m_CompleteModeState.Init(mode);

            if (m_IsInitOnAwake)
            {
                //RpcDeactivateMode();
                DeactivateMode();
            }
        }

        public void SetState(ModeState nextState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            m_CurrentState = nextState;
            nextState.Enter();
            OnStateChanged?.Invoke(nextState);
        }

        public void InitMode()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(RpcInitMode), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcInitMode), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcInitMode()
        {
            m_ModeState = ClassicModeState.MODE_INIT;
            SetState(m_InitModeState);
        }

        public void DeactivateMode()
        {
            //if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(RpcDeactivateMode), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcDeactivateMode), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcDeactivateMode()
        {
            m_ModeState = ClassicModeState.MODE_INACTIVE;
            SetState(m_InactiveModeState);
        }

        public void StartMode()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(RpcStartMode), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcStartMode), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcStartMode()
        {
            m_ModeState = ClassicModeState.MODE_START;
            SetState(m_StartModeState);
        }

        public void ReadyRound()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(RpcReadyRound), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcReadyRound), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcReadyRound()
        {
            m_ModeState = ClassicModeState.ROUND_READY;
            SetState(m_ReadyRoundState);
        }

        public void PlayRound()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(RpcPlayRound), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcPlayRound), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcPlayRound()
        {
            m_ModeState = ClassicModeState.ROUND_PLAY;
            SetState(m_PlayRoundState);
        }

        public void CompleteRound()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(RpcCompleteRound), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcCompleteRound), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcCompleteRound()
        {
            m_ModeState = ClassicModeState.ROUND_COMPLETE;
            SetState(m_CompleteRoundState);
        }

        public void CompleteMode()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(RpcCompleteMode), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcCompleteMode), RpcTarget.All);
            }
        }

        [PunRPC]
        protected void RpcCompleteMode()
        {
            m_ModeState = ClassicModeState.MODE_COMPLETE;
            SetState(m_CompleteModeState);
        }
    }
}