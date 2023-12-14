using UnityEngine;
using UnityEngine.UI;

public class MenuBookLobby : MonoBehaviour
{
    public ObjectIDPair[] objectIDPairs; // Массив пар объектов и их ID
    public ZonaBook zonaBook;
    public SaveInfoClass _SaveInfoClass;

  //  public GameObject VideoKurok;
   // public GameObject descriptor;

    public void OnButtonClick(string buttonId)
    {
      //  VideoKurok.SetActive(false);
      //  descriptor.SetActive(false);

/*
        // Перебираем все пары объектов и их ID
        foreach (ObjectIDPair pair in objectIDPairs)
        {
            // Если ID совпадает с нажатой кнопкой, активируем объект, иначе - деактивируем
            if (pair.id == buttonId)
            {
               // pair.obj.SetActive(true);
            }
            else
            {
              //  pair.obj.SetActive(false);
            }
        }
*/
        zonaBook.targetID = buttonId;
        zonaBook.UpdateTargetID(buttonId);

        _SaveInfoClass.targetID = buttonId;
    }
}

[System.Serializable]
public class ObjectIDPair
{
    public string id;
   // public GameObject obj;
}