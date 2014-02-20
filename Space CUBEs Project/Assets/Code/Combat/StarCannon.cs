// Steve Yeager
// 

using UnityEngine;
using System.Collections;
using System;

public class StarCannon : Weapon
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

        (Instantiate(Star_Prefab, myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation) as GameObject).GetComponent<Hitbox>().Initialize(myShip, hitInfo.MultiplyDamage(multiplier), time, myTransform.forward*speed);
        StartCoroutine(Cooldown(true));
        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<StarCannon>();
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