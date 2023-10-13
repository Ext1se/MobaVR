using UnityEngine;

namespace MobaVR
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "MobaVR/API/Create Build Settings", order = 1)]
    public class BuildSetting : ScriptableObject
    {
        public string City = "Arma";
        public string Path = "../Assets/Builds/";
        public string Name = "Heroes Arena";
        public PlatformType Platform = PlatformType.WINDOWS;
        public bool IsDevelopmentBuild = false;
        public bool IsAdmin = false;
    }
}