// Steve Yeager
// 3.9.2014

using UnityEngine;
using System.Collections;

public class CBombExplosion : Hitbox
{
    #region Public Fields
    
    public GameObject shockwave;
    public float radius;
    
    #endregion


    #region Hitbox Overrides

    public void Initialize(Ship sender, HitInfo hitInfo, float growTime, float explosionTime, float shrinkTime)
    {
        Initialize(sender, hitInfo);
        shockwave.layer = gameObject.layer;

        // reset
        collider.enabled = true;
        shockwave.collider.enabled = true;

        StartCoroutine(Explode(growTime, explosionTime, shrinkTime));
    }

    #endregion

    #region Private Methods

    private IEnumerator Explode(float growTime, float explosionTime, float shrinkTime)
    {
        // grow
        myTransform.localScale = Vector3.zero;
        float growSpeed = radius/growTime;
        while (growTime > 0f)
        {
            growTime -= deltaTime;
            myTransform.localScale += Vector3.one*growSpeed*deltaTime;
            yield return null;
        }

        // hold
        yield return new WaitForSeconds(explosionTime);
        collider.enabled = false;
        shockwave.collider.enabled = false;

        // shrink
        float shrinkSpeed = radius / shrinkTime;
        while (shrinkTime > 0f)
        {
            shrinkTime -=  deltaTime;
            myTransform.localScale -= Vector3.one*shrinkSpeed*deltaTime;
            yield return null;
        }

        myPoolObject.Disable();
    }

    #endregion
}