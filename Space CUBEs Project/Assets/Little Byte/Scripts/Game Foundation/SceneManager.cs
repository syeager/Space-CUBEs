// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.07.06

using System;
using UnityEngine;

/// <summary>
/// Singleton to transition between scenes.
/// </summary>
public class SceneManager : Singleton<SceneManager>
{
    #region Const Fields

    public const string MainMenu = "Main Menu";
    public const string Garage = "Garage";
    public const string Store = "Store";
    public const string Options = "Options Menu";

    #endregion

    #region Properties

    /// <summary>The scene loaded previously to this scene.</summary>
    public string PreviousScene { get; private set; }

    /// <summary>The scene to load after the Loading Screen.</summary>
    public string NextScene { get; private set; }

    #endregion

    #region Loading Methods

    /// <summary>
    /// Load the next scene and cache data.
    /// </summary>
    /// <param name="nextScene">Name of next scene.</param>
    /// <param name="loadScreen">Should the Loading Screen be loaded first?</param>
    /// <param name="unloadUnused">Should unused assests be unloaded?</param>
    /// <param name="garbageCollect">Should the garbage collector be run?</param>
    public static void LoadScene(string nextScene, bool loadScreen = false, bool unloadUnused = false, bool garbageCollect = false)
    {
        Main.PreviousScene = Application.loadedLevelName;
        Main.NextScene = nextScene;

        Application.LoadLevel(loadScreen ? "Loading Screen" : nextScene);
        if (unloadUnused) Resources.UnloadUnusedAssets();
        if (garbageCollect) GC.Collect();
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