using System;
using UnityEngine;
using BNG;
using Photon.Pun; // Убедитесь, что у вас подключен Photon PUN

//скрипт, который переключает анимации. в него приходят со всех краёв наименования анимаций, которые нужно включить,
//он их включает. И после нажатия на нужную кнопку, возвращает руки на базу
public class HandAnimationController : MonoBehaviour
{
    public ControllerDisplayScript controllerDisplayScript;//скрипт, который перемещает контроллеры к лицу
    private Animator handAnimator;
    private HandPoseBlender handPoseBlender; // Добавление переменной для компонента HandPoseBlender
    
    //всевозможные кнопки
    public ControllerBinding XButtonDown = ControllerBinding.XButtonDown;
    public ControllerBinding YButtonDown = ControllerBinding.YButtonDown;
    public ControllerBinding RightGripDown= ControllerBinding.RightGrip;
    public ControllerBinding RightTriggerDown= ControllerBinding.RightTrigger;
    
    public ControllerBinding AButtonDown= ControllerBinding.AButtonDown;
    public ControllerBinding BButtonDown= ControllerBinding.BButtonDown;
    public ControllerBinding LeftGripDown= ControllerBinding.LeftGrip;
    public ControllerBinding LeftTriggerDown= ControllerBinding.LeftTrigger;

    public string NameTriggerAnim;
    
    public Grabber grab;//рука, которая будет вибрировать (вставлять в неё граббер)
    
    public MassivHandsPlayer leftTargetScript;
    public MassivHandsPlayer rightTargetScript;
    
    private bool canVibrate = true;//вибрация
    private bool HelpVisard = false;//анимация взлёта к лицу

    public RessetObuchHansHUDVibr OffVibr;//сюда добавляем скрипт, который выключает вибрацию, при отключении этого скрипта
    
    /*
    Trigger_Left
    Trigger_Right
    Grab_Left
    Grab_Right
    Trig_Grab_Left
    Trig_Grab_Right
    Button1_Left   - это X
    Button1_Right  - это A
    Button2_Left   - это Y
    Button2_Right  - это B
    Stay
    */
    
    
    
    void Start()
    {
        // Поиск и получение компонента HandPoseBlender
        handPoseBlender = GetComponent<HandPoseBlender>();

        // Если компонент найден, отключаем его
        if (handPoseBlender != null)
        {
            handPoseBlender.enabled = false;
        }
        // Получение компонента аниматора
        handAnimator = GetComponent<Animator>();
    }

    // Вызывается при выключении скрипта или объекта
    void OnDisable()
    {
        // Включение триггера Stay
        SetTrigger("Stay");
        HelpVisard= false;
        canVibrate = false;
        OffVibr.OffVibro();//выключаем вибрацию
    }
    
    
    void OnEnable()
    {
        canVibrate = true;
    }

    // Публичная функция для установки триггера аниматора. Сюда приходит название Анимации, которую хотим включить. Она приходит именно в нужную руку.
    public void SetTrigger(string triggerName)
    {
       
        Debug.Log("Выполнили функцию. в неё пришла анимация");
        Debug.Log(triggerName);
        
        if (handAnimator != null)
        {
            Debug.Log("Обана");
            handAnimator.SetTrigger(triggerName);//типа включаем в аниматоре триггер
            NameTriggerAnim = triggerName;
            Debug.Log(NameTriggerAnim);
        }
    }

    private void Update()
    {
        if (NameTriggerAnim  == "Trigger_Right")
        {
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(RightTriggerDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                    rightTargetScript?.ActivateObject(0);
                    //отменяем повторение вибрации и сбрасываем флаг
                    CancelInvoke(nameof(StartVibration));
                    canVibrate = false;
                    SetTrigger("Stay");
                    if (controllerDisplayScript != null)
                    {
                        controllerDisplayScript.Deactivate();
                    }
                    CancelInvoke();
                    InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                    
            }
        }

        if (NameTriggerAnim  == "Trigger_Left")
        {
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            
            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(LeftTriggerDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
            }
        }
        
        if (NameTriggerAnim  == "Trig_Grab_Left")
        {
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            
            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(LeftTriggerDown) && InputBridge.Instance.GetControllerBindingValue(LeftGripDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
            }
        }
        
        if (NameTriggerAnim  == "Trig_Grab_Right")
        {
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            
            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(RightTriggerDown) && InputBridge.Instance.GetControllerBindingValue(RightGripDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
            }
        }
        
        
        if (NameTriggerAnim  == "Grab_Right")
        {
            
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            
            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(RightGripDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
            }
        }
        
        if (NameTriggerAnim  == "Grab_Left")
        {
            
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            
            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(LeftGripDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
            }
        }
        
        if (NameTriggerAnim  == "Button1_Left")
        {
            
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(XButtonDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
            }
        }
        
        
        
        
        if (NameTriggerAnim  == "Button2_Left")
        {
            
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(YButtonDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
            }
        }
   
        
        
        
        
        
        
        
        
        if (NameTriggerAnim  == "Button1_Right")
        {
            
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(AButtonDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
            }
        }
        
        if (NameTriggerAnim  == "Button2_Right")
        {
            
            //запускаем один раз анимацию у лица
            if (controllerDisplayScript != null && !HelpVisard)
            {
                controllerDisplayScript.Activate();
                HelpVisard = true;
            }

            // Если можем вибрировать, запускаем метод вибрации с задержкой
            if (canVibrate)
            {
                InvokeRepeating(nameof(StartVibration), 0f, 1.5f); // Запуск каждые 1.5 секунды
                canVibrate = false; // Устанавливаем флаг, что вибрация уже начата
            }
            // Проверка нажатия кнопки
            if (InputBridge.Instance.GetControllerBindingValue(BButtonDown))
            {
                //если нажали на кнопку, то сбрасываем контроллеры на стандартные
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
                //отменяем повторение вибрации и сбрасываем флаг
                CancelInvoke(nameof(StartVibration));
                canVibrate = false;
                SetTrigger("Stay");
                CancelInvoke();
                InputBridge.Instance.VibrateController(0f, 0f, 0f, grab.HandSide); //выключаем вибрацию
                if (controllerDisplayScript != null)
                {
                    controllerDisplayScript.Deactivate();
                }
            }
        }
        
    }
    
    // Метод для вибрации
    private void StartVibration()
    {
        InputBridge.Instance.VibrateController(1f, 1f, 0.5f, grab.HandSide);
    }

   public void StopVibration()
    {
        CancelInvoke();
    }
    
    
}