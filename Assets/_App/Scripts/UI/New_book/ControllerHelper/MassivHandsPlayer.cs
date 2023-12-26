using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //Скрипт, который хранит все элементы рук. Если к нему обращаться из других скриптов, то можно включать и выключать нужные нам руки
//обращаться можно с помощью следующей команды ссылка на объект со скриптом .ActivateObject(2);


public class MassivHandsPlayer : MonoBehaviour
{
    public GameObject[] objects; // Массив объектов

    public void ActivateObject(int index)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                objects[i].SetActive(i == index);
        }
    }
}
