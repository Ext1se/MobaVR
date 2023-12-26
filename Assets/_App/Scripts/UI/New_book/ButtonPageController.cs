using UnityEngine;
//отсылает ID какую страницу открыть, при выборе класса.
public class ButtonPageController : MonoBehaviour
{
    public PageFlipController pageFlipController; // Ссылка на скрипт управления страницами
    public int buttonID; // Идентификатор кнопки

    // Вызывается при нажатии на кнопку
    public void OnButtonPress()
    {
        pageFlipController.SetSelectedPageID(buttonID);
    }
}