// Steve Yeager
// 12.5.2013

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Health manager for objects that have a shield.
/// </summary>
public class ShieldHealth : Health
{
    #region Public Fields

    public float rechargeSpeed = 25f;
    public float rechargeDelay = 1f;

    #endregion

    #region Private Fields

    private Job rechargeJob;
    private Material ShieldHit_Mat;

    #endregion

    #region Properties

    public float maxShield;
    public float shield;

    #endregion

    #region Events

    public EventHandler<ShieldUpdateArgs> ShieldUpdateEvent;

    #endregion


    #region Monobehaviour Overrides

    protected override void Awake()
    {
        ShieldHit_Mat = GameResources.Main.ShieldHit_Mat;
        base.Awake();
    }

    #endregion

    #region Health Overrides

    public override void RecieveHit(Ship sender, HitInfo hitInfo)
    {
        if (invincible) return;

        if (ApplyDamage(hitInfo.damage))
        {
            Killed(sender);
            return;
        }
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


    public float ChangeShield(float amount)
    {
        float extraDamage = shield + amount;
        shield = Mathf.Clamp(shield + amount, 0f, maxShield);

        if (ShieldUpdateEvent != null)
        {
            ShieldUpdateEvent(this, new ShieldUpdateArgs(maxShield, amount, shield));
        }

        if (amount < 0f)
        {
            if (rechargeJob != null) rechargeJob.Kill();
            rechargeJob = new Job(Recharge());
        }

        if (extraDamage < 0f)
        {
            return extraDamage;
        }
        else
        {
            return 0f;
        }
    }


    public bool ApplyDamage(float amount)
    {
        bool dead = false;
        float extraDamage = ChangeShield(amount);
        if (extraDamage < 0f)
        {
            dead = ChangeHealth(extraDamage);
            HitMat(HealthHit_Mat);
        }
        else
        {
            HitMat(ShieldHit_Mat);
        }

        return dead;
    }


    public void Trash()
    {
        Killed(null);
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