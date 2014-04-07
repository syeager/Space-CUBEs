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

    public PoolObject prefab;
    public int preAllocate;
    public int allocateBlock = 1;
    public bool hardLimit;
    public int limit;
    public bool cull;
    public int cullLimit;
    public Transform parent;

    #endregion

    #region Private Fields

    private int poolSize;

    #endregion


    #region Constructors

    public Pool()
    {
        pool = new Stack<GameObject>();
    }


    public Pool(GameObject prefab)
    {
        this.prefab = prefab.GetComponent<PoolObject>();
        pool = new Stack<GameObject>();
    }

    #endregion

    #region Public Methods

    public void Initialize()
    {
        if (parent != null)
        {
            PoolObject[] children = parent.GetComponentsInChildren<PoolObject>(true);
            foreach (var child in children)
            {
                child.Initialize(this);
                poolSize++;
                if (!child.gameObject.activeSelf)
                {
                    Push(child);
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


    public void Initialize(Transform parent)
    {
        Initialize();
        this.parent = parent;
    }


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


    public GameObject Pop(float life)
    {
        GameObject go = Pop();
        if (go != null)
        {
            go.GetComponent<PoolObject>().StartLifeTimer(life);
            return go;
        }
        else
        {
            return null;
        }
    }


    public void Push(PoolObject poolObject)
    {
        pool.Push(poolObject.gameObject);
    }


    public void Clear()
    {
        while (pool.Count > 0)
        {
            Object.Destroy(pool.Pop());
        }

        pool.Clear();
    }


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
            go.GetComponent<PoolObject>().Initialize(this);
            pool.Push(go);
            go.SetActive(false);
            poolSize++;
        }
    }

    #endregion
}