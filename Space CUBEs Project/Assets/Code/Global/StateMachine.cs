// Steve Yeager
// 12.17.2013

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Machine to be inherited from other base classes.
/// </summary>
public class StateMachine
{
    #region Public Fields

    public string initialState;

    #endregion

    #region Private Fields

    private MonoBase owner;
    private readonly Dictionary<string, Action<Dictionary<string, object>>> enterMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    private readonly Dictionary<string, Action<Dictionary<string, object>>> exitMethods = new Dictionary<string, Action<Dictionary<string, object>>>();

    #endregion

    #region Properties

    public string currentState { get; private set; }

    #endregion


    #region Constructors

    public StateMachine(MonoBase owner)
    {
        this.owner = owner;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new currentState.
    /// </summary>
    /// <param name="stateName">State.</param>
    /// <param name="enterMethod">Enter method for currentState.</param>
    /// <param name="exitMethod">Exit method for currentState.</param>
    public void CreateState(string stateName, Action<Dictionary<string, object>> enterMethod, Action<Dictionary<string, object>> exitMethod)
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
        #if LOG
        if (owner.log)
        {
            Debugger.Log(owner.name + ": " + currentState + "→" + stateName, owner);
        }
        #endif
        

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
    public void Start(Dictionary<string, object> info)
    {
        #if LOG
        if (owner.log)
        {
            Debugger.Log(owner.name + ": Initial State = " + initialState, owner);
        }
        #endif

        currentState = initialState;
        enterMethods[initialState](info);
    }

    #endregion
}