// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.06.07

using Annotations;
using LittleByte.Data;

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

    #endregion

    #region Quality Fields

    public bool trailRenderer;

    #endregion

    #region Const Fields

    /// <summary>Data folder of setting files.</summary>
    public const string SettingsFolder = @"Settings\";

    public const string InputFolder = SettingsFolder + @"Input\";

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
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
    }

    #endregion

    #region Private Methods

    private void Load()
    {
        joystickSensitivity = SaveData.Load("Joystick Sensitivity", InputFolder, 0.35f);
        joystickSensitivity = SaveData.Load("Joystick xBuffer", InputFolder, 0.5f);
        joystickSensitivity = SaveData.Load("Joystick yBuffer", InputFolder, 0.5f);
        joystickSensitivity = SaveData.Load("Joystick Deadzone", InputFolder, 0.25f);
    }

    #endregion
}