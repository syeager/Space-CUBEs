// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.11
// Edited: 2014.06.15

using System;
using UnityEngine;

/// <summary>
/// A gameObject that is enabled and disabled by pools.
/// </summary>
public class PoolObject : MonoBehaviour
{
    #region Private Fields

    /// <summary>Associated pool.</summary>
    private Pool pool;

    #endregion

    #region Events

    /// <summary>Event fired when disabled.</summary>
    public EventHandler DisableEvent;

    #endregion

    #region Public Methods

    /// <summary>
    /// Caches pool.
    /// </summary>
    /// <param name="pool">Pool to save.</param>
    public void Initialize(Pool pool)
    {
        this.pool = pool;
    }


    /// <summary>
    /// Disable gameObject and push back to the pool.
    /// </summary>
    public void Disable()
    {
        StopAllCoroutines();
        DisableEvent.Fire(this);
        gameObject.SetActive(false);
        pool.Push(this);
    }


    /// <summary>
    /// Disable the gameObject after the timer runs out.
    /// </summary>
    /// <param name="life">Time in seconds before the gameObject is disabled.</param>
    public void StartLifeTimer(float life)
    {
        Invoke("Disable", life);
    }

    #endregion
}