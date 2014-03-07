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
    public float explosionLength;    
    
    #endregion

    #region Private Fields

    private GameObject explosion;

    #endregion


    #region MonoBehavoiur Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Detonate();
    }

    #endregion

    #region Private Methods

    private void Detonate()
    {
        explosion = PoolManager.Pop(CBombExplosion_Prefab, myTransform.position, myTransform.rotation);
        explosion.GetComponent<Hitbox>().Initialize(sender, hitInfo, explosionLength);

        GetComponent<PoolObject>().Disable();
    }

    #endregion
}