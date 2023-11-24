using System;
using System.Collections;
using System.Collections.Generic;
using MobaVR;
using UnityEngine;

public class LocalRepository
{
    public const string SERVER_MODE = "SERVER_MODE";
    public const string LAST_IP_ADDRESS = "LastIPAddress";
    public const string BACKUP_PLAYER = "BACKUP_PLAYER";
    
    public const string SESSION_END_TIME = "SESSION_END_TIME";
    public const string SESSION_START_TIME = "SESSION_START_TIME";
    
    
    public void SetLocalServer(bool isLocalServer)
    {
        PlayerPrefs.SetInt(SERVER_MODE, isLocalServer ? 0 : 1);
        PlayerPrefs.Save();
    }
    
    public void SavePlayerData(BackupPlayerData backupPlayerData)
    {
        backupPlayerData.BackupDate = DateTime.Now.Millisecond;
        string json = JsonUtility.ToJson(backupPlayerData);
        PlayerPrefs.SetString(BACKUP_PLAYER, json);
        PlayerPrefs.Save();
    }

    public void SaveIpAddress(string ipAddress)
    {
        PlayerPrefs.SetString(LAST_IP_ADDRESS, ipAddress);
        PlayerPrefs.Save();
    }

    public bool TryGetBackupPlayerData(out BackupPlayerData backupPlayerData)
    {
        string json = PlayerPrefs.GetString(BACKUP_PLAYER, "");
        if (string.IsNullOrEmpty(json))
        {
            backupPlayerData = null;
            return false;
        }

        backupPlayerData = JsonUtility.FromJson<BackupPlayerData>(json);
        return true;
    }

    public void SetStartTime(DateTime dateTime)
    {
        PlayerPrefs.SetInt(SESSION_START_TIME, dateTime.Second);
        PlayerPrefs.Save();
    }
    
    public void SetEndTime(DateTime dateTime)
    {
        PlayerPrefs.SetInt(SESSION_END_TIME, dateTime.Second);
        PlayerPrefs.Save();
    }

    private string GetIpAddress =>  PlayerPrefs.GetString(LAST_IP_ADDRESS, "");
    public bool IsLocalServer => PlayerPrefs.GetInt(SERVER_MODE, 0) == 0;
    public string LastIPAddress => PlayerPrefs.GetString(LAST_IP_ADDRESS, "");
    public long GetEndTime => PlayerPrefs.GetInt(SESSION_END_TIME, 0) * 1000;
    public long GetStartTime => PlayerPrefs.GetInt(SESSION_START_TIME, 0) * 1000;
}
