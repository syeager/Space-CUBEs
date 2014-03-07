// Steve Yeager
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    #region Public Fields

    public bool cull = true;
    public float cullDelay = 10f;
    public List<Pool> poolList;

    #endregion

    #region Static Fields

    private static Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();
    private static List<Pool> cullList = new List<Pool>();

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        Initialize();
        StartCoroutine(Cull());
    }


    private void OnDestroy()
    {
        Clear();
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
        Clear();

        foreach (Pool pool in poolList)
        {
            pools.Add(pool.prefab.gameObject, pool);
            pool.Initialize();
        }
    }


    private void Clear()
    {
        StopAllCoroutines();
        foreach (Pool pool in poolList)
        {
            pool.Clear();
        }

        pools.Clear();
    }

    #endregion

    #region Static Methods

    public static GameObject Pop(GameObject prefab)
    {
        Pool pool;
        if (pools.TryGetValue(prefab, out pool))
        {
            return pool.Pop();
        }
        else
        {
            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            pools.Add(prefab, pool);
            pool.Initialize();

            return pool.Pop();
        }
    }


    public static GameObject Pop(GameObject prefab, float life)
    {
        Pool pool;
        if (pools.TryGetValue(prefab, out pool))
        {
            return pool.Pop(life);
        }
        else
        {
            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            pools.Add(prefab, pool);
            pool.Initialize();

            return pool.Pop(life);
        }
    }


    public static GameObject Pop(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Pool pool;
        if (pools.TryGetValue(prefab, out pool))
        {
            Transform popped = pool.Pop().transform;
            popped.position = position;
            popped.rotation = rotation;
            return popped.gameObject;
        }
        else
        {
            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            pools.Add(prefab, pool);
            pool.Initialize();

            Transform popped = pool.Pop().transform;
            popped.position = position;
            popped.rotation = rotation;
            return popped.gameObject;
        }
    }


    public static GameObject Pop(GameObject prefab, Vector3 position, Quaternion rotation, float life)
    {
        Pool pool;
        if (pools.TryGetValue(prefab, out pool))
        {
            Transform popped = pool.Pop(life).transform;
            popped.position = position;
            popped.rotation = rotation;
            return popped.gameObject;
        }
        else
        {
            pool = new Pool(prefab);
            Main.poolList.Add(pool);
            pools.Add(prefab, pool);
            pool.Initialize();

            Transform popped = pool.Pop(life).transform;
            popped.position = position;
            popped.rotation = rotation;
            return popped.gameObject;
        }
    }


    public static void AddCullListener(Pool pool)
    {
        cullList.Add(pool);
    }


    private static IEnumerator Cull()
    {
        WaitForSeconds wait = new WaitForSeconds(Main.cullDelay);
        while (true)
        {
            yield return wait;
            foreach (var pool in cullList)
            {
                pool.Cull();
            }
        }
    }

    #endregion
}