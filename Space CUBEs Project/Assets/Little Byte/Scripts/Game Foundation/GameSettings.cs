﻿// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.09.01

using LittleByte.Data;
using UnityEngine;

/// <summary>
/// Singleton to hold all game settings.
/// </summary>
public class GameSettings : Singleton<GameSettings>
{
    #region Testing Fields

    public bool invincible;
    public bool jumpToBoss;

    #endregion

    #region Joystick Fields

    public float joystickSensitivity;
    public float joystickXBuffer;
    public float joystickYBuffer;
    public float joystickDeadzone;

    public const float JoystickSensitivityDefault = 0.35f;
    public const float JoystickXBufferDefault = 0.1f;
    public const float JoystickYBufferDefault = 0.1f;
    public const float JoystickDeadzoneDefault = 0.25f;

    #endregion

    #region Quality Fields

    public int qualityLevel = QualityLevelDefault;

    #endregion

    #region Const Fields

    /// <summary>Data folder of setting files.</summary>
    public const string SettingsFolder = @"Settings/";

    public const string InputFolder = SettingsFolder + @"Input/";

    public const string QualityLevelKey = "Quality Level";
    public const int QualityLevelDefault = 2;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        if (!enabled) return;

        Load();
    }

    #endregion

    #region Public Methods

    public static void Save()
    {
        SaveData.Save("Joystick Sensitivity", Main.joystickSensitivity, InputFolder);
        SaveData.Save("Joystick xBuffer", Main.joystickSensitivity, InputFolder);
        SaveData.Save("Joystick yBuffer", Main.joystickSensitivity, InputFolder);
        SaveData.Save("Joystick Deadzone", Main.joystickSensitivity, InputFolder);
        SaveData.Save(QualityLevelKey, Main.qualityLevel, SettingsFolder);
    }

    #endregion

    #region Private Methods

    private void Load()
    {
        joystickSensitivity = SaveData.Load("Joystick Sensitivity", InputFolder, JoystickSensitivityDefault);
        joystickSensitivity = SaveData.Load("Joystick xBuffer", InputFolder, JoystickXBufferDefault);
        joystickSensitivity = SaveData.Load("Joystick yBuffer", InputFolder, JoystickYBufferDefault);
        joystickSensitivity = SaveData.Load("Joystick Deadzone", InputFolder, JoystickDeadzoneDefault);
        qualityLevel = SaveData.Load(QualityLevelKey, SettingsFolder, QualityLevelDefault);
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    #endregion
}