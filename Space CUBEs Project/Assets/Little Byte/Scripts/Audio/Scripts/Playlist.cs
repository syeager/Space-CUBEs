// Little Byte Games

using Annotations;
using LittleByte.Audio;
using LittleByte.Debug.Attributes;
using UnityEngine;

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
    public AudioManager.Bus bus;

    /// <summary>Don't get destroyed on load.</summary>
    public bool persist;

    /// <summary>Local volume level scale.</summary>
    public float levelScale = 1f;

    #endregion

    #region Private Fields

    /// <summary>Percentage of volume that is considered the new max volume.</summary>
    private float maxLimit;

    /// <summary>Volume level given to by AudioManager.</summary>
    private float givenLevel;

    /// <summary>Is the audio source currently paused?</summary>
    private bool paused;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        maxLimit = myAudio.volume;

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
        givenLevel = volume.level;
        myAudio.volume = givenLevel * levelScale * maxLimit;
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
            levelScale = value;
            myAudio.volume = givenLevel * value * maxLimit;
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
    /// <param name="pause">Should the playlist be paused?</param>
    public void Pause(bool pause)
    {
        if (pause && myAudio.isPlaying)
        {
            paused = true;
            myAudio.Pause();
        }
        else if (paused)
        {
            paused = false;
            myAudio.Play();
        }
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