// Steve Yeager
// 2.10.2014

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Annotations;

/// <summary>
/// Singleton manager for all pools.
/// </summary>
public class PoolManager : Singleton<PoolManager>
{
    #region Public Fields

    /// <summary>Should the PoolManager cull pools?</summary>
    public bool cull = true;

    /// <summary>Time in seconds between culling.</summary>
    public float cullDelay = 10f;

    /// <summary>List of pools.</summary>
    public List<Pool> poolList;

    #endregion

    #region Static Fields

    /// <summary>Dictionary of pools mapped to their corresponding prefabs.</summary>
    private static readonly Dictionary<GameObject, Pool> Pools = new Dictionary<GameObject, Pool>();

    /// <summary>List of pools that want to be culled.</summary>
    private static readonly List<Pool> CullList = new List<Pool>();

    #endregion


    #region MonoBehaviour Overrides

    /// <summary>
    /// Initialize all pools and start cull loop.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        Initialize();
        if (cull)
        {
            StartCoroutine(CullLoop());
        }
    }


    /// <summary>
    /// Clear all pools.
    /// </summary>
    [UsedImplicitly]
    private void OnDestroy()
    {
        Clear();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initialize all pools.
    /// </summary>
    private void Initialize()
    {
        Clear();

        foreach (Pool pool in poolList)
        {
            Pools.Add(pool.prefab.gameObject, pool);
            pool.Initialize();
        }
    }


    /// <summary>
    /// Clear all pools.
    /// </summary>
    private void Clear()
    {
        StopAllCoroutines();
        foreach (Pool pool in poolList)
        {
            pool.Clear();
        }

        Pools.Clear();
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(GameObject prefab)
    {
        while (true)
        {
            Pool pool;
            if (Pools.TryGetValue(prefab, out pool))
            {
                return pool.Pop();
            }

            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            Pools.Add(prefab, pool);
            Transform parent = new GameObject(prefab.name).transform;
            parent.parent = Main.transform;
            pool.Initialize(parent);
        }
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(GameObject prefab, float life)
    {
        while (true)
        {
            Pool pool;
            if (Pools.TryGetValue(prefab, out pool))
            {
                return pool.Pop(life);
            }

            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            Pools.Add(prefab, pool);
            Transform parent = new GameObject(prefab.name).transform;
            parent.parent = Main.transform;
            pool.Initialize(parent);
        }
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="position">World position to give gameObject.</param>
    /// <param name="rotation">World rotation to give gameObject.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        while (true)
        {
            Pool pool;
            if (Pools.TryGetValue(prefab, out pool))
            {
                Transform popped = pool.Pop().transform;
                popped.position = position;
                popped.rotation = rotation;
                return popped.gameObject;
            }

            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            Pools.Add(prefab, pool);
            Transform parent = new GameObject(prefab.name).transform;
            parent.parent = Main.transform;
            pool.Initialize(parent);
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
    public static GameObject Pop(GameObject prefab, Vector3 position, Quaternion rotation, float life)
    {
        while (true)
        {
            Pool pool;
            if (Pools.TryGetValue(prefab, out pool))
            {
                Transform popped = pool.Pop(life).transform;
                popped.position = position;
                popped.rotation = rotation;
                return popped.gameObject;
            }

            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            Pools.Add(prefab, pool);
            Transform parent = new GameObject(prefab.name).transform;
            parent.parent = Main.transform;
            pool.Initialize(parent);
        }
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(int poolIndex)
    {
        if (poolIndex >= Pools.Count)
        {
            Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", Pools.Count, poolIndex)), Main.gameObject);
        }

        return Main.poolList[poolIndex].Pop();
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(int poolIndex, float life)
    {
        if (poolIndex >= Pools.Count)
        {
            Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", Pools.Count, poolIndex)), Main.gameObject);
        }

        return Main.poolList[poolIndex].Pop(life);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <param name="position">World position to give gameObject.</param>
    /// <param name="rotation">World rotation to give gameObject.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(int poolIndex, Vector3 position, Quaternion rotation)
    {
        if (poolIndex >= Pools.Count)
        {
            Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", Pools.Count, poolIndex)), Main.gameObject);
        }

        Transform poppped = Main.poolList[poolIndex].Pop().transform;
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
    public static GameObject Pop(int poolIndex, Vector3 position, Quaternion rotation, float life)
    {
        if (poolIndex >= Pools.Count)
        {
            Debugger.LogException(new IndexOutOfRangeException(String.Format("There are only {0} pools, trying to get number {1}.", Pools.Count, poolIndex)), Main.gameObject);
        }

        Transform poppped = Main.poolList[poolIndex].Pop(life).transform;
        poppped.position = position;
        poppped.rotation = rotation;

        return poppped.gameObject;
    }


    /// <summary>
    /// Add pool to culling list.
    /// </summary>
    /// <param name="pool">Pool to add.</param>
    public static void AddCullListener(Pool pool)
    {
        CullList.Add(pool);
    }


    /// <summary>
    /// Cull each pool in the cull list.
    /// </summary>
    public static void Cull()
    {
        foreach (var pool in CullList)
        {
            pool.Cull();
        }
    }


    /// <summary>
    /// Infinite loop with timer to cull the pools.
    /// </summary>
    private static IEnumerator CullLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(Main.cullDelay);
        while (true)
        {
            yield return wait;
            Cull();
        }
    }

    #endregion
}