// Steve Yeager
// 12.13.2013

using UnityEngine;

public class LaserBeam : Weapon
{
    #region Public Fields

    public string attackName;
    public HitInfo hitInfo;
    public Vector3 laserOffset;

    #endregion

    #region Private Fields

    private GameObject laser;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed)
    {
        if (pressed)
        {
            laser = PoolManager.Pop(attackName);
            laser.transform.SetPosRot(myTransform.position + myTransform.TransformDirection(laserOffset), myTransform.rotation);
            laser.transform.parent = myTransform;
            laser.GetComponent<Hitbox>().Initialize(myShip, hitInfo);
        }
        else
        {
            laser.GetComponent<PoolObject>().Disable();
        }
    }

    #endregion
}