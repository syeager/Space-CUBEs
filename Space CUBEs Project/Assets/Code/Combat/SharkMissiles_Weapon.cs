// Steve Yeager
// 3.4.2014

using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 
/// </summary>
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
        // find enemy
        Enemy target = null;
        float max = 0f;
        foreach (var enemy in LevelManager.Main.activeEnemies)
        {
            if (enemy.GetComponent<ShieldHealth>().strength > max)
            {
                max = enemy.GetComponent<ShieldHealth>().strength;
                target = enemy;
            }
        }

        // create missile
        SharkMissile missile = (Instantiate(SharkMissile_Prefab, myTransform.position + myTransform.TransformDirection(missileOffset), myTransform.rotation) as GameObject).GetComponent<SharkMissile>();

        // fire missile
        missile.Initialize(myShip, hitInfo.MultiplyDamage(multiplier), (target ? target.transform : null), missileDelay, missileDelaySpeed, missileHomingSpeed);
    }

    #endregion
}