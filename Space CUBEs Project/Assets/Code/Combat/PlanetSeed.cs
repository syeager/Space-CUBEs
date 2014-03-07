// Steve Yeager
// 2.19.2014

using UnityEngine;
using System.Collections;

public class PlanetSeed : Hitbox
{
    #region Public Fields
    
    public float growth;
    
    #endregion


    #region Hitbox Overrides

    public override void Initialize(Ship sender, HitInfo hitInfo, float time, Vector3 moveVec)
    {
        base.Initialize(sender, hitInfo, time, moveVec);

        GetComponent<Health>().Initialize();

        myTransform.localScale = Vector3.one;
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