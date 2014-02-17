// Steve Yeager
// 2.17.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for the Options Menu.
/// </summary>
public class OptionsMenuManager : MonoBase
{
    #region Button Methods

    public void GameReset()
    {
        PlayerPrefs.DeleteAll();
    }


    public void LoadMainMenu()
    {
        GameData.LoadLevel("Main Menu");
    }

    #endregion
}