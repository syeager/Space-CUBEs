// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.11
// Edited: 2014.06.15

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Singleton manager for all pools.
/// </summary>
public class PoolManager : ScriptableObject
{
    #region Public Fields

    /// <summary>Should the PoolManager cull pools?</summary>
    public bool cull = true;

    /// <summary>Time in seconds between culling.</summary>
    public float cullDelay = 10f;

    /// <summary>List of pools.</summary>
    public List<Pool> poolList = new List<Pool>();

    /// <summary>Parent for all pools created during runtime.</summary>
    public Transform parent;

    #endregion

    #region Private Fields

    /// <summary>Cull pool list in intervals.</summary>
    private Job cullJob;

    #endregion

    #region Readonly Fields

    /// <summary>Dictionary of pools mapped to their corresponding prefabs.</summary>
    private readonly Dictionary<PoolObject, Pool> pools = new Dictionary<PoolObject, Pool>();

    /// <summary>List of pools that want to be culled.</summary>
    private readonly List<Pool> cullList = new List<Pool>();

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize all pools.
    /// </summary>
    public void Initialize()
    {
        foreach (Pool pool in poolList)
        {
            pools.Add(pool.prefab, pool);
            pool.Initialize();
        }

        if (cull)
        {
            cullJob = new Job(Culling());
        }
    }


    /// <summary>
    /// Clear all pools.
    /// </summary>
    public void Clear()
    {
        if (cullJob != null)
        {
            cullJob.Kill();
        }

        foreach (Pool pool in poolList)
        {
            pool.Clear();
        }

        pools.Clear();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolParent"></param>
    /// <returns></returns>
    public Pool CreatePool(PoolObject prefab, Transform poolParent = null)
    {
        Pool pool = new Pool(this, prefab);
        poolList.Add(pool);
        pools.Add(prefab, pool);
        if (poolParent == null)
        {
            poolParent = new GameObject(prefab.name).transform;
            poolParent.parent = parent;
        }
        pool.Initialize(poolParent);

        return pool;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public Pool GetPool(PoolObject prefab)
    {
        return poolList.FirstOrDefault(p => p.prefab == prefab);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(PoolObject prefab)
    {
        while (true)
        {
            Pool pool;
            if (pools.TryGetValue(prefab, out pool))
            {
                return pool.Pop();
            }

            CreatePool(prefab);
        }
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(PoolObject prefab, float life)
    {
        while (true)
        {
            Pool pool;
            if (pools.TryGetValue(prefab, out pool))
            {
                return pool.Pop(life);
            }

            CreatePool(prefab);
        }
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="position">World position to give gameObject.</param>
    /// <param name="rotation">World rotation to give gameObject.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(PoolObject prefab, Vector3 position, Quaternion rotation)
    {
        while (true)
        {
            Pool pool;
            if (pools.TryGetValue(prefab, out pool))
            {
                Transform popped = pool.Pop().transform;
                popped.position = position;
                popped.rotation = rotation;
                return popped.gameObject;
            }

            CreatePool(prefab);
        }
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(PoolObject prefab, Vector3 position, Quaternion rotation, float life)
    {
        while (true)
        {
            Pool pool;
            if (pools.TryGetValue(prefab, out pool))
            {
                Transform popped = pool.Pop(life).transform;
                popped.position = position;
                popped.rotation = rotation;
                return popped.gameObject;
            }

            CreatePool(prefab);
        }
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(int poolIndex)
    {
        if (poolIndex >= pools.Count)
        {
            throw Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", pools.Count, poolIndex)));
        }

        return poolList[poolIndex].Pop();
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(int poolIndex, float life)
    {
        if (poolIndex >= pools.Count)
        {
            throw Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", pools.Count, poolIndex)));
        }

        return poolList[poolIndex].Pop(life);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <param name="position">World position to give gameObject.</param>
    /// <param name="rotation">World rotation to give gameObject.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(int poolIndex, Vector3 position, Quaternion rotation)
    {
        if (poolIndex >= pools.Count)
        {
            throw Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", pools.Count, poolIndex)));
        }

        Transform poppped = poolList[poolIndex].Pop().transform;
        poppped.position = position;
        poppped.rotation = rotation;

        return poppped.gameObject;
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public GameObject Pop(int poolIndex, Vector3 position, Quaternion rotation, float life)
    {
        if (poolIndex >= pools.Count)
        {
            throw Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", pools.Count, poolIndex)));
        }

        Transform poppped = poolList[poolIndex].Pop(life).transform;
        poppped.position = position;
        poppped.rotation = rotation;

        return poppped.gameObject;
    }


    /// <summary>
    /// Add pool to culling list.
    /// </summary>
    /// <param name="pool">Pool to add.</param>
    public void AddCullListener(Pool pool)
    {
        cullList.Add(pool);
    }


    /// <summary>
    /// Cull each pool in the cull list.
    /// </summary>
    public void Cull()
    {
        foreach (Pool pool in cullList)
        {
            pool.Cull();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 
    /// </summary>
    private IEnumerator Culling()
    {
        WaitForSeconds wait = new WaitForSeconds(cullDelay);
        while (true)
        {
            yield return wait;
            Cull();
        }
    }

    #endregion
}