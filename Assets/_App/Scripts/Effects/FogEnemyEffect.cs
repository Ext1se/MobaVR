using UnityEngine;

namespace MobaVR
{
    public class StunEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_ParticleSystem;
        [SerializeField] private Color m_TeamColor = Color.black;
        [SerializeField] private Color m_EnemyColor = Color.black;
        [SerializeField] private float m_Duration;

        public  void Show()
        {
            Color fogColor = gameSession.LocalPlayer.TeamType == teamType ? m_TeamColor : m_EnemyColor;
            ParticleSystem.MainModule particleSystemMain = m_ParticleSystem.main;
            particleSystemMain.startColor = fogColor;
            particleSystemMain.duration = m_Duration;
            
            m_ParticleSystem.Play();

            if (m_Sprite != null)
            {
                m_Sprite.color = Color.clear;
                m_Sprite.DOColor(fogColor, m_SpriteDuration);
                Invoke(nameof(ClearSprite), m_Duration - m_SpriteDuration);
            }
            
            Invoke(nameof(RpcDestroyThrowable), m_DestroyLifeTime);
        }

        private void ClearSprite()
        {
            m_Sprite.DOColor(Color.clear, m_SpriteDuration);
        }
    }
}