using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeText : MonoBehaviour
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

    private Text uiText;
    private TextMeshProUGUI tmpText;

    private void Awake()
    {
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TextMeshProUGUI>();
 

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

        if (uiText != null) 
            uiText.text = newText;
        if (tmpText != null) 
            tmpText.text = newText;
    }
}