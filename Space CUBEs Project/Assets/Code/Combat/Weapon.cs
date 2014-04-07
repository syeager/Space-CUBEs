﻿// Steve Yeager
// 12.01.2013

using System;
using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBase
{
    #region References

    protected Transform myTransform;
    protected Ship myShip;

    #endregion

    #region Public Fields

    new public string name;
    /// <summary>1 per second.</summary>
    public float cooldownSpeed;

    #endregion

    #region Protected Fields

    protected bool canActivate = true;

    #endregion

    #region Const Fields

    public const float FULLPOWER = 100f;

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
        }
        while (power < FULLPOWER);

        power = FULLPOWER;
        canActivate = true;
    }


    protected void Activated()
    {
        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
    }


    protected IEnumerator Charge()
    {
        while (power < FULLPOWER)
        {
            power += cooldownSpeed * deltaTime;
            if (PowerUpdateEvent != null)
            {
                PowerUpdateEvent(this, new ValueArgs(power));
            }
            yield return null;
        }

        power = FULLPOWER;
    }

    #endregion

    #region Virtual Methods

    public virtual void Initialize(Ship sender)
    {
        myTransform = transform;
        myShip = sender;
        power = FULLPOWER;
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

    public abstract void Activate(bool pressed, float multiplier);

    #endregion

    
}