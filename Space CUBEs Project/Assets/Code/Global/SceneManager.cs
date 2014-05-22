// Steve Yeager
// 8.18.2013

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
    public static void LoadScene(string nextScene, bool load = false, Dictionary<string, object> sceneData = null)
    {
        Main.levelData = sceneData ?? new Dictionary<string, object>();
        Main.previousScene = Application.loadedLevelName;
        Main.nextScene = nextScene;

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    }


    /// <summary>
    /// Reloads the current scene without the Loading Screen.
    /// <param name="sceneData">Data to save for the next scene.</param>
    /// </summary>
    public static void ReloadScene(Dictionary<string, object> sceneData = null)
    {
        if (sceneData != null)
        {
            Main.levelData = sceneData;
        }
        Application.LoadLevel(Application.loadedLevel);
    }

    #endregion
}