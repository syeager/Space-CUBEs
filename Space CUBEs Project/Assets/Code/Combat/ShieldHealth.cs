// Steve Yeager
// 12.5.2013

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

    #endregion

    #region Properties

    public float maxShield;// { get; private set; }
    public float shield;// { get; private set; }

    #endregion


    #region Health Overrides

    public override void RecieveHit(Ship sender, HitInfo hitInfo)
    {
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
        this.maxHealth = maxHealth;
        health = maxHealth;
        this.maxShield = maxShield;
        shield = maxShield;
    }


    public float ChangeShield(float amount)
    {
        float extraDamage = shield + amount;
        shield = Mathf.Clamp(shield + amount, 0f, maxShield);

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
        float extraDamage = ChangeShield(amount);
        if (extraDamage < 0f)
        {
            return ChangeHealth(extraDamage);
        }

        return false;
    }

    #endregion

    #region Private Methods

    private IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeDelay);
        while (shield < maxShield)
        {
            shield += rechargeSpeed * Time.deltaTime;
            yield return null;
        }

        shield = maxShield;
    }

    #endregion
}