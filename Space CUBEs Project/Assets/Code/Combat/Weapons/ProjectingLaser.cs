// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.01
// Edited: 2014.07.01

using Annotations;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class ProjectingLaser : Hitbox
{
    #region Public Fields

    /// <summary>How fast the laser grows in m/s.</summary>
    public float projectSpeed;

    /// <summary>Max laser length.</summary>
    public float maxProjectionSize;

    #endregion

    #region Private Fields

    private Vector3 scale;
    private Vector3? contact;

    #endregion

    #region MonoBehaviour Overrides

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);

        contact = other.transform.position;
    }


    [UsedImplicitly]
    private void OnTriggerExit(Collider other)
    {
        contact = null;
    }

    #endregion

    #region Hitbox Overrides

    public override void Initialize(Ship sender, float damage)
    {
        scale = new Vector3(myTransform.localScale.x, myTransform.localScale.y, 0f);
        myTransform.localScale = scale;
        gameObject.SetActive(true);
        base.Initialize(sender, damage);
        StartCoroutine(Fire());
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Stop firing laser.
    /// </summary>
    public void Stop()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        Vector3 projectVector = Vector3.forward * projectSpeed;

        while (true)
        {
            if (contact != null)
            {
                scale = new Vector3(myTransform.localScale.x, myTransform.localScale.y, Vector3.Distance(myTransform.position, contact.Value));
                myTransform.localScale = scale;
            }
            else
            {
                if (scale.z < maxProjectionSize)
                {
                    scale += projectVector * deltaTime;
                    myTransform.localScale = scale;
                }
            }

            yield return null;
        }
    }

    #endregion
}