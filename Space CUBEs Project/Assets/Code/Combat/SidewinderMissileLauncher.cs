// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.25
// Edited: 2014.06.11

using UnityEngine;
using System.Collections;

/// <summary>
/// Missile with a twisted path.
/// </summary>
public class SidewinderMissileLauncher : Weapon
{
    #region Public Fields

    public GameObject Missile_Prefab;
    public Vector3[] missilePositions;
    public float missileDelay;
    public float missileSpeed;
    public float rotationSpeed;
    public float homingTime;
    public float damage;

    /// <summary>Audio clip to play when a missile fires.</summary>
    public AudioClip fireClip;

    public int dummyTargets = 5;

    #endregion

    #region Const Fields

    /// <summary>Animation of missile laucher moving into place.</summary>
    private const string DeployAnim = "SideWeaponDeploy";

    /// <summary>Animation of missiles firing.</summary>
    private const string FireAnim = "SidewinderFire";

    #endregion

    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (pressed)
        {
            gameObject.SetActive(true);
            StartCoroutine(Fire((float)attackInfo));
        }
        else
        {
            StopAllCoroutines();
            animation.PlayReverse(DeployAnim, true);
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float deployTime)
    {
        // deploy
        animation.PlayReverse(DeployAnim, false);
        yield return new WaitForSeconds(deployTime);

        // create missiles
        WaitForSeconds wait = new WaitForSeconds(missileDelay);
        foreach (Vector3 position in missilePositions)
        {
            PoolManager.Pop(Missile_Prefab, myTransform.position + myTransform.TransformDirection(position), myTransform.rotation).
                GetComponent<SidewinderMissile>().Initialize(myShip, damage, missileSpeed, rotationSpeed, homingTime, dummyTargets, LevelManager.Main.player.transform);

            audio.Play(fireClip);
            animation.Play(FireAnim);
            
            yield return wait;

            animation.Stop();
            audio.Stop();
        }
    }

    #endregion
}