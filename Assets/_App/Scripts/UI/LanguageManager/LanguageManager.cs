using System;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public enum Language { Eng, Rus, Chn }
    public Language currentLanguage;

    public event Action LanguageChanged;

    private void Awake()
    {
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

    public void ChangeLanguage(Language newLanguage)
    {
        currentLanguage = newLanguage;
        LanguageChanged?.Invoke();
    }

    // Вызывается при нажатии кнопки для изменения на русский язык
    public void RusLangle()
    {
        ChangeLanguage(Language.Rus);
    }
    
    // Вызывается при нажатии кнопки для изменения на английский язык
    public void EngLangle()
    {
        ChangeLanguage(Language.Eng);
    }
    
    // Вызывается при нажатии кнопки для изменения на китайский язык
    public void ChnLangle()
    {
        ChangeLanguage(Language.Chn);
    }
    
    
}