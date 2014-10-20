// Little Byte Games
// Author: Steve Yeager
// Created: 2014.05.28
// Edited: 2014.09.17

using Annotations;
using LittleByte;
using SpaceCUBEs;
using UnityEngine;

/// <summary>
/// Manages the Dev and Game Splash Screens.
/// </summary>
public class SplashScreenManager : Singleton<SplashScreenManager>
{
    #region Public Fields

    /// <summary>Time in seconds to display Dev Splash.</summary>
    public float devTime;

    /// <summary>Time in seconds to display Game Splash.</summary>
    public float gameTime;

    #endregion

    #region Private Fields

    /// <summary>Splash screens needed to be shown.</summary>
    private enum States
    {
        Dev,
        Game
    }

    /// <summary>Current splash being shown.</summary>
    private States state;

    #endregion

    #region Dev Fields

    /// <summary>GUI title for Little Byte.</summary>
    public GameObject devTitle;

    #endregion

    #region Game Fields

    /// <summary>GUI title for the game.</summary>
    public GameObject gameTitle;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        Invoke("ShowDevSplash", 1f);
    }


    [UsedImplicitly]
    private void Update()
    {
#if UNITY_STANDALONE
        bool skip = Input.anyKeyDown;
#else
        bool skip = Input.GetKeyDown(KeyCode.Escape) || (Input.touchCount > 0 && Input.GetTouch(0).tapCount > 0);
#endif

        if (skip)
        {
            if (state == States.Dev)
            {
                ShowGameSplash();
            }
            else
            {
                LoadMainMenu();
            }
        }
    }

    #endregion

    #region Private Methods

    [UsedImplicitly]
    private void ShowDevSplash()
    {
        state = States.Dev;
        devTitle.SetActive(true);
        Invoke("ShowGameSplash", devTime);
    }


    private void ShowGameSplash()
    {
        CancelInvoke();
        state = States.Game;
        devTitle.SetActive(false);
        gameTitle.SetActive(true);
        Invoke("LoadMainMenu", gameTime);
    }


    private void LoadMainMenu()
    {
        CancelInvoke();
        SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.MainMenu));
    }

    #endregion
}