// Steve Yeager
// 3.26.2014

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[Obsolete("Use MasterAudio", true)]
public class AudioManager :Singleton<AudioManager>
{
    #region Public Fields

    public GameObject audioPrefab;

    #endregion

    #region Static Fields

    public enum AudioGroups
    {
        Music,
        Game,
        UI
    };

    private static Dictionary<AudioGroups, float> volumes = new Dictionary<AudioGroups, float>
    {
        {AudioGroups.Music, 1f},
        {AudioGroups.Game, 1f},
        {AudioGroups.UI, 1f}
    };

    #endregion

    #region Const Fields

    public static readonly Dictionary<AudioGroups, string> VolumePaths = new Dictionary<AudioGroups, string>
    {
        {AudioGroups.Music, "Music Volume"},
        {AudioGroups.Game, "Game Volume"},
        {AudioGroups.UI, "UI Volume"},
    };

    #endregion

    #region Properties

    public static bool muted { get; private set; }

    #endregion

    #region Static Methods

    public static void Initialize()
    {
        LoadVolumes();
    }


    public static void LoadVolumes()
    {
        foreach (var audioGroup in volumes.Keys)
        {
            volumes[audioGroup] = PlayerPrefs.GetFloat(VolumePaths[audioGroup], 1f);
        }
    }


    public static void SetVolume(AudioGroups audioGroup, float volume, bool save = true)
    {
        if (volume > 0f)
        {
            volume = 0f;
        }
        volumes[audioGroup] = volume;

        if (save)
        {
            PlayerPrefs.SetFloat(VolumePaths[audioGroup], volume);
        }
    }


    public static void Mute(bool mute)
    {
        muted = mute;
    }


    public static bool ToggleMute()
    {
        Mute(!muted);
        return muted;
    }


    public void Play(AudioClip clip, Vector3 position, AudioGroups audioGroup, float volumeScale = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, muted ? 0f : (volumes[audioGroup] * volumeScale));
    }

    #endregion
}