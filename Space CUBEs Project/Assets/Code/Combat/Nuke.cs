// Steve Yeager
// 1.14.2014

using System;
using System.Collections;
using UnityEngine;

public class Nuke : Weapon
{
    #region Public Fields

    public GameObject Nuke_Prefab;
    public GameObject Explosion_Prefab;
    public Vector3 attackOffset;
    public float time = 4f;
    public float speed;
    public float explosionLength;
    public HitInfo hitInfo;

    #endregion

    #region Private Fields

    private Transform nuke;
    private GameObject explosion;
    private float multiplier;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (!pressed) return;

         // replace with pool
        this.multiplier = multiplier;
        nuke = ((GameObject)GameObject.Instantiate(Nuke_Prefab, myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation)).transform;
        nuke.GetComponent<Hitbox>().Initialize(myShip, HitInfo.Empty, time, myTransform.forward * speed, Detonate);
        StartCoroutine(Cooldown(true));

        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<Nuke>();
        comp.index = index;
        comp.cooldownSpeed = cooldownSpeed;
        comp.Nuke_Prefab = Nuke_Prefab;
        comp.Explosion_Prefab = Explosion_Prefab;
        comp.attackOffset = attackOffset + myTransform.localPosition;
        comp.time = time;
        comp.speed = speed;
        comp.hitInfo = hitInfo;
        comp.explosionLength = explosionLength;

        return comp;
    }

    #endregion

    #region Private Methods

    private void Detonate()
    {
        explosion = (GameObject)Instantiate(Explosion_Prefab, nuke.position, nuke.rotation);
        explosion.GetComponent<Hitbox>().Initialize(myShip, hitInfo.MultiplyDamage(multiplier));

        Destroy(nuke.gameObject);
        InvokeAction(() => Destroy(explosion), explosionLength);
    }

    #endregion
}