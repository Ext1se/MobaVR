using UnityEngine;

namespace MobaVR
{
    public class InputHelper : MonoBehaviour
    {
        private void Awake()
        {
            #if !UNITY_EDITOR
                CustomComposites.Init();
            #endif
        }
    }
}