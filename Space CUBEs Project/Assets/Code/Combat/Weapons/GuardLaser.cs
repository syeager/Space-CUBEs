// Steve Yeager
// 4.14.2014

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

    public override void Activate(bool pressed, float multiplier)
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
    }

    #endregion
}