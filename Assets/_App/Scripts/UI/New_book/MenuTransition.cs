using System.Collections;
using UnityEngine;

public class MenuTransition : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Группа канваса для управления прозрачностью UI
    public GameObject nextPage; // Объект для следующей страницы меню
    public GameObject previousPage; // Объект для предыдущей страницы меню
    public AudioClip pageTurnSound; // Звуковой клип для звука перелистывания страницы
    public AudioClip textSound; // Звуковой клип для звука текста
    public Animator rightPageAnimator; // Аниматор для правой страницы
    public Animator leftPageAnimator; // Аниматор для левой страницы
    public AnimateButtons animateButtonsScript; // Ссылка на скрипт AnimateButtons
    public float fadeSpeed = 1.0f; // Скорость изменения прозрачности (Alpha)

    private AudioSource audioSource; // Приватная переменная для компонента аудио источника

    void OnEnable() // Метод Start, вызываемый при старте скрипта
    {
        audioSource = GetComponent<AudioSource>(); // Получаем компонент AudioSource с текущего объекта
        StartCoroutine(FadeIn()); // Запускаем корутину для плавного появления меню
    }

    IEnumerator FadeIn() // Корутина для плавного увеличения прозрачности
    {
        PlayTextSound(); // Воспроизводим звук текста
        yield return StartCoroutine(ChangeAlpha(0, 1)); // Изменяем Alpha с 0 до 1
        animateButtonsScript.StartAnimationButtons(); // Запускаем анимацию кнопок
    }

    public void GoToNextMenu() // Метод для перехода к следующему меню
    {
        StartCoroutine(Transition(nextPage, rightPageAnimator, "Book_list_Next")); // Запускаем корутину для перехода к следующему меню
    }

    public void GoToPreviousMenu() // Метод для перехода к предыдущему меню
    {
        StartCoroutine(Transition(previousPage, leftPageAnimator, "Book_list_Down")); // Запускаем корутину для перехода к предыдущему меню
    }

    IEnumerator Transition(GameObject nextPage, Animator animator, string triggerName) // Корутина для перехода между меню
    {
        PlayTextSound(); // Воспроизводим звук текста
        yield return StartCoroutine(ChangeAlpha(1, 0)); // Изменяем Alpha с 1 до 0
        PlayPageTurnSound(); // Воспроизводим звук перелистывания страницы
        animator.SetTrigger(triggerName); // Активируем триггер анимации
        nextPage.SetActive(true); // Активируем следующую страницу меню
        gameObject.SetActive(false); // Деактивируем текущий объект
    }

    IEnumerator ChangeAlpha(float startAlpha, float endAlpha) // Корутина для изменения прозрачности
    {
        float elapsedTime = 0; // Таймер для отслеживания времени анимации

        while (elapsedTime < fadeSpeed) // Пока таймер не достиг скорости исчезновения
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeSpeed); // Плавно изменяем Alpha
            elapsedTime += Time.deltaTime; // Увеличиваем таймер
            yield return null; // Ждем следующего кадра
        }

        canvasGroup.alpha = endAlpha; // Устанавливаем конечное значение Alpha
    }

    void PlayPageTurnSound() // Метод для воспроизведения звука перелистывания страницы
    {
        if (pageTurnSound != null) // Если звуковой клип задан
            audioSource.PlayOneShot(pageTurnSound); // Воспроизводим звук
    }

    void PlayTextSound() // Метод для воспроизведения звука текста
    {
        if (textSound != null) // Если звуковой клип задан
            audioSource.PlayOneShot(textSound); // Воспроизводим звук
    }
}
