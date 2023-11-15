using System;
using System.Collections;
using BNG;
using MobaVR.Base;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace MobaVR
{
    //public class Throwable : MonoBehaviourPun, IThrowable
    public class Throwable : GrabbableEvents, IThrowable
    {
        [Space]
        [Header("Validate throw")]
        [SerializeField] private bool m_IsThrowOnReleased = true;
        [SerializeField] private bool m_UseValidateThrow = true;
        [SerializeField] private float m_MinVelocity = 1f;
        [SerializeField] private PhysicsHandler m_PhysicsHandler;

        [Space]
        [Header("Forces")]
        [SerializeField] private float m_PrimaryForce = 4000f;
        [SerializeField] private float m_SecondaryForce = 4000f;

        [Header("Components")]
        [SerializeField] private PhotonView m_PhotonView;
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private Grabbable m_Grabbable;

        private Grabber m_Grabber;
        private bool m_IsThrown = false;
        private bool m_IsFirstThrown = true;

        #region Events

        public UnityEvent<bool> OnValidated;
        public UnityEvent OnThrown;
        public UnityEvent<Vector3> OnRedirected;
        public UnityEvent<Grabber> OnGrabbed;
        public UnityEvent OnReleased;
        public UnityEvent<Vector3, Vector3> OnAppliedVelocity;
        public UnityEvent OnDestroyed;
        //public UnityEvent<Grabber> OnReleased;

        #endregion

        #region Properties

        public bool IsThrowOnReleased => m_IsThrowOnReleased;
        public bool IsThrown => m_IsThrown;
        public bool IsGrabbable => m_Grabbable != null && m_Grabbable.IsGrabbable();
        public Grabbable Grabbable => m_Grabbable;
        public Grabber Grabber => m_Grabber;
        public PhysicsHandler PhysicsHandler => m_PhysicsHandler;

        #endregion

        #region MonoBehaviour

        private void OnValidate()
        {
            if (m_Rigidbody == null)
            {
                TryGetComponent(out m_Rigidbody);
            }

            if (m_Grabbable == null)
            {
                TryGetComponent(out m_Grabbable);
            }

            if (m_PhysicsHandler == null)
            {
                TryGetComponent(out m_PhysicsHandler);
            }

            if (m_PhotonView == null)
            {
                TryGetComponent(out m_PhotonView);
            }
        }

        private void OnDisable()
        {
            OnDestroyed?.Invoke();
        }

        private void Start()
        {
            if (m_PhotonView.IsMine)
            {
                m_Grabbable.enabled = true;
                m_Grabbable.CanBeDropped = true;
            }
        }

        #endregion

        #region Physics

        public void InitPhysics(WizardPlayer wizardPlayer)
        {
            if (m_PhysicsHandler == null)
            {
                return;
            }

            float force = m_Grabbable.ThrowForceMultiplier;
            bool useAim = false;
            GravityType gravityType = m_PhysicsHandler.GravityType;

            if (wizardPlayer.CanOverrideThrowableSettings)
            {
                force = wizardPlayer.ThrowForce;
                useAim = wizardPlayer.UseAim;
                gravityType = wizardPlayer.GravityFireballType;
            }

            /*
            m_PhotonView.RPC(nameof(RpcSetPhysics),
                             RpcTarget.AllBuffered,
                             gravityType,
                             force,
                             useAim
                             */
                             
            m_PhotonView.RPC(nameof(RpcSetPhysics),
                             RpcTarget.All,
                             gravityType,
                             force,
                             useAim
            );
        }

        public void Init(WizardPlayer wizardPlayer, TeamType teamType)
        {
            m_PhotonView.RPC(nameof(RpcSetPhysics),
                             //RpcTarget.AllBuffered,
                             RpcTarget.All,
                             wizardPlayer.GravityFireballType,
                             wizardPlayer.ThrowForce,
                             wizardPlayer.UseAim);
        }

        [PunRPC]
        private void RpcSwitchGravity(GravityType gravityType)
        {
            if (m_PhysicsHandler != null)
            {
                m_PhysicsHandler.GravityType = gravityType;
            }
        }

        [PunRPC]
        private void RpcSetPhysics(GravityType gravityType, float force, bool useAim)
        {
            if (m_PhysicsHandler != null)
            {
                m_PhysicsHandler.InitPhysics(gravityType, force, useAim);
            }
        }

        #endregion

        #region Events

        public override void OnGrab(Grabber grabber)
        {
            base.OnGrab(grabber);
            m_Grabber = grabber;
            OnGrabbed?.Invoke(grabber);
        }

        public override void OnRelease()
        {
            base.OnRelease();

            m_Grabber = null;
            bool isGoodThrow = true;
            if (m_UseValidateThrow)
            {
                isGoodThrow = m_Rigidbody.velocity.magnitude > m_MinVelocity;
            }
            
            //TODO:
            m_Grabbable.enabled = false;

            OnValidated?.Invoke(isGoodThrow);
            OnReleased?.Invoke();

            if (isGoodThrow && m_IsThrowOnReleased)
            {
                ThrowByVelocity(m_Rigidbody.velocity, m_Rigidbody.rotation, m_Rigidbody.angularVelocity);
            }
        }

        public override void OnApplyVelocity(Vector3 velocity, Vector3 angularVelocity)
        {
            base.OnApplyVelocity(velocity, angularVelocity);
            OnAppliedVelocity?.Invoke(velocity, angularVelocity);
        }

        #endregion

        #region Throw

        public void Drop()
        {
            if (m_Grabbable != null)
            {
                m_Grabbable.DropItem(true, true);
            }
        }

        public void Throw()
        {
            //m_PhotonView.RPC(nameof(RpcThrow), RpcTarget.AllBuffered);
            m_PhotonView.RPC(nameof(RpcThrow), RpcTarget.All);
        }

        public void ThrowByVelocity(Vector3 velocity, Quaternion quaternion, Vector3 angularVelocity)
        {
            m_PhotonView.RPC(nameof(RpcThrowByVelocity),
                             //RpcTarget.AllBuffered,
                             RpcTarget.All,
                             transform.position,
                             transform.rotation,
                             velocity,
                             angularVelocity);
        }

        public void ThrowByDirection(Vector3 direction)
        {
            //m_PhotonView.RPC(nameof(RpcThrowByDirection), RpcTarget.AllBuffered, direction);
            m_PhotonView.RPC(nameof(RpcThrowByDirection), RpcTarget.All, direction);
        }

        private void InitThrow()
        {
            m_Grabbable.enabled = false;

            m_Rigidbody.isKinematic = false;
            m_Rigidbody.WakeUp();

            m_IsThrown = true;
            m_IsFirstThrown = false;
        }

        [PunRPC]
        private void RpcThrow()
        {
            InitThrow();
            OnThrown?.Invoke();
        }

        [PunRPC]
        private void RpcThrowByVelocity(Vector3 position,Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
        {
            InitThrow();

            if (!m_PhotonView.IsMine)
            {
                m_Rigidbody.position = position;
                m_Rigidbody.rotation = rotation;
                m_Rigidbody.velocity = velocity;
                m_Rigidbody.angularVelocity = angularVelocity;
            }

            OnThrown?.Invoke();
        }

        [PunRPC]
        private void RpcThrowByDirection(Vector3 direction)
        {
            if (m_Grabbable != null)
            {
                m_Grabbable.DropItem(true, true);
            }

            InitThrow();

            if (m_IsFirstThrown)
            {
                m_Rigidbody.AddForce(direction * m_PrimaryForce);
                m_IsFirstThrown = false;
            }
            else
            {
                m_Rigidbody.AddForce(direction * m_SecondaryForce);
            }

            OnThrown?.Invoke();
            OnRedirected?.Invoke(direction);
        }

        #endregion
    }
}