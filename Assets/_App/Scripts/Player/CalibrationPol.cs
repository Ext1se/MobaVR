using System;
using BNG;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;

namespace MobaVR
{
    public class CalibrationPol : MonoBehaviourPun
    {
        public BaseGameSession _GameSession;
        [SerializeField] private GameObject _PlayerVR;


        [Header("Calibr traking")]
        //калировка местроположения
        public GameObject Left_Calibr;
        public GameObject Right_Calibr;
        public Transform additionalObject; // Дополнительный объект из инспектора
        public Transform additionalObjectTarget; // Цель для дополнительного объекта из инспектора
        
        
        public GameObject CalibrovkaText; //откалибруйте в первой точке
        public GameObject CalibrovkaText2; //откалибруйте во второй точке
        public GameObject CalibrovkaText3;  //откалибруйте рост
      

        public Transform pointA;
        public Transform pointB;
        public Transform pointC; //левая рука
        public Transform pointD; //правая рука

        public bool calibr;

        public bool step_1; //первый раз контроллер поставили

        //----

        [Header("Calibr rost")]
        //Калибровка роста
        public VRIK[] local_Skins; //тело игрока, которое нужно увеличит
        //public GameObject _Left_ruka;
        //public GameObject _Right_ruka;
        public rost _rost; //скрипт смены роста у сетевой модельки
        public bool _kalibr_ruka;
        public float scaleMlp = 1f;

        public float x;
        public float y;
        public float z;

        public float _rostPlayer;
        public GameObject _CenterEyeAnchor;
        //переменная скрипта калибровки
        // public GameObject _CALIBR;
        //переменная надписи Калибровка
        // public GameObject CalibrovkaText;
        public bool runRost;

        private float requiredHoldTime = 2f; // Время удержания в секундах
        private float buttonHoldTimer = 0f; //для таймера


        //логика зажатия
        private bool isButtonCalibrPressed = false;
        private bool isButtonCalibrBPressed = false;
        private bool isCalibrating = false;
        private bool areButtonsReleased = true;


        public ControllerBinding Button_Calibr = ControllerBinding.AButton; //нажатие
        public ControllerBinding Button_Calibr_B = ControllerBinding.BButton; //нажатие

        private void Start()
        {
            if (_GameSession == null)
            {
                _GameSession = FindObjectOfType<BaseGameSession>();
            }
            
            step_1 = false;
            calibr = false;
            // CalibrovkaText.SetActive(true);


            runRost = false;
            _kalibr_ruka = false;
        }
        
        
        private void OnEnable()
        {
            if (CalibrovkaText != null && CalibrovkaText2 != null && CalibrovkaText3 != null) 
            {
                CalibrovkaText.SetActive(true);
                CalibrovkaText2.SetActive(false);
                CalibrovkaText3.SetActive(false);
               
            }
        }
        


        private void Update()
        {
            //калибровка
            if (calibr == false)
            {
                //если зажата кнопка 
                if (InputBridge.Instance.GetControllerBindingValue(Button_Calibr) &&
                    InputBridge.Instance.GetControllerBindingValue(Button_Calibr_B))
                {
                    isButtonCalibrPressed = true;
                }
                else
                {
                    isButtonCalibrPressed = false;
                }

                //если мы включали первый раз кнопки, но теперь они опущены
                if (isCalibrating && !isButtonCalibrPressed)
                {
                    areButtonsReleased = true;
                }

                //если ещё не зажимали кнопки и зажаты обе кнопки
                if (!isCalibrating && isButtonCalibrPressed && areButtonsReleased)
                {
                    //калибруем по первой точке
                    isCalibrating = true;
                    areButtonsReleased = false;

                    Left_Calibr = GameObject.Find("Left_Calibr");
                    Right_Calibr = GameObject.Find("Right_Calibr");
                    pointA = Left_Calibr.transform;
                    pointB = Right_Calibr.transform;
                    MoveToPointA();
                }

                //калибруем вторую точку
                if (isCalibrating && isButtonCalibrPressed && areButtonsReleased)
                {
                    isCalibrating = false;
                    areButtonsReleased = false;

                    RotateAroundPointC();

                    if (CalibrovkaText != null)
                    {
                        CalibrovkaText.SetActive(false);
                    }

                    calibr = true;
                    
                    
                    if (CalibrovkaText != null && CalibrovkaText2 != null && CalibrovkaText3 != null) 
                    {
                        CalibrovkaText.SetActive(false);
                        CalibrovkaText2.SetActive(false);
                        CalibrovkaText3.SetActive(true);
                      
                    }
                    
                    
                    
                }
            }
            else if (calibr == true)
            {
                isCalibrating = false;
                areButtonsReleased = true;

                //возвращаем точку вращения на место к правому контроллеру
                RessetCalibr();
            }

            //рост

            //сли зажато 2 клавиши
            if (InputBridge.Instance.GetControllerBindingValue(Button_Calibr) &&
                InputBridge.Instance.GetControllerBindingValue(Button_Calibr_B))
            {
                buttonHoldTimer += Time.deltaTime;

                // Проверка, достигнуто ли требуемое время удержания
                if (buttonHoldTimer >= requiredHoldTime)
                {
                    kalibr_rost();
                }
            }
            else
            {
                buttonHoldTimer = 0f; // Обнуляем таймер
            }
        }

        private void MoveToPointA()
        {
            // вычисляем вектор разницы между текущей позицией точки С и точкой А
            Vector3 deltaPosition = pointA.position - pointC.position;

            // перемещаем родительский объект на вектор deltaPosition
            transform.position += deltaPosition;

            // перемещаем дополнительный объект к его цели нужно для калибровки одним контроллером, этот объект имитирует второй контроллер
            if (additionalObject != null && additionalObjectTarget != null)
            {
                additionalObject.SetParent(
                    additionalObjectTarget); // Устанавливаем additionalObject как дочерний объект для additionalObjectTarget
            }
            
                        
            if (CalibrovkaText != null && CalibrovkaText2 != null && CalibrovkaText3 != null) 
            {
                CalibrovkaText.SetActive(false);
                CalibrovkaText2.SetActive(true);
                CalibrovkaText3.SetActive(false);
               
            }
            
        }

        private void RotateAroundPointC()
        {
            // вычисляем вектор поворота для поворота родительского объекта вокруг точки С
            Vector3 rotationVector = pointB.position - pointA.position;
            Quaternion rotation = Quaternion.FromToRotation(pointD.position - pointC.position, rotationVector);

            // сохраняем текущее расстояние между родителем и точками С и Д
            float distance = Vector3.Distance(transform.position, pointC.position);

            // поворачиваем родительский объект вокруг точки С
            transform.rotation = rotation * transform.rotation;

            //возвращаем наклон к 0, чтобы вращался только по оси У
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            // вычисляем вектор разницы между текущей позицией точки С и точкой А
            Vector3 deltaPosition2 = pointB.position - pointD.position;

            // перемещаем родительский объект на вектор deltaPosition
            transform.position += deltaPosition2;
        }

        private void RessetCalibr()
        {
            //возвращаем точку вращения на место
            additionalObject.SetParent(pointD); // Устанавливаем additionalObject как дочерний объект для правой руки

            // Устанавливаем локальную позицию additionalObject в ноль
            additionalObject.localPosition = Vector3.zero;
        }

        [ContextMenu("Calibration Rost")]
        public void kalibr_rost()
        {
            /*
            _PlayerVR = _GameSession.Player;

            _rostPlayer = _CenterEyeAnchor.transform.position.y;
            _rostPlayer += 0.1f;

            if (_PlayerVR.TryGetComponent(out rost rost))
            {
                rost.SetHeight(_rostPlayer);
            }
            */

            _rostPlayer = _CenterEyeAnchor.transform.position.y;
            _rostPlayer += 0.1f;

            _rost.SetHeight(_rostPlayer);
            
            
            if (CalibrovkaText != null && CalibrovkaText2 != null && CalibrovkaText3 != null) 
            {
                CalibrovkaText.SetActive(false);
                CalibrovkaText2.SetActive(false);
                CalibrovkaText3.SetActive(false);
               
            }
            
            
            
        }
        
        
        //PUBLIC
        public void ResetCalibration()
        {
            photonView.RPC(nameof(RpcResetCalibration), RpcTarget.All);
        }

        [PunRPC]
        private void RpcResetCalibration()
        {
            calibr = false;
        }
        
        public void SetHeight()
        {
            photonView.RPC(nameof(RpcSetHeight), RpcTarget.All);
        }

        [PunRPC]
        private void RpcSetHeight()
        {
            if (photonView.IsMine)
            {
                kalibr_rost();
            }
        }
    }
}