// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public object levelData { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Update()
    {
        #if UNITY_EDITOR

        // clear console
        // make own method
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));

            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        #endif
    }

    #endregion

    #region Loading Methods

    /// <summary>
    /// LoadHighscore the next scene and cache data.
    /// </summary>
    /// <param name="levelData">Data to save for the next scene.</param>
    /// <param name="nextScene">Name of next scene.</param>
    public void LoadScene(string nextScene, bool load = false, object levelData = null)
    {
        this.levelData = levelData;
        previousScene = Application.loadedLevelName;
        this.nextScene = nextScene;

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    }


    /// <summary>
    /// Reloads the current scene without the Loading Screen.
    /// <param name="levelData">Data to save for the next scene.</param>
    /// </summary>
    public void ReloadScene(object levelData = null)
    {
        this.levelData = levelData;
        Application.LoadLevel(Application.loadedLevel);
    }

    #endregion
}