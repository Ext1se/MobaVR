using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Добавьте это пространство имен

public class FontData : MonoBehaviour
{
    public static FontData Instance; // Статическая ссылка на экземпляр FontData

    public Font engFont; // Шрифт для английского языка
    public Font rusFont; // Шрифт для русского языка
    public Font chnFont; // Шрифт для китайского языка

    public TMP_FontAsset engFontTMP; // TMP шрифт для английского языка
    public TMP_FontAsset rusFontTMP; // TMP шрифт для русского языка
    public TMP_FontAsset chnFontTMP; // TMP шрифт для китайского языка

    private void Awake()
    {
        // Убедитесь, что есть только один экземпляр FontData
        Instance = this;
    }
}