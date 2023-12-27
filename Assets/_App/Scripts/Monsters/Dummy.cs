using System;
using DamageNumbersPro;
using UnityEngine;

namespace MobaVR
{
    public class Dummy : MonoBehaviour
    {
        [SerializeField] private DamageNumberView m_DamageNumber;

        [SerializeField] private ParticleSystem m_HealVFX;
        [SerializeField] private ParticleSystem m_FireVFX;
        [SerializeField] private ParticleSystem m_FogVFX;

        private void Awake()
        {
            HideAllEffects();
        }

        private void HideAllEffects()
        {
            m_HealVFX.Stop();
            m_FireVFX.Stop();
            m_FogVFX.Stop();
        }

        public void ShowFog()
        {
            HideAllEffects();
            m_FogVFX.Play();
        }

        public void ShowFire()
        {
            HideAllEffects();
            m_FireVFX.Play();
        }

        public void ShowHeal()
        {
            HideAllEffects();
            m_HealVFX.Play();
            
            m_DamageNumber.SpawnNumber(m_DamageNumber.transform.position,
                10f,
                Monster.MonsterDamageType.HEAL);
        }

        public void ShowDamage(HitData hitData)
        {
            if (hitData.Action != HitActionType.Damage)
            {
                return;
            }

            m_DamageNumber.SpawnNumber(m_DamageNumber.transform.position,
                                       hitData.Amount,
                                       Monster.MonsterDamageType.HP);
        }

        public void Hit(HitData hitData)
        {
            ShowDamage(hitData);
        }
    }
}