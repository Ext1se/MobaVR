using MobaVR;
using UnityEngine;

[CreateAssetMenu(fileName = "AppSettingCity", menuName = "MobaVR/API/AppSettingCity", order = 1)]
public class AppSetting : ScriptableObject
{
    public AppData AppData = new AppData();
}