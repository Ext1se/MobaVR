using UnityEngine;

namespace MobaVR
{
    public class LogSwitcher : MonoBehaviour
    {
        public AppSetting AppSetting;
        public bool UsePlayerSettings = false;
        //public bool UseLogs = true;

        private void Awake()
        {
            if (UsePlayerSettings)
            {
                Debug.unityLogger.logEnabled = Debug.isDebugBuild;
            }
            else
            {
                if (AppSetting != null)
                {
                    Debug.unityLogger.logEnabled = AppSetting.IsDevelopmentBuild;
                }
                
                //Debug.unityLogger.logEnabled = UseLogs;
            }
        }
    }
}