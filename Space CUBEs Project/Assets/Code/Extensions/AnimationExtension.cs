// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.08

using System;
using UnityEngine;

/// <summary>
/// Extension methods for Unity's Animation component.
/// </summary>
public static class AnimationExtension
{
    /// <summary>
    /// Play an animation in reverse with reset normalized time and speed.
    /// </summary>
    /// <param name="animation">Animation component instance.</param>
    /// <param name="clip">Name of clip to play.</param>
    [Obsolete]
    public static void PlayReverse(this Animation animation, string clip, bool backwards)
    {
        animation[clip].normalizedSpeed = backwards ? -1f : 1f;
        animation[clip].normalizedTime = backwards ? 1f : 0f;
        animation.Play(clip);
    }


    /// <summary>
    /// Play an animation clip.
    /// </summary>
    /// <param name="animation">Animation component instance.</param>
    /// <param name="clip">Clip to play. If not in animation then it will be added.</param>
    /// <param name="reverse">Should the animation be played in reverse?</param>
    public static void Play(this Animation animation, AnimationClip clip, bool reverse = false)
    {
        if (clip == null) return;

        string clipName = clip.name;
        if (animation.GetClip(clipName) == null)
        {
            animation.AddClip(clip, clipName);
        }
        animation[clipName].normalizedSpeed = reverse ? -1f : 1f;
        animation[clipName].normalizedTime = reverse ? 1f : 0f;
        animation.Play(clipName);
    }
}