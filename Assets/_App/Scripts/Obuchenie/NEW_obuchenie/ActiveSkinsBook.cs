using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// скипт, который включает или выключает скины игроков у книги. Висит на страницах с информацией о скинах и выключается кнопками далее или назад

public class ActiveSkinsBook : MonoBehaviour
{
    public GameObject targetObject; // Объект, который будет активироваться или деактивироваться

   

    void OnEnable()
    {

            if (targetObject != null) // Убедитесь, что ссылка на объект задана
            {
                targetObject.SetActive(true); // Активируем  объект 
            }

    }       



    public void activateskins()
    {
      
        if (targetObject != null) // Убедитесь, что ссылка на объект задана
        {
            targetObject.SetActive(false); // деактивируем объект 
        }
        
    }

}
