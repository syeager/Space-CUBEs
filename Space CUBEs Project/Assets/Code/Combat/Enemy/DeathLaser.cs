// Steve Yeager
// 4.1.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class DeathLaser : Weapon
{
    #region Public Fields
    
    public GameObject laser;
    public HitInfo hitInfo;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            laser.GetComponent<Hitbox>().Initialize(myShip, hitInfo);
            laser.SetActive(true);
        }
        else
        {
            laser.SetActive(false);
        }
    }

    #endregion
}