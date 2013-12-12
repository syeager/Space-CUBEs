// Steve Yeager
// 12.10.2013

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds references to already instantiated objects.
/// </summary>
[System.Serializable]
public class Pool
{
    #region Public Fields

    public Stack<GameObject> pool = new Stack<GameObject>();

    public PoolObject Prefab;
    public int preAllocate;
    public int allocateBlock = 1;
    public bool hardLimit;
    public int limit;
    public bool cull;
    public int cullLimit;

    #endregion

    #region Private Fields

    private int poolSize;

    #endregion
    

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    {
        if (preAllocate > 0)
        {
            Allocate(preAllocate);
        }

        if (cull)
        {
            PoolManager.AddCullListener(this);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        // object ready
        if (pool.Count > 0)
        {
            var prefab = pool.Pop();
            prefab.SetActive(true);
            return prefab;
        }
        
        // reached hard limit        
        if (hardLimit && poolSize >= limit)
        {
            return null;
        }
        
        // allocate more
        Allocate(allocateBlock);
        return Pop();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="life"></param>
    /// <returns></returns>
    public GameObject Pop(float life)
    {
        var prefab = Pop();
        if (prefab != null)
        {
            prefab.GetComponent<PoolObject>().StartLifeTimer(life);
            return prefab;
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="po"></param>
    public void Push(PoolObject po)
    {
        pool.Push(po.gameObject);
    }


    /// <summary>
    /// Destroy all objects in pool and clear list.
    /// </summary>
    public void Clear()
    {
        foreach (var p in pool)
        {
            Object.Destroy(p.gameObject);
        }

        pool.Clear();
    }


    /// <summary>
    /// 
    /// </summary>
    public void Cull()
    {
        while (pool.Count > cullLimit)
        {
            poolSize--;
            var g = pool.Pop();
            Object.Destroy(g);
        }
    }

    #endregion

    #region Private Methods

    private void Allocate(int size)
    {
        if (hardLimit && poolSize + size > limit)
        {
            size = limit - poolSize;
        }

        for (int i = 0; i < size; i++)
        {
            var prefab = (GameObject)GameObject.Instantiate(Prefab.gameObject);
            pool.Push(prefab);
            prefab.SetActive(false);
            poolSize++;
        }
    }

    #endregion
}