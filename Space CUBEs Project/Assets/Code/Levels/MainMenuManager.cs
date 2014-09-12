﻿// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.15
// Edited: 2014.05.27

using Annotations;
using LittleByte.Data;
using SpaceCUBEs;
using UnityEngine;

/// <summary>
/// Manager for the Main Menu.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    #region Public Fields

#if DEBUG
    public Rect debugTouchRect;
#endif

    #endregion

    #region MonoBehaviour Overrides

#if DEBUG
    [UsedImplicitly]
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Debug Menu");
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2 && debugTouchRect.Contains(Input.GetTouch(0).position))
        {
            SceneManager.LoadScene("Debug Menu");
        }
#endif
    }
#endif

    #endregion

    #region Button Methods

    public void Play()
    {
        ConstructionGrid.selectedBuild = ConstructionGrid.DevBuilds[0];
        int unlocked = SaveData.Load<int>(LevelSelectManager.UnlockedLevelsKey, FormationLevelManager.LevelsFolder);
        SceneManager.LoadScene(((Levels)unlocked).ToString().SplitCamelCase(), true, true, true);
    }


    /// <summary>
    /// Exit game.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}