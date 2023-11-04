using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobaVR;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//админка у правление с компьютера
public class Adminka : MonoBehaviourPunCallbacks
{
    [Header("Тест. меню включается с нужного экрана")]
    public bool
        Test; //тестовая переменная, которая выключает включение с первого экрана, в ключит с того, который был включен

    [Header("Меню выбора сетевого режима")]
    public GameObject PanelLocad; // выбора сетевого режима
    [Header("Меню загрузки")]
    public GameObject PanelLocading; //меню загрузки

    [Header("Меню основа")]
    public GameObject PanelOsnova; //меню основа, где старт игры и стоп игры

    [Header("Меню игры")]
    public GameObject PanelGame; //меню игры

    [Header("Игроки")]
    public GameObject PanelPlayers; //меню с игроками

    [Header("Меню камер")]
    public GameObject PanelCamers; //меню камер

    [Header("Меню камер")]
    public GameObject PanelMap; //меню камер

    private bool RunGames; //проверяем заработала ли игра или нет

    [Header("Информация о картах")]
    public GameObject[] InfoMaps;


    [Header("IP-адрес")]
    public InputField ipAddressInput; // ссылка на поле ввода IP-адреса

    [SerializeField] private ClassicGameSession m_GameSession;
    [SerializeField] private SessionSettings m_SessionSettings;
    [SerializeField] private StateMachineSwitcher m_StateMachineSwitcher;
    [SerializeField] private Environment m_Environment;
    [SerializeField] private ScenesEnvironment m_SceneEnvironment;

    private void Awake()
    {
        //админку при старте игры не удаляем
        //DontDestroyOnLoad(gameObject);

        RunGames = false;
        LoadSavedIPAddress(); // Загрузить сохраненный IP-адрес при запуске приложения
        //нужен для загрузки сцен
        if (m_GameSession == null)
        {
            m_GameSession = FindObjectOfType<ClassicGameSession>();
        }

        if (m_Environment == null)
        {
            m_Environment = FindObjectOfType<Environment>();
        }

        if (m_SceneEnvironment == null)
        {
            m_SceneEnvironment = FindObjectOfType<ScenesEnvironment>();
        }

        if (m_StateMachineSwitcher == null)
        {
            m_StateMachineSwitcher = FindObjectOfType<StateMachineSwitcher>();
        }

        if (m_SessionSettings == null)
        {
            m_SessionSettings = FindObjectOfType<SessionSettings>();
        }
    }


    public void Start()
    {
        if (!Test)
        {
            //включаем только мень выбора языка и сетевого режима игры
            PanelLocad.SetActive(true);
            PanelLocading.SetActive(false);
            PanelOsnova.SetActive(false);
            PanelGame.SetActive(false);
            PanelPlayers.SetActive(false);
            PanelCamers.SetActive(false);
            PanelMap.SetActive(false);
        }
    }

    void Update()
    {
        //SetMasterClient();
    }

    //выключаем меню загрузки
    public void off_loadMenu()
    {
        SaveIPAddress(); // Сохранить IP-адрес перед загрузкой сцены
        PanelLocad.SetActive(false);
        PanelLocading.SetActive(true);
    }


    // Сохранить IP-адрес в PlayerPrefs
    private void SaveIPAddress()
    {
        string ipAddress = ipAddressInput.text;
        PlayerPrefs.SetString("LastIPAddress", ipAddress);
        PlayerPrefs.Save();
    }

    // Загрузить IP-адрес из PlayerPrefs
    private void LoadSavedIPAddress()
    {
        string lastIPAddress = PlayerPrefs.GetString("LastIPAddress", "");
        if (!string.IsNullOrEmpty(lastIPAddress))
        {
            ipAddressInput.text = lastIPAddress;
        }
    }

    public void loadCamers()
    {
        PanelLocad.SetActive(false);
        PanelLocading.SetActive(false);
        PanelOsnova.SetActive(true);
        PanelGame.SetActive(false);
        PanelPlayers.SetActive(false);
        PanelMap.SetActive(false);
        PanelCamers.SetActive(true);
    }

    public void loadGame()
    {
        PanelLocad.SetActive(false);
        PanelLocading.SetActive(false);
        PanelOsnova.SetActive(true);
        PanelGame.SetActive(true);
        PanelPlayers.SetActive(false);
        PanelCamers.SetActive(false);
        PanelMap.SetActive(false);
    }

    public void loadMap()
    {
        PanelLocad.SetActive(false);
        PanelLocading.SetActive(false);
        PanelOsnova.SetActive(true);
        PanelGame.SetActive(false);
        PanelPlayers.SetActive(false);
        PanelCamers.SetActive(false);
        PanelMap.SetActive(true);
    }

    public void loadPlayers()
    {
        PanelLocad.SetActive(false);
        PanelLocading.SetActive(false);
        PanelOsnova.SetActive(true);
        PanelGame.SetActive(false);
        PanelPlayers.SetActive(true);
        PanelCamers.SetActive(false);
        PanelMap.SetActive(false);
    }

    #region Maps

    public void ShowMap(string mapName)
    {
        m_SceneEnvironment.ShowMap(mapName);
    }

    //включение карт
    public void ShowTavernMap()
    {
        m_SceneEnvironment.ShowTavernMap();
    }

    public void ShowTavernMapPointEnter()
    {
        ActivateMap(0); // Информация о карте
    }

    /*public void ShowSkyLandMap()
    {
        m_SceneEnvironment.ShowSkyLandMap();
    }*/

    public void ShowSkyLandWithPropMap()
    {
        m_SceneEnvironment.ShowSkyLandWithPropMap();
    }

    public void ShowSkyLandWithPropMapPointEnter()
    {
        ActivateMap(1); // Информация о карте
    }

    /*public void ShowMobaMap()
    {
        m_SceneEnvironment.ShowMobaMap();
    }*/

    /*public void ShowLichMap()
    {
        m_SceneEnvironment.ShowLichMap();
    }*/

    public void ShowNecropolisMap()
    {
        m_SceneEnvironment.ShowNecropolisMap();
    }

    public void ShowNecropolisMapPointEnter()
    {
        ActivateMap(2); // Информация о карте
    }

    public void ShowTowerMap()
    {
        m_SceneEnvironment.ShowTowerMap();
    }

    public void ShowTowerMapPointEnter()
    {
        ActivateMap(3); // Информация о карте
    }

    public void ShowDungeonMap()
    {
        m_SceneEnvironment.ShowDungeonMap();
    }

    public void ShowDungeonMapPointEnter()
    {
        ActivateMap(4); // Информация о карте
    }


    public void ShowDefaultPvPMap()
    {
        m_SceneEnvironment.ShowSkyLandMap();
    }

    public void ShowDefaultPvEMap()
    {
        m_SceneEnvironment.ShowLichMap();
    }

    #endregion


    #region Game Mode

    public void StartMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.StartMode();
        }
    }

    public void CompleteMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.CompleteMode();
        }
    }

    public void DeactivateMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.DeactivateMode();
        }
    }

    #endregion

    #region PvP Mode

    public void StartPvPMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.StartPvPMode();
        }
    }

    public void CompletePvPMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.CompletePvPMode();
        }
    }

    public void DeactivatePvPMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.DeactivatePvPMode();
        }
    }

    public void SetClassMode()
    {
        if (m_StateMachineSwitcher == null)
        {
            m_StateMachineSwitcher = FindObjectOfType<StateMachineSwitcher>();
        }

        if (m_StateMachineSwitcher != null)
        {
            m_StateMachineSwitcher.SetClassMode();
        }
    }

    public void SetTimerMode()
    {
        if (m_StateMachineSwitcher == null)
        {
            m_StateMachineSwitcher = FindObjectOfType<StateMachineSwitcher>();
        }

        if (m_StateMachineSwitcher != null)
        {
            m_StateMachineSwitcher.SetTimerMode();
        }
    }

    #endregion

    /*#region PvE Mode

    public void StartPvEMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.StartPvEMode();
        }
    }

    public void CompletePvEMode()
    {
        if (m_GameSession != null)
        {
            m_GameSession.CompletePvEMode();
        }
    }
    

    #endregion*/


    // Функция, которая будет вызываться для выхода из игры
    public void QuitGame()
    {
        #if UNITY_EDITOR
        // Если игра запущена в редакторе Unity, просто остановите редактор
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Если игра запущена вне редактора, вызовите функцию завершения
            Application.Quit();
        #endif
    }

    // Функция для активации одного элемента массива и выключения остальных
    public void ActivateMap(int mapIndex)
    {
        // Перебираем все элементы массива
        for (int i = 0; i < InfoMaps.Length; i++)
        {
            // Если текущий элемент массива имеет индекс, равный mapIndex, то активируем его
            if (i == mapIndex)
            {
                InfoMaps[i].SetActive(true);
            }
            else
            {
                // Иначе выключаем все остальные элементы
                InfoMaps[i].SetActive(false);
            }
        }
    }

    ///
    ///
    /// 

    #region Photon Room

    private void SetMasterClient()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (m_GameSession != null)
            {
                m_GameSession.SetMaster();
            }
        }

        if (!RunGames)
        {
            loadGame();
            RunGames = true;
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SetMasterClient();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        SetMasterClient();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        SetMasterClient();
    }

    #endregion
}