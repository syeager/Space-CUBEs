// Steve Yeager
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

    #region Public Methods

    public void Initialize(Ship sender)
    {
        myShip = sender;
        power = FULLPOWER;
    }

    #endregion

    #region Protected Methods

    protected IEnumerator Cooldown(bool empty = false)
    {
        if (empty) power = 0f;
        canActivate = false;

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

    public virtual bool CanActivate()
    {
        return canActivate;
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// attach new Weapon to parent
    //  copy values to parent
    //  delete self
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public abstract Weapon Bake(GameObject parent);

    public abstract void Activate(bool pressed, float multiplier);

    #endregion

    
}