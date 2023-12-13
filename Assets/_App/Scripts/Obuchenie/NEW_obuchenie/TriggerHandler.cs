using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Убедитесь, что у вас подключен Photon PUN

//этот скрипт отсылает ID в скрипт MassivHandsPlayer, который будет включать нужные нам руки. в данном случае контроллеры, при входе в зону книги
public class TriggerHandler : MonoBehaviour
{
   public MassivHandsPlayer leftTargetScript;
    public MassivHandsPlayer rightTargetScript;
    
    

    //как только входим в зону обучения у нас включаются контроллеры 
    void OnTriggerEnter(Collider other)
    {
        FindFunction();
        if (other.CompareTag("TriggerLocalPlayer"))
        {
            leftTargetScript?.ActivateObject(3);
            rightTargetScript?.ActivateObject(3);
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
            }
        }
    }
}
