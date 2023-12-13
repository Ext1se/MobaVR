using UnityEngine;

public class ObjectGroupController : MonoBehaviour
{

//Включает указатели перед кнопкой, типа выбран сейчас вот этот пункт
    public GameObject[] objects; // Массив объектов

    public void ActivateObjectByID(int objectID)
    {
        // Проверяем допустимость ID
        if (objectID >= 0 && objectID < objects.Length)
        {
            // Перебираем все объекты и активируем нужный, остальные деактивируем
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                    objects[i].SetActive(i == objectID);
            }
        }
    }
}
