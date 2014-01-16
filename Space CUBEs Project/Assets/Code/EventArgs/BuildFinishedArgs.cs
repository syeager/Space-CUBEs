﻿// Steve Yeager
// 12.3.2013

using UnityEngine;
using System;
using System.Collections.Generic;

public class BuildFinishedArgs : EventArgs
{
    public readonly GameObject ship;
    public readonly float health;
    public readonly float shield;
    public readonly float speed;


    public BuildFinishedArgs(GameObject ship, float health, float shield, float speed)
    {
        this.ship = ship;
        this.health = health;
        this.shield = shield;
        this.speed = speed;
    }
}