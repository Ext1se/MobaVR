using UnityEngine;
using System.Collections;


public class PageFlipController : MonoBehaviour
{
    public Animator rightPageAnimator; // Аниматор для правой страницы
    public Animator leftPageAnimator; // Аниматор для левой страницы
    //public CanvasGroup canvasGroup; // Канвас с CanvasGroup для управления прозрачностью
	public CanvasGroup[] canvasGroups; // Канвас с CanvasGroup для управления прозрачностью

    public GameObject nextPage; // Объект для следующей страницы меню
    public GameObject previousPage; // Объект для предыдущей страницы меню
    public AudioClip textAppearSound; // Звук появления текста
    public AudioClip textAppearSound2; // Звук появления текста2
    public AudioClip pageFlipSound; // Звук перелистывания книги
    public float animationDelay = 4.0f; // Задержка для активации страницы после анимации
    public float animationText = 0.2f; // Задержка для активации расстворения текста
    public AnimateButtons animateButtonsScript; // Ссылка на скрипт AnimateButtons

    public GameObject[] pagesArray; // Массив страниц
    public int selectedPageID; // ID выбранной страницы

    private AudioSource audioSource; // Источник звука для воспроизведения звуков

    void OnEnable()
    {
        audioSource = GetComponent<AudioSource>(); // Получаем компонент AudioSource
        PlaySound(textAppearSound); // Воспроизводим звук появления текста
        //canvasGroup.alpha = 0; // Устанавливаем начальное значение альфы в 0
		foreach (var group in canvasGroups)
        {
            if (group != null)
            {
                group.alpha = 0; // Устанавливаем начальное значение альфы в 0
            }
        }
		 StartCoroutine(FadeCanvasGroupsAlphaStart(0, 1)); // Плавно увеличиваем альфу до 1
      // StartCoroutine(FadeCanvasGroupAlphaStart(0, 1)); // Плавно увеличиваем альфу до 1
      
        
    }

    // Функция для листания правой страницы
    public void FlipRightPage()
    {
        StartCoroutine(FlipPage("Book_list_Next", nextPage));
    }


    // Функция для установки выбранной страницы
    public void SetSelectedPageID(int id)
    {
        if (id >= 0 && id < pagesArray.Length)
        {
            selectedPageID = id;
        }
    }

    // Новая функция, аналогичная FlipRightPage, но работающая с массивом страниц
    public void FlipRightPageMassive()
    {
        GameObject targetPage = pagesArray[selectedPageID];
        if (targetPage != null)
        {
            StartCoroutine(FlipPage("Book_list_Next", targetPage));
        }
    }





    // Функция для листания левой страницы
    public void FlipLeftPage()
    {
        StartCoroutine(FlipPage("Book_list_Down", previousPage));
    }

    IEnumerator FlipPage(string trigger, GameObject targetPage)
    {
        

        PlaySound(textAppearSound2); // Воспроизводим звук появления текста
        yield return StartCoroutine(FadeCanvasGroupsAlpha(1, 0)); // Плавно уменьшаем альфу до 0
       
        // Активируем соответствующий триггер аниматора
        if (trigger == "Book_list_Next")
        {
            rightPageAnimator.SetTrigger(trigger);
        }
        else if (trigger == "Book_list_Down")
        {
            leftPageAnimator.SetTrigger(trigger);
        }

        PlaySound(pageFlipSound); // Воспроизводим звук перелистывания книги
        // Ожидаем заданное время после анимации
        yield return new WaitForSeconds(animationDelay);

        // Активация целевой страницы и деактивация текущей
        targetPage.SetActive(true);
        gameObject.SetActive(false);
    }



    IEnumerator FadeCanvasGroupsAlpha(float startAlpha, float endAlpha)
    {
        float duration = 2.0f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            foreach (var group in canvasGroups)
            {
                if (group != null)
                {
                    group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var group in canvasGroups)
        {
            if (group != null)
            {
                group.alpha = endAlpha;
            }
        }
    }

    IEnumerator FadeCanvasGroupsAlphaStart(float startAlpha, float endAlpha)
    {
        yield return new WaitForSeconds(animationText);
        
        float duration = 2.0f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            foreach (var group in canvasGroups)
            {
                if (group != null)
                {
                    group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var group in canvasGroups)
        {
            if (group != null)
            {
                group.alpha = endAlpha;
            }
        }

        animateButtonsScript.StartAnimationButtons(); // Запускаем анимацию кнопок
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }



}
