using System.Collections.Generic;
using BNG;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class HammerSpell : ThrowableSpell
    {
        [Space]
        [Header("Magic")]
        [SerializeField] private GameObject m_HammerMesh;

        [SerializeField] private GameObject m_ProjectileFx;
        [SerializeField] private GameObject m_ExplosionFx;

        [Header("Destroy Time")]
        [SerializeField] private float m_DestroyExplosion = 4.0f;
        [SerializeField] private float m_DestroyChildren = 2.0f;

        [Header("Components")]
        [SerializeField] private Grabbable m_Grabbable;
        [SerializeField] private List<Collider> m_CollisionColliders;
        [SerializeField] private List<Collider> m_TriggerColliders;

        private Grabber m_Grabber;

        public Grabbable Grabbable => m_Grabbable;
        public Throwable Throwable => m_Throwable;

        protected override void OnEnable()
        {
            //base.OnEnable();
            //Invoke(nameof(RpcDestroy), m_DestroyLifeTime); //TODO

            m_HammerMesh.SetActive(false);
            m_ProjectileFx.SetActive(false);
            m_ExplosionFx.SetActive(false);

            if (m_Throwable != null)
            {
                m_Throwable.OnThrown.AddListener(OnThrown);
                m_Throwable.OnRedirected.AddListener(OnRedirected);
                m_Throwable.OnValidated.AddListener(OnValidated);
                m_Throwable.OnGrabbed.AddListener(OnGrabbed);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (m_Throwable != null)
            {
                m_Throwable.OnThrown.RemoveListener(OnThrown);
                m_Throwable.OnRedirected.RemoveListener(OnRedirected);
                m_Throwable.OnValidated.RemoveListener(OnValidated);
            }
        }
        
        protected override void HandleCollision(Transform interactable)
        {
            RpcDestroyThrowable();
        }

        [PunRPC]
        protected override void RpcDestroyThrowable()
        {
            if (m_IsDestroyed)
            {
                return;
            }

            m_ExplosionFx.SetActive(true);
            m_ExplosionFx.transform.parent = null;

            OnDestroySpell?.Invoke();

            Destroy(m_ExplosionFx, m_DestroyExplosion);
            
            base.RpcDestroyThrowable();
            /*
            if (photonView.IsMine)
            {
                gameObject.SetActive(false);
                Invoke(nameof(DelayDestroy), 4f);
                //PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
            */
        }

        public override void Init(WizardPlayer wizardPlayer, TeamType teamType)
        {
            base.Init(wizardPlayer, teamType);

            if (m_Throwable != null)
            {
                m_Throwable.InitPhysics(wizardPlayer);
            } 
        }

        protected override float CalculateDamage()
        {
            return m_DefaultDamage;
        }

        private void ShowWeapon()
        {
            m_HammerMesh.SetActive(true);
            photonView.RPC(nameof(RpcShowWeapon), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void RpcShowWeapon()
        {
            m_HammerMesh.SetActive(true);
        }

        private void OnGrabbed(Grabber grabber)
        {
            m_Grabber = grabber;
            //Invoke(nameof(ShowWeapon), 0.2f);
            ShowWeapon();

            foreach (Collider collisionCollider in m_CollisionColliders)
            {
                //collisionCollider.enabled = false;
            }
        }

        private void OnThrown()
        {
            if (m_IsThrown)
            {
                return;
            }
            
            m_IsThrown = true;

            foreach (Collider collisionCollider in m_CollisionColliders)
            {
                //collisionCollider.enabled = false;
            }

            m_ProjectileFx.SetActive(true);
            Invoke(nameof(DestroySpell), m_DestroyLifeTime);
        }

        private void OnValidated(bool isGoodThrow)
        {
            if (!isGoodThrow)
            {
                //photonView.RPC(nameof(RpcDestroyThrowable), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcDestroyThrowable), RpcTarget.All);
                StopAllCoroutines();
            }
        }

        private void OnRedirected(Vector3 arg0)
        {
            m_IsThrown = true;
            m_ProjectileFx.SetActive(true);
        }
    }
}