using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

    public class VirtualKeyboardManager : MonoBehaviour
    {
        public AudioSource audioSource; // Ссылка на компонент AudioSource
        public AudioClip soundClip; // Звуковой файл, который хранится в переменной

        public InputField InputField; // Ссылка на InputField
        public bool ButtonClicked = false;

        public GameObject KlavaEn;
        public GameObject KlavaRus;
        public GameObject KlavaChisla;

        // Метод для вызова из ваших кнопок
        public void TypeKey(string value)
        {
            if (!ButtonClicked)
            {
                switch (value)
                {
                    case "del":
                        DeleteLastCharacter();
                        break;
                    case "space":
                        InputField.text += " ";
                        break;
                    default:
                        InputField.text += value;
                        break;
                }

                ButtonClicked = true;
                TriggerHapticFeedback();
            }

        }

        // Удалить последний символ
        private void DeleteLastCharacter()
        {
            if (InputField.text.Length > 0)
            {
                InputField.text = InputField.text.Substring(0, InputField.text.Length - 1);
            }
        }

        // Воспроизвести вибрацию на контроллере или звук
        private void TriggerHapticFeedback()
        {
            if (audioSource != null && soundClip != null)
            {
                audioSource.clip = soundClip;
                audioSource.Play();
            }
        }
        
        // Выключаем нажатие
        public void ExitDown()
        {
            ButtonClicked = false;
        }
        
        public void KlavaRusRun()
        {
            if (!ButtonClicked)
            {
                KlavaEn.SetActive(false);
                KlavaRus.SetActive(true);
                KlavaChisla.SetActive(false);
                ButtonClicked = true;
            }
        }
        
        public void KlavaEnRun()
        {
            if (!ButtonClicked)
            {
                KlavaEn.SetActive(true);
                KlavaRus.SetActive(false);
                KlavaChisla.SetActive(false);
                ButtonClicked = true;
            }
        }
        
        public void KlavaChislaRun()
        {
            if (!ButtonClicked)
            {
                KlavaEn.SetActive(false);
                KlavaRus.SetActive(false);
                KlavaChisla.SetActive(true);
                ButtonClicked = true;
            }
        }
    }