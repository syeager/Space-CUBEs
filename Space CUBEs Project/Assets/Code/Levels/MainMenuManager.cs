// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.15
// Edited: 2014.05.27

using Annotations;
using UnityEngine;
using System.Linq;

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

    public void LoadPlay()
    {
        SceneManager.Main.currentBuild = GameStart.DevBuilds.ElementAt(0).Key;
        SceneManager.LoadScene("Level Select Menu");
    }


    public void LoadGarage()
    {
        SceneManager.LoadScene("Garage");
    }


    public void LoadStore()
    {
        SceneManager.LoadScene("Store", true);
    }


    public void LoadOptions()
    {
        SceneManager.LoadScene("Options Menu");
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