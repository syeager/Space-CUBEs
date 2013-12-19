﻿// Steve Yeager
// 12.16.2013

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all enemies.
/// </summary>
[RequireComponent(typeof(PoolObject))]
public class Enemy : Ship
{
    #region References

    protected PoolObject poolObject;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // references
        poolObject = GetComponent<PoolObject>();

        // states
        stateMachine.CreateState(DyingState, DieEnter, info => { });
    }


    protected override void Start()
    {
        base.Start();

        stateMachine.Start(new Dictionary<string, object>());
    }

    #endregion

    #region State Methods

    private void DieEnter(Dictionary<string, object> info)
    {
        // send hitinfo to player

        poolObject.Disable();
    }

    #endregion

    #region Public Methods

    public void Spawn()
    {
        myHealth.Initialize();
        stateMachine.Start(new Dictionary<string, object>());
    }

    #endregion
}