// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.14
// Edited: 2014.06.25

using UnityEngine;

/// <summary>
/// Laser shot by Guard enemy.
/// </summary>
public class GuardLaser : Weapon
{
    #region Public Fields

    public GameObject laser;
    public float damage;

    #endregion

    #region Weapon Overrides

    public override Coroutine Activate(bool pressed)
    {
        if (pressed)
        {
            laser.SetActive(true);
            laser.GetComponent<Hitbox>().Initialize(myShip, damage);
        }
        else
        {
            laser.SetActive(false);
        }

        return null;
    }

    #endregion
}