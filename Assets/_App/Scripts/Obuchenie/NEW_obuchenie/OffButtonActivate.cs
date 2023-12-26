using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Подключение пространства имен для работы с UI
public class OffButtonActivate : MonoBehaviour
{
    private Button button; // Переменная для хранения ссылки на компонент Button

    void Awake()
    {
        // Получаем компонент Button, прикрепленный к этому же объекту
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        // Включаем кнопку при активации объекта
        if (button != null)
        {
            button.interactable = true;
        }
    }

    // Публичный метод для отключения кнопки
    public void DisableButton()
    {
        if (button != null)
        {
            button.interactable = false;
        }
    }
}
