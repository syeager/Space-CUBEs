// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.10
// Edited: 2014.08.24

using Annotations;
using UnityEngine;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    #region References

    public UILabel levelToLoad;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        // set up GUI
        levelToLoad.text = SceneManager.NextScene + "...";

        // clean
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }


    [UsedImplicitly]
    private void Start()
    {
        Application.LoadLevel(SceneManager.NextScene);
    }

    #endregion
}