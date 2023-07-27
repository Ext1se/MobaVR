using System;
using BNG;
using Photon.Pun;
using UnityEngine;
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

        private void Awake()
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
            m_Bow.OnReleaseArrow.AddListener(OnReleaseArrow);
        }

        public void Init(WizardPlayer wizardPlayer, TeamType teamType)
        {
            m_Owner = wizardPlayer;
            m_TeamType = teamType;
            
            photonView.RPC(nameof(RpcInit), RpcTarget.All, teamType);
        }

        [PunRPC]
        public virtual void RpcInit(TeamType teamType)
        {
            if (m_TeamItem != null)
            {
                m_TeamItem.SetTeam(teamType);
            }
        }

        public void Show(bool isShow)
        {
            photonView.RPC(nameof(RpcShow), RpcTarget.AllBuffered, isShow);
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
                
                photonView.RPC(nameof(RpcReleaseArrow), 
                               RpcTarget.AllBuffered,
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
            photonView.RPC(nameof(RpcRelease), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void RpcRelease()
        {
            if (m_IsThrown)
            {
                return;
            }

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

            if (other.CompareTag("RemotePlayer") && other.transform.TryGetComponent(out WizardPlayer wizardPlayer))
            {
                if (wizardPlayer == Owner)
                {
                    return;
                }
                
                if (wizardPlayer.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                {
                    return;
                }

                if (photonView.IsMine)
                {
                    wizardPlayer.Hit(m_Damage);
                }

                HandleCollision(other.transform);
            }

            if (other.CompareTag("LifeCollider") && other.transform.TryGetComponent(out HitCollider damagePlayer))
            {
                if (damagePlayer.WizardPlayer == Owner)
                {
                    return;
                }
                
                if (damagePlayer.WizardPlayer.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                {
                    return;
                }

                if (photonView.IsMine)
                {
                    damagePlayer.WizardPlayer.Hit(m_Damage);
                }

                HandleCollision(other.transform);
            }

            if (other.CompareTag("Item"))
            {
                Shield shield = other.GetComponentInParent<Shield>();
                if (shield != null)
                {
                    if (photonView.IsMine)
                    {
                        shield.Hit(m_Damage);
                    }

                    HandleCollision(other.transform);
                }

                if (other.TryGetComponent(out BigShield bigShield))
                {
                    if (bigShield.Team.TeamType != m_TeamType)
                    {
                        HandleCollision(other.transform);
                    }
                }
            }
            
            if (other.CompareTag("Enemy") && other.TryGetComponent(out IHit hitEnemy))
            {
                if (photonView.IsMine)
                {
                    hitEnemy.RpcHit(m_Damage);
                }

                HandleCollision(other.transform);
                return;
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