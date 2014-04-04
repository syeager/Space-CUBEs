﻿// Steve Yeager
// 2.17.2014

using UnityEngine;

/// <summary>
/// Manager for the Options Menu.
/// </summary>
public class OptionsMenuManager : MonoBase
{
    #region Public Fields

    public UISlider joystickSensitivity;
    public UISlider joystickXBuffer;
    public UISlider joystickYBuffer;
    public UISlider joystickDeadzone;
    public UIToggle trailRenderer;

    #endregion


    #region MonoBehaviour Overrides

    internal void Start()
    {
        joystickSensitivity.value = GameSettings.Main.joystickSensitivity;
        joystickXBuffer.value = GameSettings.Main.joystickXBuffer;
        joystickYBuffer.value = GameSettings.Main.joystickYBuffer;
        joystickDeadzone.value = GameSettings.Main.joystickDeadzone;
        trailRenderer.value = GameSettings.Main.trailRenderer;
    }

    #endregion

    #region Button Methods

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