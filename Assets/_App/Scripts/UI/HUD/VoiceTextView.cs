using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class VoiceTextView : MonoBehaviour
    {
        [SerializeField] private List<string> m_Phrases = new(); // Объект, который нужно включить/выключить
        [SerializeField] GameObject m_VoicePanel; // Объект, который нужно включить/выключить
        [SerializeField] TextMeshProUGUI m_Text; // Объект, который нужно включить/выключить
        [SerializeField] float m_Delay = 5f; // время, через которое объект нужно отключить

        private void OnEnable()
        {
            Hide();
        }

        public void Show()
        {
            CancelInvoke(nameof(Hide));
            
            m_VoicePanel.SetActive(true);
            string phrase = m_Phrases[Random.Range(0, m_Phrases.Count)];
            m_Text.text = phrase;

            Invoke(nameof(Hide), m_Delay);
        }

        public void Hide()
        {
            m_VoicePanel.SetActive(false);
        }
    }
}