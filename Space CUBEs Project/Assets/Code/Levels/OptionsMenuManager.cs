// Steve Yeager
// 2.17.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for the Options Menu.
/// </summary>
public class OptionsMenuManager : MonoBase
{
    #region Public Fields

    public UILabel invincibility_Label;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        invincibility_Label.text = "Invincibility is " + (GameSettings.Main.invincible ? "on" : "off");
    }

    #endregion

    #region Button Methods

    public void ToggleInvincibility()
    {
        GameSettings.Main.invincible = !GameSettings.Main.invincible;
        invincibility_Label.text = "Invincibility is " + (GameSettings.Main.invincible ? "on" : "off");
    }


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