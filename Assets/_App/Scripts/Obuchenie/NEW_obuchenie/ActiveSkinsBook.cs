using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// скипт, который включает или выключает скины у книги. Висит в двух страницах. на первой - он включает, при включении первой страницы, на вводе имени, он выключает.

public class ActiveSkinsBook : MonoBehaviour
{
    public GameObject targetObject; // Объект, который будет активироваться или деактивироваться
    public bool activateOnEnable; // Булева переменная, которая определяет, активировать или деактивировать объект

    void OnEnable()
    {
        // Проверяем, нужно ли активировать или деактивировать объект
        if (targetObject != null) // Убедитесь, что ссылка на объект задана
        {
            targetObject.SetActive(
                activateOnEnable); // Активируем или деактивируем объект в зависимости от значения activateOnEnable
        }
    }

}
