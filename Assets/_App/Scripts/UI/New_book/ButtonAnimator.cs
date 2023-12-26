using UnityEngine;
using System.Collections;

//Скорипт, который начинает плавно увеличивать кнопку Далее, если выбран один из пунктов меню

public class ButtonAnimator : MonoBehaviour
{
    public RectTransform buttonTransform; // RectTransform кнопки
    public float animationDuration = 1.0f; // Длительность одного цикла анимации (увеличение или уменьшение)

    private Vector3 originalScale; // Исходный размер кнопки
    private Vector3 originalScaleOLD; // Созраняем Исходный размер кнопки

    void OnEnable()
    {
        if (buttonTransform != null)
        {
            originalScale = Vector3.one; // Устанавливаем исходный размер как 1, 1, 1
            originalScale = buttonTransform.localScale; // Сохраняем исходный размер
            originalScaleOLD = originalScale;
            StartCoroutine(AnimateButton());
        }
    }

    void OnDisable()
    {
        if (buttonTransform != null)
        {
            originalScale = originalScaleOLD;
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
