﻿// Steve Yeager
// 12.5.2013

using System;
using UnityEngine;

/// <summary>
/// Health manager for all objects that can be destroyed.
/// </summary>
public class Health : MonoBase
{
    #region Public Fields

    public bool invincible;

    #endregion

    #region Properties

    public float maxHealth;// { get; protected set; }
    public float health;// { get; protected set; }

    #endregion

    #region Events

    public EventHandler<HealthUpdateArgs> HealthUpdateEvent;
    public EventHandler<DieArgs> DieEvent;

    #endregion


    #region Public Methods

    public void Initialize()
    {
        health = maxHealth;
    }


    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
    }


    public virtual void RecieveHit(Ship sender, HitInfo hitInfo)
    {
        if (invincible) return;

        ChangeHealth(hitInfo.damage);
    }


    public bool ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, maxHealth);
        if (HealthUpdateEvent != null)
        {
            HealthUpdateEvent(this, new HealthUpdateArgs(maxHealth, amount, health));
        }
        return health == 0f;
    }

    #endregion
}