// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.25
// Edited: 2014.06.25

using UnityEngine;
using System.Collections;

/// <summary>
/// Fires lasers in quick succession at the player.
/// </summary>
public class HornetCannon : Weapon
{
    #region Public Fields

    /// <summary>Laser prefab.</summary>
    public PoolObject laserPrefab;

    /// <summary>Local displacement for firing laser.</summary>
    public Vector3 cannon;

    /// <summary>Number of times to fire.</summary>
    public int fireCount = 3;

    /// <summary>Delay in seconds before firing startings.</summary>
    public float activateDelay;

    /// <summary>Delay in seconds after each shot.</summary>
    public float fireDelay;

    /// <summary>Damage each shot does.</summary>
    public float damage;

    /// <summary>Speed in m/s to fire lasers.</summary>
    public float laserSpeed;

    #endregion

    #region Weapon Overrides

    public override Coroutine Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            return StartCoroutine(Fire());
        }
        else
        {
            StopAllCoroutines();
            return null;
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        Transform player = LevelManager.Main.playerTransform;
        if (player == null) yield break;
        Vector3 direction = (player.position - myTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.back);

        yield return new WaitForSeconds(activateDelay);

        WaitForSeconds delay = new WaitForSeconds(fireDelay);
        for (int i = 0; i < fireCount; i++)
        {
            Prefabs.Pop(laserPrefab, myTransform.position + myTransform.TransformDirection(cannon), targetRotation).GetComponent<Hitbox>().Initialize(myShip, damage, direction * laserSpeed);
            yield return delay;
        }
    }

    #endregion
}