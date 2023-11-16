using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//скрипт включающий клавиатуру. при нажатии на textinput
public class KlaviaturaActivade : MonoBehaviour
{
    public GameObject Klava;

    private void Start()
    
    {
        Klava.SetActive(false);
    }
    public void runKlava()
    {
        Klava.SetActive(true);
    }
}
