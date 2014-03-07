// Steve Yeager
// 12.01.2013

using System;
using System.Collections;
using UnityEngine;

public class PlasmaCannon : Weapon
{
    #region Public Fields

    public GameObject Laser_Prefab;
    public HitInfo hitInfo;
    public Vector3 laserOffset;
    public float fireRate;
    public float speed;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            StartCoroutine(Fire(multiplier));
        }
        else
        {
            StopAllCoroutines();
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<PlasmaCannon>();
        comp.index = index;
        comp.Laser_Prefab = Laser_Prefab;
        comp.cooldownSpeed = cooldownSpeed;
        comp.hitInfo = hitInfo;
        comp.laserOffset = laserOffset + myTransform.localPosition;
        comp.fireRate = fireRate;
        comp.speed = speed;

        return comp;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float multiplier)
    {
        WaitForSeconds time = new WaitForSeconds(1f/fireRate);
        while (true)
        {
            GameObject laser = PoolManager.Pop(Laser_Prefab);
            laser.transform.SetPosRot(myTransform.position + myTransform.TransformDirection(laserOffset), myTransform.rotation);
            laser.GetComponent<Hitbox>().Initialize(myShip, hitInfo.MultiplyDamage(multiplier), myTransform.forward * speed);

            yield return time;
        }
    }

    #endregion
}