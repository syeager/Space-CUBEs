// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.01.12
// Edited: 2014.06.13

using System;

using UnityEngine;

public class CBomb_Weapon : Weapon
{
    #region Public Fields

    public PoolObject bombPrefab;
    public Vector3 attackOffset;
    public float time = 4f;
    public float speed;
    public float damage;

    #endregion

    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (!pressed) return;

        // replace with pool
        GameObject bomb = Prefabs.Pop(bombPrefab, myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation);
        bomb.GetComponent<Hitbox>().Initialize(myShip, damage * multiplier, time, myTransform.forward * speed);
        StartCoroutine(Cooldown(true));

        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<CBomb_Weapon>();
        comp.index = index;
        comp.cooldownTime = cooldownTime;
        comp.bombPrefab = bombPrefab;
        comp.attackOffset = attackOffset + myTransform.localPosition;
        comp.time = time;
        comp.speed = speed;
        comp.damage = damage;

        return comp;
    }

    #endregion
}