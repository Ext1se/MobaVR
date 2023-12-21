using UnityEngine;

public class BookAnimationAndMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость перемещения к цели
    public Animator animator; // Ссылка на компонент аниматора
    public Transform targetBookHand; // Цель для перемещения
    private bool isMoving = false; // Указывает, перемещается ли книга
    public bool ExitBook = false; // Указывает на необходимость выхода из скрипта

    private bool animationEnded = false; // Флаг завершения анимации
    public AudioSource audioSource;//звук который будет воспроизводится, после того как книга прилетит в руку, типа нажмите на Х, чтобы получить подсказки

    void Start()
    {
        targetBookHand = GameObject.Find("TargetBookHand").transform;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.SetTrigger("runPoletBook");
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PoletBook") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !animationEnded)
        {
            isMoving = true;
            animationEnded = true;
            animator.enabled = false; // Отключить компонент Animator
            
            if (audioSource != null)
            {
                audioSource.enabled = true; // Включить компонент AudioSource
                audioSource.Play(); // Воспроизвести звук
            }

        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetBookHand.position, moveSpeed * Time.deltaTime);
        }

        if (ExitBook)
        {
            gameObject.SetActive(false);
        }
    }
}