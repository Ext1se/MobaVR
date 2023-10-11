using MobaVR;
using UnityEngine;

[CreateAssetMenu(fileName = "AppSettingCity", menuName = "MobaVR/API/AppSettingCity", order = 1)]
public class AppSetting : ScriptableObject
{
    public bool IsDevelopmentBuild = false;
    public PlatformType Platform = PlatformType.WINDOWS;
    public bool IsAdmin = false;
    public string City = "Arma";
}