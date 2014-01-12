// Steve Yeager
// 1.11.2014

using System;

public class WaveUpdateArgs : EventArgs
{
    public readonly int wave;


    public WaveUpdateArgs(int wave)
    {
        this.wave = wave;
    }
}