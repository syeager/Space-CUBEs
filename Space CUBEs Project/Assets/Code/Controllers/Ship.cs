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

    #region State Fields

    protected string currentState;
    protected string initialState;
    private readonly Dictionary<string, Action<Dictionary<string, object>>> enterMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    private readonly Dictionary<string, Action<Dictionary<string, object>>> exitMethods = new Dictionary<string, Action<Dictionary<string, object>>>();

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
    }

    protected virtual void Start()
    {
        // combat
        myWeapons.Initialize(this);

        // register events
        myHealth.DieEvent += OnDie;
    }

    #endregion

    #region State Methods

    /// <summary>
    /// Create a new currentState.
    /// </summary>
    /// <param name="stateName">State.</param>
    /// <param name="enterMethod">Enter method for currentState.</param>
    /// <param name="exitMethod">Exit method for currentState.</param>
    protected void CreateState(string stateName, Action<Dictionary<string, object>> enterMethod, Action<Dictionary<string, object>> exitMethod)
    {
        enterMethods.Add(stateName, enterMethod);
        exitMethods.Add(stateName, exitMethod);
    }


    /// <summary>
    /// Exit the current state and enter the new one.
    /// </summary>
    /// <param name="stateName">State to transition to.</param>
    /// <param name="info">Info to pass to the exit and enter states.</param>
    public void SetState(string stateName, Dictionary<string, object> info)
    {
        Log(name + ": " + currentState + "→" + stateName);

        // save previous state
        if (info == null)
        {
            info = new Dictionary<string, object>();
        }
        info.Add("previous state", currentState);

        // exit state
        exitMethods[currentState](info);

        // enter state
        currentState = stateName;
        enterMethods[currentState](info);
    }


    /// <summary>
    /// Start the initial state. Doesn't call any exit methods.
    /// </summary>
    /// <param name="info">Info to pass to state enter method.</param>
    protected void StartInitialState(Dictionary<string, object> info)
    {
        Log(name + ": Initial State = " + initialState);

        currentState = initialState;
        enterMethods[initialState](info);
    }

    #endregion

    #region Public Methods

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
        SetState(DyingState, new Dictionary<string, object> { { "sender", sender } });
    }

    #endregion
}