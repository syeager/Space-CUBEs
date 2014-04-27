// Steve Yeager
// 12.22.2013

using System.Collections;
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
        gameObject.SetActive(false);
        pool.Push(this);
    }


    /// <summary>
    /// Disable the gameObject after the timer runs out.
    /// </summary>
    /// <param name="life">Time in seconds before the gameObject is disabled.</param>
    public void StartLifeTimer(float life)
    {
        StartCoroutine(LifeTimer(life));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Timer till gameObject is disabled.
    /// </summary>
    /// <param name="time">Time in seconds to run the timer.</param>
    private IEnumerator LifeTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Disable();
    }

    #endregion
}