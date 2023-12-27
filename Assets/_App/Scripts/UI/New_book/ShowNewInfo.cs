using UnityEngine;
using System.Collections;

public class ShowNewInfo : MonoBehaviour
{
    //отвечает за плавное включение информации на страницах, при переключении кнопок меню, не переворачивая страницу
    public GameObject[] playerInfoObjects; // Массив объектов с информацией об игроках
    public AudioClip textDisappearSound; // Звук исчезновения текста
    public AudioClip textAppearSound; // Звук появления текста
    public float fadeDuration = 1.0f; // Длительность затухания/появления текста

    private AudioSource audioSource;
    private GameObject currentActiveInfo;

 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Первоначально выключаем все объекты, кроме тех, которые активируются по умолчанию
        foreach (var infoObject in playerInfoObjects)
        {
            if (infoObject != null)
            {
                InfoObjectInitializer initializer = infoObject.GetComponent<InfoObjectInitializer>();
                if (initializer == null || !initializer.activateOnStart)
                {
                    infoObject.SetActive(false);
                }
                else
                {
                    // Запоминаем активный объект, если он активирован по умолчанию
                    currentActiveInfo = infoObject;
                }
            }
        }
    }

    public void ShowInfo(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < playerInfoObjects.Length)
        {
            GameObject selectedInfo = playerInfoObjects[playerIndex];
            if (selectedInfo != currentActiveInfo)
            {
                if (currentActiveInfo != null)
                {
                    StartCoroutine(FadeOutInfo(currentActiveInfo));
                }
                StartCoroutine(FadeInInfo(selectedInfo));
                currentActiveInfo = selectedInfo;
            }
        }
    }

    IEnumerator FadeOutInfo(GameObject infoObject)
    {
        PlaySound(textDisappearSound);
        CanvasGroup canvasGroup = infoObject.GetComponent<CanvasGroup>();
        yield return StartCoroutine(ChangeAlpha(canvasGroup, 1, 0, fadeDuration));
        infoObject.SetActive(false);
    }

    IEnumerator FadeInInfo(GameObject infoObject)
    {
        infoObject.SetActive(true);
        CanvasGroup canvasGroup = infoObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 0; // Начинаем с полной прозрачности

        PlaySound(textAppearSound);
        yield return StartCoroutine(ChangeAlpha(canvasGroup, 0, 1, fadeDuration));
    }

    IEnumerator ChangeAlpha(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }


}