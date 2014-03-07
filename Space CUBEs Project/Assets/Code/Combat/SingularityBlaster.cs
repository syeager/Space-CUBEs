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
    
    public GameObject BlackHoleMissile_Prefab;
    public Vector3 missileOffset;
    public HitInfo hitInfo;
    public float missileTime;
    public float missileSpeed;
    
    #endregion

    #region Private Fields

    private BlackHoleMissile missile;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            if (missile != null)
            {
                Release();
            }
            Fire(multiplier);
        }
        else if (missile != null)
        {
            Release();
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<SingularityBlaster>();
        comp.index = index;
        comp.cooldownSpeed = cooldownSpeed;
        comp.BlackHoleMissile_Prefab = BlackHoleMissile_Prefab;
        comp.missileOffset = missileOffset + myTransform.localPosition;
        comp.hitInfo = hitInfo;
        comp.missileTime = missileTime;
        comp.missileSpeed = missileSpeed;

        return comp;
    }

    #endregion

    #region Private Methods

    private void Fire(float multiplier)
    {
        missile = PoolManager.Pop(BlackHoleMissile_Prefab, myTransform.position + myTransform.TransformDirection(missileOffset), myTransform.rotation).GetComponent<BlackHoleMissile>();
        missile.Initialize(myShip, hitInfo.MultiplyDamage(multiplier), missileTime, myTransform.forward*missileSpeed);
    }


    private void Release()
    {
        missile.Explode();
        missile = null;
        StartCoroutine(Cooldown(true));
        Activated();
    }

    #endregion
}