// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.05
// Edited: 2014.06.25

using UnityEngine;

public class SharkMissiles_Weapon : PlayerWeapon
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

    public override Coroutine Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            if (firing) return null;

            firing = true;
            FireMissile(multiplier);
        }
        else
        {
            if (!firing) return null;

            firing = false;
            FireMissile(multiplier);

            StartCoroutine(Cooldown(true));
            ActivatedEvent.Fire(this);
        }

        return null;
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