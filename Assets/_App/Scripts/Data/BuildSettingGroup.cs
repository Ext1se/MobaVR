using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    [CreateAssetMenu(fileName = "BuildSettingsGroup", menuName = "MobaVR/API/Create Build Settings Group", order = 1)]
    public class BuildSettingGroup : ScriptableObject
    {
        [Header("Override Data")]
        public bool IsOverrideCity = false;
        public string CityName = "Arma";
        
        [Space]
        public List<BuildSetting> BuildSettings = new List<BuildSetting>();
    }
}