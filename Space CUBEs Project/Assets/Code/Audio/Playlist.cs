// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.17
// Edited: 2014.06.19

using Annotations;
using LittleByte.Debug.Attributes;
using UnityEngine;
using Bus = AudioManager.Bus;

/// <summary>
/// Plays a music track.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Playlist : MonoBehaviour
{
    #region Public Fields

    [NotNull]
    public AudioSource myAudio;

    [NotEmpty]
    public string playlistName;

    /// <summary>AudioManager bus.</summary>
    public Bus bus;

    /// <summary>Don't get destroyed on load.</summary>
    public bool persist;

    /// <summary>Local volume level scale.</summary>
    public float levelScale = 1f;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        AudioManager.AddPlaylist(playlistName, this);
    }


    [UsedImplicitly]
    private void OnDestroy()
    {
        AudioManager.RemovePlaylist(playlistName);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Update playlist's volume.
    /// </summary>
    /// <param name="volume">New volume to update to.</param>
    public void UpdateVolume(Volume volume)
    {
        myAudio.volume = volume.level * levelScale;
        myAudio.mute = volume.muted;
    }


    /// <summary>
    /// Set local volume level scale.
    /// </summary>
    /// <param name="value">Value to set to. 0-1.</param>
    public void UpdateLevelScale(float value)
    {
        if (value == 0f)
        {
            levelScale = 0f;
            myAudio.volume = 0f;
        }
        else
        {
            myAudio.volume /= levelScale;
            levelScale = value;
            myAudio.volume *= value;
        }
    }


    /// <summary>
    /// Start playing audio from beginning.
    /// </summary>
    public void Play()
    {
        myAudio.Play();
    }


    /// <summary>
    /// Pause audio.
    /// </summary>
    public void Pause()
    {
        myAudio.Pause();
    }


    /// <summary>
    /// Stop audio.
    /// </summary>
    public void Stop()
    {
        myAudio.Stop();
    }

    #endregion
}