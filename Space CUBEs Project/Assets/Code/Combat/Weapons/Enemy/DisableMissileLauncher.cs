// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.16
// Edited: 2014.07.17

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class DisableMissileLauncher : Weapon
{
    #region Public Fields

    public DisableMissile missilePrefab;

    /// <summary>Time in seconds to disable a player's weapon.</summary>
    public float disableTime;

    /// <summary>Time in seconds to wait before firing more missiles.</summary>
    public float fireBuffer;

    /// <summary>Amount of damage to do.</summary>
    public float damage;

    /// <summary>Local offset to create missile.</summary>
    public Vector3 offset;

    #endregion

    #region Weapon Overrides

    public Coroutine Activate(bool pressed, int missileCount)
    {
        if (pressed)
        {
            return StartCoroutine(Fire(missileCount));
        }
        else if (gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
        }

        return null;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(int missileCount)
    {
        WaitForSeconds wait = new WaitForSeconds(fireBuffer);
        for (int i = 0; i < missileCount; i++)
        {
            Prefabs.Pop(missilePrefab.myPoolObject, myTransform.position + myTransform.TransformDirection(offset), myTransform.rotation).
                    GetComponent<DisableMissile>().Initialize(myShip, damage, disableTime);

            yield return wait;
        }
    }

    #endregion
}