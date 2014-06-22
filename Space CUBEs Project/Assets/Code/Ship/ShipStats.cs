// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.20
// Edited: 2014.06.21

using System;

/// <summary>
/// Holds stats for a ship build.
/// </summary>
[Serializable]
public class ShipStats
{
    #region Public Fields

    public float health;
    public float shield;
    public float speed;
    public float damage;

    #endregion

    #region Constructors

    public ShipStats()
    {
    }


    public ShipStats(float health, float shield, float speed, float damage)
    {
        this.health = health;
        this.shield = shield;
        this.speed = speed;
        this.damage = damage;
    }

    #endregion
}