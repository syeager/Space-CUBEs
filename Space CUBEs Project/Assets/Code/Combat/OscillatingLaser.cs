﻿// Steve Yeager
// 3.25.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// Laser that moves up and down.
/// </summary>
public class OscillatingLaser : Weapon
{
    #region Public Fields
    
    public GameObject Laser_Prefab;
    public Vector3 laserOffset;
    public int cycles;
    public float speed;
    public float time;
    public HitInfo hitInfo;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            gameObject.SetActive(true);
            StartCoroutine(Fire());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    public override Weapon Bake(GameObject parent)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        Transform laser = PoolManager.Pop(Laser_Prefab, myTransform.position + myTransform.InverseTransformDirection(laserOffset), myTransform.rotation).transform;
        laser.parent = myTransform;
        laser.GetComponent<Hitbox>().Initialize(myShip, hitInfo);

        float direction = 1f;
        float cycleTime = time/cycles;

        // first
        float timer = cycleTime/2f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            laser.Rotate(Vector3.back, direction * speed * Time.deltaTime, Space.World);

            yield return null;
        }

        direction *= -1f;

        for (int i = 0; i < cycles; i++)
        {
            timer = cycleTime;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                laser.Rotate(Vector3.back, direction * speed * Time.deltaTime, Space.World);

                yield return null;
            }

            direction *= -1f;
        }

        laser.GetComponent<PoolObject>().Disable();
    }

    #endregion
}