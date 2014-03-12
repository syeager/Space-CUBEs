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
    public float spinSpeed;
    
    #endregion


    #region MonoBehaviour Overrides

    protected override void OnTriggerStay(Collider other)
    {
        Transform otherTransform = other.transform;

        // pull
        Vector3 distance = myTransform.position - otherTransform.position;
        float pull = ((pullRadius - distance.sqrMagnitude) / pullRadius * pullStrength);

        // move
        otherTransform.position -= distance.normalized * pull * deltaTime;

        // damage
        var oppHealth = other.gameObject.GetComponent<Health>();
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, new HitInfo { damage = hitInfo.damage * Mathf.Abs(pull) * deltaTime });
        }
    }


    private void OnDisabled()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Hitbox Overrides

    public override void Initialize(Ship sender, HitInfo hitInfo)
    {
        base.Initialize(sender, hitInfo);

        StartCoroutine(Spin());
    }

    #endregion

    #region Private Methods

    private IEnumerator Spin()
    {
        while (true)
        {
            myTransform.Rotate(Vector3.back, spinSpeed*deltaTime, Space.World);
            yield return null;
        }
    }
    
    #endregion
}