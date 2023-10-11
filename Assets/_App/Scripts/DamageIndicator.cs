using System;
using UnityEngine;
using UnityEngine.UI;

namespace MobaVR
{
    public class DamageIndicator : BaseDamageIndicator
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private bool m_IsOverrideColor;
        [SerializeField] private Image m_DamageImage;
        [SerializeField] private Color m_BloodColor;
        [SerializeField] private AudioClip[] m_DamageSounds;  // массив звуков
        [SerializeField] private AudioSource m_AudioSource;  // аудио источник

        private readonly int m_ShowAnimId = Animator.StringToHash("damage");
        private int m_CurrentSoundIndex = 0;  // текущий индекс звука

        private void Awake()
        {
            if (m_IsOverrideColor)
            {
                m_DamageImage.color = m_BloodColor;
            }

            m_DamageImage.enabled = false;

            if (m_AudioSource == null)
            {
                m_AudioSource = gameObject.GetComponent<AudioSource>();
                if (m_AudioSource == null)
                {
                    Debug.LogWarning("AudioSource component is missing");
                }
            }
        }

        private void ResetTriggers()
        {
            m_Animator.ResetTrigger(m_ShowAnimId);
        }

        public override void Show()
        {
            m_DamageImage.enabled = true;
            m_Animator.SetTrigger(m_ShowAnimId);

            if (m_AudioSource != null && m_DamageSounds.Length > 0)
            {
                m_AudioSource.PlayOneShot(m_DamageSounds[m_CurrentSoundIndex]);
                m_CurrentSoundIndex = (m_CurrentSoundIndex + 1) % m_DamageSounds.Length;
            }

            Invoke(nameof(ResetTriggers), 0.4f);
        }

        public override void Hide()
        {
        }
    }
}