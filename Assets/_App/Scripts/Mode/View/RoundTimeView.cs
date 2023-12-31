﻿using System;
using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class RoundTimeView : BaseTimeView
    {
        [SerializeField] private TextMeshPro m_TimeText;

       public GameObject Obuchenie; //баннер с обучением
        
        public override void Show()
        {
            gameObject.SetActive(true);
            m_TimeText.enabled = true;
            if (Obuchenie != null)
            {
                Obuchenie.SetActive(true);
            }
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
            m_TimeText.enabled = false;
            if (Obuchenie != null)
            {
                Obuchenie.SetActive(false);
            }
            
        }

        public override void UpdateTime(float time)
        {
            if (time < 0)
            {
                time = 0f;
            }

            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            string timeString = timeSpan .ToString(@"mm\:ss");
            m_TimeText.text = $"{timeString}";
            //m_TimeText.text = $"{time:F1}";
        }
    }
}