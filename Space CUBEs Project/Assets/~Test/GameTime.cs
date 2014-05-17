// Steve Yeager
// 5.17.2014

using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Wrapper for UnityEngine.Time. Handles pausing.
/// </summary>
public static class GameTime
{
    #region Public Fields

    /// <summary>TimeScale before the game is paused.</summary>
    private static float cachedTimeScale;

    /// <summary>Unmodified Time.fixedDeltaTime.</summary>
    private static float fixedDeltaTime;

    #endregion

    #region Properties

    /// <summary>Time in seconds since last frame.</summary>
    public static float deltaTime
    {
        get { return Time.deltaTime; }
    }

    /// <summary>Member for timeScale.</summary>
    private static float _timeScale = 1f;

    /// <summary>Keeps Time.timeScale and Time.fixedDeltaTime in sync.</summary>
    public static float timeScale
    {
        get { return _timeScale; }
        set
        {
            _timeScale = value;
            Time.timeScale = _timeScale;
            Time.fixedDeltaTime = _timeScale * fixedDeltaTime;
        }
    }

    /// <summary>FPS cap. FrameRate won't exceed but isn't guaranteed to reach.</summary>
    public static int targetFPS { get; set; }

    /// <summary>Is the game paused?</summary>
    public static bool paused { get; private set; }

    #endregion

    #region Const Fields

    /// <summary>Data path for targetFPS.</summary>
    private const string TargetFPSPath = "Target FPS";

    /// <summary>Max cap for FPS.</summary>
    public const int MaxFPS = 60;

    /// <summary>Min cap for FPS.</summary>
    public const int MinFPS = 30;

    #endregion

    #region Events

    /// <summary>Fired when the game paused state is changed.</summary>
    public static EventHandler<PauseArgs> PausedEvent;

    #endregion


    #region Public Methods

    /// <summary>
    /// Cache fixedDeltaTime.
    /// </summary>
    public static void Initialize()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
        CapFPS(PlayerPrefs.GetInt(TargetFPSPath, 60));
    }


    /// <summary>
    /// Pause the game.
    /// </summary>
    /// <param name="pause">Should the game be paused?</param>
    /// <param name="zeroTimeScale">Should the timeScale be set to 0 if paused?</param>
    /// <param name="pauser">The Object pausing/unpausing the game.</param>
    public static void Pause(bool pause, bool zeroTimeScale = true, Object pauser = null)
    {
        Debugger.Log("Game " + (pause ? "Paused" : "Unpaused"), pauser, Debugger.LogTypes.LevelEvents);
        paused = pause;
        PausedEvent.Fire(pauser, new PauseArgs(paused));

        if (paused && zeroTimeScale)
        {
            cachedTimeScale = timeScale;
            timeScale = 0f;
        }
        else
        {
            timeScale = cachedTimeScale;
        }
    }


    /// <summary>
    /// Toggle game's paused state.
    /// </summary>
    /// <param name="zeroTimeScale">Should the timeScale be set to 0 if paused?</param>
    /// <param name="pauser">The Object pausing/unpausing the game.</param>
    /// <returns>True, if the game is now paused.</returns>
    public static bool TogglePause(bool zeroTimeScale = true, Object pauser = null)
    {
        Pause(!paused, zeroTimeScale, pauser);
        return paused;
    }


    /// <summary>
    /// Set targetFPS.
    /// </summary>
    /// <param name="targetFPS">FPS limit.</param>
    public static void CapFPS(int targetFPS)
    {
        targetFPS = Mathf.Clamp(targetFPS, MinFPS, MaxFPS);
        GameTime.targetFPS = targetFPS;
        Application.targetFrameRate = targetFPS;
        PlayerPrefs.SetInt(TargetFPSPath, targetFPS);
    }

    #endregion
}