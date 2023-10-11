using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class Text_krika : MonoBehaviour
    {
        public GameObject objectToToggle; // Объект, который нужно включить/выключить
        private bool isObjectActive; // Переменная для отслеживания состояния объекта
        private float timer; // Таймер для отсчета времени

        public ControllerBinding RunText = ControllerBinding.YButtonDown;


        void Update()
        {
            // Проверяем, была ли нажата кнопка E
            if (InputBridge.Instance.GetControllerBindingValue(RunText))
            {
                // Включаем объект и обновляем переменные
                objectToToggle.SetActive(true);
                isObjectActive = true;
                timer = 5f; // Устанавливаем таймер на 5 секунд
            }

            // Если объект активен и таймер еще не истек
            if (isObjectActive && timer > 0)
            {
                timer -= Time.deltaTime; // Уменьшаем таймер

                // Если таймер истек
                if (timer <= 0)
                {
                    // Выключаем объект и обновляем переменную
                    objectToToggle.SetActive(false);
                    isObjectActive = false;
                }
            }
        }
    }
}