// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Singleton to hold game data and handle player sign in.
/// </summary>
public class GameData : Singleton<GameData>
{
    #region Loading Fields

    public string previousScene { get; private set; }
    public string nextScene { get; private set; }

    #endregion

    #region Pause Fields

    public bool paused { get; private set; }

    #endregion

    #region Level Fields

    public Dictionary<string, object> levelData { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
 	     base.Awake();

         levelData = new Dictionary<string, object>();
    }

    #endregion

    #region Loading Methods

    /// <summary>
    /// Load the next scene and cache data.
    /// </summary>
    /// <param name="levelData">Data to save for the next scene.</param>
    /// <param name="nextScene">Name of next scene.</param>
    public static void LoadLevel(string nextScene, bool load = false, Dictionary<string, object> levelData = null)
    {
        if (levelData == null)
        {
            Main.levelData = new Dictionary<string, object>();
        }
        else
        {
            Main.levelData = levelData;
        }
        Main.previousScene = Application.loadedLevelName;
        Main.nextScene = nextScene;

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    }


    /// <summary>
    /// Reloads the current scene without the Loading Screen.
    /// <param name="levelData">Data to save for the next scene.</param>
    /// </summary>
    public static void ReloadLevel(Dictionary<string, object> levelData = null)
    {
        if (levelData != null)
        {
            Main.levelData = levelData;
        }
        Application.LoadLevel(Application.loadedLevel);
    }

    #endregion
}