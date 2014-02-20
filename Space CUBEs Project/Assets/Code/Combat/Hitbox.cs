// Steve Yeager
// 12.8.2013

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passes HitInfo from attack to reciever.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
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

    #region Private Fields

    private HitInfo hitInfo;
    private Ship sender;
    private int hitCount;
    private Action CollisionMethod;

    #endregion


    #region MonoBehavoiur Overrides

    private void Awake()
    {
        myTransform = transform;
        myPoolObject = GetComponent<PoolObject>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (continuous) return;

        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            // send damage
            oppHealth.RecieveHit(sender, hitInfo);

            // call collision hit
            if (CollisionMethod != null) CollisionMethod();

            // disable if applicable
            if (hitNumber > 0)
            {
                hitCount++;
                if (hitCount >= hitNumber)
                {
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
            oppHealth.RecieveHit(sender, new HitInfo { damage = hitInfo.damage * deltaTime });
        }
    }

    #endregion

    #region Public Methods

    public virtual void Initialize(Ship sender, HitInfo hitInfo, Action CollisionMethod = null)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;
        this.CollisionMethod = CollisionMethod;

        gameObject.layer = sender.gameObject.layer;
        hitCount = 0;
    }


    public virtual void Initialize(Ship sender, HitInfo hitInfo, Vector3 moveVec, Action CollisionMethod = null)
    {
        Initialize(sender, hitInfo, CollisionMethod);
        StartCoroutine(Move(moveVec));
    }


    public virtual void Initialize(Ship sender, HitInfo hitInfo, float time, Action CollisionMethod = null)
    {
        Initialize(sender, hitInfo, CollisionMethod);
        myPoolObject.StartLifeTimer(time);
    }


    public virtual void Initialize(Ship sender, HitInfo hitInfo, float time, Vector3 moveVec, Action CollisionMethod = null)
    {
        Initialize(sender, hitInfo, time, CollisionMethod);
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