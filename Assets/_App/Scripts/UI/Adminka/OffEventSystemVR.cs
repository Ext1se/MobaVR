using UnityEngine;

public class OffEventSystemVR : MonoBehaviour
{
    private ManagerDevice _managerDevice; 
    
    private void Awake()
    {
        _managerDevice = FindObjectOfType<ManagerDevice>();
    }

    private void Start()
    {
        if (_managerDevice.IsAdmin)
        {
            gameObject.SetActive(false);
        }
    }
}