// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.05
// Edited: 2014.08.05

using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class EMPBlaster : Weapon
{
    #region Public Fields

    public Transform energyBall;
    public PoolObject blastPrefab;

    public float chargeTime;
    public float chargeSpeed;

    public float damage;
    public float spreadSpeed;
    public float attackTime;

    #endregion

    #region Weapon Overrides

    public override Coroutine Activate(bool pressed)
    {
        if (pressed)
        {
            return StartCoroutine(Fire());
        }
        else
        {
            // deactivate
            return null;
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        // charge
        energyBall.localScale = Vector3.one;
        energyBall.gameObject.SetActive(true);

        for (float timer = 0f; timer < chargeTime; timer += deltaTime)
        {
            energyBall.localScale += Vector3.one * chargeSpeed * deltaTime;
            yield return null;
        }

        // fire
        Prefabs.Pop(blastPrefab, myTransform.position, myTransform.rotation).GetComponent<EMPBlast>().Initialize(myShip, damage, attackTime, spreadSpeed);

        // cooldown
        yield return CoolDown();
    }

    #endregion
}