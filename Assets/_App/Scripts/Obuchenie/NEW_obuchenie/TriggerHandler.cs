using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Убедитесь, что у вас подключен Photon PUN

//этот скрипт отсылает ID в скрипт MassivHandsPlayer, который будет включать нужные нам руки. в данном случае контроллеры, при входе в зону книги
//этот стрипт так же находит руку для обучения нажатия клавиш, и включает нужншую анимацию.
public class TriggerHandler : MonoBehaviour
{
   public MassivHandsPlayer leftTargetScript;
    public MassivHandsPlayer rightTargetScript;

    public HandAnimationController triggerRight;//рука с анимациями нажатия на курок
    public HandAnimationController triggerLeft;//рука с анимациями нажатия на курок

    public string NameAnimationRight;//название анимации для правой руки
    public string NameAnimationLeft;//название анимации для левой руки

    public bool RunCollider = false;
    

    //как только входим в зону обучения у нас включаются контроллеры 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TriggerLocalPlayer"))
        {
            Start_Trigger();//Если зашёл в коллайдер, то говорим включить показ триггера
        }
    }

    //как только входим выключаются
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TriggerLocalPlayer"))
            {
                //Если выходим из коллайдера, то выключаем показ контроллеров
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);

            }
    }

    
    void FindFunction()
    {
        // Найти все объекты с тегом LeftTargetScript и проверить их PhotonView
        foreach (GameObject leftTarget in GameObject.FindGameObjectsWithTag("LeftTargetScript"))
        {
            PhotonView photonView = leftTarget.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                leftTargetScript = leftTarget.GetComponent<MassivHandsPlayer>();
            }
        }

        // Найти все объекты с тегом RightTargetScript и проверить их PhotonView
        foreach (GameObject rightTarget in GameObject.FindGameObjectsWithTag("RightTargetScript"))
        {
            PhotonView photonView = rightTarget.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                rightTargetScript = rightTarget.GetComponent<MassivHandsPlayer>();
            }
        }

        //Включаем наши контроллеры
        leftTargetScript?.ActivateObject(2);
        rightTargetScript?.ActivateObject(2);
        
        
        // Аналогично для TriggerRight и TriggerLeft
        foreach (GameObject trigger in GameObject.FindGameObjectsWithTag("TriggerRight"))
        {
            PhotonView photonView = trigger.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                triggerRight = trigger.GetComponent<HandAnimationController>();
            }
        }

        foreach (GameObject trigger in GameObject.FindGameObjectsWithTag("TriggerLeft"))
        {
            PhotonView photonView = trigger.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                triggerLeft = trigger.GetComponent<HandAnimationController>();
            }
        }
    }

    
    
        
    public void Start_Trigger()
    {
        
        NameAnimationRight = "Trigger_Right";
        NameAnimationLeft = "Stay";

        if (leftTargetScript == null && rightTargetScript == null && triggerRight == null && triggerLeft == null)
        {
            FindFunction();
            triggerRight.SetTrigger(NameAnimationRight);
            triggerLeft.SetTrigger(NameAnimationLeft);
        }
        else
        {
            leftTargetScript?.ActivateObject(2);
            rightTargetScript?.ActivateObject(2);

            // Отправляем команду для запуска триггера
            if (triggerRight != null)
            {
                triggerRight.SetTrigger(NameAnimationRight);
            }
            // Отправляем команду для запуска триггера
            if (triggerLeft != null)
            {
                triggerLeft.SetTrigger(NameAnimationLeft);
            }
        }
    }
    
    
    
    
    public void Button_x()
    {
        
        NameAnimationRight = "Stay";
        NameAnimationLeft = "Button1_Left";

        if (leftTargetScript == null && rightTargetScript == null && triggerRight == null && triggerLeft == null)
        {
            FindFunction();
            triggerRight.SetTrigger(NameAnimationRight);
            triggerLeft.SetTrigger(NameAnimationLeft);
        }
        else
        {
            leftTargetScript?.ActivateObject(2);
            rightTargetScript?.ActivateObject(2);

            // Отправляем команду для запуска триггера
            if (triggerRight != null)
            {
                triggerRight.SetTrigger(NameAnimationRight);
            }
            // Отправляем команду для запуска триггера
            if (triggerLeft != null)
            {
                triggerLeft.SetTrigger(NameAnimationLeft);
            }
        }
    }
    
    
    
    

}
