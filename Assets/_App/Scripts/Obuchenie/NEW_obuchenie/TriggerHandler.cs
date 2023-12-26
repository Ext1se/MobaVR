using System;
using System.Collections;
using System.Collections.Generic;
using MobaVR;
using UnityEngine;
using Photon.Pun; // Убедитесь, что у вас подключен Photon PUN

//этот скрипт отсылает ID в скрипт MassivHandsPlayer, который будет включать нужные нам руки. в данном случае контроллеры, при входе в зону книги
//этот стрипт так же находит руку для обучения нажатия клавиш, и включает нужншую анимацию.
public class TriggerHandler : MonoBehaviour
{
    public MassivHandsPlayer leftTargetScript;
    public MassivHandsPlayer rightTargetScript;

    public HandAnimationController triggerRight; //рука с анимациями нажатия на курок
    public HandAnimationController triggerLeft; //рука с анимациями нажатия на курок

    public string NameAnimationRight; //название анимации для правой руки
    public string NameAnimationLeft; //название анимации для левой руки

    public bool RunCollider = false;

    private ClassicGameSession m_GameSession;

    private void OnEnable()
    {
        if (m_GameSession != null)
        {
            m_GameSession.OnAddPlayer += OnAddPlayer;
        }
    }

    private void OnDisable()
    {
        if (m_GameSession != null)
        {
            m_GameSession.OnAddPlayer -= OnAddPlayer;
        }
    }

    private void OnAddPlayer(PlayerVR player)
    {
        if (m_GameSession.LocalPlayer == player)
        {
            FindFunction_New();
            m_GameSession.OnAddPlayer -= OnAddPlayer;
        }
    }

    private void Awake()
    {
        m_GameSession = FindObjectOfType<ClassicGameSession>();
    }

    //как только входим в зону обучения у нас включаются контроллеры 
    void OnTriggerEnter(Collider other)
    {
        //FindFunction();

        //TODO
        if (other.CompareTag("TriggerLocalPlayer"))
        {
            NameAnimationRight = "Trigger_Right";
            NameAnimationLeft = "Stay";
            
            FindFunction_New();
            
            leftTargetScript?.ActivateObject(2);
            rightTargetScript?.ActivateObject(2);
            
            triggerLeft.SetTrigger(NameAnimationLeft);
            triggerRight.SetTrigger(NameAnimationRight);
        }
    }

    //как только входим выключаются
    void OnTriggerExit(Collider other)
    {
        //FindFunction();
        //TODO
        if (other.CompareTag("TriggerLocalPlayer"))
        {
            FindFunction_New();
            
            EndColider();
            leftTargetScript?.ActivateObject(0);
            rightTargetScript?.ActivateObject(0);
        }
    }

    void FindFunction_New()
    {
        if (m_GameSession == null || m_GameSession.LocalPlayer == null)
        {
            return;
        }

        //TODO: update find dependencies
        PlayerVR playerVR = m_GameSession.LocalPlayer;
        Transform leftHand = playerVR.transform.Find("Targets/LeftHand/LeftTarget");
        if (leftHand != null)
        {
            leftHand.TryGetComponent(out leftTargetScript);
            triggerLeft = leftHand.GetComponentInChildren<HandAnimationController>(true);

            if (triggerLeft != null)
            {
                triggerLeft.SetTrigger(NameAnimationLeft);
            }
        }

        Transform rightHand = playerVR.transform.Find("Targets/RightHand/RightTarget");
        if (rightHand != null)
        {
            rightHand.TryGetComponent(out rightTargetScript);

            triggerRight = rightHand.GetComponentInChildren<HandAnimationController>(true);

            if (triggerRight != null)
            {
                triggerRight.SetTrigger(NameAnimationRight);
            }
        }
    }


    void FindFunction()
    {
        // Найти все объекты с компонентом PhotonView
        PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();

        foreach (var photonView in allPhotonViews)
        {
            // Проверяем, принадлежит ли объект текущему игроку
            if (photonView.IsMine)
            {
                // Проверяем имя объекта
                if (photonView.gameObject.name == "LeftTarget")
                {
                    leftTargetScript = photonView.GetComponent<MassivHandsPlayer>();
                }
                else if (photonView.gameObject.name == "RightTarget")
                {
                    rightTargetScript = photonView.GetComponent<MassivHandsPlayer>();
                }
                else if (photonView.gameObject.name == "TriggerRight")
                {
                    triggerRight = photonView.GetComponent<HandAnimationController>();

                    // Отправляем команду для запуска триггера
                    if (triggerRight != null)
                    {
                        triggerRight.GetComponent<HandAnimationController>().SetTrigger(NameAnimationRight);
                    }
                }
                else if (photonView.gameObject.name == "TriggerLeft")
                {
                    triggerLeft = photonView.GetComponent<HandAnimationController>();

                    // Отправляем команду для запуска триггера
                    if (triggerLeft != null)
                    {
                        triggerLeft.GetComponent<HandAnimationController>().SetTrigger(NameAnimationLeft);
                    }
                }
            }
        }
    }


    //включаем помощь для контроллеров нажатине на Х
    public void Button_x()
    {
        leftTargetScript?.ActivateObject(2);
        rightTargetScript?.ActivateObject(2);


        // Отправляем команду для запуска триггера
        if (triggerRight != null)
        {
            NameAnimationRight = "Stay";
            triggerRight.SetTrigger(NameAnimationRight);
        }

        // Отправляем команду для запуска триггера
        if (triggerLeft != null)
        {
            NameAnimationLeft = "Button1_Left";
            triggerLeft.SetTrigger(NameAnimationLeft);
        }
    }


    public void EndColider()
    {
        leftTargetScript?.ActivateObject(2);
        rightTargetScript?.ActivateObject(2);


        // Отправляем команду для запуска триггера
        if (triggerRight != null)
        {
            NameAnimationRight = "Stay";
        }

        // Отправляем команду для запуска триггера
        if (triggerLeft != null)
        {
            NameAnimationLeft = "Stay";
        }
    }
}