// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.04
// Edited: 2014.07.04

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SyringeLaser : ProjectingLaser
{
    #region Public Fields

    public float stealingModifier;

    #endregion

    #region MonoBehaviour Overrides

    protected override void OnTriggerStay(Collider other)
    {
        var oppHealth = other.gameObject.GetComponent(typeof(ShieldHealth)) as ShieldHealth;
        if (oppHealth != null)
        {
            sender.MyHealth.ChangeHealth(-oppHealth.RecieveHit(sender, damage * deltaTime, true) * stealingModifier);
        }
    }

    #endregion
}