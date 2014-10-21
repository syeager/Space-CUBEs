// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.05
// Edited: 2014.07.04

using System;
using System.Collections;
using LittleByte;
using UnityEngine;

/// <summary>
/// Health manager for objects that have a shield.
/// </summary>
public class ShieldHealth : Health
{
    #region Public Fields

    public float rechargeSpeed = 20f;
    public float rechargeDelay = 3f;
    public float maxShield;
    public float shield;

    #endregion

    #region Private Fields

    private Job rechargeJob;
    private Material shieldHitMat;

    #endregion

    #region Properties

    public float Strength
    {
        get { return health + shield; }
    }

    #endregion

    #region Events

    public EventHandler<ShieldUpdateArgs> ShieldUpdateEvent;

    #endregion

    #region Monobehaviour Overrides

    protected override void Awake()
    {
        shieldHitMat = GameResources.Main.ShieldHit_Mat;
        HealthHit_Mat = GameResources.Main.HealthHit_Mat;
        base.Awake();
    }

    #endregion

    #region Health Overrides

    public override void Initialize()
    {
        base.Initialize();
        shield = maxShield;
    }


    public override float RecieveHit(Ship sender, float damage)
    {
        if (invincible) return 0f;
        if (!enabled) return 0f;

        float damageToHealth = damage - shield;
        float damageDone = ChangeShield(-damage);
        if (damageToHealth > 0f)
        {
            damageDone += ChangeHealth(-damageToHealth);
            HitMat(HealthHit_Mat);
        }
        else
        {
            HitMat(shieldHitMat);
        }

        if (health <= 0f)
        {
            Killed(sender);
        }

        return damageDone;
    }


    public float RecieveHit(Ship sender, float damage, bool bypassShield)
    {
        if (!bypassShield) return RecieveHit(sender, damage);
        if (invincible) return 0f;

        float damageDone = ChangeHealth(-damage);
        HitMat(HealthHit_Mat);
        if (health <= 0f)
        {
            Killed(sender);
        }

        return damageDone;
    }


    protected override void Killed(Ship sender)
    {
        if (rechargeJob != null)
        {
            rechargeJob.Kill();
        }
        base.Killed(sender);
    }

    #endregion

    #region Public Methods

    public void Initialize(float maxHealth, float maxShield)
    {
        this.maxShield = maxShield;
        shield = maxShield;

        Initialize(maxHealth);
    }


    /// <summary>
    /// Either deal damage to or recover shield.
    /// </summary>
    /// <param name="amount">Amount to add to shield.</param>
    /// <returns>Amount actually applied.</returns>
    public float ChangeShield(float amount)
    {
        if (!enabled) return 0f;

        float amountAdded;
        if (amount > 0)
        {
            amountAdded = shield + amount > maxShield ? maxShield - shield : amount;
        }
        else
        {
            amountAdded = shield + amount < 0f ? shield : amount;
        }
        shield = Mathf.Clamp(shield + amount, 0f, maxShield);

        if (ShieldUpdateEvent != null)
        {
            ShieldUpdateEvent(this, new ShieldUpdateArgs(maxShield, amountAdded, shield));
        }

        if (amount < 0f)
        {
            if (rechargeJob != null) rechargeJob.Kill();
            rechargeJob = new Job(Recharge());
        }

        return amountAdded;
    }

    #endregion

    #region Private Methods

    private IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeDelay);
        while (shield < maxShield)
        {
            ChangeShield(rechargeSpeed * Time.deltaTime);
            yield return null;
        }
    }

    #endregion
}