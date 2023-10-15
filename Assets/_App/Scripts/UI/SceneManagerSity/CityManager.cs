using UnityEngine;
using UnityEngine.SceneManagement; // Для работы со сценами.

public class CityManager : MonoBehaviour
{
    public static CityManager Instance;
    public AppSetting appSettings; // Добавьте ссылку на ваш ScriptableObject где есть название города
    
    private bool isSceneLoading = false;//проверяем, загрузилась ли сцена или нет, чтобы не включать 2 одновременно

    public BannerDropScript bannerDropScript;// скрипт для баннера
    private void Awake()
    {
        Instance = this;
        /*
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        */
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isSceneLoading = false;
    }
    
    
    public void RegisterBannerDropScript(BannerDropScript script)
    {
        bannerDropScript = script;
    }
    

    public void LoadCityScene(string baseSceneName)
    {
        if (isSceneLoading) return; // Если идет загрузка, прекратить выполнение

        isSceneLoading = true; 

        string sceneName = $"{baseSceneName}_{appSettings.AppData.City}";

      
            // bannerDropScript = FindObjectOfType<BannerDropScript>();
        if (bannerDropScript != null && baseSceneName != "Taverna")
        {
        
            //bannerDropScript.SetSceneToLoadNext(sceneName);
            bannerDropScript.TriggerLowerShield(sceneName);
        }
        else
        {
            
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }

}
