using UnityEngine;
using BNG; 
using Photon.Pun; // Убедитесь, что у вас подключен Photon PUN

    //скрипт управления анимацией книги

public class BookAnimationAndMovement : MonoBehaviour
{
    public TriggerHandler triggerHandlerScript;// скрипт, в котором я активирую визуальных помощников рук, которые показывают как нажать на Х
    
    public PoletBookMulayg poletBookMulaygScript;//Скрипт который отвечает за полёт книги к руке
    public Animator animator; // Ссылка на компонент аниматора
    
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

            animationEnded = true;
            animator.enabled = false; // Отключить компонент Animator
            
            // Проверка, что ссылка на скрипт PoletBookMulayg установлена
            if (poletBookMulaygScript != null)
            {
                // Изменение переменной isMoving в скрипте PoletBookMulayg теперь книга будет летать за моей рукой
                poletBookMulaygScript.isMoving = true;
            }
            
            //включаем звук, который говорит нажать на X
            if (soundClip != null)
            {
                AudioSource.PlayClipAtPoint(soundClip, transform.position);
            }
            if (triggerHandlerScript != null)
            {
                triggerHandlerScript.Button_x();
            }
            

        }
        

        // Проверка нажатия кнопки X, после чего включаем звук и выключаем книгу
        if (InputBridge.Instance.GetControllerBindingValue(Button_X) && !Exit)
        {
            OffBook();
        }

        if (Application.platform != RuntimePlatform.Android && Input.GetKeyDown(KeyCode.X) && !Exit)
        {
            OffBook();
        }
        
    }




}