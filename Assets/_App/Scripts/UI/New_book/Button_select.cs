using UnityEngine;
using UnityEngine.EventSystems; // Для работы с событиями UI

public class Button_select : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject object1; // Первый объект для активации
    public AudioClip hoverSound; // Звук наведения на кнопку
    public AudioClip clickSound; // Звук нажатия на кнопку

    private AudioSource audioSource; // Компонент для воспроизведения звука

    void Start()
    {
        // Получаем компонент AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    // Метод, вызываемый при наведении курсора на кнопку
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (object1 != null)
            object1.SetActive(true); // Активируем первый объект

       

        PlaySound(hoverSound); // Воспроизводим звук наведения
    }

    // Метод, вызываемый при убирании курсора с кнопки
    public void OnPointerExit(PointerEventData eventData)
    {
        if (object1 != null)
            object1.SetActive(false); // Деактивируем первый объект

       
    }

    // Метод, вызываемый при нажатии на кнопку
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickSound); // Воспроизводим звук нажатия
    }

    // Метод для воспроизведения звуков
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}