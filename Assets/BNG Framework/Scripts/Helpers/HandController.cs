using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// An example hand controller that sets animation values depending on Grabber state
    /// </summary>
    public class HandController : MonoBehaviour {

        [Tooltip("HandController parent will be set to this on Start if specified")]
        public Transform HandAnchor;// Публичная переменная для установки родителя контроллера руки

        [Tooltip("If true, this transform will be parented to HandAnchor and it's position / rotation set to 0,0,0.")]
        public bool ResetHandAnchorPosition = true;// Флаг для сброса позиции и поворота руки

        public Animator HandAnimator; // Аниматор руки для управления анимациями

        [Tooltip("(Optional) If specified, this HandPoser can be used when setting poses retrieved from a grabbed Grabbable.")]
        public HandPoser handPoser;// Опциональный компонент HandPoser для установки поз руки

        [Tooltip("(Optional) If specified, this AutoPoser component can be used when if set on the Grabbable, or if AutoPose is set to true")]
        public AutoPoser autoPoser;// Опциональный компонент AutoPoser для автоматической позиции руки

        // Компонент для смешивания поз руки между открытой и закрытой позицией
        HandPoseBlender poseBlender;

        [Tooltip("How to handle the hand when nothing is being grabbed / idle. Ex : Can use an Animator to control the hand via blending, a HandPoser to control via blend states, AutoPoser to continually auto pose while nothing is being held, or 'None' if you want to handle the idle state yourself.")]
        public HandPoserType IdlePoseType = HandPoserType.HandPoser;// Тип обработки позы руки в режиме ожидания

        [Tooltip("If true, the idle hand pose will be determined by the connected Valve Index Controller's finger tracking. Requires the SteamVR SDK. Make sure IdlePoseType is set to 'HandPoser'")]
        public bool UseIndexFingerTracking = true;// Использовать ли трекинг пальцев от контроллера Valve Index

        /// <summary>
        /// How fast to Lerp the Layer Animations
        /// </summary>
        [Tooltip("How fast to Lerp the Layer Animations")]
        public float HandAnimationSpeed = 20f;// Скорость анимации руки

        [Tooltip("Check the state of this grabber to determine animation state. If null, a child Grabber component will be used.")]
        public Grabber grabber; // Компонент Grabber для управления захватом объектов

        [Header("Shown for Debug : ")]
        /// <summary>
        /// 0 = Open Hand, 1 = Full Grip
        /// </summary>
        public float GripAmount;// Степень сжатия руки (0 - открыта, 1 - закрыта)
        private float _prevGrip;// Предыдущее значение сжатия руки (для интерполяции)

        /// <summary>
        /// 0 = Index Curled in,  1 = Pointing Finger
        /// </summary>
        public float PointAmount;// Степень вытягивания указательного пальца (0 - согнут, 1 - вытянут)
        private float _prevPoint; // Предыдущее значение вытягивания указательного пальца (для интерполяции)

        /// <summary>
        /// 0 = Thumb Down, 1 = Thumbs Up
        /// </summary>
        public float ThumbAmount;// Степень поднятия большого пальца (0 - вниз, 1 - вверх)
        private float _prevThumb; // Предыдущее значение поднятия большого пальца (для интерполяции)
        
        // Raw input values
        private bool _thumbIsNear = false;// Близко ли большой палец к контроллеру
        private bool _indexIsNear = false; // Близко ли указательный палец к контроллеру
        private float _triggerValue = 0f;// Значение триггера на контроллере
        private float _gripValue = 0f;// Значение захвата на контроллере

        public int PoseId; // Идентификатор позы руки
        
        // Дополнительные приватные переменные для управления смещением и вводом
        ControllerOffsetHelper offset;
        InputBridge input;
        Rigidbody rigid;
        Transform offsetTransform;

        // Свойства для получения смещенных значений позиции и поворота
        Vector3 offsetPosition {
            get {
                if(offset) {
                    return offset.OffsetPosition;
                }
                return Vector3.zero;
            }
        }

        Vector3 offsetRotation {
            get {
                if (offset) {
                    return offset.OffsetRotation;
                }
                return Vector3.zero;
            }
        }

        void Start() {
            // Получение компонентов Rigidbody, ControllerOffsetHelper и создание нового объекта для управления смещением
            rigid = GetComponent<Rigidbody>();
            offset = GetComponent<ControllerOffsetHelper>();
            offsetTransform = new GameObject("OffsetHelper").transform;
            offsetTransform.parent = transform;

            // Установка родительского объекта для руки и смещение, если указано
            if (HandAnchor) {
                transform.parent = HandAnchor;
                offsetTransform.parent = HandAnchor;

                if (ResetHandAnchorPosition) {
                    transform.localPosition = offsetPosition;
                    transform.localEulerAngles = offsetRotation;
                }
            }
            // Поиск компонента Grabber, если он не указан
            if(grabber == null) {
                grabber = GetComponentInChildren<Grabber>();
            }

            // Подписка на события захвата и освобождения
            if(grabber != null) {
                grabber.onAfterGrabEvent.AddListener(OnGrabberGrabbed);
                grabber.onReleaseEvent.AddListener(OnGrabberReleased);
            }

            // Попытка получения дочернего аниматора
            SetHandAnimator();
            // Получение экземпляра InputBridge для ввода данных
            input = InputBridge.Instance;
        }

        public void Update() {
            // Проверка изменения состояния захвата
            CheckForGrabChange();

            // Обновление состояний руки в соответствии с вводом
            UpdateFromInputs();
            
            // Обновление состояний в зависимости от того, держится ли что-то в руке
            if(HoldingObject()) {
                UpdateHeldObjectState();
            }
            else {
                UpdateIdleState();
            }
        }

        // Обновление состояний объекта, удерживаемого в руке
        public virtual void UpdateHeldObjectState() {
            // Если удерживаемый объект использует аниматор, обновить анимационные состояния
            if (IsAnimatorGrabbable()) {
                UpdateAnimimationStates();
            }
            // Если удерживаемый объект использует HandPoser, обновить HandPoser
            else if (IsHandPoserGrabbable()) {                
                UpdateHandPoser();
            }
            // Если удерживаемый объект использует AutoPoser, обновить AutoPoser
            else if (IsAutoPoserGrabbable()) {
                //EnableAutoPoser();
            }
        }

        // Обновление состояний покоя
        public virtual void UpdateIdleState() {
            // Если ничего не удерживается, обновить состояние покоя
            if (IdlePoseType == HandPoserType.Animator) {
                UpdateAnimimationStates();
            }
            else if (IdlePoseType == HandPoserType.HandPoser) {
                //UpdateHandPoser();
                UpdateHandPoserIdleState();

            }
            else if (IdlePoseType == HandPoserType.AutoPoser) {
                EnableAutoPoser(true);
            }
        }
        // Предыдущий удерживаемый объект для отслеживания изменений захвата
        public GameObject PreviousHeldObject;
        
        // Проверка, удерживается ли что-то в руке
        public virtual bool HoldingObject() {

            if(grabber != null && grabber.HeldGrabbable != null) {
                return true;
            }

            return false;
        }
        // Проверка на изменение захвата
        public virtual void CheckForGrabChange() {
            if(grabber != null) {

                // Проверка на отсутствие объекта, но включенный аниматор
                if(grabber.HeldGrabbable == null && PreviousHeldObject != null) {                    
                    OnGrabDrop();
                }
                // Проверка на изменение удерживаемого объекта
                else if(grabber.HeldGrabbable != null && !GameObject.ReferenceEquals(grabber.HeldGrabbable.gameObject, PreviousHeldObject)) {
                    OnGrabChange(grabber.HeldGrabbable.gameObject);
                }
            }
        }

        // Вызывается при изменении захвата
        public virtual void OnGrabChange(GameObject newlyHeldObject) {

            // Обновление состояний компонентов, если удерживаемый объект изменился
            if(HoldingObject()) {

                // Переключение компонентов в зависимости от свойств удерживаемого объекта
                // Аниматор
                if (grabber.HeldGrabbable.handPoseType == HandPoseType.AnimatorID) {
                    EnableHandAnimator();
                }
                // Auto Poser - Once
                else if (grabber.HeldGrabbable.handPoseType == HandPoseType.AutoPoseOnce) {
                    EnableAutoPoser(false);
                }
                // Auto Poser - Continuous
                else if (grabber.HeldGrabbable.handPoseType == HandPoseType.AutoPoseContinuous) {
                    EnableAutoPoser(true);
                }
                // Hand Poser
                else if (grabber.HeldGrabbable.handPoseType == HandPoseType.HandPose) {
                    // Если у нас есть допустимая поза руки, используйте ее, в противном случае вернитесь к стандартной закрытой позе
                    if (grabber.HeldGrabbable.SelectedHandPose != null) {
                        EnableHandPoser();

                        // Убедитесь, что смешивание не активно
                        if(poseBlender != null) {
                            poseBlender.UpdatePose = false;
                        }

                        if(handPoser != null) {
                            handPoser.CurrentPose = grabber.HeldGrabbable.SelectedHandPose;
                        }
                    }
                    else {
                        // Debug.Log("No Selected Hand Pose was found.");
                    }
                }
            }

            PreviousHeldObject = newlyHeldObject;
        }

        // Вызывается, когда удерживаемый объект отпущен - в руках ничего нет
        /// <summary>
        /// Dropped our held item - nothing currently in our hands
        /// </summary>
        public virtual void OnGrabDrop() {

            // Использовать автоматическую позу, когда в руке ничего нет?
            if (IdlePoseType == HandPoserType.AutoPoser) {
                EnableAutoPoser(true);
            }
            else if (IdlePoseType == HandPoserType.HandPoser) {
                DisableAutoPoser();
            }
            else if (IdlePoseType == HandPoserType.Animator) {
                DisablePoseBlender();
                EnableHandAnimator();
                DisableAutoPoser();
            }

            PreviousHeldObject = null;
        }       

        // Установка аниматора руки
        public virtual void SetHandAnimator() {
            if (HandAnimator == null || !HandAnimator.gameObject.activeInHierarchy) {
                HandAnimator = GetComponentInChildren<Animator>();
            }
        }

        /// <summary>
        /// Update GripAmount, PointAmount, and ThumbAmount based raw input from InputBridge
        /// </summary>
        public virtual void UpdateFromInputs() {

            // Grabber может быть деактивирован
            if (grabber == null || !grabber.isActiveAndEnabled) {
                grabber = GetComponentInChildren<Grabber>();
                GripAmount = 0;
                PointAmount = 0;
                ThumbAmount = 0;
                return;
            }

            // Обновление необработанных значений в зависимости от стороны руки
            if (grabber.HandSide == ControllerHand.Left) {
                _indexIsNear = input.LeftTriggerNear;
                _thumbIsNear = input.LeftThumbNear;
                _triggerValue = input.LeftTrigger;
                _gripValue = input.LeftGrip;
            }
            else if (grabber.HandSide == ControllerHand.Right) {
                _indexIsNear = input.RightTriggerNear;
                _thumbIsNear = input.RightThumbNear;
                _triggerValue = input.RightTrigger;
                _gripValue = input.RightGrip;
            }

            // Массажирование необработанных значений для получения лучшего набора значений, которые аниматор может использовать
            GripAmount = _gripValue;
            ThumbAmount = _thumbIsNear ? 0 : 1;

            // Количество точек может изменяться в зависимости от касания или нашего источника ввода
            PointAmount = 1 - _triggerValue;// Диапазон от 0 до 1. 1 == палец полностью вытянут
            PointAmount *= InputBridge.Instance.InputSource == XRInputSource.SteamVR ? 0.25F : 0.5F;// Уменьшить количество, когда палец указывает, если Oculus или XRInput

            // Если палец не находится у спускового крючка, вытяните палец
            if (input.SupportsIndexTouch && _indexIsNear == false && PointAmount != 0) {
                PointAmount = 1f;
            }
            // Не поддерживает касание, вытяните палец, как если бы он указывал, если спусковой крючок не найден
            else if (!input.SupportsIndexTouch && _triggerValue == 0) {
                PointAmount = 1;
            }
        }

        // Определение, следует ли обновлять анимационные состояния
        public bool DoUpdateAnimationStates = true;
        // Определение, следует ли обновлять HandPoser
        public bool DoUpdateHandPoser = true;

        // Обновление анимационных состояний каждый кадр
        public virtual void UpdateAnimimationStates()
        {
            if(DoUpdateAnimationStates == false) {
                return;
            }

            // Включение аниматора, если он был отключен HandPoser
            if(IsAnimatorGrabbable() && !HandAnimator.isActiveAndEnabled) {
                EnableHandAnimator();
            }

            // Обновление информации аниматора руки
            if (HandAnimator != null && HandAnimator.isActiveAndEnabled && HandAnimator.runtimeAnimatorController != null) {

                _prevGrip = Mathf.Lerp(_prevGrip, GripAmount, Time.deltaTime * HandAnimationSpeed);
                _prevThumb = Mathf.Lerp(_prevThumb, ThumbAmount, Time.deltaTime * HandAnimationSpeed);
                _prevPoint = Mathf.Lerp(_prevPoint, PointAmount, Time.deltaTime * HandAnimationSpeed);

                // 0 = открытая рука, 1 = полный захват                          
                HandAnimator.SetFloat("Flex", _prevGrip);

                // 0 = большой палец вниз, 1 = большой палец вверх
                HandAnimator.SetLayerWeight(1, _prevThumb);

                //// 0 = указательный палец внутрь, 1 = указывает наружу    
                //// Точка воспроизводится как смесь
                //// Рядом с курком? Нажмите палец вниз немного
                HandAnimator.SetLayerWeight(2, _prevPoint);

                // Должны ли мы использовать пользовательскую позу руки?
                if (grabber != null && grabber.HeldGrabbable != null) {
                    HandAnimator.SetLayerWeight(0, 0);
                    HandAnimator.SetLayerWeight(1, 0);
                    HandAnimator.SetLayerWeight(2, 0);

                    PoseId = (int)grabber.HeldGrabbable.CustomHandPose;

                    if (grabber.HeldGrabbable.ActiveGrabPoint != null) {

                        // Установите сжатие по умолчанию в 1 при удержании предмета
                        HandAnimator.SetLayerWeight(0, 1);
                        HandAnimator.SetFloat("Flex", 1);

                        // Получить минимальное / максимальное значение нашего смешивания пальцев, если оно установлено пользователем
                        // Это позволяет позе смешиваться между состояниями
                        // Указательный палец
                        setAnimatorBlend(grabber.HeldGrabbable.ActiveGrabPoint.IndexBlendMin, grabber.HeldGrabbable.ActiveGrabPoint.IndexBlendMax, PointAmount, 2);

                        // Большой палец
                        setAnimatorBlend(grabber.HeldGrabbable.ActiveGrabPoint.ThumbBlendMin, grabber.HeldGrabbable.ActiveGrabPoint.ThumbBlendMax, ThumbAmount, 1);                       
                    }
                    else {
                        // Если нет текущей точки захвата, убедитесь, что слои возвращены в стандартное состояние
                        if (grabber.HoldingItem) {
                            GripAmount = 1;
                            PointAmount = 0;
                            ThumbAmount = 0;
                        }
                    }
                    // Это позволяет нам использовать разные слои для нашего захвата, если необходимо
                    HandAnimator.SetInteger("Pose", PoseId);
                    
                }
                else {
                    HandAnimator.SetInteger("Pose", 0);
                }
            }
        }

        void setAnimatorBlend(float min, float max, float input, int animationLayer) {
            HandAnimator.SetLayerWeight(animationLayer, min + (input) * max - min);
        }

        /// <summary>
        /// Returns true if there is a valid animator and the held grabbable is set to use an Animation ID
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAnimatorGrabbable() {
            return HandAnimator != null && grabber != null && grabber.HeldGrabbable != null && grabber.HeldGrabbable.handPoseType == HandPoseType.AnimatorID;
        }

        public GameObject specifiedObject;//переменная. в которой будем искать дочерние объекты с handPoser
        
        
        // Обновление состояния HandPoser
        public virtual void UpdateHandPoser() {

            if (DoUpdateHandPoser == false) {
                return;
            }

            /*//если ханд позер равен нулю, то ищем в дочерних элементах объект, у которого его есть  HandPoser
            // HandPoser may have changed - check for new component
            if (handPoser == null || !handPoser.isActiveAndEnabled) {
                handPoser = GetComponentInChildren<HandPoser>();
            }    */               
            
            
            // Если handPoser равен нулю, то ищем в дочерних элементах определенного объекта, указанного через инспектор
            if (handPoser == null || !handPoser.isActiveAndEnabled) {
                if (specifiedObject != null) {
                    handPoser = specifiedObject.GetComponentInChildren<HandPoser>();
                    HandAnimator = specifiedObject.GetComponentInChildren<Animator>();
                } 
            }
            
            // Bail early if missing any info
            if(handPoser == null || grabber == null || grabber.HeldGrabbable == null || grabber.HeldGrabbable.handPoseType != HandPoseType.HandPose) {
                return;
            }

            // Make sure blending isn't active
            if(poseBlender != null && poseBlender.UpdatePose) {
                poseBlender.UpdatePose = false;
            }

            // Update hand pose if changed
            if (handPoser.CurrentPose == null || handPoser.CurrentPose != grabber.HeldGrabbable.SelectedHandPose) {
                UpdateCurrentHandPose();
            }
        }

        public virtual bool IsHandPoserGrabbable() {
            return handPoser != null && grabber != null && grabber.HeldGrabbable != null && grabber.HeldGrabbable.handPoseType == HandPoseType.HandPose;
        }

        public virtual void UpdateHandPoserIdleState() {

            // Makde sure animator isn't firing while we do our idle state
            DisableHandAnimator();

            // Check if we need to set up the pose blender
            if(!SetupPoseBlender()) {
                // If Pose Blender couldn't be setup we should just exit
                return;
            }

            // Make sure poseBlender updates the pose
            poseBlender.UpdatePose = true;

            // Check for Valve Index Knuckles finger tracking
            if (UseIndexFingerTracking && InputBridge.Instance.IsValveIndexController) {
                UpdateIndexFingerBlending();
                return;
            }

            // Update pose blender depending on inputs from controller
            // Thumb near can be counted as 'thumbTouch', primaryTouch, secondaryTouch, or primary2DAxisTouch (such as on knuckles controller)
            poseBlender.ThumbValue = Mathf.Lerp(poseBlender.ThumbValue, _thumbIsNear ? 1 : 0, Time.deltaTime * handPoser.AnimationSpeed);

            // Use Trigger for Index Finger
            float targetIndexValue = _triggerValue;

            // If the index finger is on the trigger we can bring the finger in a bit
            if (targetIndexValue < 0.1f && _indexIsNear) {
                targetIndexValue = 0.1f;
            }
            poseBlender.IndexValue = Mathf.Lerp(poseBlender.IndexValue, targetIndexValue, Time.deltaTime * handPoser.AnimationSpeed);

            // Grip
            poseBlender.GripValue = _gripValue;
        }



        public virtual void UpdateIndexFingerBlending() {
#if STEAM_VR_SDK
            if (grabber.HandSide == ControllerHand.Left) {
                poseBlender.IndexValue = InputBridge.Instance.LeftIndexCurl;
                poseBlender.ThumbValue = InputBridge.Instance.LeftThumbCurl;
                poseBlender.MiddleValue = InputBridge.Instance.LeftMiddleCurl;
                poseBlender.RingValue = InputBridge.Instance.LeftRingCurl;
                poseBlender.PinkyValue = InputBridge.Instance.LeftPinkyCurl;
            }
            else if (grabber.HandSide == ControllerHand.Right) {
                poseBlender.IndexValue = InputBridge.Instance.RightIndexCurl;
                poseBlender.ThumbValue = InputBridge.Instance.RightThumbCurl;
                poseBlender.MiddleValue = InputBridge.Instance.RightMiddleCurl;
                poseBlender.RingValue = InputBridge.Instance.RightRingCurl;
                poseBlender.PinkyValue = InputBridge.Instance.RightPinkyCurl;
            }
#endif
        }

        public virtual bool SetupPoseBlender() {

            // Make sure we have a valid handPoser to work with
            if(handPoser == null || !handPoser.isActiveAndEnabled) {
                //handPoser = GetComponentInChildren<HandPoser>(false);
                if (specifiedObject != null) {
                    handPoser = specifiedObject.GetComponentInChildren<HandPoser>();
                    
                    //SetHandAnimator();//заодно проверяем на аниматор
                    HandAnimator = specifiedObject.GetComponentInChildren<Animator>();
                    
                    
                    
                } 
                
                
            }
            
            
            
            

            // No HandPoser is found, we should just exit
            if (handPoser == null) {
                return false;
                // Debug.Log("Adding Hand Poser to " + transform.name);
                // handPoser = this.gameObject.AddComponent<HandPoser>();
            }

           
            //Если pose blender не найден, добавьте его и настройте так, чтобы мы могли использовать его в Update()
            if (poseBlender == null || !poseBlender.isActiveAndEnabled)
            {
                //Debug.Log("переменная poseBlender == null");
                poseBlender = specifiedObject.GetComponentInChildren<HandPoseBlender>();
                // Debug.Log("Назначили этой переменной компонент HandPoseBlender");
            }
 
            // Если pose blender не найден, добавьте его и настройте так, чтобы мы могли использовать его в Update()
            if (poseBlender == null) {
                if(handPoser != null) {
                    //  Debug.Log("У нас есть handPoser, теперь добавим к нему HandPoseBlender");
                    poseBlender = handPoser.gameObject.AddComponent<HandPoseBlender>();
                }
                else {
                    //  Debug.Log("У нас нет handPoser, теперь добавим к нему HandPoseBlender");
                    poseBlender = this.gameObject.AddComponent<HandPoseBlender>();
                }

                // Не обновляйте позу в обновлении, так как мы будем контролировать это сами
                poseBlender.UpdatePose = false;

                // Настройте наложение так, чтобы использовать некоторые позы по умолчанию
                poseBlender.Pose1 = GetDefaultOpenPose();
                poseBlender.Pose2 = GetDefaultClosedPose();
            }

            return true;
        }

        public virtual HandPose GetDefaultOpenPose() {
            return Resources.Load<HandPose>("Open");
        }

        public virtual HandPose GetDefaultClosedPose() {
            return Resources.Load<HandPose>("Closed");
        }

        public virtual void EnableHandPoser() {
            // Disable the hand animator if we have a valid hand pose to use
            if(handPoser != null) {
                // Just need to make sure animator isn't enabled
                DisableHandAnimator();
            }
        }

        public virtual void EnableAutoPoser(bool continuous) {

            // Check if AutoPoser was set
            if (autoPoser == null || !autoPoser.gameObject.activeInHierarchy) {

                if(handPoser != null) {
                    autoPoser = handPoser.GetComponent<AutoPoser>();
                }
                // Check for active children
                else {
                    autoPoser = GetComponentInChildren<AutoPoser>(false);
                }
            }

            // Do the auto pose
            if (autoPoser != null) {
                autoPoser.UpdateContinuously = continuous;

                if(!continuous) {
                    autoPoser.UpdateAutoPoseOnce();
                }

                DisableHandAnimator();

                // Disable pose blending updates
                DisablePoseBlender();
            }
        }

        public virtual void DisablePoseBlender() {
            if (poseBlender != null) {
                poseBlender.UpdatePose = false;
            }
        }

        public virtual void DisableAutoPoser() {
            if (autoPoser != null) {
                autoPoser.UpdateContinuously = false;
            }
        }

        public virtual bool IsAutoPoserGrabbable() {
            return autoPoser != null && grabber != null && grabber.HeldGrabbable != null && (grabber.HeldGrabbable.handPoseType == HandPoseType.AutoPoseOnce || grabber.HeldGrabbable.handPoseType == HandPoseType.AutoPoseContinuous);
        }

        public virtual void EnableHandAnimator() {
            if (HandAnimator != null && HandAnimator.enabled == false) {
                HandAnimator.enabled = true;
            }

            // If using a hand poser reset the currennt pose so it can be set again later
            if(handPoser != null) {
                handPoser.CurrentPose = null;
            }
        }

        public virtual void DisableHandAnimator() {
            if (HandAnimator != null && HandAnimator.enabled) {
                HandAnimator.enabled = false;
            }
        }

        public virtual void OnGrabberGrabbed(Grabbable grabbed) {
            // Set the Hand Pose on our component
            if (grabbed.SelectedHandPose != null) {
                UpdateCurrentHandPose();
            }
            else if(grabbed.handPoseType == HandPoseType.HandPose && grabbed.SelectedHandPose == null) {
                // Debug.Log("No HandPose selected for object '" + grabbed.transform.name + "'. Falling back to default hand pose.");

                // Fall back to the closed pose if no selected hand pose was found
                grabbed.SelectedHandPose = GetDefaultClosedPose();
                UpdateCurrentHandPose();
            }
        }

        public virtual void UpdateCurrentHandPose() {
            if(handPoser != null) {
                // Update the pose
                handPoser.CurrentPose = grabber.HeldGrabbable.SelectedHandPose;
                handPoser.OnPoseChanged();
            }
        }

        public virtual void OnGrabberReleased(Grabbable released) {
            OnGrabDrop();
        }
    }
    
    public enum HandPoserType {
        HandPoser,
        Animator,
        AutoPoser,
        None
    }
}