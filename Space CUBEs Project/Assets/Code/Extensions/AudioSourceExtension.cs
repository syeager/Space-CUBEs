// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.11
// Edited: 2014.06.11

using UnityEngine;

/// <summary>
/// Extension methods for Unity's AudioSource component.
/// </summary>
public static class AudioSourceExtension
{
    /// <summary>
    /// Set and play audio clip.
    /// </summary>
    /// <param name="audio">AudioSource instance.</param>
    /// <param name="clip">Clip to play.</param>
    /// <returns>Clip length in seconds.</returns>
    public static float Play(this AudioSource audio, AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
        return clip.length;
    }
}