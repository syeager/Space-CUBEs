// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.15
// Edited: 2014.06.15

using System;

/// <summary>
/// Holds volume strength and if a volume is currently muted.
/// </summary>
[Serializable]
public class Volume
{
    #region Public Fields

    /// <summary>Strength of the volume. 0-1.</summary>
    public float level;

    /// <summary>Is the volume muted?</summary>
    public bool muted;

    #endregion

    #region Constructors

    public Volume()
    {
        level = 1f;
        muted = false;
    }


    public Volume(float level, bool muted)
    {
        this.level = level;
        this.muted = muted;
    } 

    #endregion

    #region Operators

    public static float operator *(Volume left, Volume right)
    {
        return left.level * right.level;
    }


    public static float operator *(Volume left, float right)
    {
        return left.level * right;
    }


    public static implicit operator bool(Volume volume)
    {
        return volume.muted;
    } 

    #endregion
}