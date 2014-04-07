// Steve Yeager
// 3.8.2014

using UnityEngine;

public class CBombShockWave : Hitbox
{
    #region Public Fields
    
    public float strength;
    
    #endregion


    #region MonoBehaviour Overrides

    protected override void OnTriggerEnter(Collider other)
    {
        other.rigidbody.AddExplosionForce(strength, myTransform.position, 0f);
    }

    #endregion
}