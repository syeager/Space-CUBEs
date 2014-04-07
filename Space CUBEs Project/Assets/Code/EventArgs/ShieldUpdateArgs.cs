// Steve Yeager
// 2.20.2014

using System;

public class ShieldUpdateArgs : EventArgs
{
    public readonly float max;
    public readonly float amount;
    public readonly float shield;


    public ShieldUpdateArgs(float max, float amount, float shield)
    {
        this.max = max;
        this.amount = amount;
        this.shield = shield;
    }
}