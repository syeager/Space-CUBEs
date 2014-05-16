// Steve Yeager
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
    public ShieldHealth myHealth { get; protected set; }

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


    protected virtual void OnEnable()
    {
        LevelManager.PausedEvent += OnPause;
    }


    protected virtual void OnDisable()
    {
        LevelManager.PausedEvent -= OnPause;
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


    private void OnPause(object sender, PauseArgs args)
    {
        if (stateMachine.update != null)
        {
            stateMachine.update.Pause(args.paused);
        }
    }

    #endregion
}