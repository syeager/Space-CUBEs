// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.05.03
// Edited: 2014.05.31

using Annotations;
using UnityEngine;

/// <summary>
/// Damages enemies when barrel rolling.
/// </summary>
public class BrassKnuckles : Augmentation
{
    #region References

    private GameObject myGameObject;
    private Collider myCollider;

    #endregion

    #region Public Fields

    public float damage = 10f;

    #endregion

    #region Private Fields

    /// <summary>Physics layer for PlayerShip.</summary>
    private int shipLayer;

    /// <summary>Physics layer for PlayerWeapon.</summary>
    private int weaponLayer;

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

        myGameObject = gameObject;
        myCollider = collider;
        shipLayer = LayerMask.NameToLayer("PlayerShip");
        weaponLayer = LayerMask.NameToLayer("PlayerWeapon");
    }


    public override Augmentation Bake(GameObject player)
    {
        var comp = (BrassKnuckles)player.AddComponent(typeof(BrassKnuckles));
        comp.index = index;
        comp.damage = damage;

        return comp;
    }

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void OnTriggerEnter(Collider other)
    {
        if (!dealingDamage) return;

        var enemyHealth = other.GetComponent(typeof(Health)) as Health;
        if (enemyHealth != null)
        {
            enemyHealth.RecieveHit(ship, damage * multiplier);
        }
    }

    #endregion

    #region Event Handlers

    public void OnBarrelRoll(object sender, ValueArgs args)
    {
        var rolling = (bool)args.value;
        if (rolling)
        {
            dealingDamage = true;
            myCollider.isTrigger = true;
            myCollider.enabled = true;
            myGameObject.layer = weaponLayer;
            multiplier = ship.myWeapons.damageMultiplier;
        }
        else
        {
            dealingDamage = false;
            myCollider.isTrigger = false;
            myGameObject.layer = shipLayer;
        }
    }

    #endregion
}