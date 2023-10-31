using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class VoiceView : MonoBehaviourPun
    {
        [SerializeField] private SpellBehaviour m_SpellBehaviour;
        [SerializeField] private VoiceTextView m_VoiceTextView;

        private void OnEnable()
        {
            if (photonView.IsMine)
            {
                //m_SpellBehaviour.OnCompleted += OnCompleted;
                m_SpellBehaviour.OnPerformed += OnPerformed;
                m_VoiceTextView.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (photonView.IsMine)
            {
                m_SpellBehaviour.OnPerformed -= OnPerformed;
                m_VoiceTextView.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (photonView.IsMine && m_SpellBehaviour.isActiveAndEnabled)
            {
                m_VoiceTextView.gameObject.SetActive(true);
            }
            else
            {
                m_VoiceTextView.gameObject.SetActive(false);
            }
        }

        private void OnPerformed()
        {
            m_VoiceTextView.Show();
        }
    }
}