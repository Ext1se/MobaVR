using UnityEngine;

namespace MobaVR
{
    public class VoiceTextView : MonoBehaviour
    {
        [SerializeField] GameObject m_VoiceText; // Объект, который нужно включить/выключить
        [SerializeField] float m_Delay = 5f; // время, через которое объект нужно отключить

        private void OnEnable()
        {
            Hide();
        }

        public void Show()
        {
            CancelInvoke(nameof(Hide));
            
            m_VoiceText.SetActive(true);
            Invoke(nameof(Hide), m_Delay);
        }

        public void Hide()
        {
            m_VoiceText.SetActive(false);
        }
    }
}