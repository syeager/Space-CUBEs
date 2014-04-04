// Steve Yeager
// 3.3.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class CBomb : Hitbox
{
    #region Public Fields

    public GameObject CBombExplosion_Prefab;
    public float growTime;
    public float explosionLength;
    public float shrinkTime;
    
    #endregion


    #region MonoBehavoiur Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        if (disabled) return;

        base.OnTriggerEnter(other);

        Detonate();
    }

    #endregion

    #region Private Methods

    private void Detonate()
    {
        PoolManager.Pop(CBombExplosion_Prefab, myTransform.position, myTransform.rotation).GetComponent<CBombExplosion>().Initialize(sender, damage, growTime, explosionLength, shrinkTime);

        GetComponent<PoolObject>().Disable();
    }

    #endregion
}