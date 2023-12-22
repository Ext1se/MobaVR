using UnityEngine;
using BNG; 
//скрипт, который активирует книгу с подсказками на руке. И убирает тот муляж, котоырй прилетел после анимации появления
public class ShowHints : MonoBehaviour
{
    public GameObject hintsBook; // Объект книги с подсказками
    public GameObject mockupBook; // Объект книги-муляжа, её отключаю
    public ControllerBinding Button_X = ControllerBinding.XButtonDown;

    public bool BookHelpActive; // Включена сейчас книга или нет 
    
    public bool test = false;

    void Start()
    {
        
        // Убедитесь, что книга с подсказками изначально выключена
        if (hintsBook != null)
        {
            hintsBook.SetActive(false);
            BookHelpActive = false;
        }
        else
        {
            Debug.LogError("Hints book is not assigned in the inspector");
        }
    }

    private void Update()
    {
        if (test)
        {
            ToggleBooks();
        }
        
        // Проверка нажатия кнопки X
        if (InputBridge.Instance.GetControllerBindingValue(Button_X))
        {
            ToggleBooks();
        }
    }

    private void ToggleBooks()
    {
        
        // Проверяем, активна ли книга с подсказками
        if (hintsBook != null && !BookHelpActive)
        {
           
            //включаем книгу на руке
            if (hintsBook != null)
            {
                hintsBook.SetActive(true);
            }

            BookHelpActive = true;

        }
        else
        {
            // Показать книгу с подсказками и скрыть книгу-муляж, если она есть
            if (hintsBook != null)
            {
                hintsBook.SetActive(false);
            }

            BookHelpActive = false;
        }
    }
}