using UnityEngine;
using System.Collections;
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
