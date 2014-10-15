// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.10.15

using System;
using UnityEngine;

public class BuildFinishedArgs : EventArgs
{
    public readonly GameObject ship;
    public readonly ShipStats stats;


    public BuildFinishedArgs(GameObject ship, ShipStats stats)
    {
        this.ship = ship;
        this.stats = stats;
    }
}