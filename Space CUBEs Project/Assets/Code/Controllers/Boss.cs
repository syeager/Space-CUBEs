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

    #region Protected Fields

    protected int stage = 1;

    #endregion

    #region Private Fields

    private UISprite healthBar;

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

        MyHealth.HealthUpdateEvent += OnHit;
    }

    #endregion

    #region Protected Methods

    protected void InitializeHealth()
    {
        healthBar = HUD.Main.bossHealth;
        healthBar.gameObject.SetActive(true);
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

        // update HUD
        healthBar.fillAmount = args.health / args.max;

        // next stage
        if (args.health < stages[stage - 1])
        {
            stage++;
            NextStageEvent(this, new ValueArgs(stage));
        }
    }

    #endregion
}