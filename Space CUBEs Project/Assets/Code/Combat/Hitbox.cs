// Steve Yeager
// 12.8.2013

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passes HitInfo from attack to reciever.
/// </summary>
public class Hitbox : MonoBase
{
    #region References

    protected Transform myTransform;
    protected PoolObject myPoolObject;

    #endregion

    #region Public Fields

    public bool continuous;
    public int hitNumber = 1;

    #endregion

    #region Protected Fields

    protected float damage;
    protected Ship sender;
    protected int hitCount;
    protected bool disabled;

    #endregion


    #region MonoBehavoiur Overrides

    private void Awake()
    {
        myTransform = transform;
        myPoolObject = GetComponent<PoolObject>();
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        if (continuous || disabled) return;

        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            // send damage
            oppHealth.RecieveHit(sender, damage);

            // disable if applicable
            if (hitNumber > 0)
            {
                hitCount++;
                if (hitCount >= hitNumber)
                {
                    disabled = true;
                    GetComponent<PoolObject>().Disable();
                }
            }
        }
    }


    protected virtual void OnTriggerStay(Collider other)
    {
        if (!continuous) return;

        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, damage * deltaTime);
        }
    }

    #endregion

    #region Public Methods

    public virtual void Initialize(Ship sender, float damage)
    {
        disabled = false;

        this.sender = sender;
        this.damage = damage;

        gameObject.layer = sender.gameObject.layer;
        hitCount = 0;
    }


    public virtual void Initialize(Ship sender, float damage, Vector3 moveVec)
    {
        Initialize(sender, damage);
        StartCoroutine(Move(moveVec));
    }


    public virtual void Initialize(Ship sender, float damage, float time)
    {
        Initialize(sender, damage);
        myPoolObject.StartLifeTimer(time);
    }


    public virtual void Initialize(Ship sender, float damage, float time, Vector3 moveVec)
    {
        Initialize(sender, damage, time);
        StartCoroutine(Move(moveVec));
    }

    #endregion

    #region Private Methods

    private IEnumerator Move(Vector3 moveVec)
    {
        while (true)
        {
            myTransform.Translate(moveVec*deltaTime, Space.World);
            yield return null;
        }
    }

    #endregion
}