using MobaVR;
using UnityEngine;

[CreateAssetMenu(fileName = "AppSettingCity", menuName = "MobaVR/API/AppSettingCity", order = 1)]
public class AppSetting : ScriptableObject
{
    public int IdClub = -1;
    public int IdGame = -1;
    public AppData AppData = new AppData();
}