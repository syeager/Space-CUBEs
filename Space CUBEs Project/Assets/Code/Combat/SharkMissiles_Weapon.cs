// Steve Yeager
// 3.4.2014

using UnityEngine;
using System.Collections;
using System.Linq;

public class SharkMissiles_Weapon : Weapon
{
    #region Public Fields

    public GameObject SharkMissile_Prefab;
    public Vector3 missileOffset = new Vector3(0f, 0f, 1f);
    public float missileDelay = 1f;
    public float missileDelaySpeed;
    public float missileHomingSpeed;
    public HitInfo hitInfo;

    #endregion

    #region Private Fields

    private bool firing;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
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
        comp.cooldownSpeed = cooldownSpeed;
        comp.SharkMissile_Prefab = SharkMissile_Prefab;
        comp.missileOffset = missileOffset + myTransform.localPosition;
        comp.missileDelay = missileDelay;
        comp.missileDelaySpeed = missileDelaySpeed;
        comp.missileHomingSpeed = missileHomingSpeed;
        comp.hitInfo = hitInfo;

        return comp;
    }

    #endregion

    #region Private Fields

    private void FireMissile(float multiplier)
    {
        // create missile
        SharkMissile missile = PoolManager.Pop(SharkMissile_Prefab, myTransform.position + myTransform.TransformDirection(missileOffset), myTransform.rotation).GetComponent<SharkMissile>();

        // fire missile
        missile.Initialize(myShip, hitInfo.MultiplyDamage(multiplier), missileDelay, missileDelaySpeed, missileHomingSpeed);
    }

    #endregion
}