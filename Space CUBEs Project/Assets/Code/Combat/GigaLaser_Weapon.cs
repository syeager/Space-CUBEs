// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2013.12.13
// Edited: 2014.06.13

using System;
using System.Collections;
using Annotations;

using UnityEngine;

public class GigaLaser_Weapon : Weapon
{
    #region Public Fields

    public PoolObject laserPrefab;
    public PoolObject chargePrefab;
    public string attackName;
    public Vector3 attackOffset;
    public float damage;
    public float chargeSpeed;
    public float chargeTime;
    public float maxSize;
    public float attackTime;
    public float maxRange;

    #endregion

    #region Private Fields

    private Transform laser;
    private Transform charge;
    private RaycastHit rayInfo;

    #endregion

    #region Weapon Overrides

    public override Coroutine Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (pressed)
        {
            if (power > 0)
            {
                StopAllCoroutines();
                StartCoroutine("Fire", multiplier);
            }
        }
        else
        {
            StopCoroutine("Fire");
            EndAttack();
        }

        return null;
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<GigaLaser_Weapon>();
        comp.index = index;
        comp.cooldownTime = cooldownTime;
        comp.laserPrefab = laserPrefab;
        comp.chargePrefab = chargePrefab;
        comp.attackName = attackName;
        comp.damage = damage;
        comp.attackOffset = attackOffset + transform.localPosition;
        comp.chargeSpeed = chargeSpeed;
        comp.chargeTime = chargeTime;
        comp.maxSize = maxSize;
        comp.attackTime = attackTime;
        comp.maxRange = maxRange;

        return comp;
    }

    #endregion

    #region Private Methods

    [UsedImplicitly]
    private IEnumerator Fire(float multiplier)
    {
        // create charge
        charge = Prefabs.Pop(chargePrefab).transform;
        charge.parent = myTransform;
        charge.SetPosRot(myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation);
        charge.localScale = Vector3.one;

        // increase charge
        float timer = chargeTime;
        while (timer > 0f)
        {
            timer -= deltaTime;
            charge.localScale += Vector3.one * chargeSpeed * deltaTime;
            yield return null;
        }
        charge.GetComponent<PoolObject>().Disable();

        // fire
        laser = Prefabs.Pop(laserPrefab).transform;
        laser.parent = myTransform;
        laser.SetPosRot(myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation);
        laser.GetComponent<Hitbox>().Initialize(myShip, damage * multiplier);
        while (power > 0f)
        {
            power -= FullPower / attackTime * deltaTime;
            float size = maxSize * power / FullPower;
            if (Physics.Raycast(laser.position, myTransform.forward, out rayInfo, maxRange) && rayInfo.collider.GetComponent<Health>() != null)
            {
                laser.localScale = new Vector3(size, size, rayInfo.distance);
            }
            else
            {
                laser.localScale = new Vector3(size, size, maxRange);
            }
            yield return null;
        }
        power = 0f;
        EndAttack();
    }


    private void EndAttack()
    {
        if (charge != null && charge.gameObject.activeSelf)
        {
            charge.GetComponent<PoolObject>().Disable();
        }
        else if (laser != null && laser.gameObject.activeSelf)
        {
            laser.GetComponent<PoolObject>().Disable();
        }
        else
        {
            return;
        }

        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
        StartCoroutine(Cooldown(false, false));
    }

    #endregion
}