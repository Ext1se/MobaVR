using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "MobaVR/API/Create Build Settings", order = 1)]
    public class BuildSetting : ScriptableObject
    {
        public AppData AppData = new AppData();

        [Space]
        [HideInInspector] public string Path = "../Assets/Builds/";
        [HideInInspector] public string Name = "Heroes Arena";
    }
}