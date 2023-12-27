using UnityEngine;
using BNG;
using System.Collections;

public class Defender_Ultimate : MonoBehaviour
{
    [System.Serializable]
    public class Lesson
    {
        public GameObject BannerTutorial;
        public AudioClip SoundTutorial;
        public string animTutorial;
        public ControllerBinding Button1 = ControllerBinding.LeftGrip;
        public ControllerBinding Button2 = ControllerBinding.LeftGrip;
        public ControllerBinding Button3 = ControllerBinding.LeftGrip;
        public ControllerBinding Button4 = ControllerBinding.LeftGrip;
        public bool test = false;
        public bool autoStop;
        [HideInInspector] public bool isPlaying;
    }
    public int NomerUroka;
    public SaveInfoClass _SaveInfoClass;// получаем класс ирока
    public string ID;//вводим класс игрока, который будем сравнивать с полученным
    private CharacterActions characterActions; // Ссылка на ваш скрипт CharacterActions
    private AudioSource audioSource; // Аудиосоурс на этом объекте
    public Lesson[] lessons;
    private int currentLesson = 0;
    private bool RunUrok0;
    private bool timerStarted = false;
    private float timerDuration = 2f;
    
    private void Start()
    {
        RunUrok0=false;

        characterActions = GetComponent<CharacterActions>();
        audioSource = GetComponent<AudioSource>(); // Получаем компонент аудиосоурс
        // Отключаем все баннеры уроков в начале
        foreach (var lesson in lessons)
        {
            lesson.BannerTutorial.SetActive(false);
        }
    }

    
    void OnEnable()
    {
        currentLesson = 0;
        RunUrok0=false;
        foreach (var lesson in lessons)
        {
            lesson.BannerTutorial.SetActive(false);
        }

    }
    
    private void Update()
    {

        if (ID == _SaveInfoClass.targetID)
        {
            if (characterActions && characterActions.tutorialSteps[NomerUroka].mainTaskSoundRun && !RunUrok0)
                    {
                        if (currentLesson < lessons.Length && !lessons[currentLesson].isPlaying)
                        {
                            RunUrok0=true;
                            StartLesson(0);
                        }
                    }
            
                    if (currentLesson < lessons.Length && RunUrok0)
                    {
                        Lesson currentLessonObj = lessons[currentLesson];
            
                        if (InputBridge.Instance.GetControllerBindingValue(currentLessonObj.Button1) && InputBridge.Instance.GetControllerBindingValue(currentLessonObj.Button2) && InputBridge.Instance.GetControllerBindingValue(currentLessonObj.Button3) && InputBridge.Instance.GetControllerBindingValue(currentLessonObj.Button4) || currentLessonObj.test || currentLessonObj.autoStop)
                        {
                            if (currentLessonObj.autoStop)
                            {
                               
                               // Debug.Log("Запускаем звук");
                                StartCoroutine(StartEndLessonAuto());
                                currentLessonObj.autoStop = false;
                            }
                            else if (!timerStarted)
                            {
                                currentLessonObj.test = false;
                                timerStarted = true;
                               // Debug.Log("Запускаем ожидание");
                                StartCoroutine(StartEndLessonTimer());
                            }
                        }
                        else
                        {
                            // Если кнопка больше не нажимается, сбрасываем таймер
                            timerStarted = false;
                        }
                    }
        }
        
    }

    private void StartLesson(int lessonIndex)
    {
        Lesson lesson = lessons[lessonIndex];
        lesson.isPlaying = true;
        
     //   Debug.Log("Включаем баннер");
        lesson.BannerTutorial.SetActive(true);
        
      //  Debug.Log("Запускаем аниматор");
        Animator anim = this.gameObject.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool(lesson.animTutorial, true);
        }

      //  Debug.Log("Включаем звук");
        PlaySound(lesson.SoundTutorial);
    }

    private void PlaySound(AudioClip soundClip)
    {
        if (soundClip != null && audioSource != null)
        {
           
            audioSource.clip = soundClip;
            audioSource.Play();
           // Debug.Log("Звук включен");
        }
    }

    private void EndLesson()
    {
        Lesson lesson = lessons[currentLesson];

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        Animator anim = this.gameObject.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool(lesson.animTutorial, false);
        }
        
        lesson.BannerTutorial.SetActive(false);
        lesson.isPlaying = false;

        // Переходим к следующему уроку
        currentLesson++;

        // Проверяем, есть ли еще уроки
        if (currentLesson < lessons.Length)
        {
            // Запускаем следующий урок
            StartLesson(currentLesson);
        }
        else
        {
            // Уроки закончились, вызываем завершающую функцию
           // Debug.Log("Уроки кончились");
            if (characterActions.tutorialSteps.Length > NomerUroka)
            {
                characterActions.tutorialSteps[NomerUroka].isTaskCompleted = true;
            }
            
            characterActions.OnTutorialsCompleted();
        }
    }

    private IEnumerator StartEndLessonTimer()
    {
        yield return new WaitForSeconds(timerDuration);
        //Debug.Log("4 секунды прошло");
        // Здесь можно запустить EndLesson()
        EndLesson();
    }    
    
    private IEnumerator StartEndLessonAuto()
    {
        // Проверяем, воспроизводится ли звук
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        
        // Проверяем, есть ли еще уроки
        if (currentLesson < lessons.Length)
        {
            // Здесь можно запустить EndLesson()
            EndLesson();
        }
    }

}
