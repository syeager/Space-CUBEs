﻿// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.11
// Edited: 2014.06.13

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

//namespace LittleByte.Pools
//{
    /// <summary>
    /// Singleton manager for all pools.
    /// </summary>
    [Serializable]
    public class PoolManager : ScriptableObject
    {
        #region Public Fields

        /// <summary>Should the PoolManager cull pools?</summary>
        public bool cull = true;

        /// <summary>Time in seconds between culling.</summary>
        public float cullDelay = 10f;

        /// <summary>List of pools.</summary>
        [SerializeField]
        public List<Pool> poolList = new List<Pool>();

        #endregion

        #region Private Fields

        /// <summary>Timer that runs culling on elapse.</summary>
        private Timer cullTimer;

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
            Clear();

            foreach (Pool pool in poolList)
            {
                pools.Add(pool.prefab, pool);
                pool.Initialize();
            }

            if (cull)
            {
                cullTimer = new Timer(cullDelay);
                cullTimer.Elapsed += (sender, args) => Cull();
            }
        }


        /// <summary>
        /// Clear all pools.
        /// </summary>
        public void Clear()
        {
            if (cullTimer != null)
            {
                cullTimer.Stop();
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
        /// <param name="parent"></param>
        /// <returns></returns>
        public Pool CreatePool(PoolObject prefab, Transform parent = null)
        {
            Pool pool = new Pool(this, prefab, parent);
            pools.Add(prefab, pool);
            poolList.Add(pool);
            return pool;
        }


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

                pool = new Pool(this, prefab);
                poolList.Add(pool);
                pools.Add(prefab, pool);
                Transform parent = new GameObject(prefab.name).transform;
                pool.Initialize(parent);
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

                pool = new Pool(this, prefab);
                poolList.Add(pool);
                pools.Add(prefab, pool);
                Transform parent = new GameObject(prefab.name).transform;
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

                pool = new Pool(this, prefab);
                poolList.Add(pool);
                pools.Add(prefab, pool);
                Transform parent = new GameObject(prefab.name).transform;
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

                pool = new Pool(this, prefab);
                poolList.Add(pool);
                pools.Add(prefab, pool);
                Transform parent = new GameObject(prefab.name).transform;
                pool.Initialize(parent);
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
    } 
//}