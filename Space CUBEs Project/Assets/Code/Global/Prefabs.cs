// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.13
// Edited: 2014.06.13

using Annotations;

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Prefabs : Singleton<Prefabs>
{
    #region Private Fields

    public PoolManager poolManager;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Reset()
    {
        poolManager = ScriptableObject.CreateInstance<PoolManager>();
    }


    /// <summary>
    /// Initialize all pools and start cull loop.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        poolManager.Initialize();
    }


    /// <summary>
    /// Clear all pools.
    /// </summary>
    [UsedImplicitly]
    private void OnDestroy()
    {
        poolManager.Clear();
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(PoolObject prefab)
    {
        return Main.poolManager.Pop(prefab);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(PoolObject prefab, float life)
    {
        return Main.poolManager.Pop(prefab, life);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="position">World position to give gameObject.</param>
    /// <param name="rotation">World rotation to give gameObject.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(PoolObject prefab, Vector3 position, Quaternion rotation)
    {
        return Main.poolManager.Pop(prefab, position, rotation);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="prefab">Type of gameObject to return.</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(PoolObject prefab, Vector3 position, Quaternion rotation, float life)
    {
        return Main.poolManager.Pop(prefab, position, rotation, life);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(int poolIndex)
    {
        return Main.poolManager.Pop(poolIndex);
    }


    /// <summary>
    /// Retrieve the next available gameObject from the corresponding pool. Creates a new pool if necessary.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to Pop.</param>
    /// <param name="life">Time in seconds till gameObject gets disabled.</param>
    /// <returns>Enabled gameObject.</returns>
    public static GameObject Pop(int poolIndex, float life)
    {
        return Main.poolManager.Pop(poolIndex, life);
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
        return Main.poolManager.Pop(poolIndex, position, rotation);
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
        return Main.poolManager.Pop(poolIndex, position, rotation, life);
    }

    #endregion
}