// Little Byte Games
// Author: Steve Yeager
// Created: 2014.01.10
// Edited: 2014.10.17

using System;
using System.Collections;
using Annotations;
using LittleByte.Extensions;
using UnityEngine;

/// <summary>
/// Hnadles the player's score.
/// </summary>
[Serializable]
public class ScoreManager
{
    #region Public Fields

    public float multiplierLife = 1.5f;

    #endregion

    #region Private Fields

    private int cursor;
    private Job multiplierJob;

    [SerializeField, UsedImplicitly]
    private int groupSize = 10;

    [SerializeField, UsedImplicitly]
    private int maxMultiplier = 10;

    #endregion

    #region Properties

    private int multiplier = 1;

    private int Multiplier
    {
        get { return multiplier; }
        set { multiplier = Mathf.Clamp(value, 1, maxMultiplier); }
    }

    public int points; // { get; private set; }

    #endregion

    #region Events

    public EventHandler<PointsUpdateArgs> PointsUpdateEvent;
    public EventHandler<MultiplierUpdateArgs> MultiplierUpdateEvent;

    #endregion

    #region Public Methods

    public void RecieveScore(int amount)
    {
        points += Mathf.CeilToInt(amount * multiplier);
        if (PointsUpdateEvent != null)
        {
            PointsUpdateEvent(this, new PointsUpdateArgs(amount, points));
        }
    }


    public void IncreaseMultiplier(int amount = 1)
    {
        cursor += amount;
        int mult = cursor / groupSize + 1;

        if (multiplierJob != null) multiplierJob.Kill();
        multiplierJob = new Job(MultiplierLife());

        MultiplierUpdateEvent.Fire(this, new MultiplierUpdateArgs(mult - Multiplier, mult));
        Multiplier = mult;
    }

    #endregion

    #region Private Methods

    private IEnumerator MultiplierLife()
    {
        yield return new WaitForSeconds(multiplierLife);
        if (MultiplierUpdateEvent != null)
        {
            MultiplierUpdateEvent(this, new MultiplierUpdateArgs(-Multiplier + 1, 1));
        }
        multiplier = 1;
        cursor = 0;
    }

    #endregion
}