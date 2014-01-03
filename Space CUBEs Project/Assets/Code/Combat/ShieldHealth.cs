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

    public float rechargeSpeed;
    public float rechargeDelay;

    #endregion

    #region Private Fields

    private Job rechargeJob;
    private Material ShieldHit_Mat;

    #endregion

    #region Properties

    public float maxShield;// { get; private set; }
    public float shield;// { get; private set; }

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
            if (DieEvent != null)
            {
                rechargeJob.Kill();
                DieEvent(sender, new DieArgs());
            }
        }
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
            if (changeMat != null)
            {
                changeMat.Kill();
            }
            changeMat = new Job(ChangeMat(HealthHit_Mat));
        }
        else
        {
            if (changeMat != null)
            {
                changeMat.Kill();
            }
            changeMat = new Job(ChangeMat(ShieldHit_Mat));
        }

        return dead;
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