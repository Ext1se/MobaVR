using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobaVR
{
    public class VoiceTextView : MonoBehaviour
    {
        [SerializeField] private List<string> m_PhrasesEng = new(); // Фразы на английском
        [SerializeField] private List<string> m_PhrasesRus = new(); // Фразы на русском
        [SerializeField] private List<string> m_PhrasesChn = new(); // Фразы на китайском
        [SerializeField] private GameObject m_VoicePanel;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private float m_Delay = 5f;

        private List<string> m_CurrentPhrases = new List<string>();

        private void OnEnable()
        {
            Hide();
        }

        private void Start()
        {
            if (LanguageManager.Instance != null)
            {
                switch (LanguageManager.Instance.currentLanguage)
                {
                    case LanguageManager.Language.Eng:
                        m_CurrentPhrases = m_PhrasesEng;
                        break;
                    case LanguageManager.Language.Rus:
                        m_CurrentPhrases = m_PhrasesRus;
                        break;
                    case LanguageManager.Language.Chn:
                        m_CurrentPhrases = m_PhrasesChn;
                        break;
                    default:
                        m_CurrentPhrases = m_PhrasesRus;
                        break;
                }
            }
        }

        public void Show()
        {
            CancelInvoke(nameof(Hide));

            m_VoicePanel.SetActive(true);
            //List<string> currentPhrases = GetCurrentPhrases();
            //string phrase = currentPhrases[Random.Range(0, currentPhrases.Count)];
            
            string phrase = m_CurrentPhrases[Random.Range(0, m_CurrentPhrases.Count)];
            m_Text.text = phrase;

            Invoke(nameof(Hide), m_Delay);
        }

        public void Hide()
        {
            m_VoicePanel.SetActive(false);
        }

        private List<string> GetCurrentPhrases()
        {
            switch (LanguageManager.Instance.currentLanguage)
            {
                case LanguageManager.Language.Eng:
                    return m_PhrasesEng;
                case LanguageManager.Language.Rus:
                    return m_PhrasesRus;
                case LanguageManager.Language.Chn:
                    return m_PhrasesChn;
                default:
                    return m_PhrasesEng; // Вернуть английский как язык по умолчанию
            }
        }
    }
}