// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.11.25
// Edited: 2014.06.25

using System.Collections.Generic;
using Annotations;
using UnityEngine;

/// <summary>
/// Base class for all ships.
/// </summary>
[RequireComponent(typeof(ShipMotor))]
[RequireComponent(typeof(ShieldHealth))]
public class Ship : MonoBase
{
    #region References

    protected Transform myTransform;
    public ShipMotor MyMotor { get; protected set; }
    public ShieldHealth MyHealth { get; protected set; }

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
        MyMotor = GetComponent<ShipMotor>() ?? gameObject.AddComponent<ShipMotor>();
        MyHealth = GetComponent<ShieldHealth>() ?? gameObject.AddComponent<ShieldHealth>();
    }


    protected virtual void Start()
    {
        // register events
        MyHealth.DieEvent += OnDie;
    }


    protected virtual void OnEnable()
    {
        GameTime.PausedEvent += OnPause;
    }


    protected virtual void OnDisable()
    {
        GameTime.PausedEvent -= OnPause;
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
            stateMachine.SetState(DyingState, new Dictionary<string, object> {{"sender", sender}});
        }
    }


    private void OnPause(object sender, PauseArgs args)
    {
        if (stateMachine.update != null)
        {
            stateMachine.update.Pause(args.paused);
        }
    }

    #endregion
}