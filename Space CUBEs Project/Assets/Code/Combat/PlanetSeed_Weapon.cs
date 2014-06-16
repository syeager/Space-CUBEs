// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.02.19
// Edited: 2014.06.13


using UnityEngine;

public class PlanetSeed_Weapon : Weapon
{
    #region Public Fields

    public PoolObject starPrefab;
    public float damage;
    public float speed;
    public float time;
    public Vector3 attackOffset;

    #endregion

    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
    {
        if (!pressed) return;

        Prefabs.Pop(starPrefab, myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation).GetComponent<Hitbox>().Initialize(myShip, damage * multiplier, time, myTransform.forward * speed);
        StartCoroutine(Cooldown(true));
        Activated();
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<PlanetSeed_Weapon>();
        comp.index = index;
        comp.cooldownTime = cooldownTime;
        comp.starPrefab = starPrefab;
        comp.damage = damage;
        comp.speed = speed;
        comp.time = time;
        comp.attackOffset = attackOffset + myTransform.localPosition;

        return comp;
    }

    #endregion
}