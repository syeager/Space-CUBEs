// Steve Yeager
// 2.20.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class BlackHole : Hitbox
{
    #region Public Fields
    
    public float pullRadius;
    public float pullStrength;
    
    #endregion


    #region Hitbox Overrides

    protected override void OnTriggerStay(Collider other)
    {
        Transform otherTransform = other.transform;

        // pull
        Vector3 distance = myTransform.position - otherTransform.position;
        float pull = ((pullRadius-distance.sqrMagnitude) / pullRadius * pullStrength);
                
        // move
        otherTransform.position -= distance.normalized * pull * deltaTime;

        // damage
        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, new HitInfo { damage = hitInfo.damage * Mathf.Abs(pull) * deltaTime });
        }
    }

    #endregion
}