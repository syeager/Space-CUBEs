// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.10.22

using System;
using UnityEngine;

namespace SpaceCUBEs
{
    public class BuildFinishedArgs : EventArgs
    {
        public readonly GameObject ship;
        public readonly ShipStats stats;
        public readonly int trimColor;


        public BuildFinishedArgs(GameObject ship, ShipStats stats, int trimColor)
        {
            this.ship = ship;
            this.stats = stats;
            this.trimColor = trimColor;
        }
    }
}