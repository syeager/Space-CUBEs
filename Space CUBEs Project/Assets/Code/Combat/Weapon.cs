﻿// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.01
// Edited: 2014.06.16

using System;
using System.Collections;
using Annotations;
using UnityEngine;

public abstract class Weapon : MonoBase
{
    #region References

    protected Transform myTransform;
    protected Ship myShip;

    #endregion

    #region Public Fields

    public new string name;

    /// <summary>Time in seconds to completely refill power gauge.</summary>
    public float cooldownTime;

    /// <summary>Power regenerated every second.</summary>
    public float cooldownSpeed { get; set; }

    #endregion

    #region Protected Fields

    protected bool canActivate = true;

    #endregion

    #region Const Fields

    public const float FullPower = 100f;

    #endregion

    #region Properties

    public float power { get; protected set; }
    public int index { get; set; }

    #endregion

    #region Events

    public EventHandler<ValueArgs> PowerUpdateEvent;
    public EventHandler ActivatedEvent;

    #endregion

    #region MonoBehaviours

    [UsedImplicitly]
    private void Awake()
    {
        // get references
        myTransform = transform;
    }

    #endregion

    #region Protected Methods

    protected IEnumerator Cooldown(bool empty = false, bool deActivate = true)
    {
        if (empty) power = 0f;
        if (deActivate) canActivate = false;

        do
        {
            power += cooldownSpeed * deltaTime;
            if (PowerUpdateEvent != null)
            {
                PowerUpdateEvent(this, new ValueArgs(power));
            }
            yield return null;
        } while (power < FullPower);

        power = FullPower;
        canActivate = true;
    }


    protected void Activated()
    {
        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Virtual Methods

    public virtual void Initialize(Ship sender)
    {
        myTransform = transform;
        myShip = sender;
        power = FullPower;
        cooldownSpeed = FullPower / cooldownTime;
    }


    public virtual bool CanActivate()
    {
        return canActivate;
    }


    /// <summary>
    /// attach new Weapon to parent
    //  copy values to parent
    //  delete self
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public virtual Weapon Bake(GameObject parent)
    {
        return null;
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Activate weapon with extra info.
    /// </summary>
    /// <param name="pressed">Is the weapon being pressed?</param>
    /// <param name="multiplier">Damage multiplier.</param>
    /// <param name="attackInfo">Data to be passed and converted on the weapon side.</param>
    public abstract Coroutine Activate(bool pressed, float multiplier, object attackInfo = null);

    #endregion
}