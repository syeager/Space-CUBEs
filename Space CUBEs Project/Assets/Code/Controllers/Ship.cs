﻿// Steve Yeager
// 11.25.2013

using System.Collections.Generic;
using Annotations;
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

    #region Const Fields

    protected const string DyingState = "Dying";

    #endregion

    #region Properties

    public StateMachine stateMachine { get; protected set; }

    #endregion


    #region Unity Overrides

    protected virtual void Awake()
    {
        // get references
        myTransform = transform;
        myMotor = GetComponent<ShipMotor>() ?? gameObject.AddComponent<ShipMotor>();
        myWeapons = GetComponent<WeaponManager>() ?? gameObject.AddComponent<WeaponManager>();
        myHealth = GetComponent<ShieldHealth>() ?? gameObject.AddComponent<ShieldHealth>();
    }

    protected virtual void Start()
    {
        // register events
        myHealth.DieEvent += OnDie;
    }


    [UsedImplicitly]
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