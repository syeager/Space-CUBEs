using UnityEngine;
using System.Collections;
using System;

public class HealthUpdateArgs : EventArgs
{
    public readonly float max;
    public readonly float amount;
    public readonly float health;


    public HealthUpdateArgs(float max, float amount, float health)
    {
        this.max = max;
        this.amount = amount;
        this.health = health;
    }
}
