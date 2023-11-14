using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FontManager : MonoBehaviour
{
    public bool UserFont;
    public Font engFont; // Шрифт для английского языка
    public Font rusFont; // Шрифт для русского языка
    public Font chnFont; // Шрифт для китайского языка

    public TMP_FontAsset engFontTMP; // TMP шрифт для английского языка
    public TMP_FontAsset rusFontTMP; // TMP шрифт для русского языка
    public TMP_FontAsset chnFontTMP; // TMP шрифт для китайского языка

    private Text textComponent;
    private TMP_Text textMeshProComponent;

    private void Start()
    {
        textComponent = GetComponent<Text>();
        textMeshProComponent = GetComponent<TMP_Text>();

        LanguageManager.Instance.LanguageChanged += UpdateFont;
        UpdateFont();
    }

    private void UpdateFont()
    {
        if (UserFont)
        {
            if (textComponent != null)
            {
                switch (LanguageManager.Instance.currentLanguage)
                {
                    case LanguageManager.Language.Eng:
                        textComponent.font = engFont;
                        break;
                    case LanguageManager.Language.Rus:
                        textComponent.font = rusFont;
                        break;
                    case LanguageManager.Language.Chn:
                        textComponent.font = chnFont;
                        break;
                    default:
                        break;
                }
            }

            if (textMeshProComponent != null)
            {
                switch (LanguageManager.Instance.currentLanguage)
                {
                    case LanguageManager.Language.Eng:
                        textMeshProComponent.font = engFontTMP;
                        break;
                    case LanguageManager.Language.Rus:
                        textMeshProComponent.font = rusFontTMP;
                        break;
                    case LanguageManager.Language.Chn:
                        textMeshProComponent.font = chnFontTMP;
                        break;
                    default:
                        break;
                }
            }
        }
        else if (!UserFont)
        {
            // Для простоты, предположим, что у вас есть FontData, которое также хранит TMP_FontAsset
            if (textComponent != null)
            {
                switch (LanguageManager.Instance.currentLanguage)
                {
                    case LanguageManager.Language.Eng:
                        textComponent.font = FontData.Instance.engFont;
                        break;
                    case LanguageManager.Language.Rus:
                        textComponent.font = FontData.Instance.rusFont;
                        break;
                    case LanguageManager.Language.Chn:
                        textComponent.font = FontData.Instance.chnFont;
                        break;
                    default:
                        break;
                }
            }

            if (textMeshProComponent != null)
            {
                switch (LanguageManager.Instance.currentLanguage)
                {
                    case LanguageManager.Language.Eng:
                        textMeshProComponent.font = FontData.Instance.engFontTMP;
                        break;
                    case LanguageManager.Language.Rus:
                        textMeshProComponent.font = FontData.Instance.rusFontTMP;
                        break;
                    case LanguageManager.Language.Chn:
                        textMeshProComponent.font = FontData.Instance.chnFontTMP;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.LanguageChanged -= UpdateFont;
    }
}
