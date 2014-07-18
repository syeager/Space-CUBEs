// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.26
// Edited: 2014.06.25

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

    public AnimationClip deployClip;
    public AnimationClip retractClip;
    public AudioPlayer fireAudio;

    #endregion

    #region Private Fields

    private AudioPlayer currentPlayer;

    #endregion

    #region Weapon Overrides

    public Coroutine Activate(bool pressed, float deployTime)
    {
        if (pressed)
        {
            gameObject.SetActive(true);
            StartCoroutine(Fire(deployTime));
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

    private IEnumerator Fire(float deployTime)
    {
        // deploy
        animation.Play(deployClip);
        yield return new WaitForSeconds(deployTime);

        // fire
        laser.SetActive(true);
        ((Hitbox)laser.GetComponent(typeof(Hitbox))).Initialize(myShip, damage);
        currentPlayer = AudioManager.Play(fireAudio);

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
    }


    private IEnumerator Retract()
    {
        laser.SetActive(false);
        if (currentPlayer != null)
        {
            currentPlayer.Stop();
        }
        animation.Play(retractClip);
        yield return new WaitForSeconds(retractClip.length);
        gameObject.SetActive(false);
    }

    #endregion
}