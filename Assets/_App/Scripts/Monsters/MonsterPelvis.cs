using System;
using UnityEngine;

namespace MobaVR
{
    //public class MonsterPelvis : MonoBehaviour, IHit
    public class MonsterPelvis : Damageable, IExploding
    {
        [SerializeField] private NetworkDamageable m_ParentDamageable;
        [SerializeField] private Monster m_Monster;
        [SerializeField] private Collider m_Collider;

        private void Awake()
        {
            if (m_ParentDamageable == null)
            {
                m_ParentDamageable = GetComponentInParent<NetworkDamageable>();
            }
        }

        public void SetEnabled(bool isEnable)
        {
            m_Collider.enabled = isEnable;
        }
        
        /*
        public void RpcHit(float damage)
        {
            m_Monster.RpcHit(damage);
        }
        */

        public override void Hit(HitData hitData)
        {
            if (m_ParentDamageable != null)
            {
                m_ParentDamageable.Hit(hitData);
            }
        }

        public override void Die()
        {
        }

        public override void Reborn()
        {
        }

        public void Explode(float explosionForce, Vector3 position, float radius, float modifier)
        {
            m_Monster.Explode(explosionForce, position, radius, modifier);
        }
    }
}