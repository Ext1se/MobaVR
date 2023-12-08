using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MobaVR
{
    [CreateAssetMenu(fileName = "BuildSettingsGroup", menuName = "MobaVR/API/Create Build Settings Group", order = 1)]
    public class BuildSettingGroup : ScriptableObject
    {
        [Header("Override Data")]
        public bool IsOverrideCity = false;
        [ShowIf("IsOverrideCity")] public string CityName = "Arma";

        public bool IsOverrideRoom = false;
        [ShowIf("IsOverrideRoom")] public string RoomName = "Arma";

        [Space]
        public List<BuildSetting> BuildSettings = new List<BuildSetting>();
    }
}