using System;
using UnityEngine;

namespace MobaVR
{
    public class Menu : MonoBehaviour
    {
        private LocalRepository m_LocalRepository;

        private DateTime m_StartDateTime;
        private DateTime m_EndDateTime;

        private void Awake()
        {
            m_LocalRepository = new LocalRepository();
        }

        #region Language

        public void ChangeLanguage(LanguageManager.Language newLanguage)
        {
            if (LanguageManager.Instance == null)
            {
                return;
            }

            LanguageManager.Instance.ChangeLanguage(newLanguage);
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

        #endregion

        #region Game Session

        public void StartSession()
        {
            m_StartDateTime = DateTime.Now;
        }

        public void CompleteSession()
        {
            m_EndDateTime = DateTime.Now;
        }

        #endregion
    }
}