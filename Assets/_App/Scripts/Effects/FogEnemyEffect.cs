using UnityEngine;

namespace MobaVR
{
    public class FogEnemyEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_ParticleSystem;
        [SerializeField] private float m_Duration;

        public void Show(float duration)
        {
            ParticleSystem.MainModule particleSystemMain = m_ParticleSystem.main;
            particleSystemMain.duration = duration;
            m_ParticleSystem.Play();
        }
        
        public void Show()
        {
            Show(m_Duration);
        }
    }
}