// Steve Yeager
// 1.14.2014

using System;
using System.Collections;
using UnityEngine;

public class CBomb_Weapon : Weapon
{
    #region Public Fields

    public GameObject CBomb_Prefab;
    public Vector3 attackOffset;
    public float time = 4f;
    public float speed;
    public float damage;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (!pressed) return;

         // replace with pool
        GameObject bomb = PoolManager.Pop(CBomb_Prefab, myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation);
        bomb.GetComponent<Hitbox>().Initialize(myShip, damage*multiplier, time, myTransform.forward * speed);
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
        comp.cooldownSpeed = cooldownSpeed;
        comp.CBomb_Prefab = CBomb_Prefab;
        comp.attackOffset = attackOffset + myTransform.localPosition;
        comp.time = time;
        comp.speed = speed;
        comp.damage = damage;

        return comp;
    }

    #endregion
}