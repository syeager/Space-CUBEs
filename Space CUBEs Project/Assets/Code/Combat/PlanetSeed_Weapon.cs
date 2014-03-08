﻿// Steve Yeager
// 

using UnityEngine;
using System.Collections;
using System;

public class PlanetSeed_Weapon : Weapon
{
    #region Public Fields
    
    public GameObject Star_Prefab;
    public HitInfo hitInfo;
    public float speed;
    public float time;
    public Vector3 attackOffset;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (!pressed) return;

        PoolManager.Pop(Star_Prefab, myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation).GetComponent<Hitbox>().Initialize(myShip, hitInfo.MultiplyDamage(multiplier), time, myTransform.forward*speed);
        StartCoroutine(Cooldown(true));
        Activated();
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<PlanetSeed_Weapon>();
        comp.index = index;
        comp.cooldownSpeed = cooldownSpeed;
        comp.Star_Prefab = Star_Prefab;
        comp.hitInfo = hitInfo;
        comp.speed = speed;
        comp.time = time;
        comp.attackOffset = attackOffset + myTransform.localPosition;

        return comp;
    }

    #endregion
}