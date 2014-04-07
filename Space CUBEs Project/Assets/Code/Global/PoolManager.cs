// Steve Yeager
// 2.10.2014

using System.Collections;
using System.Collections.Generic;
using Annotations;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    #region Public Fields

    public bool cull = true;
    public float cullDelay = 10f;
    public List<Pool> poolList;

    #endregion

    #region Static Fields

    private static readonly Dictionary<GameObject, Pool> Pools = new Dictionary<GameObject, Pool>();
    private static readonly List<Pool> CullList = new List<Pool>();

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        Initialize();
        if (cull)
        {
            StartCoroutine(Cull());
        }
    }


    [UsedImplicitly]
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
            Pools.Add(pool.prefab.gameObject, pool);
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

        Pools.Clear();
    }

    #endregion

    #region Static Methods

    public static GameObject Pop(GameObject prefab)
    {
        while (true)
        {
            Pool pool;
            if (Pools.TryGetValue(prefab, out pool))
            {
                return pool.Pop();
            }
            else
            {
                pool = new Pool(prefab);
                Main.poolList.Add(pool);
                Pools.Add(prefab, pool);
                pool.Initialize(new GameObject(prefab.name).transform);
            }
        }
    }


    public static GameObject Pop(GameObject prefab, float life)
    {
        while (true)
        {
            Pool pool;
            if (Pools.TryGetValue(prefab, out pool))
            {
                return pool.Pop(life);
            }
            else
            {
                pool = new Pool(prefab);
                Main.poolList.Add(pool);
                Pools.Add(prefab, pool);
                pool.Initialize(new GameObject(prefab.name).transform);
            }
        }
    }


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
            else
            {
                pool = new Pool(prefab);
                Main.poolList.Add(pool);
                Pools.Add(prefab, pool);
                pool.Initialize(new GameObject(prefab.name).transform);
            }
        }
    }


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
            else
            {
                pool = new Pool(prefab);
                Main.poolList.Add(pool);
                Pools.Add(prefab, pool);
                pool.Initialize(new GameObject(prefab.name).transform);
            }
        }
    }


    public static void AddCullListener(Pool pool)
    {
        CullList.Add(pool);
    }


    private static IEnumerator Cull()
    {
        WaitForSeconds wait = new WaitForSeconds(Main.cullDelay);
        while (true)
        {
            yield return wait;
            foreach (var pool in CullList)
            {
                pool.Cull();
            }
        }
    }

    #endregion
}