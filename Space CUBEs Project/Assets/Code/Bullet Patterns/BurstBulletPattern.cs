// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.03.27
// Edited: 2014.06.25

using UnityEngine;

/// <summary>
/// Fires bullets in a circle.
/// </summary>
public class BurstBulletPattern : Weapon
{
    #region Public Fields

    public PoolObject bulletPrefab;
    public AudioPlayer fireAudio;
    public int number;
    public float speed;
    public float damage;
    public Vector3 offset;
    public float startAngle;

    #endregion

    #region Weapon Overrides

    public override Coroutine Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            Fire();
        }

        return null;
    }

    #endregion

    #region Private Methods

    private void Fire()
    {
        float angle = 360f / number;
        Vector3 position = myTransform.position + myTransform.TransformDirection(offset);

        AudioManager.Play(fireAudio);

        for (int i = 0; i < number; i++)
        {
            Vector3 rotation = Utility.RotateVector(Vector3.left, angle * i + startAngle, Vector3.back);
            Prefabs.Pop(bulletPrefab, position, Quaternion.LookRotation(rotation, Vector3.back)).
                GetComponent<Hitbox>().Initialize(myShip, damage, rotation * speed);
        }
    }

    #endregion
}