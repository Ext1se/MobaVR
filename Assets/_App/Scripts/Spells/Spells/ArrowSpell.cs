using System;
using BNG;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Bow = MobaVR.Weapons.Bow.Bow;

namespace MobaVR
{
    public class ArrowSpell : Spell
    {
        [SerializeField] protected Weapons.Bow.Arrow m_Arrow;
        [SerializeField] protected GameObject m_ExplosionFx;
        [SerializeField] protected GameObject m_ExplosionPrefab;
        [SerializeField] protected TrailRenderer m_TrailRenderer;
        [SerializeField] protected Grabbable m_Grabbable;
        [SerializeField] protected PhotonTransformView m_PhotonTransformView;
        [SerializeField] protected Rigidbody m_Rigidbody;
        [SerializeField] protected Collider[] m_CollisionColliders;
        [SerializeField] protected Collider[] m_TriggerColliders;
        [SerializeField] protected float m_Damage = 10f;
        [SerializeField] protected float m_DestroyExplosion = 4f;

        protected Bow m_Bow;
        protected bool m_IsDamaged = false;
        protected bool m_IsThrown = false;

        public Action OnThrown;
        
        public Weapons.Bow.Arrow Arrow => m_Arrow;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (m_Arrow == null)
            {
                TryGetComponent(out m_Arrow);
            }
            
            if (m_Grabbable == null)
            {
                TryGetComponent(out m_Grabbable);
            }

            if (m_Rigidbody == null)
            {
                TryGetComponent(out m_Rigidbody);
            }
            
            if (m_PhotonTransformView == null)
            {
                TryGetComponent(out m_PhotonTransformView);
            }
        }


        public override void OnEnable()
        {
            base.OnEnable();

            m_ExplosionFx.SetActive(false);
            m_TrailRenderer.enabled = false;

            if (m_Arrow != null)
            {
                m_Arrow.OnAttach.AddListener(OnAttach);
            }
        }

        private void OnDestroy()
        {
            if (m_Arrow != null)
            {
                m_Arrow.OnAttach.RemoveListener(OnAttach);
            }
        }

        protected virtual void Awake()
        {
            if (!photonView.IsMine)
            {
                if (m_Grabbable != null)
                {
                    //m_Grabbable.enabled = false;
                }
            }
        }

        protected virtual void OnAttach(Bow bow)
        {
            m_Bow = bow;
            m_Bow.OnReleaseArrow.RemoveListener(OnReleaseArrow);
            m_Bow.OnReleaseArrow.AddListener(OnReleaseArrow);
        }

        public void Show(bool isShow)
        {
            //photonView.RPC(nameof(RpcShow), RpcTarget.AllBuffered, isShow);
            photonView.RPC(nameof(RpcShow), RpcTarget.All, isShow);
        }

        [PunRPC]
        private void RpcShow(bool isShow)
        {
            gameObject.SetActive(isShow);
        }
        
        private void OnReleaseArrow(Weapons.Bow.Arrow arrow, Vector3 force)
        {
            if (arrow == m_Arrow)
            {
                m_Grabbable.enabled = false;
                if (m_PhotonTransformView != null)
                {
                    m_PhotonTransformView.enabled = false;
                }
                
                /*
                photonView.RPC(nameof(RpcReleaseArrow), 
                               RpcTarget.AllBuffered,
                               m_Arrow.transform.position, 
                               m_Arrow.transform.rotation, 
                               force);
                               */
                
                photonView.RPC(nameof(RpcReleaseArrow), 
                               RpcTarget.All,
                               m_Arrow.transform.position, 
                               m_Arrow.transform.rotation, 
                               force);
            }
        }

        [PunRPC]
        protected virtual void RpcReleaseArrow(Vector3 position, Quaternion rotation, Vector3 force)
        {
            if (m_Arrow != null)
            {
                if (m_PhotonTransformView != null)
                {
                    m_PhotonTransformView.enabled = false;
                }
                
                if (!photonView.IsMine)
                {
                    m_Arrow.transform.position = position;
                    m_Arrow.transform.rotation = rotation;
                    
                    foreach (Collider collisionCollider in m_CollisionColliders)
                    {
                        collisionCollider.enabled = false;
                    }
                    m_Rigidbody.isKinematic = false;
                    m_Arrow.ShootArrow(force);
                    
                    //m_Rigidbody.constraints = RigidbodyConstraints.None;
                    //m_Rigidbody.isKinematic = false;
                    //m_Rigidbody.useGravity = true;
                    //m_Rigidbody.AddForce(force, ForceMode.VelocityChange);
                }

                RpcRelease();
            }
        }

        public void Release()
        {
            //photonView.RPC(nameof(RpcRelease), RpcTarget.AllBuffered);
            photonView.RPC(nameof(RpcRelease), RpcTarget.All);
        }

        [PunRPC]
        private void RpcRelease()
        {
            if (m_IsThrown)
            {
                return;
            }

            OnThrown?.Invoke();
            
            m_Grabbable.enabled = false;
            m_TrailRenderer.enabled = true;

            m_IsThrown = true;
            //transform.parent = null;
            Invoke(nameof(DestroySpell), m_DestroyLifeTime);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!photonView.IsMine)
            {
                //return;
            }

            if (m_IsThrown)
            {
                HandleCollision(collision.transform);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.transform == transform)
            {
                return;
            }
            
            if (m_IsDamaged)
            {
                //return;
            }

            if (!m_IsThrown)
            {
                return;
            }
            
            HitData hitData = new HitData()
            {
                Amount = m_Damage,
                Player = PhotonNetwork.LocalPlayer,
                PhotonOwner = photonView,
                //PhotonOwner = Owner.photonView,
                PhotonView = photonView,
                PlayerVR = Owner.PlayerVR,
                TeamType = TeamType
            };
            
            if (other.transform.TryGetComponent(out Damageable damageable))
            {
                if (photonView.IsMine)
                {
                    damageable.Hit(hitData);
                }

                HandleCollision(other.transform);
            }

            if (other.CompareTag("Item"))
            {
                /*
                Shield shield = other.GetComponentInParent<Shield>();
                if (shield != null)
                {
                    if (shield.TeamType == m_TeamType)
                    {
                        //return;
                    }
                    
                    //TODO:Костыль, проверка стрелы
                    if (shield.photonView.IsMine)
                    {
                        return;
                    }
                    
                    if (photonView.IsMine)
                    {
                        shield.Hit(m_Damage);
                    }

                    HandleCollision(other.transform);
                }
                */

                if (other.TryGetComponent(out BigShield bigShield))
                {
                    if (bigShield.Team.TeamType != m_TeamType)
                    {
                        HandleCollision(other.transform);
                    }
                }
            }
        }

        protected virtual void HandleCollision(Transform interactable)
        {
            m_IsDamaged = true;
            //transform.parent = interactable;
            CancelInvoke(nameof(DestroySpell));
            //Invoke(nameof(RpcDestroyThrowable), m_DestroyDelay);

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

            m_TrailRenderer.transform.parent = null;
            Destroy(m_TrailRenderer.gameObject, 2f);
            
            OnDestroySpell?.Invoke();

            Destroy(m_ExplosionFx, m_DestroyExplosion);

            base.RpcDestroyThrowable();
        }

        private void Hide()
        {
            if (PhotonNetwork.IsMasterClient && photonView != null)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}