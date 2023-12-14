using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManagerDevice : MonoBehaviour
{
    //нужно сделать, чтобы он переносился в неуничтажаемые объекты
    //нужно добавить условие, что если он не мастер сервер, то становится им и проверяет это постоянно
    //нужно добавить функции включения разных карт
    //нужно добавить функции старта режимов


    // Start is called before the first frame update
    public AppSetting AppSetting;
    //[Header("Тестируем андройд")]
    //private bool TestAndroid; //ставим галочку, если хотим запустить в редакторе версию для андройда
    //public bool IsAdmin; //ставим галочку, если хотим запустить в редакторе версию для андройда
    [Header("Объекты с камерами")]
    public GameObject PlayerVR; // префаб игрока для ВР
    public GameObject PlayerPC; // префаб игрока для компьютера
    
    public EventSystem EventSystemVR; // префаб игрока для ВР
    public EventSystem EventSystemPC; // префаб игрока для компьютера

    public bool CanCreatePlayer; //создаём игрока или нет

    public bool IsAdmin => AppSetting.AppData.IsAdmin;

    private void Awake()
    {
        //менеджер при старте игры не удаляем
        //DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        //роверяем на каком устройстве запущена игра
        //DetectPlatformAndExecuteFunction();
        SimpleDetectPlatform();
    }

    private void SimpleDetectPlatform()
    {
        //if (IsAdmin)
        if (AppSetting.AppData.IsAdmin)
        {
            FunctionForWindows();
        }
        else
        {
            FunctionForAndroid();
        }
    }

    /*
    private void DetectPlatformAndExecuteFunction()
    {
        // Проверяем, на какой платформе запущена игра
        if (Application.isEditor && !TestAndroid)
        {
            // Если игра запущена в редакторе Unity
            FunctionForEditor();
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // Если игра запущена на Windows
            if (TestAndroid)
            {
                // Запустим как админ
                //FunctionForWindows();
                FunctionForAndroid();
            }
            else
            {
                // Запустим как клиент Виндовс
                FunctionForWindows();
            }
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            // Если игра запущена на Android
            FunctionForAndroid();
        }
        else if (Application.isEditor && TestAndroid)
        {
            // Если игра запущена в редакторе, но тестим для андройда
            FunctionForAndroid();
        }
    }
    */

    private void FunctionForEditor()
    {
        EventSystemVR.gameObject.SetActive(false);
        EventSystemPC.gameObject.SetActive(true);
        
        PlayerVR.SetActive(false);
        PlayerPC.SetActive(true);

        CanCreatePlayer = false; //не создаём игрока
    }

    private void FunctionForWindows()
    {
        EventSystemVR.gameObject.SetActive(false);
        EventSystemPC.gameObject.SetActive(true);
        
        PlayerVR.SetActive(false);
        PlayerPC.SetActive(true);
        
        CanCreatePlayer = false; //не создаём игрока
    }

    private void FunctionForAndroid()
    {
        EventSystemPC.gameObject.SetActive(false);
        EventSystemVR.gameObject.SetActive(true);
        
        PlayerPC.SetActive(false);
        PlayerVR.SetActive(true);

        CanCreatePlayer = true; //создаём игрока
    }
}