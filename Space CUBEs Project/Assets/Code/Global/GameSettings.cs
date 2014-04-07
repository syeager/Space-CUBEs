// Steve Yeager
// 8.17.2013

using Annotations;
using UnityEngine;

/// <summary>
/// Singleton to hold all game settings.
/// </summary>
public class GameSettings : Singleton<GameSettings>
{
    #region Testing Fields

#if DEBUG
    public bool invincible;
    public bool jumpToBoss;
#endif

    #endregion

    #region Volume Fields

    public float volumeSE;
    public float volumeMusic;

    #endregion

    #region Screen Fields

    public static Vector2 aspectRatio { get; private set; }

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
        PlayerPrefs.SetFloat("Joystick Sensitivity", Main.joystickSensitivity);
        PlayerPrefs.SetFloat("Joystick xBuffer", Main.joystickXBuffer);
        PlayerPrefs.SetFloat("Joystick yBuffer", Main.joystickYBuffer);
        PlayerPrefs.SetFloat("Joystick Deadzone", Main.joystickDeadzone);
    }

    #endregion

    #region Private Methods

    private void Load()
    {
        joystickSensitivity = PlayerPrefs.GetFloat("Joystick Sensitivity", 0.5f);
        joystickXBuffer = PlayerPrefs.GetFloat("Joystick xBuffer", 0.05f);
        joystickYBuffer = PlayerPrefs.GetFloat("Joystick yBuffer", 0.05f);
        joystickDeadzone = PlayerPrefs.GetFloat("Joystick Deadzone", 0.3f);
    }


    private void GetAspectRatio()
    {
        double ratio = System.Math.Round((float)Screen.width / (float)Screen.height, 2);

        if (ratio >= 1.7)
        {
            aspectRatio = new Vector2(16f, 9f);
        }
        else if (ratio >= 1.6)
        {
            aspectRatio = new Vector2(16f, 10f);
        }
    }

    #endregion
}