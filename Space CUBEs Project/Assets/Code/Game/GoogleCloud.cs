// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.11
// Edited: 2014.10.11

using System;
using Annotations;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using LittleByte.Data;
using UnityEngine;

public class GoogleCloud : Singleton<GoogleCloud>, OnStateLoadedListener
{
    #region MonoBehaviour Overrides

#if UNITY_ANDROID
    [UsedImplicitly]
    private void OnApplicationQuit()
    {
        Save();
    }
#endif

    #endregion

    #region Public Methods

    public void Save()
    {
        if (!Social.localUser.authenticated) return;

        ((PlayGamesPlatform)Social.Active).UpdateState(0, SaveData.ExportGameState(), this);
    }

    public void Load()
    {
        if (!Social.localUser.authenticated) return;

        ((PlayGamesPlatform) Social.Active).LoadState(0, this);
    }

    #endregion

    #region OnStateLoadedListener

    public void OnStateLoaded(bool success, int slot, byte[] data)
    {
        Debugger.Log("Save Loaded: " + success, this, Debugger.LogTypes.Data);

        if (success && data != null)
        {
            SaveData.RestoreGameState(data);
        }
    }


    public byte[] OnStateConflict(int slot, byte[] localData, byte[] serverData)
    {
        Debugger.Log("Save Conflict", this, Debugger.LogTypes.Data);
        return serverData;
    }


    public void OnStateSaved(bool success, int slot)
    {
        Debugger.Log("Save Saved: " + success, this, Debugger.LogTypes.Data);
    }

    #endregion
}