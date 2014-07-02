// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.25
// Edited: 2014.06.26

using System;

/// <summary>
/// Base class for Bosses.
/// </summary>
public class Boss : Enemy
{
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