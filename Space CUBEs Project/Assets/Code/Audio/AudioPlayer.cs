// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.17
// Edited: 2014.06.15

using UnityEngine;
using Bus = AudioManager.Bus;

/// <summary>
/// Plays an audio clip and responds
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : PoolObject
{
    #region References

    [HideInInspector]
    public AudioSource myAudio;

    [HideInInspector]
    public Transform myTransform;

    #endregion

    #region Public Fields

    /// <summary>Bus that this clip belongs to.</summary>
    public Bus bus;

    #endregion

    #region Private Fields

    /// <summary>Volume level scale for this clip.</summary>
    private float levelScale;

    #endregion

    #region Public Methods

    /// <summary>
    /// Play audio clip in world position.
    /// </summary>
    /// <param name="position">World position to play clip.</param>
    /// <param name="levelScale">Local level scale to cache.</param>
    /// <param name="level">Volume level from Audio Manager.</param>
    /// <param name="muted">Mute from Audio Manager.</param>
    public void PlayClipAtPoint(Vector3 position, float levelScale, float level, bool muted)
    {
        myTransform.position = position;
        Play(levelScale, level, muted);
    }


    /// <summary>
    /// Play the audio clip.
    /// </summary>
    /// <param name="levelScale">Local level scale to cache.</param>
    /// <param name="level">Volume level from Audio Manager.</param>
    /// <param name="muted">Mute from Audio Manager.</param>
    public void Play(float levelScale, float level, bool muted)
    {
        this.levelScale = levelScale;
        myAudio.mute = muted;
        myAudio.volume = levelScale * level;
        myAudio.Play();
        Invoke("Disable", myAudio.clip.length);
    }


    /// <summary>
    /// Set the audio's volume level.
    /// </summary>
    /// <param name="level">Volume level from Audio Manager.</param>
    public void SetLevel(float level)
    {
        myAudio.volume = levelScale * level;
    }


    /// <summary>
    /// Set the audio's mute status.
    /// </summary>
    /// <param name="muted">Mute from Audio Manager.</param>
    public void SetMuted(bool muted)
    {
        myAudio.mute = muted;
    }


    /// <summary>
    /// Update volume.
    /// </summary>
    /// <param name="level">Volume level from Audio Manager.</param>
    /// <param name="muted">Mute from Audio Manager.</param>
    public void UpdateVolume(float level, bool muted)
    {
        myAudio.volume = levelScale * level;
        myAudio.mute = muted;
    }

    #endregion
}