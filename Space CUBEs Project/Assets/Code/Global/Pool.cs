// Steve Yeager
// 12.10.2013

using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds references to already instantiated objects.
/// </summary>
[System.Serializable]
public class Pool
{
    #region Public Fields

    /// <summary>List of inactive gameObjects.</summary>
    public Stack<GameObject> pool = new Stack<GameObject>();

    /// <summary>GameObject to use for this pool.</summary>
    public PoolObject prefab;

    /// <summary>Should the pool instantiate gameObjects when initialized?</summary>
    public int preAllocate;

    /// <summary>How many gameObjects to instantiate if preAllocating.</summary>
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

    /// <summary>Number of active and inactive gameObjects in the pool.</summary>
    private int poolSize;

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
    /// <param name="prefab"></param>
    public Pool(GameObject prefab)
    {
        this.prefab = prefab.GetComponent(typeof(PoolObject)) as PoolObject;
        pool = new Stack<GameObject>();
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
                poolSize++;
                if (!poolObject.gameObject.activeSelf)
                {
                    Push(poolObject);
                }
            }
        }

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
                return go;
            }

            // reached hard limit
            if (hardLimit && poolSize >= limit)
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
            poolSize--;
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
        if (hardLimit && poolSize + size > limit)
        {
            size = limit - poolSize;
        }

        for (int i = 0; i < size; i++)
        {
            GameObject go = Object.Instantiate(prefab.gameObject) as GameObject;
            go.transform.parent = parent;
            ((PoolObject)go.GetComponent(typeof(PoolObject))).Initialize(this);
            pool.Push(go);
            go.SetActive(false);
            poolSize++;
        }
    }

    #endregion
}