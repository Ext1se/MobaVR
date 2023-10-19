using System.Collections.Generic;
using UnityEngine;

namespace MobaVR
{
    [CreateAssetMenu(fileName = "BuildSettingsGroup", menuName = "MobaVR/API/Create Build Settings Group", order = 1)]
    public class BuildSettingGroup : ScriptableObject
    {
        public List<BuildSetting> BuildSettings = new List<BuildSetting>();
    }
}