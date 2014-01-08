// Steve Yeager
// 1.5.2014

using System;

public class MultiplierUpdateArgs : EventArgs
{
    public readonly int multiplierGained;
    public readonly int multiplier;


    public MultiplierUpdateArgs(int multiplierGained, int multiplier)
    {
        this.multiplierGained = multiplierGained;
        this.multiplier = multiplier;
    }
}
