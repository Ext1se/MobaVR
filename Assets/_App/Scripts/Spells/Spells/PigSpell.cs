using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class PigSpell : ThrowableSpell
    {
        [Space]
        [Header("Magic")]
        [SerializeField] private GameObject m_Trail;
        [SerializeField] private GameObject m_ProjectileFx;
        [SerializeField] private GameObject m_ExplosionFx;
        [SerializeField] private bool m_UseGravity = false;

        [SerializeField] private float m_DestroyExplosion = 4.0f;
        [SerializeField] private float m_DestroyChildren = 2.0f;
        [SerializeField] private float m_Delay = 0.1f;
        [SerializeField] private float m_KMonsterDamage = 5f;

        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private Collider m_Collider;

        private bool m_IsTransformed = false;
        
        protected override void OnValidate()
        {
            base.OnValidate();
            if (m_Rigidbody == null)
            {
                TryGetComponent(out m_Rigidbody);
            }

            if (m_Collider == null)
            {
                TryGetComponent(out m_Collider);
                m_Collider.enabled = false;
            }
        }

        protected override void OnEnable()
        {
            //base.OnEnable();
            Invoke(nameof(RpcDestroyThrowable), m_DestroyLifeTime);

            m_Collider.enabled = false;

            m_ProjectileFx.SetActive(true);
            m_ExplosionFx.SetActive(false);
        }

        protected override void HandleCollision(Transform interactable)
        {
            if (!m_IsTransformed && photonView.IsMine)
            {
                if (interactable.transform.TryGetComponent(out HitCollider hitCollider))
                {
                    hitCollider.WizardPlayer.PlayerVR.SkinCollection.SetAnimalSkin();
                }

                m_IsTransformed = true;
            }

            RpcDestroyThrowable();
        }

        protected override float CalculateDamage()
        {
            return m_DefaultDamage;
        }

        [PunRPC]
        protected override void RpcDestroyThrowable()
        {
            if (m_IsDestroyed)
            {
                return;
            }

            OnDestroySpell?.Invoke();

            m_Trail.transform.parent = null;
            Destroy(m_Trail.gameObject, m_DestroyChildren);

            m_ExplosionFx.SetActive(true);
            m_ExplosionFx.transform.parent = null;
            Destroy(m_ExplosionFx, m_DestroyExplosion);

            base.RpcDestroyThrowable();

            //Destroy(gameObject);

            /*
            if (photonView.IsMine)
            {
                gameObject.SetActive(false);
                Invoke(nameof(DelayDestroy), 4f);
            }
            else
            {
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            */

            //PhotonNetwork.Destroy(gameObject);
        }

        private IEnumerator EnableCollider()
        {
            m_Collider.enabled = false;
            yield return new WaitForSeconds(m_Delay);
            m_IsThrown = true;
            m_Collider.enabled = true;
        }

        public void Shoot(Vector3 direction)
        {
            photonView.RPC(nameof(RpcShoot), RpcTarget.All, transform.position, direction);
        }

        [PunRPC]
        private void RpcShoot(Vector3 position, Vector3 direction)
        {
            //OnThrown?.Invoke();

            m_Rigidbody.isKinematic = false;
            m_Rigidbody.useGravity = m_UseGravity;
            m_Rigidbody.position = position;
            m_Rigidbody.AddForce(direction * m_Force);

            m_ProjectileFx.SetActive(true);

            StartCoroutine(EnableCollider());
        }
    }
}