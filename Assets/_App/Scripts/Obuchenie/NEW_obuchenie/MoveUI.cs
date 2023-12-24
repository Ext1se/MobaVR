using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Для использования класса Image
using System.Collections;



    ///скрипт который перемещает UI во время обучения, чтобы игроку было более понятно, что от него хотят. Как учитывать здоровье ультимейт и прочие шалости )
public class MoveUI : MonoBehaviour
{
        /// <summary>
        ///эти перменные мы используем для управления скриптом
        /// </summary>
    public RectTransform targetPosition; // Используем RectTransform для цели
    private GameObject objectToMove; // Объект для перемещения
    private GameObject AnimatorButton; // Анимация. которая показывает какие кнопки включать
    private Image imageToFill; // Компонент Image для управления fill amount
    private Vector3 originalScale; // Для сохранения исходного размера объекта
    private RectTransform rectTransform;// Для сохранения исходного размера объекта
    public AudioClip movingSound; // Аудиоклип, который будет воспроизводиться при движении объекта
    private AudioSource audioSource; // Источник звука для воспроизведения аудиоклипа
    
    
        /// <summary>
        /// в эти переменные мы закилдываем наши данные из инспектора
        /// </summary>
  
        
        public GameObject Head; // Объект для перемещения Жизни
        public Image imageToFillHead; // Компонент Image для управления fill amount в Жизнях
        public GameObject Ulta; // Объект для перемещения Ультиимейт
        public Image[] imageToFillUlta; // Компонент Image для управления fill amount в Ультимейте
    
        public GameObject Animator_Ulta;//анимация, которая включает кнопки на контроллере
        public GameObject Voise; // Объект для перемещения Крик
        public Image imageToFillVoise; // Компонент Image для управления fill amount в Крике
        public GameObject Animator_Voise;//анимация, которая включает кнопки на контроллере
    
        /// <summary>
        /// Эти переменные нужны для того, чтобы управлять скоростью хода
        /// </summary>
    public float moveDuration = 2f; // Время перемещения в секундах
    public float delayAtTarget = 2f; // Время задержки на цели в секундах
    public float fillDuration = 1f; // Время для изменения fill amount

    private Vector2 originalAnchoredPosition; // Для сохранения исходного положения внутри Canvas

    private Coroutine moveCoroutine; // Для остановки корутины, если это необходимо

    public bool test = false; //для теста нужно
    public bool test2 = false; //для теста нужно
    public bool test3 = false; //для теста нужно

    private void Start()
    { 
        audioSource = gameObject.AddComponent<AudioSource>();

        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Отписываемся от события загрузки сцены
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    //Запускаем анимацию жизней
    public void StartMovingHead()
    {
        //если корутина остановилась и все объекты на месте, то можно включать 
        if (moveCoroutine == null)
        {
            // Останавливаем предыдущую корутину, если она активна
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // Сохраняем исходное положение объектов внутри Canvas
            originalAnchoredPosition = Head.GetComponent<RectTransform>().anchoredPosition;
            rectTransform = Head.GetComponent<RectTransform>();
            originalScale = rectTransform.localScale; // Сохраняем исходный масштаб

            objectToMove = Head;
            imageToFill = imageToFillHead;

            // Запускаем корутину для перемещения объекта
            moveCoroutine = StartCoroutine(MoveObjectRoutine());
        }
    }
    
    //Запускаем анимацию Ультимейта
    public void StartMovingUlta()
    {
        //если корутина остановилась и все объекты на месте, то можно включать 
        if (moveCoroutine == null)
        {
            // Останавливаем предыдущую корутину, если она активна
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // Сохраняем исходное положение объектов внутри Canvas
            originalAnchoredPosition = Ulta.GetComponent<RectTransform>().anchoredPosition;
            rectTransform = Ulta.GetComponent<RectTransform>();
            originalScale = rectTransform.localScale; // Сохраняем исходный масштаб

            //применяем настройки. с этими объектами работаем дальше
            objectToMove = Ulta;
            AnimatorButton = Animator_Ulta;
            foreach (var image in imageToFillUlta)
            {
                if (image.gameObject.activeInHierarchy)
                {
                    imageToFill = image;
                    break; // Выходим из цикла, так как нашли активный объект
                }
            }

            // Запускаем корутину для перемещения объекта
            moveCoroutine = StartCoroutine(MoveObjectRoutine());
        }
    }

    
    //Запускаем анимацию Крика
    public void StartMovingVoise()
    {
        //если корутина остановилась и все объекты на месте, то можно включать 
        if (moveCoroutine == null)
        {
            // Останавливаем предыдущую корутину, если она активна
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // Сохраняем исходное положение объектов внутри Canvas
            originalAnchoredPosition = Voise.GetComponent<RectTransform>().anchoredPosition;
            rectTransform = Voise.GetComponent<RectTransform>();
            originalScale = rectTransform.localScale; // Сохраняем исходный масштаб
            objectToMove = Voise;
            imageToFill = imageToFillVoise;
            AnimatorButton = Animator_Voise;

            // Запускаем корутину для перемещения объекта
            moveCoroutine = StartCoroutine(MoveObjectRoutine());
        }
    }

    

    private IEnumerator MoveObjectRoutine()
    {

        audioSource.PlayOneShot(movingSound);//включаем звук
        float timer = 0;
        // Задаем конечный размер
        Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // Увеличиваем размер объекта и двигаем объект
        while (timer < moveDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(originalAnchoredPosition, targetPosition.anchoredPosition, timer / moveDuration);
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        
        //отключаем анимацию кнопок, если есть
        if (AnimatorButton != null)
        {
            AnimatorButton.SetActive(false);
        }


        // Анимация fill amount от 1 до 0
        timer = 0;
        while (timer < fillDuration)
        {
            imageToFill.fillAmount = Mathf.Lerp(1, 0, timer / fillDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Задержка на цели
        yield return new WaitForSeconds(delayAtTarget);

        // Анимация fill amount от 0 до 1
        timer = 0;
        while (timer < fillDuration)
        {
            imageToFill.fillAmount = Mathf.Lerp(0, 1, timer / fillDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        
        if (AnimatorButton != null)
        {
            AnimatorButton.SetActive(true);
        }

        
        
        // Уменьшаем размер объекта и возвращаем объект а место
        timer = 0;
        while (timer < moveDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(targetPosition.anchoredPosition, originalAnchoredPosition, timer / moveDuration);
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        moveCoroutine = null;
    }

    //если вдруг выходим из сцены, т овсё резко возвращаем на место
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        // Остановка корутины, если она активна
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        
        // Мгновенное возвращение UI элемента в его исходное положение
        objectToMove.GetComponent<RectTransform>().anchoredPosition = originalAnchoredPosition;
        
        // Мгновенное возвращение UI элемента к его исходному размеру
        rectTransform.localScale = originalScale;

        imageToFill.fillAmount = 1;
        
        if (AnimatorButton != null)
        {
            AnimatorButton.SetActive(true);
        }

    }


    private void Update()
    {
        if (test == true)
        {
            StartMovingHead();
            test = false;
        }
        
        if (test2 == true)
        {
            StartMovingUlta();
            test2 = false;
        }
        
        if (test3 == true)
        {
            StartMovingVoise();
            test3 = false;
        }
       
    }
}
