using System;
using MobaVR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_ParticleSystem;
    [SerializeField] private TextMeshProUGUI m_HealthText;
    [SerializeField] private string m_Postfix = "<size=24>HP</size>";
    [SerializeField] private float m_WarningHealth = 40f;

    public Image healthImage; // Перетащите ваш Image компонент сюда в редакторе
    private float maxHealth = 100;
    public float currentHealth;

    private WizardPlayer m_WizardPlayer;

    private void OnEnable()
    {
        if (m_WizardPlayer != null)
        {
            m_WizardPlayer.OnHealthChange += OnHealthChange;
        }
    }

    private void OnDisable()
    {
        if (m_WizardPlayer != null)
        {
            m_WizardPlayer.OnHealthChange -= OnHealthChange;
        }
    }

    private void Awake()
    {
        m_WizardPlayer = GetComponentInParent<WizardPlayer>();
    }

    private void OnHealthChange(float value)
    {
        if (value < 0)
        {
            value = 0;
        }
        
        m_HealthText.text = $"{value}{m_Postfix}";
        healthImage.fillAmount = value / m_WizardPlayer.MaxHp;
        
        if (value > m_WarningHealth)
        {
            if (m_ParticleSystem.isPlaying)
            {
                m_ParticleSystem.Stop();
            }
        }
        else
        {
            if (!m_ParticleSystem.isPlaying)
            {
                m_ParticleSystem.Play();
            }
        }
    }
}