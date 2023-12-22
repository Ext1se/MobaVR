using UnityEngine;
using BNG; 

public class BookAnimationAndMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость перемещения к цели
    public Animator animator; // Ссылка на компонент аниматора
    public Transform targetBookHand; // Цель для перемещения
    private bool isMoving = false; // Указывает, перемещается ли книга
    private bool Exit = false; // Выходим из скрипта
    private bool animationEnded = false; // Флаг завершения анимации
    public AudioClip soundClip;//звук который будет воспроизводится, после того как книга прилетит в руку, типа нажмите на Х, чтобы получить подсказки
    
    // Ссылка на Urok_02, чтобы завершить обучение в книге
    public Urok_02 urok02Script;

    public GameObject BookMulyag;//муляж книги, которая будет перемещаться в заданную точку

   public AudioSource audioSourceToPlay; // Ссылка на AudioSource для воспроизведения звука который возпроизводится, когда игрок нажал на кнопку включения книги
   
   public ControllerBinding Button_X = ControllerBinding.XButtonDown;
   
    private void OffBook()
    {
      // Проверяем, что у нас есть ссылка на AudioSource и он не равен null
        if (audioSourceToPlay != null)
        {
            // Воспроизводим звук из указанного AudioSource
            audioSourceToPlay.Play();
            
        }

        BookMulyag.SetActive(false);//выключаем книгу

        Exit = true;
        
        //функция для запуска FinishUrok02() если он включен т.е. если обучение идёт
        if (urok02Script != null)
        {
            Debug.Log("включаю второй урок этап 1");
            // Проверка, является ли объект, на котором находится скрипт Urok_02, активным
            if (urok02Script.gameObject.activeInHierarchy)
            {
                Debug.Log("включаю второй урок этап 1");
                urok02Script.FinishUrok02();
            }
        }
    }
    
    
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
            
            //включаем звук, который говорит нажать на X
            if (soundClip != null)
            {
                AudioSource.PlayClipAtPoint(soundClip, transform.position);
            }
            
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetBookHand.position, moveSpeed * Time.deltaTime);
        }

        // Проверка нажатия кнопки X, после чего включаем звук и выключаем книгу
        if (InputBridge.Instance.GetControllerBindingValue(Button_X) && !Exit)
        {
            OffBook();
        }
        
    }
    
    
}