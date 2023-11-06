using System;

namespace MobaVR
{
    [Serializable]
    public class AppData
    {
        public PlatformType Platform = PlatformType.WINDOWS;
        public bool IsDevBuild = false; // Если true, то проверка лицензия пропускается. А клиент может становится хостом
        public bool UseLogs = false;
        public bool IsAdmin = false;
        public bool UseVR = false;
        public string City = "Arma";
    }
}