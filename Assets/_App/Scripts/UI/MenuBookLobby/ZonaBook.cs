using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobaVR;
using BNG;

//скрипт, который включает книгу, переносит в точку все скины игрока, включает туман


public class ZonaBook : MonoBehaviour
{
    public GameObject objectToActivate; // GameObject, который будет активироваться
    public GameObject Book_Menu; // GameObject меню книги, который будет активироваться или деактивироваться
    public GameObject Book; // GameObject книги для деактивации или активации, когда меню активно
    public Transform destinationParent; // Родитель, куда будут перемещаться объекты
    private int playerCount = 0; // Счетчик игроков, находящихся в зоне
    public GameObject meshRenderer; // Объект для деактивации

    public string targetID; // ID цели. Если совпадает с ID объекта, тот будет активирован
    public GameObject[] childObjects;

    // Аудио
    public AudioClip soundOnEnter; // Звук при входе в триггер
    public AudioClip soundOnExit; // Звук при выходе из триггера

    private AudioSource audioSource;

    // Туман
    public float fogDensityTarget = 1f; // Целевая плотность тумана
    public float transitionDuration = 1f; // Длительность перехода тумана

    private bool isFogCompleted = true; // Завершен ли переход тумана
    private bool isInTrigger = false; // Находится ли в триггере
    private float initialFogDensity; // Начальная плотность тумана
    private float transitionTimer = 0f; // Таймер для перехода тумана
    
    public GameObject[] BookPapper;//страницы книги, когда выходим из книги, чтобы страницы перематывались в исходные.

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Получение компонента AudioSource
        initialFogDensity = RenderSettings.fogDensity; // Сохранение начальной плотности тумана
    }

    private void Update()
    {
        if (isFogCompleted)
        {
            return; // Если переход тумана завершен, то не выполнять дальнейшие действия
        }

        if (isInTrigger)
        {
            // Если в триггере, изменить плотность тумана в соответствии с заданным временем
            if (transitionTimer < transitionDuration)
            {
                transitionTimer += Time.deltaTime;
                float t = transitionTimer / transitionDuration;
                float newDensity = Mathf.Lerp(initialFogDensity, fogDensityTarget, t);
                RenderSettings.fogDensity = newDensity;
            }
            else
            {
                RenderSettings.fogDensity = fogDensityTarget; // Установить целевую плотность тумана
                isFogCompleted = true; // Отметить переход тумана как завершенный
            }
        }
        else
        {
            // Если не в триггере, изменить плотность тумана обратно
            if (transitionTimer > 0f)
            {
                transitionTimer -= Time.deltaTime;
                float t = transitionTimer / transitionDuration;
                float newDensity = Mathf.Lerp(initialFogDensity, fogDensityTarget, t);
                RenderSettings.fogDensity = newDensity;
            }
            else
            {
                RenderSettings.fogDensity = initialFogDensity; // Восстановить начальную плотность тумана
                isFogCompleted = true; // Отметить переход тумана как завершенный
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TriggerLocalPlayer"))
        {
            Debug.Log("Зашёл в триггер");

            playerCount++;
            if (playerCount == 1)
            {
                if (soundOnEnter != null)
                {
                    audioSource.PlayOneShot(soundOnEnter); // Воспроизвести звук при входе
                }

                isInTrigger = true; // Установить флаг нахождения в триггере
                isFogCompleted = false; // Начать переход тумана

                meshRenderer.SetActive(false); // Скрыть meshRenderer
                objectToActivate.SetActive(true); // Активировать objectToActivate
                Book_Menu.SetActive(true); // Активировать меню книги
                Book.SetActive(false); // Скрыть книгу

                int childCount = other.transform.childCount;
                childObjects = new GameObject[childCount];

                // Копирование всех дочерних объектов другого объекта
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = other.transform.GetChild(i);
                    childObjects[i] = child.gameObject;
                }

                // Перемещение всех дочерних объектов в новый родительский объект
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = childObjects[i].transform;

                    child.SetParent(destinationParent);
                    child.localPosition = Vector3.zero;
                    child.localRotation = Quaternion.identity;
                    child.localScale = Vector3.one;
                }
                
                

                UpdateTargetID(targetID); // Обновление ID цели
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TriggerLocalPlayer"))
        {
            playerCount--;
            if (playerCount == 0)
            {
                if (soundOnExit != null)
                {
                    audioSource.PlayOneShot(soundOnExit); // Воспроизвести звук при выходе
                }

                isInTrigger = false; // Сбросить флаг нахождения в триггере
                isFogCompleted = false; // Начать переход тумана обратно

                meshRenderer.SetActive(true); // Показать meshRenderer
                objectToActivate.SetActive(false); // Скрыть objectToActivate
                Book_Menu.SetActive(false); // Скрыть меню книги
                Book.SetActive(true); // Показать книгу

                int childCount = destinationParent.transform.childCount;
                GameObject[] childObjects = new GameObject[childCount];

                // Подготовка к возврату всех дочерних объектов на свои исходные позиции
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = destinationParent.transform.GetChild(i);
                    childObjects[i] = child.gameObject;
                }

                // Перемещение всех дочерних объектов обратно к исходному родителю
                foreach (GameObject childObject in childObjects)
                {
                    Transform childTransform = childObject.transform;
                    childTransform.SetParent(other.transform);
                    childTransform.localPosition = Vector3.zero;
                    childTransform.localRotation = Quaternion.identity;
                    childTransform.localScale = Vector3.one;
                    childObject.SetActive(false);

                    Collider[] colliders = childTransform.GetComponentsInChildren<Collider>();
                    foreach (Collider hitCollider in colliders)
                    {
                        hitCollider.enabled = false;
                    }
                }
                
                
                // Выключение всех страниц в книге
                foreach (GameObject paper in BookPapper)
                {
                    if (paper != null)
                    {
                        paper.SetActive(false);
                    }
                }
                
            }
        }
    }

   public void UpdateTargetID(string newTargetID)
{	
    targetID = newTargetID; // Обновляем значение targetID на новое, полученное в параметре функции

    // Проходим по всем дочерним объектам объекта, который находится в переменной destinationParent
    foreach (Transform child in destinationParent)
    {
        // Пытаемся получить компонент Skin с дочернего объекта или его дочерних элементов
        Skin skin = child.GetComponentInChildren<Skin>();

        if (skin != null) // Проверяем, найден ли компонент Skin
        {
            skin.gameObject.SetActive(false); // Деактивируем GameObject, на котором находится компонент Skin

            // Если ID компонента Skin совпадает с новым targetID, то...
            if (skin.ID == targetID)
            {
                skin.gameObject.SetActive(true); // Активируем GameObject
            }
        }
    }
}

}
