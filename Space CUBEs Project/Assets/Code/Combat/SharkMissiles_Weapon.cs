// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.05
// Edited: 2014.06.13

using LittleByte.Pools;
using UnityEngine;

public class SharkMissiles_Weapon : Weapon
{
    #region Public Fields

    public PoolObject missilePrefab;
    public Vector3 missileOffset = new Vector3(0f, 0f, 1f);
    public float missileDelay = 1f;
    public float missileDelaySpeed;
    public float missileHomingSpeed;
    public float damage;

    #endregion

    #region Private Fields

    private bool firing;

    #endregion

    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (pressed)
        {
            if (firing) return;

            firing = true;
            FireMissile(multiplier);
        }
        else
        {
            if (!firing) return;

            firing = false;
            FireMissile(multiplier);

            StartCoroutine(Cooldown(true));
            Activated();
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<SharkMissiles_Weapon>();
        comp.index = index;
        comp.cooldownTime = cooldownTime;
        comp.missilePrefab = missilePrefab;
        comp.missileOffset = missileOffset + myTransform.localPosition;
        comp.missileDelay = missileDelay;
        comp.missileDelaySpeed = missileDelaySpeed;
        comp.missileHomingSpeed = missileHomingSpeed;
        comp.damage = damage;

        return comp;
    }

    #endregion

    #region Private Fields

    private void FireMissile(float multiplier)
    {
        // create missile
        SharkMissile missile = Prefabs.Pop(missilePrefab, myTransform.position + myTransform.TransformDirection(missileOffset), myTransform.rotation).GetComponent<SharkMissile>();

        // fire missile
        missile.Initialize(myShip, damage * multiplier, missileDelay, missileDelaySpeed, missileHomingSpeed);
    }

    #endregion
}