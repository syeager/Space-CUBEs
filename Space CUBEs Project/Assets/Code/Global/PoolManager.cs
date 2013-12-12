// Steve Yeager
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    #region Public Fields

    public float cullDelay = 10f;
    public Pool[] PoolList;

    #endregion

    #region Private Fields

    private static Dictionary<string, Pool> Pools = new Dictionary<string, Pool>();

    #endregion

    #region Static Fields

    private static List<Pool> CullList = new List<Pool>();

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        Initialize();
        StartCoroutine(Cull());
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 
    /// </summary>
    private void Initialize()
    {
        foreach (var pool in PoolList)
        {
            Pools.Add(pool.Prefab.name, pool);
            pool.Initialize();
        }
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="PrefabName"></param>
    /// <returns></returns>
    public static GameObject Pop(string PrefabName)
    {
        Pool pool;
        if (Pools.TryGetValue(PrefabName, out pool))
        {
            return Pools[PrefabName].Pop();
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="PrefabName"></param>
    /// <param name="life"></param>
    /// <returns></returns>
    public static GameObject Pop(string PrefabName, float life)
    {
        Pool pool;
        if (Pools.TryGetValue(PrefabName, out pool))
        {
            return Pools[PrefabName].Pop(life);
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
    public static void Push(PoolObject po)
    {
        if (po == null) return;
        Pool pool;
        if (!Pools.TryGetValue(po.PrefabName, out pool)) return;
        pool.Push(po);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pool"></param>
    public static void AddCullListener(Pool pool)
    {
        CullList.Add(pool);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pool"></param>
    private static IEnumerator Cull()
    {
        float cullDelay = Main.cullDelay;
        while (true)
        {
            yield return new WaitForSeconds(cullDelay);
            foreach (var pool in CullList)
            {
                pool.Cull();
            }
        }
    }

    #endregion
}