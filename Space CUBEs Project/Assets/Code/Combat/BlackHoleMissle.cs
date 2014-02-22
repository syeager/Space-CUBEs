// Steve Yeager
// 

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class BlackHoleMissle : Hitbox
{
    #region Public Fields
    
    public GameObject BlackHole_Prefab;
    public float explosionTime;
    
    #endregion


    #region Public Methods

    public void Explode()
    {
        (Instantiate(BlackHole_Prefab, myTransform.position, myTransform.rotation) as GameObject).GetComponent<BlackHole>().Initialize(sender, hitInfo, explosionTime);
        Destroy(gameObject);
    }

    #endregion
}