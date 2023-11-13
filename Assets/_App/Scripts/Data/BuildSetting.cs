using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "MobaVR/API/Create Build Settings", order = 1)]
    public class BuildSetting : ScriptableObject
    {
        public AppData AppData = new AppData();
        
        [Space]
        public bool IsShowDialogPath = true;
        [EnableIf("IsShowDialogPath", false)]
        [ShowIf("IsShowDialogPath")]
        public string Path = "../Assets/Builds/";
        public string Name = "Heroes Arena";
    }
}