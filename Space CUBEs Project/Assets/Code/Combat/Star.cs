// Steve Yeager
// 2.19.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Star : Hitbox
{
    #region Public Fields
    
    public float growth;
    
    #endregion


    #region MonoBehaviour Overrides

    //protected override void OnTriggerStay(Collider other)
    //{
    //    // damage
    //    base.OnTriggerStay(other);

    //    // movement

    //}

    #endregion

    #region Hitbox Overrides

    public override void Initialize(Ship sender, HitInfo hitInfo, float time, Vector3 moveVec, System.Action CollisionMethod = null)
    {
        base.Initialize(sender, hitInfo, time, moveVec, CollisionMethod);

        StartCoroutine(Grow());
    }

    #endregion

    #region Private Methods

    private IEnumerator Grow()
    {
        while (true)
        {
            myTransform.localScale += Vector3.one * growth * deltaTime;
            yield return null;
        }
    }

    #endregion
}