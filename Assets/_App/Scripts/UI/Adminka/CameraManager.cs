using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public Camera[] camerasUI; // Массив камер для отображения в меню
    public Camera[] cameras; // Массив камер для второго экрана
    public GameObject FreeCamera; // Свободная камера для второго экрана
   
    public RawImage uiImage; // RawImage для отображения изображения на UI
    public bool MonitorDouble = false; // Второй экран
    private int currentCameraIndexUI = 0; // Текущая активная камера для UI
    
    void Start()
    {      
       

 
        // Изначально отображаем первую камеру для UI
        ShowCamera(currentCameraIndexUI);
        // Выключаем все камеры второго экрана
        // ToggleSecondScreen();
    }

    void Update()
    {
        // Обновляем изображение на UI
        UpdateUI();
    }

    public void SwitchCamera(int cameraIndex)
    {
        // Переключение на выбранную камеру в UI
        if (cameraIndex >= 0 && cameraIndex < camerasUI.Length)
        {
            currentCameraIndexUI = cameraIndex;
            ShowCamera(currentCameraIndexUI);
        }
    }

    //включаем управление свободнйо камеры
    public void FreeCameraRun()
    {
        FreeCamera.SetActive(true);
        if (FreeCamera.TryGetComponent(out FreeCameraController freeCameraController))
        {
            freeCameraController.SetActiveControl(true);
        }
    }    
    
    //отключаем свободную камеру
    public void FreeCameraStop()
    {
        FreeCamera.SetActive(false); 
        
        if (FreeCamera.TryGetComponent(out FreeCameraController freeCameraController))
        {
            freeCameraController.SetActiveControl(false);
        }
    }

    public void ToggleSecondScreen()
    {
      
        
        MonitorDouble = !MonitorDouble; // Инвертируем значение MonitorDouble при каждом вызове метода

        if (MonitorDouble)
        {
            //включаем второй экран
            for (int i = 0; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
            
            // Включаем камеру для второго экрана, которая сейчас активна в UI
            ShowCamera(currentCameraIndexUI);
        }
        else
        {
            // Отключаем все камеры для второго экрана
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].enabled = false;
            }
        }
    }

    void ShowCamera(int index)
    {
		// Выключаем все камеры UI
    	for (int i = 0; i < camerasUI.Length; i++)
    	{
      	  	camerasUI[i].enabled = false;
    	}

    	// Выключаем все камеры для второго экрана
    	if (MonitorDouble)
    	{
        	for (int i = 0; i < cameras.Length; i++)
        	{
            cameras[i].enabled = false;
        	}
   		}


        // Отображаем выбранную камеру UI
        for (int i = 0; i < camerasUI.Length; i++)
        {
            camerasUI[i].enabled = (i == index);
        }

        if (MonitorDouble)
        {
            // Отображаем выбранную камеру для второго экрана
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].enabled = (i == index);
            }
        }
        
    }

    void UpdateUI()
    {
        // Обновляем изображение на UI из текущей камеры
        if (uiImage != null && camerasUI.Length > 0)
        {
            Texture cameraTexture = camerasUI[currentCameraIndexUI].targetTexture;
            uiImage.texture = cameraTexture;
        }
    }
}
