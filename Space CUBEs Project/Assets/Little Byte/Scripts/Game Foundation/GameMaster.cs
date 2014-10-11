// Little Byte Games
// Author: Steve Yeager
// Created: 2013.11.27
// Edited: 2014.10.10

using System;
using Annotations;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    #region References

    private static GameMaster Main;

    #endregion

    #region Events

    public static Action QuitEvent;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
            DontDestroyOnLoad(gameObject);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    [UsedImplicitly]
    private void OnApplicationQuit()
    {
        if (QuitEvent != null)
        {
            QuitEvent();
        }
    }

    #endregion
}