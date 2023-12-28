using UnityEngine;

public class HelpInSmallBook : MonoBehaviour
{
    public GameObject firstObject; // Первый объект для активации
    public GameObject secondObject; // Второй объект для активации

    private bool isFirstActivation = true; // Флаг первой активации

    private void Start()
    {
        // Активируем первый объект при старте
        if (firstObject != null)
        {
            firstObject.SetActive(true);
        }

        // Отключаем второй объект при старте
        if (secondObject != null)
        {
            secondObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (isFirstActivation)
        {
            // Первая активация уже была, отключаем первый объект и включаем второй
            if (firstObject != null)
            {
                firstObject.SetActive(false);
            }

            if (secondObject != null)
            {
                secondObject.SetActive(true);
            }

            // Обновляем флаг, так как первая активация уже произошла
            isFirstActivation = false;
        }
        else
        {
            // При последующих активациях всегда включаем только второй объект
            if (secondObject != null)
            {
                secondObject.SetActive(true);
                firstObject.SetActive(false);
            }
        }
    }
}