// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.19
// Edited: 2014.07.19

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class HelixLaser : Weapon
{
    #region Public Fields

    public PoolObject laserPrefab;

    public float damage;

    public float attackTime;

    public float frequency;

    public float amplitude;

    public float speed;

    public float fireRate;

    #endregion

    #region Weapon Overrides

    public override Coroutine Activate(bool pressed)
    {
        if (pressed)
        {
            return StartCoroutine(Fire());
        }

        StopAllCoroutines();
        return null;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        float cannon = 0f;
        float attackTimer = attackTime;
        for (float timer = 0; timer < attackTime; timer += deltaTime)
        {
            // attack
            if (attackTimer <= 0f)
            {
                attackTimer = attackTime;

                // fire 1
                Prefabs.Pop(laserPrefab, myTransform.position + myTransform.right * cannon, myTransform.rotation).AddComponent<Hitbox>().Initialize(myShip, damage, myTransform.forward * speed);
                // fire 2
                Prefabs.Pop(laserPrefab, myTransform.position + myTransform.right * -cannon, myTransform.rotation).AddComponent<Hitbox>().Initialize(myShip, damage, myTransform.forward * speed);
            }

            // move
            cannon = amplitude * (float)Math.Sin(timer * frequency);

            yield return null;
        }
    }

    #endregion
}