// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.07.02

using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton to transition between scenes.
/// </summary>
public class SceneManager : Singleton<SceneManager>
{
    #region Loading Fields

    public string previousScene { get; private set; }
    public string nextScene { get; private set; }

    #endregion

    #region Level Fields

    [Obsolete("Don't need. Removing Level Overview Screen.")]
    public Dictionary<string, object> levelData { get; private set; }
    public string currentBuild = "Test Build";

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
    /// <param name="nextScene">Name of next scene.</param>
    /// <param name="load">Should the Loading Screen be loaded first?</param>
    /// <param name="sceneData">Data to save for the next scene.</param>
    [Obsolete("remove scene data and add unload and collect")]
    public static void LoadScene(string nextScene, bool load = false, Dictionary<string, object> sceneData = null)
    {
        Main.levelData = sceneData ?? new Dictionary<string, object>();
        Main.previousScene = Application.loadedLevelName;
        Main.nextScene = nextScene;

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    }


    /// <summary>
    /// Reloads the current scene without the Loading Screen.
    /// </summary>
    public static void ReloadScene(bool load = false, bool unload = false, bool collect = false)
    {
        if (unload) Resources.UnloadUnusedAssets();
        if (collect) GC.Collect();
        Application.LoadLevel(Application.loadedLevel);
    }

    #endregion
}