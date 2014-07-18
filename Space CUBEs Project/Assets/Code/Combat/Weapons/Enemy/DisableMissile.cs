// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.16
// Edited: 2014.07.16

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class DisableMissile : Hitbox
{
    #region Public Fields

    public float speed;

    #endregion

    #region Private Fields

    private Job disabledJob;
    private float disableTime;
    private Weapon disabledWeapon;

    #endregion

    #region MonoBehaviour Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var oppHealth = (Health)other.gameObject.GetComponent(typeof(Health));
        oppHealth.RecieveHit(sender, damage);

        disabledWeapon = LevelManager.Main.PlayerController.Weapons.weapons[Random.Range(0, Player.Weaponlimit)];
        if (disabledJob != null) disabledJob.Kill();
        disabledJob = new Job(Disable());

        myPoolObject.Disable();
    }

    #endregion

    #region Hitbox Overrides

    public override void Initialize(Ship sender, float damage, float disableTime)
    {
        this.disableTime = disableTime;
        Initialize(sender, damage);
    }

    #endregion

    #region Private Methods

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(disableTime);
        disabledWeapon.canActivate = true;
    }

    #endregion
}