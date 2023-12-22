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

   
            FindFunction();
            if (other.CompareTag("TriggerLocalPlayer"))
            {
                leftTargetScript?.ActivateObject(2);
                rightTargetScript?.ActivateObject(2);
               
            }
        

    }

    //как только входим выключаются
    void OnTriggerExit(Collider other)
    {
      
            FindFunction();
            if (other.CompareTag("TriggerLocalPlayer"))
            {
                leftTargetScript?.ActivateObject(0);
                rightTargetScript?.ActivateObject(0);
               
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
                    if (triggerLeft  != null)
                    {
                        triggerLeft .GetComponent<HandAnimationController>().SetTrigger(NameAnimationLeft);
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
            triggerRight.GetComponent<HandAnimationController>().SetTrigger("Stay");
           
        }
        
        // Отправляем команду для запуска триггера
        if (triggerLeft  != null)
        {
            triggerLeft .GetComponent<HandAnimationController>().SetTrigger("Button1_Left");
           
        }
        
    }

}
