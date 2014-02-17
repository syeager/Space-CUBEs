// Steve Yeager
// 11.25.2013

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all ships.
/// </summary>
[RequireComponent(typeof(ShipMotor))]
[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(ShieldHealth))]
public class Ship : MonoBase
{
    #region References

    protected Transform myTransform;
    protected ShipMotor myMotor;
    public WeaponManager myWeapons;
    protected ShieldHealth myHealth;

    #endregion

    #region Protected Fields

    protected StateMachine stateMachine;

    #endregion

    #region Const Fields

    protected const string SpawningState = "Spawning";
    protected const string DyingState = "Dying";

    private const float COLLISIONDAMAGE = -20f;

    #endregion


    #region Unity Overrides

    protected virtual void Awake()
    {
        // get references
        myTransform = transform;
        myMotor = GetComponent<ShipMotor>() ?? gameObject.AddComponent<ShipMotor>();
        myWeapons = GetComponent<WeaponManager>() ?? gameObject.AddComponent<WeaponManager>();
        myHealth = GetComponent<ShieldHealth>() ?? gameObject.AddComponent<ShieldHealth>();

        stateMachine = new StateMachine(this);
    }

    protected virtual void Start()
    {
        // register events
        myHealth.DieEvent += OnDie;
    }


    private void OnTriggerEnter(Collider other)
    {
        Health otherHealth = other.GetComponent<Health>();
        if (otherHealth != null)
        {
            otherHealth.RecieveHit(this, new HitInfo { damage = COLLISIONDAMAGE });
        }
    }


    private void OnDestroy()
    {
        stateMachine = null;
    }

    #endregion

    #region Event Handlers

    private void OnDie(object sender, DieArgs args)
    {
        if (stateMachine.currentState != DyingState)
        {
            stateMachine.SetState(DyingState, new Dictionary<string, object> { { "sender", sender } });
        }
    }

    #endregion
}