// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.02.20
// Edited: 2014.06.13

using LittleByte.Pools;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BlackHoleMissile : Hitbox
{
    #region Public Fields

    public PoolObject blackHolePrefab;
    public float explosionTime;

    #endregion

    #region Public Methods

    public void Explode()
    {
        Prefabs.Pop(blackHolePrefab, myTransform.position, myTransform.rotation).GetComponent<BlackHole>().Initialize(sender, damage, explosionTime);
        GetComponent<PoolObject>().Disable();
    }

    #endregion
}