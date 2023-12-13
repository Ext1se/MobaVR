using System.Collections;
using UnityEngine;

public class AnimateButtons : MonoBehaviour
{
    public GameObject[] buttons; // Массив объектов кнопок
    public float scaleAmount = 1.2f; // Насколько увеличивается масштаб
    public float animationTime = 0.5f; // Длительность анимации
    public AudioClip magicSound; // Звуковой эффект

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //StartCoroutine(AnimateButtonsCoroutine());
    }

    public void StartAnimationButtons()
    {
        StartCoroutine(AnimateButtonsCoroutine());
    }
    
    IEnumerator AnimateButtonsCoroutine()
    {
        // Проходим массив в прямом порядке
        foreach (GameObject button in buttons)
        {
            yield return AnimateButton(button);
        }

        // Проходим массив в обратном порядке
        for (int i = buttons.Length - 1; i >= 0; i--)
        {
            yield return AnimateButton(buttons[i]);
        }
    }

    IEnumerator AnimateButton(GameObject button)
    {
        // Увеличиваем масштаб
        yield return StartCoroutine(ScaleButton(button, Vector3.one * scaleAmount, animationTime));
        // Воспроизводим звук
        PlayMagicSound();
        // Уменьшаем масштаб
        yield return StartCoroutine(ScaleButton(button, Vector3.one, animationTime));
    }

    void PlayMagicSound()
    {
        if (magicSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(magicSound);
        }
    }

    IEnumerator ScaleButton(GameObject button, Vector3 targetScale, float duration)
    {
        Vector3 originalScale = button.transform.localScale;
        float time = 0;

        while (time < duration)
        {
            button.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        button.transform.localScale = targetScale;
    }
}