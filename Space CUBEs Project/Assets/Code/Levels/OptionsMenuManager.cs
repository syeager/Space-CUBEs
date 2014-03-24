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
    public UISlider joystickSensitivity;
    public UISlider joystickXBuffer;
    public UISlider joystickYBuffer;
    public UISlider joystickDeadzone;
    public UIToggle trailRenderer;

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        invincibility_Label.text = "Invincibility is " + (GameSettings.Main.invincible ? "on" : "off");
        joystickSensitivity.value = GameSettings.Main.joystickSensitivity;
        joystickXBuffer.value = GameSettings.Main.joystickXBuffer;
        joystickYBuffer.value = GameSettings.Main.joystickYBuffer;
        joystickDeadzone.value = GameSettings.Main.joystickDeadzone;
        trailRenderer.value = GameSettings.Main.trailRenderer;
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
        Save();
        GameData.LoadLevel("Main Menu");
    }

    #endregion

    #region Private Methods

    private void Save()
    {
        GameSettings.Main.joystickSensitivity = joystickSensitivity.value;
        GameSettings.Main.joystickXBuffer = joystickXBuffer.value;
        GameSettings.Main.joystickYBuffer = joystickYBuffer.value;
        GameSettings.Main.joystickDeadzone = joystickDeadzone.value;
        GameSettings.Main.trailRenderer = trailRenderer.value;

        GameSettings.Save();
    }

    #endregion
}