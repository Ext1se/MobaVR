﻿using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

namespace MobaVR.Weapons.Bow
{
    public class MagicArrow : ArrowSpell
    {
        [SerializeField] private GameObject m_MagicMesh;
        [SerializeField] private float m_MaxScale = 5f;
        [SerializeField] private float m_DurationScale = 1f;

        protected override void OnAttach(Bow bow)
        {
            base.OnAttach(bow);
            m_MagicMesh.gameObject.SetActive(true);
            foreach (Collider collisionCollider in m_CollisionColliders)
            {
                collisionCollider.enabled = false;
            }
        }

        [PunRPC]
        protected override void RpcReleaseArrow(Vector3 position, Quaternion rotation, Vector3 force)
        {
            foreach (Collider collisionCollider in m_CollisionColliders)
            {
                collisionCollider.enabled = false;
            }

            if (m_Arrow != null)
            {
                Vector3 scale = transform.lossyScale * m_MaxScale;
                transform.DOScale(scale, m_DurationScale);
            }

            Invoke(nameof(RpcDestroyThrowable), m_DestroyLifeTime);

            base.RpcReleaseArrow(position, rotation, force);
        }

        protected override void HandleCollision(Transform interactable)
        {
            //base.HandleCollision(interactable);
            if (interactable.TryGetComponent(out Collider interactableCollider))
            {
                /*
                GameObject explosion = Instantiate(m_ExplosionPrefab, 
                                                   interactableCollider.ClosestPoint(transform.position), 
                                                   transform.rotation);
                Destroy(explosion, m_DestroyExplosion);
                */
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (m_IsThrown)
            {
                HandleCollision(other.transform);
            }
        }
    }
}