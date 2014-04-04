// Steve Yeager
// 3.27.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class BurstBulletPattern : Weapon
{
    #region Public Fields
    
    public GameObject Bullet_Prefab;
    public int number;
    public float speed;
    public float damage;
    public Vector3 offset;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            Fire();
        }
    }

    #endregion

    #region Private Methods

    private void Fire()
    {
        float angle = 360f/number;
        Vector3 position = myTransform.position + myTransform.TransformDirection(offset);

        Vector3 rotation;

        for (int i = 0; i < number; i++)
        {
            rotation = Utility.RotateVector(Vector3.left, angle*i, Vector3.back);
            PoolManager.Pop(Bullet_Prefab, position, Quaternion.LookRotation(rotation, Vector3.back)).
                GetComponent<Hitbox>().Initialize(myShip, damage, rotation*speed);
        }
    }

    #endregion
}