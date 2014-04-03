// Steve Yeager
// 

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


    public override void Activate(bool pressed, float multiplier)
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