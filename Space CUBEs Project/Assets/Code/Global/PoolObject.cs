// Steve Yeager
// 12.22.2013

using System.Collections;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    #region Readonly Fields

    private Pool pool;

    #endregion


    #region Public Methods

    public void Initialize(Pool pool)
    {
        this.pool = pool;
    }


    public void Disable()
    {
        gameObject.SetActive(false);
        pool.Push(this);
    }


    public void StartLifeTimer(float life)
    {
        StartCoroutine(LifeTimer(life));
    }

    #endregion

    #region Private Methods

    private IEnumerator LifeTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Disable();
    }

    #endregion
}