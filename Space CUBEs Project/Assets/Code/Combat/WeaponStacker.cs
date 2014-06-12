// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.31
// Edited: 2014.06.11

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class WeaponStacker : Weapon
{
    #region Public Fields

    public Weapon[] weapons;
    public float[] delays;

    #endregion

    #region Weapon Overrides

    public override void Initialize(Ship sender)
    {
        base.Initialize(sender);

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Initialize(sender);
        }
    }


    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (pressed)
        {
            StartCoroutine(Fire(multiplier));
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float multiplier)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (delays[i] > 0f)
            {
                yield return new WaitForSeconds(delays[i]);
            }

            weapons[i].Activate(true, multiplier);
        }
    }

    #endregion
}