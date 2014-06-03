﻿// Steve Yeager
// 2.202.2014

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class BlackHoleMissile : Hitbox
{
    #region Public Fields
    
    public GameObject BlackHole_Prefab;
    public float explosionTime;
    
    #endregion


    #region Public Methods

    public void Explode()
    {
        PoolManager.Pop(BlackHole_Prefab, myTransform.position, myTransform.rotation).GetComponent<BlackHole>().Initialize(sender, damage, explosionTime);
        GetComponent<PoolObject>().Disable();
    }

    #endregion
}