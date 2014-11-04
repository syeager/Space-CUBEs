// Little Byte Games

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Holds references to already instantiated objects.
/// </summary>
[Serializable]
[Obsolete("HardLimit is broken.")]
// TODO: Rename fields. Cull vs HardLimit.
// TODO: Serialized private fields
public class Pool
{
    #region Public Fields

    /// <summary>GameObject to use for this pool.</summary>
    public PoolObject prefab;

    /// <summary>How many gameObjects to created when initialized.</summary>
    public int preAllocate;

    /// <summary>How many gameObjects to instantiate if when allocating.</summary>
    public int allocateBlock = 1;

    /// <summary>Should the pool be limited to a set number?</summary>
    public bool hardLimit;

    /// <summary>How many gameObjects the pool can hold before returning null.</summary>
    public int limit;

    /// <summary>Should this pool be culled by the PoolManager?</summary>
    public bool cull;

    /// <summary>The limit of allowed inactive gameObjects in the pool. All others will be deleted during culling.</summary>
    public int cullLimit;

    /// <summary>Parent to create gameObjects under. Adds any preinstantiated gameObjects to the pool during initialization.</summary>
    public Transform parent;

    #endregion

    #region Private Fields

    /// <summary>PoolManager this pool belongs to.</summary>
    public PoolManager poolManager;

    /// <summary>List of inactive gameObjects.</summary>
    private readonly Stack<GameObject> pool = new Stack<GameObject>();

    #endregion

    #region Properties

    /// <summary>Number of active gameObjects in the pool.</summary>
    public int ActiveCount { get; private set; }

    /// <summary>Number of inactive gameObjects in the pool.</summary>
    public int InactiveCount
    {
        get { return pool.Count; }
    }

    /// <summary>Number of active and inactive gameObjects in the pool.</summary>
    public int PoolCount
    {
        get { return ActiveCount + InactiveCount; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Needed to satisfy Unity. Creates new pool.
    /// </summary>
    public Pool()
    {
        pool = new Stack<GameObject>();
    }

    /// <summary>
    /// Create pool and cache prefab.
    /// </summary>
    /// <param name="poolManager"></param>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    public Pool(PoolManager poolManager, PoolObject prefab, Transform parent = null)
    {
        this.poolManager = poolManager;
        if (prefab != null)
        {
            this.prefab = prefab;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set up pool and instantiate gameObjects as needed.
    /// </summary>
    /// <param name="parent">Parent to cache.</param>
    public void Initialize(Transform parent = null)
    {
        if (parent != null)
        {
            this.parent = parent;
        }

        // grab all preinitialized gameObjects in parent
        if (this.parent != null)
        {
            Component[] children = this.parent.GetComponentsInChildren(typeof(PoolObject), true);
            foreach (PoolObject poolObject in children.Select(child => child as PoolObject))
            {
                poolObject.Initialize(this);
                if (!poolObject.gameObject.activeSelf)
                {
                    Push(poolObject);
                    ActiveCount++;
                }
            }
        }

        if (preAllocate > 0)
        {
            Allocate(preAllocate);
        }

        if (cull)
        {
            poolManager.AddCullListener(this);
        }
    }

    public Stack<GameObject> GetInactive()
    {
        return pool;
    }

    /// <summary>
    /// Get the next available gameObject from the pool.
    /// </summary>
    /// <returns>Active gameObject from pool.</returns>
    public GameObject Pop()
    {
        while (true)
        {
            // object ready
            if (pool.Count > 0)
            {
                GameObject go = pool.Pop();
                go.SetActive(true);
                ActiveCount++;
                return go;
            }

            // reached hard limit
            if (hardLimit && PoolCount >= limit)
            {
                return null;
            }

            // allocate more
            Allocate(allocateBlock);
        }
    }

    /// <summary>
    /// Get the next available gameObject from the pool and disable after a timer.
    /// </summary>
    /// <param name="life">Time in seconds before gameObject is disabled.</param>
    /// <returns>Active gameObject from pool.</returns>
    public GameObject Pop(float life)
    {
        GameObject go = Pop();
        if (go != null)
        {
            ((PoolObject)go.GetComponent(typeof(PoolObject))).StartLifeTimer(life);
            return go;
        }

        return null;
    }

    /// <summary>
    /// Push gameObject back into the pool.
    /// </summary>
    /// <param name="poolObject">GameObject to return to the pool.</param>
    public void Push(PoolObject poolObject)
    {
        pool.Push(poolObject.gameObject);
        ActiveCount--;
    }

    /// <summary>
    /// Destroy all gameObjects in pool.
    /// </summary>
    public void Clear()
    {
        while (pool.Count > 0)
        {
            Object.Destroy(pool.Pop());
        }

        pool.Clear();
    }

    /// <summary>
    /// Destroy extra gameObjects.
    /// </summary>
    public void Cull()
    {
        while (pool.Count > cullLimit)
        {
            Object.Destroy(pool.Pop());
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Create more gameObjects and add them to the pool.
    /// </summary>
    /// <param name="size">Amount of gameObjects to create.</param>
    private void Allocate(int size)
    {
        if (hardLimit && PoolCount + size > limit)
        {
            size = limit - PoolCount;
        }

        for (int i = 0; i < size; i++)
        {
            GameObject go = Object.Instantiate(prefab.gameObject) as GameObject;
            go.transform.parent = parent;
            ((PoolObject)go.GetComponent(typeof(PoolObject))).Initialize(this);
            pool.Push(go);
            go.SetActive(false);
        }
    }

    #endregion
}