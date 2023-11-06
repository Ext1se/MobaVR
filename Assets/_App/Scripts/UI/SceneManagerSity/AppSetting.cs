using MobaVR;
using UnityEngine;

[CreateAssetMenu(fileName = "AppSettingCity", menuName = "MobaVR/API/AppSettingCity", order = 1)]
public class AppSetting : ScriptableObject
{
    [Header("Game")]
    public int IdGame = -1;
    public string GameVersion = "1.0.0";
    
    [Space]
    [Header("Build")]
    public int IdClub = -1;
    [Space]
    public AppData AppData = new AppData();
}