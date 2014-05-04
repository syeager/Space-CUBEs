// Steve Yeager
// 5.3.2014

using System;
using UnityEngine;

/// <summary>
/// Damages enemies when barrel rolling.
/// </summary>
public class BrassKnuckles : Augmentation
{
    #region Public Fields

    public float damage;
    
    #endregion

    #region Private Fields

    private Ship ship;
    private bool dealingDamage;
    private float multiplier;

    #endregion


    #region Augmentation Overrides

    public override void Initialize(Player player)
    {
        ship = player;
        player.BarrelRollEvent += OnBarrelRoll;
    }


    public override Augmentation Bake(GameObject player)
    {
        BrassKnuckles comp = player.AddComponent(typeof(BrassKnuckles)) as BrassKnuckles;
        comp.damage = damage;

        return comp;
    }

    #endregion

    #region MonoBehaviour Overrides

    private void OnTriggerEnter(Collider other)
    {
        if (!dealingDamage) return;

        Health enemyHealth = other.GetComponent(typeof(Health)) as Health;
        if (enemyHealth != null)
        {
            enemyHealth.RecieveHit(ship, damage * multiplier);
        }
    }

    #endregion

    #region Event Handlers

    public void OnBarrelRoll(object sender, ValueArgs args)
    {
        bool rolling = (bool)args.value;
        if (rolling)
        {
            dealingDamage = true;
            collider.enabled = true;
            multiplier = ship.myWeapons.damageMultiplier;
        }
        else
        {
            dealingDamage = false;
        }
    }

    #endregion
}