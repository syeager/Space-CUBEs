// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.31
// Edited: 2014.06.11

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class WeaponStacker : Weapon
{
    #region Public Fields

    public AudioPlayer deployAudio;
    public AnimationClip deployClip;
    public AudioPlayer retractAudio;
    public AnimationClip retractClip;

    public Weapon[] weapons;
    public float[] delays;

    #endregion

    #region Weapon Overrides

    public override void Initialize(Ship sender)
    {
        base.Initialize(sender);

        foreach (Weapon weapon in weapons)
        {
            weapon.Initialize(sender);
        }
    }


    public override Coroutine Activate(bool pressed)
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

    #region Protected Methods

    protected virtual IEnumerator Fire()
    {
        // deploy
        AudioManager.Play(deployAudio);
        animation.Play(deployClip);

        // fire
        for (int i = 0; i < weapons.Length; i++)
        {
            if (delays[i] > 0f)
            {
                yield return new WaitForSeconds(delays[i]);
            }

            weapons[i].Activate(true);
        }
    }


    private IEnumerator Retract()
    {
        animation.Play(retractClip);
        AudioManager.Play(retractAudio);
        yield return new WaitForSeconds(retractClip.length);
    }

    #endregion
}