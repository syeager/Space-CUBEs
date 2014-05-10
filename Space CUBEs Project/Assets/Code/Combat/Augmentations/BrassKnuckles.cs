// Steve Yeager
// 5.3.2014

using Annotations;
using UnityEngine;

/// <summary>
/// Damages enemies when barrel rolling.
/// </summary>
public class BrassKnuckles : Augmentation
{
    #region Public Fields

    public float damage = 10f;
    
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
        BrassKnuckles comp = (BrassKnuckles)player.AddComponent(typeof(BrassKnuckles));
        comp.damage = damage;

        return comp;
    }

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void OnTriggerEnter(Collider other)
    {
        if (!dealingDamage) return;

        Health enemyHealth = (Health)other.GetComponent(typeof(Health));
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