using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalRepository
{
    public const string SERVER_MODE = "SERVER_MODE";
    
    public void SetLocalServer(bool isLocalServer)
    {
        PlayerPrefs.SetInt(SERVER_MODE, isLocalServer ? 0 : 1);
        PlayerPrefs.Save();
    }

    public bool IsLocalServer => PlayerPrefs.GetInt(SERVER_MODE, 0) == 0;
}
