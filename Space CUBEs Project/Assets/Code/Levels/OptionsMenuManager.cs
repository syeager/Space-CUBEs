// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.02.17
// Edited: 2014.06.04

using LittleByte.Data;
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
    public ActivateButton[] fpsToggles;

    #endregion

    #region MonoBehaviour Overrides

    internal void Start()
    {
        joystickSensitivity.value = GameSettings.Main.joystickSensitivity;
        joystickXBuffer.value = GameSettings.Main.joystickXBuffer;
        joystickYBuffer.value = GameSettings.Main.joystickYBuffer;
        joystickDeadzone.value = GameSettings.Main.joystickDeadzone;
        trailRenderer.value = GameSettings.Main.trailRenderer;

        foreach (ActivateButton button in fpsToggles)
        {
            button.GetComponent<UIToggle>().value = int.Parse(button.value) == GameTime.targetFPS;
        }
    }

    #endregion

    #region Button Methods

    public void GameReset()
    {
        PlayerPrefs.DeleteAll();
        SaveData.DeleteAll();
        GameStart.Main.UpdateVersions(true);
    }


    public void LoadMainMenu()
    {
        Save();
        SceneManager.LoadScene("Main Menu");
    }


    public void SetFPS(string value)
    {
        GameTime.CapFPS(int.Parse(value));
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