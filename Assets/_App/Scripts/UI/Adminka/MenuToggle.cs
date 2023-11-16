using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    // Массив объектов, которые вы хотите скрыть или показать
    public GameObject[] menuObjects;

    // Массив папок (или объектов), к которым вы хотите прикрепить камеру
    public Transform[] cameraParentFolders;

    // Начальное состояние: показывать или скрывать объекты при запуске
    public bool showOnStart = true;
    
    // Ссылка на основную камеру
    public Camera mainCam;

    private void Start()
    {
        // Устанавливаем начальное состояние объектов при запуске
        SetMenuObjects(showOnStart);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftAlt))
        {
            ToggleMenu();
        }
        
        /*
        // Проверка на нажатие кнопки 'h'
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleMenu();
        }
        */
    }

    // Функция, вызываемая кнопкой, чтобы переключить видимость объектов
    public void ToggleMenu()
    {
        // Проверяем один из объектов массива для определения текущего состояния 
        // (предполагая, что все объекты имеют одинаковое состояние видимости)
        bool isCurrentlyActive = menuObjects[0].activeSelf;

        // Устанавливаем обратное состояние для всех объектов
        SetMenuObjects(!isCurrentlyActive);
    }

    // Функция для установки состояния объектов (активно / неактивно)
    private void SetMenuObjects(bool activeState)
    {
        foreach (GameObject obj in menuObjects)
        {
            obj.SetActive(activeState);
        }
    }

    // Функция для перемещения основной камеры в папку (или объект) в зависимости от ID
    public void MoveMainCameraToFolder(int folderID)
    {
        if (folderID >= 0 && folderID < cameraParentFolders.Length)
        {
          

            // Перемещаем камеру в папку и обнуляем локальные координаты
            mainCam.transform.SetParent(cameraParentFolders[folderID]);
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("Folder ID is out of range!");
        }
    }
}
