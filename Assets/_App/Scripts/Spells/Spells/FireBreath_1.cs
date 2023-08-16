using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class FireBreath_1 : Spell
    {
        [SerializeField] private float m_Damage = 2f;
        [SerializeField] private ParticleSystem m_ParticleSystem;
        [SerializeField] private Collider m_Collider;

        [Header("Balls")]
        [SerializeField] private bool m_UseParticleTriggers;
        [SerializeField] private ParticleTrigger m_ParticleTriggerPrefab;
        [SerializeField] private float m_Speed;

        private void Awake()
        {
            RpcShow(false);
            ParticleSystem.TriggerModule particleSystemTrigger = m_ParticleSystem.trigger;
            particleSystemTrigger.enabled = photonView.IsMine;
        }

        public void Show(bool isShow)
        {
            photonView.RPC(nameof(RpcShow), RpcTarget.All, isShow);

            if (photonView.IsMine)
            {
                if (m_UseParticleTriggers)
                {
                    SpawnParticleTriggers(isShow);
                }
                else
                {
                    InitColliders();
                }
            }
        }

        private void InitColliders()
        {
            WizardPlayer[] wizardPlayers = FindObjectsOfType<WizardPlayer>();
            List<WizardPlayer> filterPlayer = new();
            foreach (WizardPlayer wizardPlayer in wizardPlayers)
            {
            }
        }

        private void SpawnParticleTriggers(bool isShow)
        {
            if (isShow)
            {
                InvokeRepeating(nameof(CreateSphere), 0, 0.2f);
            }
            else
            {
                CancelInvoke(nameof(CreateSphere));
            }
        }

        private void CreateSphere()
        {
            ParticleTrigger particleTrigger = Instantiate(m_ParticleTriggerPrefab,
                                                          transform.position,
                                                          transform.rotation);

            particleTrigger.OnParticleTriggerEnter += OnParticleTriggerEnter;
            particleTrigger.OnParticleCollisionEnter += OnParticleCollisionEnter;
            particleTrigger.OnDestroyTrigger += () =>
            {
                particleTrigger.OnParticleTriggerEnter -= OnParticleTriggerEnter;
                particleTrigger.OnParticleCollisionEnter -= OnParticleCollisionEnter;
            };

            particleTrigger.Init(m_ParticleSystem, transform);
            particleTrigger.Shoot();
        }

        private void OnParticleCollisionEnter(Collision collision)
        {
        }

        private void OnParticleTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (other.transform == transform)
            {
                return;
            }

            HitData hitData = new HitData()
            {
                Amount = m_Damage,
                Player = PhotonNetwork.LocalPlayer,
                PhotonOwner = photonView,
                PlayerVR = Owner.PlayerVR,
                TeamType = TeamType
            };

            /*
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
                    //wizardPlayer.Hit(m_Damage);
                    wizardPlayer.Hit(hitData);
                }
            }
            */

            if (other.transform.TryGetComponent(out Damageable damageable))
            {
                /*
                if (damageable.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                {
                    return;
                }
                */

                if (photonView.IsMine)
                {
                    //wizardPlayer.Hit(this, CalculateDamage());
                    damageable.Hit(hitData);
                }
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
                    //TODO: DAMAGE
                    //damagePlayer.WizardPlayer.Hit(hitData);
                    //damagePlayer.WizardPlayer.Hit(m_Damage);
                }
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
                }

                if (other.TryGetComponent(out BigShield bigShield))
                {
                    if (bigShield.Team.TeamType != m_TeamType)
                    {
                        //HandleCollision(other.transform);
                    }
                }
            }

            if (other.CompareTag("Enemy") && other.TryGetComponent(out IHit hitEnemy))
            {
                if (photonView.IsMine)
                {
                    hitEnemy.RpcHit(m_Damage);
                }
            }
        }

        [PunRPC]
        public virtual void RpcShow(bool isShow)
        {
            //if (photonView.IsMine)
            {
                if (isShow)
                {
                    //m_Collider.enabled = true;
                    m_ParticleSystem.Play();
                }
                else
                {
                    //m_Collider.enabled = false;
                    m_ParticleSystem.Stop();
                }
            }
        }

        /*
        private void OnParticleTrigger()
        {
            Debug.Log($"{name}: OnParticleTrigger");
            List<ParticleSystem.Particle> enter = new();
            int numEnter = m_ParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
            Debug.Log($"{name}: OnParticleTrigger: {numEnter}");
        }
        */

        private int colCollision = 0;

        private void OnParticleCollision(GameObject other)
        {
            colCollision++;
            Debug.Log("PARTICLE OnParticleCollision " + other.name + colCollision);
        }

        /*
        private void OnTriggerStay(Collider other)
        {
            OnTriggerStay(other);
        }
        */
    }
}