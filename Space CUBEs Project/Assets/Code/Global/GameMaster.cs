// Steve Yeager
// 11.26.2013

using System;
using Annotations;
using LittleByte.Data;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    #region References

    private static GameMaster Main;

    #endregion

    #region Events

    public static EventHandler QuitEvent;

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
            SaveData.Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    [UsedImplicitly]
    private void OnApplicationQuit()
    {
        QuitEvent.Fire();
    }

    #endregion
}