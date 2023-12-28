using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Убедитесь, что у вас подключен Photon PUN

//этот скрипт отсылает ID в скрипт MassivHandsPlayer, который будет включать нужные нам руки. в данном случае контроллеры, при входе в зону книги
//этот стрипт так же находит руку для обучения нажатия клавиш, и включает нужншую анимацию.
//В этот скрипт приходят команды какие кнопки показать с обучения персонажа уже в тире
public class LessonAnimatorHands : MonoBehaviour
{
   public MassivHandsPlayer leftTargetScript;
    public MassivHandsPlayer rightTargetScript;

    public HandAnimationController triggerRight;//рука с анимациями нажатия на курок
    public HandAnimationController triggerLeft;//рука с анимациями нажатия на курок

    public string NameAnimationRight;//название анимации для правой руки
    public string NameAnimationLeft;//название анимации для левой руки

    public bool RunCollider = false;
    
 
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

    
    


    //включаем помощь
    public void ActivateHelpHands(string nameAnimationRight, string nameAnimationLeft)
    {
        
        NameAnimationRight = nameAnimationRight;
        NameAnimationLeft = nameAnimationLeft;

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

