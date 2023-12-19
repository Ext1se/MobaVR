using UnityEngine;
//скрипт, который обнуляет анимации листочков, когда выходишь из книги, чтобы они не подвисали на перелистывании


public class ResetAnimatorOnEnableDisable : MonoBehaviour
{
    private Animator animator;
    public string defaultStateName = "Idle"; // Название состояния аниматора по умолчанию
    public string nextAnim = "Book_list_Next"; // Название состояния аниматора по умолчанию
    public string downAnim = "Book_list_Down"; // Название состояния аниматора по умолчанию
    public string[] triggersToReset = new string[] { "Book_list_Down", "Book_list_Next" }; // Триггеры для сброса

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
      //  ResetAnimatorState();
    }

    void OnDisable()
    {
        ResetAnimatorState();
    }

    void ResetAnimatorState()
    {
        if (animator == null)
        {
            return;
        }

        // Сброс всех триггеров
        foreach (string trigger in triggersToReset)
        {
            animator.ResetTrigger(trigger);
        }

        // Переход к состоянию по умолчанию
        animator.Play(defaultStateName, 0, 0f);
        animator.Play(nextAnim, 0, 0f);
        animator.Play(downAnim , 0, 0f);
    }
}