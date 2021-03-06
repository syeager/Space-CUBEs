﻿// Little Byte Games
// Author: Steve Yeager
// Created: 2013.12.17
// Edited: 2014.09.19

using System;
using System.Collections;
using System.Collections.Generic;
using LittleByte.Extensions;

/// <summary>
/// State Machine to be inherited from other base classes.
/// </summary>
[Serializable]
public class StateMachine
{
    #region Private Fields

    /// <summary>MonoBase that this statemachine belongs to. Used for logging.</summary>
    private readonly MonoBase owner;

    #endregion

    #region Readonly Fields

    private readonly Dictionary<string, Action<Dictionary<string, object>>> enterMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    private readonly Dictionary<string, Action<Dictionary<string, object>>> exitMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    private readonly string initialState;

    #endregion

    #region Properties

    public string currentState { get; private set; }
    public string previousState { get; private set; }
    public Job update { get; private set; }

    #endregion

    #region Events

    public EventHandler<StateChangedArgs> StateChangedEvent;

    #endregion

    #region Constructors/Deconstructors

    public StateMachine(MonoBase owner, string initialState)
    {
        this.owner = owner;
        this.initialState = initialState;
    }


    ~StateMachine()
    {
        if (update != null)
        {
            update.Kill();
            update = null;
        }
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
    public void SetState(string stateName, Dictionary<string, object> info = null)
    {
#if DEBUG
        if (owner.log)
        {
            Debugger.Log(owner.name + ": " + currentState + "→" + stateName, owner, Debugger.LogTypes.StateMachines, false);
        }
#endif

        StateChangedEvent.Fire(this, new StateChangedArgs(previousState, stateName));

        // save previous state
        previousState = currentState;

        if (update != null)
        {
            update.End();
            update = null;
        }

        // exit state
        exitMethods[currentState](info);
        if (update != null)
        {
            update.Kill();
            update = null;
        }

        // enter state
        currentState = stateName;
        enterMethods[currentState](info);
    }


    /// <summary>
    /// Start the initial state. Doesn't call any exit methods.
    /// </summary>
    /// <param name="info">Info to pass to state enter method.</param>
    public void Start(Dictionary<string, object> info = null)
    {
#if DEBUG
        if (owner.log)
        {
            Debugger.Log(owner.name + ": Initial State = " + initialState, owner, Debugger.LogTypes.StateMachines, false);
        }
#endif

        if (update != null)
        {
            update.End();
            update = null;
        }

        // enter state
        currentState = initialState;
        previousState = initialState;
        enterMethods[currentState](info);
    }


    public Job SetUpdate(IEnumerator updateJob)
    {
        if (update != null)
        {
            update.Kill();
        }
        update = new Job(updateJob);

        return update;
    }


    public void Kill()
    {
        if (update != null)
        {
            update.End();
            update = null;
        }
    }


    public bool IsCurrentState(string stateName)
    {
        return currentState == stateName;
    }

    #endregion
}

#region Classes

public class StateChangedArgs : EventArgs
{
    public readonly string previousState;
    public readonly string currentState;


    public StateChangedArgs(string previousState, string currentState)
    {
        this.previousState = previousState;
        this.currentState = currentState;
    }
}

#endregion