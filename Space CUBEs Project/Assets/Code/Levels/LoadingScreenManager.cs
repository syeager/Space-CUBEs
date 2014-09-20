// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.10
// Edited: 2014.08.24

using Annotations;
using UnityEngine;
using System;
using LittleByte;

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
    }


    [UsedImplicitly]
    private void Start()
    {
        Application.LoadLevel(SceneManager.NextScene);
    }

    #endregion
}