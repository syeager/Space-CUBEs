// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.03
// Edited: 2014.10.02

using System;
using UnityEngine;

public class BuildFinishedArgs : EventArgs
{
    public readonly GameObject ship;
    public readonly float health;
    public readonly float shield;
    public readonly float speed;
    public readonly float damage;


    public BuildFinishedArgs(GameObject ship, float health, float shield, float speed, float damage)
    {
        this.ship = ship;
        this.health = health;
        this.shield = shield;
        this.speed = speed;
        this.damage = damage;
    }
}