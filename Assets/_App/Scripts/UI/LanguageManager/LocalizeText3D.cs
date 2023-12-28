using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeText3D : MonoBehaviour
{
    [TextArea(3, 5)]
    [SerializeField]
    private string textEng;

    [TextArea(3, 5)]
    [SerializeField]
    private string textRus;

    [TextArea(3, 5)]
    [SerializeField]
    private string textChn;

    private TextMeshPro tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshPro>();
        
        LanguageManager.Instance.LanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.LanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        string newText = "";
        switch (LanguageManager.Instance.currentLanguage)
        {
            case LanguageManager.Language.Eng:
                newText = textEng;
                break;
            case LanguageManager.Language.Rus:
                newText = textRus;
                break;
            case LanguageManager.Language.Chn:
                newText = textChn;
                break;
        }

        if (tmpText != null) 
            tmpText.text = newText;
    }
}