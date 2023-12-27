using UnityEngine;

// Скрипт для управления анимацией вспомогательного HUD.
// Позволяет найти объект с тегом "HudHelp", получить компонент MoveUI
// и вызвать определенную функцию из заданного списка.
public class HelpHudAnimation : MonoBehaviour
{
    // Перечисление доступных функций
    public enum MoveUIFunction
    {
        StartMovingHead,
        StartMovingUlta,
        StartMovingVoise
    }

    public MoveUIFunction selectedFunction; // Выбранная функция для выполнения

    private void OnEnable()
    {
        // Находим объект по тегу
        GameObject hudHelp = GameObject.FindGameObjectWithTag("HudHelp");
        if (hudHelp != null)
        {
            // Получаем компонент MoveUI
            MoveUI moveUIScript = hudHelp.GetComponent<MoveUI>();
            if (moveUIScript != null)
            {
                // Вызываем выбранную функцию
                switch (selectedFunction)
                {
                    case MoveUIFunction.StartMovingHead:
                        moveUIScript.StartMovingHead();
                        break;
                    case MoveUIFunction.StartMovingUlta:
                        moveUIScript.StartMovingUlta();
                        break;
                    case MoveUIFunction.StartMovingVoise:
                        moveUIScript.StartMovingVoise();
                        break;
                }
            }
        }
    }
}