// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.25
// Edited: 2014.07.16

using System;
using UnityEngine;

/// <summary>
/// Base class for Bosses.
/// </summary>
public class Boss : Enemy
{
    #region References

    protected Animation myAnimation;

    #endregion

    #region Public Fields

    public float[] stages;

    #endregion

    #region Properties

    public int CurrentStage { get; protected set; }

    #endregion

    #region Events

    public EventHandler<ValueArgs> NextStageEvent;

    /// <summary>Fired after death animation.</summary>
    public EventHandler DeathEvent;

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // references
        myAnimation = animation;

        CurrentStage = 1;
        MyHealth.HealthUpdateEvent += OnHit;
    }

    #endregion

    #region Event Handlers

    private void OnHit(object sender, HealthUpdateArgs args)
    {
        // already dead
        if (args.health <= 0f)
        {
            MyHealth.HealthUpdateEvent -= OnHit;
            return;
        }

        // next stage
        if (args.health < stages[CurrentStage - 1])
        {
            CurrentStage++;
            NextStageEvent(this, new ValueArgs(CurrentStage));
        }
    }

    #endregion
}