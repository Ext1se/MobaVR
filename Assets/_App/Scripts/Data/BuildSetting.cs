using System;
using UnityEngine;

namespace MobaVR
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "MobaVR/API/Create Build Settings", order = 1)]
    public class BuildSetting : ScriptableObject
    {
        public AppData AppData = new AppData();
        public string Path = "../Assets/Builds/";
        public string Name = "Heroes Arena";
    }
}