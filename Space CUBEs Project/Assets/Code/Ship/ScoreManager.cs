// Steve Yeager
// 1.5.2014

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 
/// </summary>
[Serializable]
public class ScoreManager
{
    #region Public Fields

    public float multiplierLife = 3f;

    #endregion

    #region Private Fields

    private int multiplier = 1;
    private Job multiplierJob;

    #endregion

    #region Const Fields

    private const string SCOREPATH = "Score";

    #endregion

    #region Properties

    public int points { get; private set; }

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
        multiplier += amount;
        if (multiplierJob != null)
        {
            multiplierJob.Kill();
        }
        multiplierJob = new Job(MultiplierLife());

        if (MultiplierUpdateEvent != null)
        {
            MultiplierUpdateEvent(this, new MultiplierUpdateArgs(amount, multiplier));
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator MultiplierLife()
    {
        while (multiplier > 1)
        {
            yield return new WaitForSeconds(multiplierLife);
            if (MultiplierUpdateEvent != null)
            {
                MultiplierUpdateEvent(this, new MultiplierUpdateArgs(-multiplier+1, 1));
            }
            multiplier = 1;
        }
    }

    #endregion
}
