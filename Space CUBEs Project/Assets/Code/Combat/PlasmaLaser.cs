// Steve Yeager
// 12.01.2013

using System;
using UnityEngine;

public class PlasmaLaser : Weapon
{
    #region Public Fields

    public string attackName;
    public HitInfo hitInfo;
    public Vector3 laserOffset;
    public float speed;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed)
    {
        if (!pressed) return;

        var laser = PoolManager.Pop(attackName);
        laser.transform.SetPosRot(myTransform.position + myTransform.TransformDirection(laserOffset), myTransform.rotation);
        laser.GetComponent<Hitbox>().Initialize(myShip, hitInfo, myTransform.forward*speed);
        power = 0f;
        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
        StartCoroutine(Cooldown());
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<PlasmaLaser>();
        comp.index = index;
        comp.attackName = attackName;
        comp.cooldownSpeed = cooldownSpeed;
        comp.hitInfo = hitInfo;
        comp.laserOffset = laserOffset + transform.localPosition;
        comp.speed = speed;

        return comp;
    }

    #endregion
}