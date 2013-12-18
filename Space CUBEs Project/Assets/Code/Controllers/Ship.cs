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
    protected WeaponManager myWeapons;
    protected ShieldHealth myHealth;

    #endregion

    #region Protected Fields

    protected StateMachine stateMachine;

    #endregion

    #region Const Fields

    protected const string SpawningState = "Spawning";
    protected const string DyingState = "Dying";

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
        // combat
        myWeapons.Initialize(this);

        // register events
        myHealth.DieEvent += OnDie;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a collider for the ship that encompasses all CUBEs.
    /// </summary>
    public void GenerateCollider()
    {
        Bounds bounds = new Bounds();
        bounds.center = myTransform.position;

        var children = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var child in children)
        {
            bounds.Encapsulate(child.bounds);
        }

        var boxCol = gameObject.AddComponent<BoxCollider>();
        boxCol.isTrigger = true;
        boxCol.size = new Vector3(bounds.size.y, 10f, bounds.size.x);
        boxCol.center = new Vector3(0f, 0, -0.5f); // z is wrong
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