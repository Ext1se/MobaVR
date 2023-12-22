using UnityEngine;
using UnityEngine.XR;

//скрипт который включает внешний вид контроллера, или от Пико или от Окулуса
public class PlatformControllerActivator : MonoBehaviour
{
    public GameObject PicoController;
    public GameObject OculusController;

    void Start()
    {
        ActivateControllersBasedOnPlatform();
    }

    void ActivateControllersBasedOnPlatform()
    {
        // Выключить оба контроллера по умолчанию
        if (PicoController != null) PicoController.SetActive(false);
        if (OculusController != null) OculusController.SetActive(false);

        // Определить текущую платформу и включить соответствующий контроллер
        if (XRSettings.loadedDeviceName.Contains("Pico"))
        {
            if (PicoController != null) PicoController.SetActive(true);
        }
        else if (XRSettings.loadedDeviceName.Contains("Oculus") || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (OculusController != null) OculusController.SetActive(true);
        }
        else
        {
            // На случай, если платформа не распознана, можно включить контроллеры по умолчанию или ничего не делать
            // Пример: включить OculusController или оставить все выключенными
            if (OculusController != null) OculusController.SetActive(true);
        }
    }
}