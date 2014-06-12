// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.08
// Edited: 2014.06.08

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
    public static void PlayReverse(this Animation animation, string clip, bool backwards)
    {
        animation[clip].normalizedSpeed = backwards ? -1f : 1f;
        animation[clip].normalizedTime = backwards ? 1f : 0f;
        animation.Play(clip);
    }
}