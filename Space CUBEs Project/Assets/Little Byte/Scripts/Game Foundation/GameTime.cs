// Little Byte Games

using System;
using System.Collections;
using LittleByte.Data;
using LittleByte.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Wrapper for UnityEngine.Time. Handles pausing.
/// </summary>
public static class GameTime
{
    #region Public Fields

    /// <summary>TimeScale before the game is paused.</summary>
    private static float cachedTimeScale = 1f;

    #endregion

    #region Private Fields

    private static Job unscaledTimeJob;

    #endregion

    #region Properties

    /// <summary>Time in seconds since last frame.</summary>
    public static float DeltaTime
    {
        get { return Time.deltaTime; }
    }

    /// <summary>Member for timeScale.</summary>
    private static float timeScale = 1f;

    /// <summary>Keeps Time.timeScale and Time.fixedDeltaTime in sync.</summary>
    public static float TimeScale
    {
        get { return timeScale; }
        set
        {
            timeScale = value;
            Time.timeScale = timeScale;
        }
    }

    /// <summary>Delta time that is uneffected by time scale.</summary>
    public static float UnscaledDeltaTime { get; private set; }

    /// <summary>FPS cap. FrameRate won't exceed but isn't guaranteed to reach.</summary>
    private static int targetFPS;

    /// <summary>FPS cap. FrameRate won't exceed but isn't guaranteed to reach.</summary>
    public static int TargetFPS
    {
#if UNITY_EDITOR
        get { return Application.isPlaying ? targetFPS : EditorPrefs.GetInt(TargetFPSKey); }
        set
        {
            if (Application.isPlaying)
            {
                targetFPS = value;
            }
            else
            {
                EditorPrefs.SetInt(TargetFPSKey, value);
            }
        }
#else
        get { return targetFPS; }
        set { targetFPS = value; }
#endif
    }

    /// <summary>Is the game paused?</summary>
    public static bool Paused { get; private set; }

    #endregion

    #region Const Fields

    /// <summary>Data path for targetFPS.</summary>
    private const string TargetFPSKey = "Target FPS";

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
        CapFPS(SaveData.Load(TargetFPSKey, GameSettings.SettingsFolder, MaxFPS));
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
        Paused = pause;
        PausedEvent.Fire(pauser, new PauseArgs(Paused));

        if (Paused && zeroTimeScale)
        {
            cachedTimeScale = TimeScale;
            TimeScale = 0f;
            unscaledTimeJob = new Job(UnscaledTimer());
        }
        else
        {
            TimeScale = cachedTimeScale;
            if (unscaledTimeJob != null)
            {
                unscaledTimeJob.Kill();
            }
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
        Pause(!Paused, zeroTimeScale, pauser);
        return Paused;
    }

    /// <summary>
    /// Set targetFPS.
    /// </summary>
    /// <param name="targetFPS">FPS limit.</param>
    public static void CapFPS(int targetFPS)
    {
        targetFPS = Mathf.Clamp(targetFPS, MinFPS, MaxFPS);
        TargetFPS = targetFPS;
        Application.targetFrameRate = targetFPS;
        SaveData.Save(TargetFPSKey, targetFPS, GameSettings.SettingsFolder);
    }

    #endregion

    #region Private Methods

    private static IEnumerator UnscaledTimer()
    {
        float lastTime = Time.realtimeSinceStartup;
        while (true)
        {
            yield return null;
            UnscaledDeltaTime = Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;
        }
    }

    #endregion
}