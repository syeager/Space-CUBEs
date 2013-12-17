// Steve Yeager
// 12.5.2013

using System;
using UnityEngine;

/// <summary>
/// Health manager for all objects that can be destroyed.
/// </summary>
public class Health : MonoBase
{
    #region Protected Fields

    protected int lastHitID = 0;

    #endregion

    #region Properties

    public float maxHealth;// { get; protected set; }
    public float health;// { get; protected set; }

    #endregion

    #region Events

    public EventHandler<DieArgs> DieEvent;

    #endregion


    #region Public Methods

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
    }


    public virtual void RecieveHit(Ship sender, int hitID, HitInfo hitInfo)
    {
        if (hitID == lastHitID) return;

        lastHitID = hitID;
        ChangeHealth(hitInfo.damage);
    }


    public bool ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, maxHealth);
        return health == 0f;
    }

    #endregion
}