﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaVR
{
    public class Explosion : MonoBehaviour
    {
        [Header("Explosion Wave")]
        [SerializeField] private bool m_UseExplosionWave = false;
        [SerializeField] private LayerMask m_ExplosionLayers;
        [SerializeField] private float m_ExplosionCollisionRadius = 10f;
        [SerializeField] private float m_ExplosionForce = 400f;
        [SerializeField] private float m_ExplosionForceRadius = 4f;
        [SerializeField] private float m_ExplosionModifier = 2f;

        private Spell m_Spell;

        private void Awake()
        {
            m_Spell = GetComponent<Spell>();
        }

        private void OnEnable()
        {
            if (m_Spell != null)
            {
                m_Spell.OnDestroySpell += OnDestroySpell;
            }
        }


        private void OnDisable()
        {
            if (m_Spell != null)
            {
                m_Spell.OnDestroySpell -= OnDestroySpell;
            }
        }

        private void Explode()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position,
                                                         //m_ExplosionCollisionRadius,
                                                         m_ExplosionCollisionRadius,
                                                         m_ExplosionLayers,
                                                         QueryTriggerInteraction.Collide);

            foreach (Collider hit in colliders)
            {
                if (hit.TryGetComponent(out HitCollider hitCollider)
                    && hit.TryGetComponent(out Rigidbody hitRigidbody))
                {
                    //if (!hitRigidbody.isKinematic)
                    {
                        if (m_UseSpecifiedNames && !m_BoneNames.Contains(hitRigidbody.name))
                        {
                            continue;
                        }

                        hitRigidbody.AddExplosionForce(m_ExplosionForce,
                                                       transform.position,
                                                       m_ExplosionForceRadius,
                                                       m_ExplosionModifier);
                    }
                }
            }

            /*
            foreach (Collider enemy in colliders)
            {
                if (enemy.TryGetComponent(out IExploding hitEnemy))
                {
                    //hitEnemy.RpcHit(CalculateDamage());
                    hitEnemy.Explode(m_ExplosionForce,
                                     transform.position,
                                     m_ExplosionForceRadius,
                                     m_ExplosionModifier);
                }
            }

            Explode(interactable);
            */
        }

        [SerializeField] private float m_Delay = 0.1f;
        [SerializeField] private bool m_UseSpecifiedNames = true;
        [SerializeField] private List<string> m_BoneNames = new List<string>();

        private void OnDestroySpell()
        {
            if (m_Delay > 0)
            {
                Invoke(nameof(Explode), m_Delay);
            }
            else
            {
                Explode();
            }
        }
    }
}