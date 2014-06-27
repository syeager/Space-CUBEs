// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.25
// Edited: 2014.06.25

using UnityEngine;
using System.Collections;

/// <summary>
/// Missile with a twisted path.
/// </summary>
public class SidewinderMissileLauncher : Weapon
{
    #region Public Fields

    public PoolObject missilePrefab;
    public Vector3[] missilePositions;
    public float missileDelay;
    public float missileSpeed;
    public float rotationSpeed;
    public float homingTime;
    public float damage;

    public AnimationClip deployClip;
    public AnimationClip fireClip;
    public AnimationClip retractClip;

    /// <summary>Audio clip to play when a missile fires.</summary>
    public AudioPlayer fireAudio;

    public int dummyTargets = 5;

    #endregion

    #region Weapon Overrides

    public Coroutine Activate(bool pressed, float multiplier, float deployTime)
    {
        if (pressed)
        {
            gameObject.SetActive(true);
            StartCoroutine(Fire(deployTime));
        }
        else
        {
            StopAllCoroutines();
            animation.Play(retractClip);
        }

        return null;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float deployTime)
    {
        // deploy
        animation.Play(deployClip);
        yield return new WaitForSeconds(deployTime);

        // create missiles
        WaitForSeconds wait = new WaitForSeconds(missileDelay);
        foreach (Vector3 position in missilePositions)
        {
            animation.Stop();
            Prefabs.Pop(missilePrefab, myTransform.position + myTransform.TransformDirection(position), myTransform.rotation).
                GetComponent<SidewinderMissile>().Initialize(myShip, damage, missileSpeed, rotationSpeed, homingTime, dummyTargets, LevelManager.Main.player.transform);

            AudioManager.Play(fireAudio);
            animation.Play(fireClip);

            yield return wait;
        }
    }

    #endregion
}