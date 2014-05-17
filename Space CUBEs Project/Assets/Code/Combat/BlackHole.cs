﻿// Steve Yeager
// 2.20.2014

using Annotations;
using UnityEngine;
using System.Collections;

/// <summary>
/// Sucks ships and other objects into its center while dealing damage.
/// </summary>
public class BlackHole : Hitbox
{
    #region Public Fields
    
    public float pullStrength;
    public float spinSpeed;
    
    #endregion


    #region MonoBehaviour Overrides

    protected override void OnTriggerStay(Collider other)
    {
        Rigidbody otherRigidbody = other.rigidbody;
        if (otherRigidbody == null) return;

        // pull
        Vector3 distance = myTransform.position - otherRigidbody.position;

        // move
        otherRigidbody.AddForce(distance.normalized * pullStrength * deltaTime, ForceMode.Impulse);

        // damage
        var oppHealth = other.gameObject.GetComponent(typeof(Health)) as Health;
        if (oppHealth != null)
        {
            oppHealth.RecieveHit(sender, damage * deltaTime);
        }
    }


    [UsedImplicitly]
    private void OnDisabled()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Hitbox Overrides

    public override void Initialize(Ship sender, float damage)
    {
        base.Initialize(sender, damage);

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