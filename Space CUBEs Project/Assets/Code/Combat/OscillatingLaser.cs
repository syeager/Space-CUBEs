// Steve Yeager
// 3.25.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// Laser that moves up and down.
/// </summary>
public class OscillatingLaser : Weapon
{
    #region Public Fields

    public Transform laser;
    public int cycles;
    public float speed;
    public float time;
    public float damage;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            //gameObject.SetActive(true); // TODO: should always be active before calling
            StartCoroutine(Fire());
        }
        else
        {
            StopAllCoroutines();
            laser.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire()
    {
        // reset laser
        laser.rotation = myTransform.rotation;
        laser.gameObject.SetActive(true);
        ((Hitbox)laser.GetComponent(typeof(Hitbox))).Initialize(myShip, damage);

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

        ((PoolObject)laser.GetComponent(typeof(PoolObject))).Disable();
    }

    #endregion
}