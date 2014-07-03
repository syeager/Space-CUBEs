// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.06.27
// Edited: 2014.06.27

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class PlasmaMachineGun : Weapon
{
    #region Public Fields

    public PoolObject laserPrefab;
    public AnimationClip deployClip;
    public AnimationClip retractClip;

    /// <summary>Damage each laser inflicts.</summary>
    public float damage;

    /// <summary>Gun's fire rate in lasers/sec.</summary>
    public float fireRate;

    /// <summary>Time in seconds to fire gun.</summary>
    public float fireTime;

    /// <summary>Laser speed in m/s.</summary>
    public float laserSpeed;

    /// <summary>Local offset.</summary>
    public Vector3 laserOffset;

    /// <summary>Targeting angular speed.</summary>
    public float targetSpeed;

    #endregion

    #region Weapon Overrides

    public  Coroutine Activate(bool pressed)
    {
        if (pressed)
        {
            gameObject.SetActive(true);
            return StartCoroutine(Fire());
        }
        else if (gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            return StartCoroutine(Retract());
        }

        return null;
    }

    #endregion

    #region Private Methods

    private IEnumerator Target()
    {
        Transform player = LevelManager.Main.playerTransform;
        while (true)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.position - myTransform.position, myTransform.up);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, targetSpeed * deltaTime);
            yield return null;
        }
    }


    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(animation.Play(deployClip));

        StartCoroutine(Target());

        float delay = 1f / fireRate;
        WaitForSeconds wait = new WaitForSeconds(delay);
        for (float timer = 0; timer < fireTime; timer += delay)
        {
            Hitbox laser = (Hitbox)Prefabs.Pop(laserPrefab, myTransform.position + myTransform.TransformDirection(laserOffset), myTransform.rotation).GetComponent(typeof(Hitbox));
            laser.Initialize(myShip, damage, myTransform.forward * laserSpeed);
            yield return wait;
        }

        StopAllCoroutines();
    }


    private IEnumerator Retract()
    {
        yield return new WaitForSeconds(animation.Play(retractClip));
        gameObject.SetActive(false);
    }

    #endregion
}