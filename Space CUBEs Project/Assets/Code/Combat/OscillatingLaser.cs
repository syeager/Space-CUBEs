// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.26
// Edited: 2014.06.11

using Annotations;

using UnityEngine;
using System.Collections;

/// <summary>
/// Laser that moves up and down.
/// </summary>
public class OscillatingLaser : Weapon
{
    #region Public Fields

    public GameObject laser;
    public int cycles;
    public float speed;
    public float time;
    public float damage;

    #endregion

    #region Private Fields

    private Quaternion startRotation;

    #endregion

    #region Const Fields

    /// <summary>Animation of laser moving into place.</summary>
    private const string DeployAnim = "SideWeaponDeploy";

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Start()
    {
        startRotation = myTransform.rotation;
    }

    #endregion

    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (pressed)
        {
            gameObject.SetActive(true);
            animation.PlayReverse(DeployAnim, false);
            StartCoroutine(Fire((float)attackInfo));
        }
        else
        {
            StopAllCoroutines();
            laser.SetActive(false);
            animation.PlayReverse(DeployAnim, true);
            myTransform.rotation = startRotation;
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float deployTime)
    {
        // deploy
        animation.PlayReverse(DeployAnim, false);
        yield return new WaitForSeconds(deployTime);
        animation.Stop();

        // reset laser
        laser.SetActive(true);
        ((Hitbox)laser.GetComponent(typeof(Hitbox))).Initialize(myShip, damage);

        float direction = 1f;
        float cycleTime = time / cycles;

        // first
        float timer = cycleTime / 2f;
        while (timer > 0f)
        {
            timer -= deltaTime;

            myTransform.Rotate(Vector3.back, direction * speed * deltaTime, Space.World);

            yield return null;
        }

        direction *= -1f;

        for (int i = 0; i < cycles; i++)
        {
            timer = cycleTime;
            while (timer > 0f)
            {
                timer -= deltaTime;

                myTransform.Rotate(Vector3.back, direction * speed * deltaTime, Space.World);

                yield return null;
            }

            direction *= -1f;
        }

        ((PoolObject)laser.GetComponent(typeof(PoolObject))).Disable();
    }

    #endregion
}