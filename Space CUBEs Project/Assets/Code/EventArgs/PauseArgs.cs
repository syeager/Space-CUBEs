// Steve Yeager
// 5.15.2014

using System;

/// <summary>
/// 
/// </summary>
public class PauseArgs : EventArgs
{
    public readonly bool paused;


    public PauseArgs(bool paused)
    {
        this.paused = paused;
    }
}