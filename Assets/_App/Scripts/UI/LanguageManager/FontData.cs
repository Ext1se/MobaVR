using UnityEngine;
using UnityEngine.UI;

public class FontData : MonoBehaviour
{
    public static FontData Instance; // Статическая ссылка на экземпляр FontData

    public Font engFont; // Шрифт для английского языка
    public Font rusFont; // Шрифт для русского языка
    public Font chnFont; // Шрифт для китайского языка

    private void Awake()
    {
        // Убедитесь, что есть только один экземпляр FontData
        Instance = this;
        /*
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        */
    }
}