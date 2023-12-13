using UnityEngine;
using System.Collections;

public class ButtonAnimator : MonoBehaviour
{
    public RectTransform buttonTransform; // RectTransform кнопки
    public float animationDuration = 1.0f; // Длительность одного цикла анимации (увеличение или уменьшение)

    private Vector3 originalScale; // Исходный размер кнопки

    void OnEnable()
    {
        if (buttonTransform != null)
        {
            originalScale = buttonTransform.localScale; // Сохраняем исходный размер
            StartCoroutine(AnimateButton());
        }
    }

    IEnumerator AnimateButton()
    {
        while (true) // Бесконечный цикл для постоянной анимации
        {
            // Анимация увеличения размера
            yield return StartCoroutine(ScaleButton(originalScale * 1.1f)); // Увеличиваем на 10%

            // Анимация уменьшения размера
            yield return StartCoroutine(ScaleButton(originalScale)); // Возвращаем к исходному размеру
        }
    }

    IEnumerator ScaleButton(Vector3 targetScale)
    {
        float currentTime = 0;

        while (currentTime < animationDuration)
        {
            buttonTransform.localScale = Vector3.Lerp(buttonTransform.localScale, targetScale, currentTime / animationDuration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        buttonTransform.localScale = targetScale; // Убедитесь, что цель достигнута
    }
}
