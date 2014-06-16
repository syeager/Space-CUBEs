// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.02.20
// Edited: 2014.06.13


using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SingularityBlaster : Weapon
{
    #region Public Fields

    public PoolObject missilePrefab;
    public Vector3 missileOffset;
    public float damage;
    public float missileTime;
    public float missileSpeed;

    #endregion

    #region Private Fields

    private BlackHoleMissile missile;

    #endregion

    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
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
        comp.cooldownTime = cooldownTime;
        comp.missilePrefab = missilePrefab;
        comp.missileOffset = missileOffset + myTransform.localPosition;
        comp.damage = damage;
        comp.missileTime = missileTime;
        comp.missileSpeed = missileSpeed;

        return comp;
    }

    #endregion

    #region Private Methods

    private void Fire(float multiplier)
    {
        missile = Prefabs.Pop(missilePrefab, myTransform.position + myTransform.TransformDirection(missileOffset), myTransform.rotation).GetComponent<BlackHoleMissile>();
        missile.Initialize(myShip, damage * multiplier, missileTime, myTransform.forward * missileSpeed);
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