using UnityEngine;
using System.Collections;

public class CharacterActions : MonoBehaviour
{
    // Ссылка на Transform игрока.
    public Transform playerTransform; 
   

    // Время ожидания перед следующим действием.
    public float waitingTime = 3.0f;

    // Максимальное расстояние до игрока, на котором будет срабатывать действие.
    public float maxDistanceToPlayer = 2.0f;

    // Приватные переменные для управления персонажем, звуком и анимацией.
    private CharacterMover mover; //шаги
    private EndTutorialHandler endTutorialHandler; //конец обучения
    public AudioSource audioSource;
    private Animator animator;
    private float waitingTimeStart = 1.0f;
    private bool playerApproached = false;//проверяем дошёл ли игрок до персонажа
    
    // Массив шагов обучения.
    public TutorialStep[] tutorialSteps;
   
    
    // Индекс текущего шага обучения.
    private int currentStepIndex = 0;
    
    public int CurrentStepIndex 
    {
        get { return currentStepIndex; }
    }
    

    // Индекс последнего воспроизведенного звука.
    private int lastPlayedSoundIndex = -1;
    
    private IEnumerator _rotateCoroutine;
    private bool continueTutorial = false;
    
    
    // Новая переменная для хранения объекта, к которому будет двигаться персонаж.
    public GameObject TargetWay;
    

    // Класс, описывающий каждый шаг обучения.
    [System.Serializable]
    public class TutorialStep
    {
        public int NomerUroka;
        public string description; // Описание шага.
        public AudioClip startSound; // Звук при старте шага.
        
        public Transform TransformTargetWay; //точка в которую будет стрелять луч от игрока
        public string startAnimation; // Анимация при старте шага.
        public GameObject SubStart;//субтитры старта
        public bool startSoundRun; //звук проигрался
        public Transform targetPoint; // Точка, к которой должен двигаться персонаж.
        public bool targetPointRun; //Персонаж подошёл к точке
        public AudioClip[] waitingForPlayerSounds; // Звуки ожидания игрока.
        public string[] waitingForPlayerAnimations; // Анимации ожидания игрока.
        public bool layerGoPersRun; //Игрок подошёл к персонажу
        public AudioClip mainTaskSound; // Главный звук задания.
        public string mainTaskAnimation; // Главная анимация задания.
        public GameObject SubMainTask;//субтитры главного звука
        public bool mainTaskSoundRun; //Основной звук проигрался
        public bool stopUrok; // Переменная ожидания выполнения скрипта
        public AudioClip[] waitingForTaskCompletionSounds; // Звуки ожидания завершения задания.
        public string[] waitingForTaskCompletionAnimations; // Анимации ожидания завершения задания.
        public bool isTaskCompleted = false; // Флаг завершения задания.
    }

    // Метод вызывается при инициализации объекта.
    private void Awake()
    {
        // Получаем компоненты объекта.
        mover = GetComponent<CharacterMover>();
        endTutorialHandler  = GetComponent<EndTutorialHandler>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Метод вызывается при активации объекта.
    void OnEnable()
    {
        
        //сохраняем данные
        SaveTutorialStepsData();
        // Запускаем обучение.
        StartTutorial();
    }
   
    void OnDisable()
    {
        lastPlayedSoundIndex = -1;
        
        currentStepIndex = 0;
        
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
        LoadTutorialStepsData();
    }
    

    // Метод запуска обучения.
    public void StartTutorial()
    {
        // Если есть шаги обучения, запускаем первый шаг.
        if (tutorialSteps.Length > 0)
        {
            StartCoroutine(ExecuteStep(tutorialSteps[0]));
        }
    }

    // Корутина выполнения шага обучения.
    public IEnumerator ExecuteStep(TutorialStep step)
    {

       
        //Debug.Log("Шаг: ");
        
        // Если есть звук, , включаем его и ждем его завершения.
        if (step.startSound)
        {
            //Debug.Log("Звук есть, запускаем корутину  PlaySoundAndAnimation и отправляем в неё звук из урока номер: ");
            
            /*
            // Перемещаем указатель куда нужно идти в заданную точку
            if (step.TransformTargetWay != null && TargetWay != null)
            {
                TargetWay.transform.position = step.TransformTargetWay.position;
            }*/
            
            
            //включаем субтитры
            if (step.SubMainTask)
            {step.SubMainTask.SetActive(false);}

            if (step.SubStart)
            {
                step.SubStart.SetActive(true); 
             
            }
            
            
            
            
            // Воспроизводим звук и анимацию начала шага. передаёт в функцию  PlaySoundAndAnimation звук и анимацию
            PlaySoundAndAnimation(step.startSound, step.startAnimation);
            
           // Debug.Log("Ждём пока воспроизводится звук");
            //стоит до тех пор, пока звук не воспроизведётся
            yield return new WaitForSeconds(step.startSound.length);
            
           // Debug.Log("Звук воспроизвелся");
            
            step.startSoundRun = true; //флаг, что звук проигран, можно использовать в других скриптах
        }
        else
        {
           // Debug.Log("Звука нет");
        }

        // Если есть целевая точка, двигаемся к ней.
        if (step.targetPoint)
        {
            // Перемещаем указатель куда нужно идти в заданную точку и делаем его дочерним элементом
            if (step.TransformTargetWay)
            {
                // Устанавливаем родительский элемент для TargetWay
                TargetWay.transform.SetParent(step.TransformTargetWay);

                // Устанавливаем локальные позицию и поворот в 0, чтобы TargetWay был точно в позиции TransformTargetWay
                TargetWay.transform.localPosition = Vector3.zero;
                TargetWay.transform.localRotation = Quaternion.identity;
            }
            
            //отправляем игрока к точке
            mover.MoveToPoint(step.targetPoint.position);
        
           //Останавливаем корутину которая управляет поворотами перса
            if(_rotateCoroutine != null) 
            {
                StopCoroutine(_rotateCoroutine);
                _rotateCoroutine = null; // Не забывайте обнулить ссылку
            }
            
            // Ждем, пока персонаж не достигнет своего пункта назначения
            yield return new WaitWhile(() => !mover.HasReachedDestination());
            step.targetPointRun = true;//персонаж дошёл до точки
            
           //Запускаем корутину которая управляет поворотами перса
            _rotateCoroutine = RotateTowardsPlayerRoutine();
            StartCoroutine(_rotateCoroutine);
        }

        // Запускаем корутину ожидания игрока.
        yield return StartCoroutine(WaitForPlayerRoutine(step));
        

    }

   // Корутина ожидания игрока.
    private IEnumerator WaitForPlayerRoutine(TutorialStep step)
    {
        
        //главный звук ещё не воспроизвелся
        bool hasPlayedMainTaskSound = false;

        // Цикл продолжается, пока задание не выполнено.
        while (!step.isTaskCompleted)
        {
            
            // Если идет воспроизведение звука, ждем его завершения.
            if (audioSource.isPlaying)
            {
                yield return new WaitWhile(() => audioSource.isPlaying);
            }

            
            
          
            
            // Ждем установленное время. 1 сек
            yield return new WaitForSeconds(waitingTimeStart);
            
            #region далее идут условися первое: если игрок дошёл и главный звук ещё не воспроизвелся, второй если игрок далеко, и есть звуки ожидания.
            //далее идут условися первое: если игрок дошёл и главный звук ещё не воспроизвелся, второй если игрок далеко, и есть звуки ожидания.

            // Если игрок находится в пределах допустимого расстояния и главный звук задания еще не воспроизводился.
            if (Vector3.Distance(transform.position, playerTransform.position) <= maxDistanceToPlayer && !hasPlayedMainTaskSound && !audioSource.isPlaying)
            {
                step.layerGoPersRun = true; //игрок дошёл до персонажа
                playerApproached = true; //Игрок подошёл
                
                //включаем субтитры
                if (step.SubStart)
                {step.SubStart.SetActive(false); }
                if (step.SubMainTask)
                {step.SubMainTask.SetActive(true);}
                
                
                //если есть звук основной 
                if (step.mainTaskSound)
                {
                    // Воспроизводим главный звук и анимацию задания.
                    PlaySoundAndAnimation(step.mainTaskSound, step.mainTaskAnimation);
                        //ждём пока воспроизводится звук
                    yield return new WaitForSeconds(step.mainTaskSound.length);
                        //говорим, что главный звук воспроизвёлся
                    step.mainTaskSoundRun = true;
    
                    if (step.stopUrok) // Проверяем, нужно ли остановить урок
                    {
                        yield return new WaitUntil(() => continueTutorial);
                    }
                    
                }

                //основной звук отыгран, вне зависимости был он или нет.
                hasPlayedMainTaskSound = true;
                
                //говорим, что первый урок пройден
                if (currentStepIndex == 0)
                {
                    step.isTaskCompleted = true;
                }
                
            }
            
            // Если игрок далеко и есть звуки ожидания игрока и главный звук не воспроизвёлся
            else if(Vector3.Distance(transform.position, playerTransform.position) > maxDistanceToPlayer && step.waitingForPlayerSounds.Length > 0 && !hasPlayedMainTaskSound)
            {
                
                // Ждем установленное время.
                yield return new WaitForSeconds(waitingTime);
                // Если есть условие для прерывания ожидания, выполнить прерывание.
                if (playerApproached)
                {
                    playerApproached = false; // Игрок далеко
                    yield return null; // Сразу завершить текущую итерацию корутины
                    continue; // Пропустить остальной код и начать следующую итерацию цикла
                }
                
                //генерирование рандомного звука и анимации из всех предоставленных в массивах
                int randomSoundIndex = GetRandomSoundIndex(step.waitingForPlayerSounds.Length);
                int randomAnimationIndex = GetRandomAnimationIndex(step.waitingForPlayerAnimations.Length);

                    //воспроизводим звук и анимацию
                     PlaySoundAndAnimation(step.waitingForPlayerSounds[randomSoundIndex], step.waitingForPlayerAnimations[randomAnimationIndex]);
            }

            #endregion
            
            
            
                // Если есть звуки ожидания завершения задания, и главный звук был воспроизведён, и урок не окончен
                if (step.waitingForTaskCompletionSounds.Length > 0 && hasPlayedMainTaskSound && !step.isTaskCompleted)
                {
                    // Ждем установленное время.
                    yield return new WaitForSeconds(waitingTime);
                        //делаем индекс рандомный для звука
                    int randomSoundIndex = GetRandomSoundIndex(step.waitingForTaskCompletionSounds.Length);

                    // Проверьте, что массив не пустой и индекс находится в пределах допустимого диапазона и звук не воспроизводится
                    if (step.waitingForTaskCompletionSounds.Length > 0 && randomSoundIndex < step.waitingForTaskCompletionSounds.Length &&  !audioSource.isPlaying) 
                    {
                        
                        //воспроизводит звук рандомный один раз
                        audioSource.PlayOneShot(step.waitingForTaskCompletionSounds[randomSoundIndex]);
                        
                            //делает рандомный индекс для анимации
                        int randomAnimationIndex = GetRandomAnimationIndex(step.waitingForTaskCompletionAnimations.Length);
                        
                        // Точно такая же проверка для массива анимаций.
                        if (step.waitingForTaskCompletionAnimations.Length > 0 && randomAnimationIndex < step.waitingForTaskCompletionAnimations.Length && !string.IsNullOrEmpty(step.waitingForTaskCompletionAnimations[randomAnimationIndex])) 
                        {
                            SetAnimationParameter(step.waitingForTaskCompletionAnimations[randomAnimationIndex], true);
                        }

                    }


                    
                }

                // Если задание выполнено.
                if (step.isTaskCompleted)
                {
                    
                    //выключаем субтитры
                    if (step.SubStart)
                    {step.SubStart.SetActive(false); }
                    if (step.SubMainTask)
                    {step.SubMainTask.SetActive(false);}
                    
                // Переходим к следующему шагу обучения.
                NextStep();
                }
        }
    }

    
    // Получить случайный индекс звука, отличный от последнего воспроизведенного.
    private int GetRandomSoundIndex(int soundsCount)
    {
        int randomSoundIndex;
        do
        {
            randomSoundIndex = Random.Range(0, soundsCount);
        } while (randomSoundIndex == lastPlayedSoundIndex);

        lastPlayedSoundIndex = randomSoundIndex;
        return randomSoundIndex;
    }

    // Получить случайный индекс анимации.
    private int GetRandomAnimationIndex(int animationsCount)
    {
        return Random.Range(0, animationsCount);
    }

    // Метод воспроизводит звук и анимацию рандомную
    private void PlaySoundAndAnimation(AudioClip sound, string animationParameter)
    {
        // Debug.Log("Корутина включилась");
        
        //если у нас есть звук и он не проигрывается
        if (sound)
        {
           // Debug.Log("есть звук, воспроизводим его один раз");
            audioSource.PlayOneShot(sound);

           // Debug.Log("Хотим воспроизвести анимацию");
            // Воспроизводим анимацию, только если у нас есть соответствующий параметр для этого звука
            if (!string.IsNullOrEmpty(animationParameter))
            {
               // Debug.Log("Воспроизводим анимацию, она есть");
                SetAnimationParameter(animationParameter, true);
                
                // Останавливаем анимацию, когда звук закончит играть
                StartCoroutine(StopAnimationWhenSoundStops(sound.length, animationParameter));
            }
        }
       
        
    }


    //эта корутина будет вместо PlaySoundAndAnimation
    private IEnumerator PlaySoundAndChangeAnimationParameter(AudioClip sound, string animationParameter)
    {
        if (sound && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(sound);
            SetAnimationParameter(animationParameter, true);
            yield return new WaitForSeconds(sound.length);
            SetAnimationParameter(animationParameter, false);
        }
    }
    
    
    // Метод перехода к следующему шагу обучения.
    public void NextStep()
    {
       
        currentStepIndex++;
        // Если следующий шаг существует, запускаем его.
        if (currentStepIndex < tutorialSteps.Length)
        {
            StartCoroutine(ExecuteStep(tutorialSteps[currentStepIndex]));
        }
        else
        {
            endTutorialHandler.EndTutorialSequence();
            // Если обучение завершено, выводим сообщение в консоль.
           // Debug.Log("Tutorial completed!");
        }
    }
  
    //корутина поворота
    private IEnumerator RotateTowardsPlayerRoutine()
    {
        while (true)
        {
            Vector3 targetPoint = playerTransform.position;
            //if (!mover.Agent.isStopped)
            
            if (!mover.HasReachedDestination())
            {
                mover.Agent.updateRotation = true;
                targetPoint = mover.Agent.destination;
            }
            else
            {
                mover.Agent.updateRotation = false;
            }
                
                //Vector3 directionToPlayer = playerTransform.position - transform.position;
                Vector3 directionToPlayer = targetPoint - transform.position;
                directionToPlayer.y = 0; // Убедитесь, что вращение происходит только по горизонтали
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.5f);

                yield return null; // Это будет выполнять корутину каждый кадр

        }
    }
    
    //стопорим звук
    public void StopAllSounds()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    //в этой функции мы безопасно применяем параметру значение true
    private void SetAnimationParameter(string parameter, bool value)
    {
        if (!string.IsNullOrEmpty(parameter))
        {
            animator.SetBool(parameter, value);
        }
    }
    
    private IEnumerator StopAnimationWhenSoundStops(float soundDuration, string animationParameter)
    {
        // Ждем, пока звук закончит воспроизводиться
        yield return new WaitForSeconds(soundDuration);

        // Устанавливаем параметр анимации обратно в false
        SetAnimationParameter(animationParameter, false);
    }
    
    public bool IsMainTaskSoundPlaying(int stepIndex)
    {
        // Проверяем, воспроизводится ли звук mainTaskSound заданного шага обучения.
        if(stepIndex < 0 || stepIndex >= tutorialSteps.Length)
        {
            Debug.LogError("Invalid step index provided!");
            return false;
        }
    
        return audioSource.isPlaying && audioSource.clip == tutorialSteps[stepIndex].mainTaskSound;
    }
    
    //ожидание выполнения скрипта, после звука главного
    public void OnTutorialsCompleted()
    {
        continueTutorial = true;
    }
    
    //методы сохранения изначальных значений переменных
    public void SaveTutorialStepsData()
    {
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            PlayerPrefs.SetInt($"TutorialStep_{i}_startSoundRun", tutorialSteps[i].startSoundRun ? 1 : 0);
            PlayerPrefs.SetInt($"TutorialStep_{i}_targetPointRun", tutorialSteps[i].targetPointRun ? 1 : 0);
            PlayerPrefs.SetInt($"TutorialStep_{i}_layerGoPersRun", tutorialSteps[i].layerGoPersRun ? 1 : 0);
            PlayerPrefs.SetInt($"TutorialStep_{i}_mainTaskSoundRun", tutorialSteps[i].mainTaskSoundRun ? 1 : 0);
            PlayerPrefs.SetInt($"TutorialStep_{i}_isTaskCompleted", tutorialSteps[i].isTaskCompleted ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadTutorialStepsData()
    {
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].startSoundRun = PlayerPrefs.GetInt($"TutorialStep_{i}_startSoundRun", 0) == 1;
            tutorialSteps[i].targetPointRun = PlayerPrefs.GetInt($"TutorialStep_{i}_targetPointRun", 0) == 1;
            tutorialSteps[i].layerGoPersRun = PlayerPrefs.GetInt($"TutorialStep_{i}_layerGoPersRun", 0) == 1;
            tutorialSteps[i].mainTaskSoundRun = PlayerPrefs.GetInt($"TutorialStep_{i}_mainTaskSoundRun", 0) == 1;
            tutorialSteps[i].isTaskCompleted = PlayerPrefs.GetInt($"TutorialStep_{i}_isTaskCompleted", 0) == 1;
        }
    }
    
    
}
