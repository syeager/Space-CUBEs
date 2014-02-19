// Steve Yeager
// 12.13.2013

using System;
using System.Collections;
using UnityEngine;

public class LaserBeam : Weapon
{
    #region Public Fields

    public GameObject Laser_Prefab;
    public GameObject Charge_Prefab;
    public string attackName;
    public Vector3 attackOffset;
    public HitInfo hitInfo;
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

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            if (power > 0)
            {
                StopCoroutine("Charge");
                StartCoroutine("Fire", multiplier);
            }
        }
        else
        {
            StopCoroutine("Fire");
            EndAttack();
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<LaserBeam>();
        comp.index = index;
        comp.cooldownSpeed = cooldownSpeed;
        comp.Laser_Prefab = Laser_Prefab;
        comp.Charge_Prefab = Charge_Prefab;
        comp.attackName = attackName;
        comp.hitInfo = hitInfo;
        comp.attackOffset = attackOffset+transform.localPosition;
        comp.chargeSpeed = chargeSpeed;
        comp.chargeTime = chargeTime;
        comp.maxSize = maxSize;
        comp.attackTime = attackTime;
        comp.maxRange = maxRange;

        return comp;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float multiplier)
    {
        // create charge
        charge = ((GameObject)Instantiate(Charge_Prefab)).transform;
        charge.parent = myTransform;
        charge.SetPosRot(myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation);
        
        // increase charge
        float timer = chargeTime;
        while (timer > 0f)
        {
            timer -= deltaTime;
            charge.localScale += Vector3.one*chargeSpeed*deltaTime;
            yield return null;
        }
        Destroy(charge.gameObject);

        // fire
        laser = ((GameObject)Instantiate(Laser_Prefab)).transform;
        laser.parent = myTransform;
        laser.SetPosRot(myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation);
        laser.GetComponent<Hitbox>().Initialize(myShip, hitInfo.MultiplyDamage(multiplier));
        while (power > 0f)
        {
            power -= FULLPOWER/attackTime*deltaTime;
            float size = maxSize * power / FULLPOWER;
            if (Physics.Raycast(laser.position, myTransform.forward, out rayInfo, maxRange) && rayInfo.collider.GetComponent<Ship>() != null)
            {
                laser.localScale = new Vector3(size, size, rayInfo.distance); // change width
            }
            else
            {
                laser.localScale = new Vector3(size, size, maxRange); // change width
            }
            yield return null;
        }
        power = 0f;
        EndAttack();
    }


    private void EndAttack()
    {
        if (charge != null) Destroy(charge.gameObject);
        else if (laser != null) Destroy(laser.gameObject);
        else return;

        if (ActivatedEvent != null)
        {
            ActivatedEvent(this, EventArgs.Empty);
        }
        StartCoroutine("Charge");
    }

    #endregion
}