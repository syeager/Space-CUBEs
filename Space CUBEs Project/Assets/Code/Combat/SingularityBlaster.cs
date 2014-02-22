// Steve Yeager
// 2.20.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class SingularityBlaster : Weapon
{
    #region Public Fields
    
    public GameObject BlackHoleMissle_Prefab;
    public Vector3 missleOffset;
    public HitInfo hitInfo;
    public float missleTime;
    public float missleSpeed;
    
    #endregion

    #region Private Fields

    private BlackHoleMissle missle;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            if (missle != null)
            {
                Release();
            }
            Fire(multiplier);
        }
        else if (missle != null)
        {
            Release();
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<SingularityBlaster>();
        comp.index = index;
        comp.cooldownSpeed = cooldownSpeed;
        comp.BlackHoleMissle_Prefab = BlackHoleMissle_Prefab;
        comp.missleOffset = missleOffset + myTransform.localPosition;
        comp.hitInfo = hitInfo;
        comp.missleTime = missleTime;
        comp.missleSpeed = missleSpeed;

        return comp;
    }

    #endregion

    #region Private Methods

    private void Fire(float multiplier)
    {
        missle = (Instantiate(BlackHoleMissle_Prefab, myTransform.position + myTransform.TransformDirection(missleOffset), myTransform.rotation) as GameObject).GetComponent<BlackHoleMissle>();
        missle.Initialize(myShip, hitInfo.MultiplyDamage(multiplier), missleTime, myTransform.forward*missleSpeed);
    }


    private void Release()
    {
        missle.Explode();
        missle = null;
        StartCoroutine(Cooldown(true));
        Activated();
    }

    #endregion
}