// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.04.01
// Edited: 2014.06.25

using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class DeathLaser : Weapon
{
    #region Public Fields

    public GameObject laser;
    public float damage;

    /// <summary>Time in seconds to delay firing after deploying.</summary>
    public float fireDelay;

    /// <summary>Deploying animation.</summary>
    public AnimationClip deployClip;

    /// <summary>Audio to play when deploying.</summary>
    public AudioPlayer deployAudio;

    /// <summary>Audio to play while charging.</summary>
    public AudioPlayer chargeAudio;

    /// <summary>Audio to play when firing.</summary>
    public AudioPlayer fireAudio;

    /// <summary>Retracting animation.</summary>
    public AnimationClip retractClip;

    /// <summary>Laser time in seconds.</summary>
    public float fireTime;

    #endregion

    #region Weapon Overrides

    public Coroutine Activate(bool pressed)
    {
        if (pressed)
        {
            gameObject.SetActive(true);
            return StartCoroutine(Fire());
        }
        else if (gameObject.activeInHierarchy)
        {
            laser.SetActive(false);
            animation.Play(retractClip);
            InvokeAction(() => gameObject.SetActive(false), retractClip.length);
        }

        return null;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        // deploy
        animation.Play(deployClip);
        yield return new WaitForSeconds(deployClip.length);

        // charge
        AudioManager.Play(chargeAudio);
        yield return new WaitForSeconds(chargeAudio.Length);

        // fire
        laser.GetComponent<Hitbox>().Initialize(myShip, damage);
        laser.SetActive(true);
        AudioPlayer player = AudioManager.Play(fireAudio);
        yield return new WaitForSeconds(fireTime);
        player.Stop();
        laser.SetActive(false);
    }

    #endregion
}