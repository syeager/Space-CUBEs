// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.10
// Edited: 2014.07.12

using LittleByte;
using UnityEngine;
using System.Collections;
using System;

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

    #endregion

    #region Const Fields

    private const int GroupSize = 10;
    private const int MaxMultiplier = 10;

    #endregion

    #region Properties

    private int multiplier = 1;

    private int Multiplier
    {
        get { return multiplier; }
        set { multiplier = Mathf.Clamp(value, 1, MaxMultiplier); }
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
        int mult = cursor / GroupSize + 1;

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
        cursor = 0;
    }

    #endregion
}