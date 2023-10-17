using System;
using UnityEngine;

namespace MobaVR
{
    public class Menu : MonoBehaviour
    {
        private LanguageManager m_LanguageManager;
        
        private void Awake()
        {
            m_LanguageManager = FindObjectOfType<LanguageManager>();
        }
        
        public void ChangeLanguage(LanguageManager.Language newLanguage)
        {
            if (m_LanguageManager == null)
            {
                return;
            }

            m_LanguageManager.ChangeLanguage(newLanguage);
        }
        
        public void SetRusLanguage()
        {
            ChangeLanguage(LanguageManager.Language.Rus);
        }
    
        public void SetEnglishLanguage()
        {
            ChangeLanguage(LanguageManager.Language.Eng);
        }
    
        public void SetChinaLanguage()
        {
            ChangeLanguage(LanguageManager.Language.Chn);
        }
    }
}