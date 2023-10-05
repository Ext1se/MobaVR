using System;
using Michsky.MUIP;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace MobaVR
{
    public class ManaView : MonoBehaviourPun
    {
        //public Image healthImage; // Это для индикатора здоровья
        public SpellImage m_SpellImage; // Это для индикатора здоровья
        private float currentHealth;
        
        [SerializeField] private SpellBehaviour m_SpellBehaviour;
        [SerializeField] private float m_CooldownTime = 30f;
        [SerializeField] private ProgressBar m_ProgressBar;
        
        private float m_CurrentTime = 0f;
        private bool m_IsCompleted = true;
        
        private void Start()
        {
            //TODO:
            /*
            if (photonView.IsMine && m_SpellBehaviour.isActiveAndEnabled)
            {
                m_ProgressBar.gameObject.SetActive(true);
                m_ProgressBar.currentPercent = 0;
                m_ProgressBar.UpdateUI();
            }
            else
            {
                m_ProgressBar.gameObject.SetActive(false);
            }
            */
            
            if (photonView.IsMine && m_SpellBehaviour.isActiveAndEnabled)
            {
                m_SpellImage.gameObject.SetActive(true);
            }
            else
            {
                m_SpellImage.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            //m_SpellBehaviour.OnCompleted += OnCompleted;
            m_SpellBehaviour.OnPerformed += OnPerformed;
            m_CooldownTime = m_SpellBehaviour.CooldownTime;

            if (photonView.IsMine)
            {
                //TODO: progressBar spell
                //m_ProgressBar.gameObject.SetActive(true);

                if (m_SpellImage != null)
                {
                    m_SpellImage.gameObject.SetActive(true);
                }
            }
        }

        private void OnDisable()
        {
            m_SpellBehaviour.OnCompleted -= OnCompleted;

            if (photonView.IsMine)
            {
                //TODO: progressBar spell
                //m_ProgressBar.gameObject.SetActive(false);
                
                if (m_SpellImage != null)
                {
                    m_SpellImage.gameObject.SetActive(false);
                }
            }
        }

        private void OnPerformed()
        {
            m_CurrentTime = 0f;
            if (m_ProgressBar != null)
            {
                m_ProgressBar.currentPercent = 0f;
                m_ProgressBar.UpdateUI();
            }

            UpdateHealthImage();//обновляем картинку
            
            
            m_IsCompleted = false;
        }
        
        private void OnCompleted()
        {
            /*
            if (!m_SpellBehaviour.IsPerformed())
            {
                return;
            }
            */
            
            m_CurrentTime = 0f;
            if (m_ProgressBar != null)
            {
                m_ProgressBar.currentPercent = 0f;
                m_ProgressBar.UpdateUI();
            }

            UpdateHealthImage();//обновляем картинку
            m_IsCompleted = true;
        }

        
        
        private void UpdateHealthImage()
        {
            if (m_SpellImage != null && m_ProgressBar != null) 
            {
                m_SpellImage.OnImage.fillAmount = m_ProgressBar.currentPercent / 100.0f;
            }
            
            /*
            if (healthImage != null) 
            {
                healthImage.fillAmount = m_ProgressBar.currentPercent / 100.0f;
            }
            */
        }

        
        
        
        
        
        
        private void Update()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (m_SpellBehaviour.IsAvailable || !m_SpellBehaviour.UseCooldown)
            {
                if (m_ProgressBar != null)
                {
                    m_ProgressBar.currentPercent = 100f;
                    m_ProgressBar.UpdateUI();
                }

                UpdateHealthImage();//обновляем картинку
                
            }
            else
            {
                m_CurrentTime = m_SpellBehaviour.CurrentTime;
                if (m_ProgressBar != null)
                {
                    m_ProgressBar.currentPercent = m_CurrentTime / m_CooldownTime * 100f;
                    m_ProgressBar.UpdateUI();

                }

                UpdateHealthImage();//обновляем картинку
            }
            
            /*
            if ((m_SpellBehaviour.IsAvailable && !m_SpellBehaviour.IsPerformed()) || !m_SpellBehaviour.UseCooldown)
            {
                m_ProgressBar.currentPercent = 100;
                m_ProgressBar.UpdateUI();
            }
            else
            {
                if (m_IsCompleted)
                {
                    m_CurrentTime += Time.deltaTime;
                    m_ProgressBar.currentPercent = m_CurrentTime / m_CooldownTime * 100f;
                    m_ProgressBar.UpdateUI();
                }
            }
            */
        }
    }
}