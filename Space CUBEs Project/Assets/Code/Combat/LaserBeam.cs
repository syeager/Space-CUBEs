// Steve Yeager
// 12.13.2013

using System.Collections;
using UnityEngine;

public class LaserBeam : Weapon
{
    #region Public Fields

    public string attackName;
    public HitInfo hitInfo;
    public Vector3 laserOffset;
    public float recharge;
    public float rechargeDelay;
    public float discharge;

    #endregion

    #region Private Fields

    private GameObject laser;
    public float charge = MAXCHARGE;

    #endregion

    #region Const Fields

    private const float MAXCHARGE = 100f;

    #endregion


    #region Weapon Overrides

    public override bool CanActivate()
    {
        return base.CanActivate() && charge > 0f;
    }


    public override void Activate(bool pressed)
    {
        if (pressed)
        {
            laser = PoolManager.Pop(attackName);
            laser.transform.SetPosRot(myTransform.position + myTransform.TransformDirection(laserOffset), myTransform.rotation);
            laser.transform.parent = myTransform;
            laser.GetComponent<Hitbox>().Initialize(myShip, hitInfo, MAXCHARGE/discharge);
            StartCoroutine("Discharge");
        }
        else
        {
            Disable();
        }
    }

    #endregion

    #region Private Methods

    private void Disable()
    {
        laser.GetComponent<PoolObject>().Disable();
        StopCoroutine("Discharge");
        StartCoroutine("Recharge");
    }


    private IEnumerator Discharge()
    {
        while (charge > 0f)
        {
            charge -= discharge * deltaTime;
            yield return null;
        }
        charge = 0f;
        Disable();
    }


    private IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeDelay);
        while (charge < MAXCHARGE)
        {
            charge += recharge * deltaTime;
            yield return null;
        }
        charge = MAXCHARGE;
    }

    #endregion
}