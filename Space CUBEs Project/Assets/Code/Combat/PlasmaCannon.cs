// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.01
// Edited: 2014.06.13

using System.Collections;

using UnityEngine;

public class PlasmaCannon : Weapon
{
    #region Public Fields

    public PoolObject laserPrefab;
    public float damage;
    public Vector3 laserOffset;
    public float speed;

    #endregion

    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (pressed)
        {
            StartCoroutine(Fire(multiplier));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(Cooldown(true, false));
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<PlasmaCannon>();
        comp.index = index;
        comp.laserPrefab = laserPrefab;
        comp.cooldownTime = cooldownTime;
        comp.damage = damage;
        comp.laserOffset = laserOffset + myTransform.localPosition;
        comp.speed = speed;

        return comp;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float multiplier)
    {
        while (true)
        {
            GameObject laser = Prefabs.Pop(laserPrefab);
            laser.transform.SetPosRot(myTransform.position + myTransform.TransformDirection(laserOffset), myTransform.rotation);
            laser.GetComponent<Hitbox>().Initialize(myShip, damage * multiplier, myTransform.forward * speed);

            Activated();
            yield return StartCoroutine(Cooldown(true, false));
        }
    }

    #endregion
}