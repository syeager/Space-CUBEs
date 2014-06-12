// Steve Yeager
// 3.27.2014

using UnityEngine;

/// <summary>
/// Fires shot at player.
/// </summary>
public class EnemyCannon : Weapon
{
    #region Public Fields
    
    public GameObject bulletPrefab;
    public float speed;
    public float damage;
    public Vector3 offset;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier, object attackInfo = null)
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
        Vector3 position = myTransform.position + myTransform.TransformDirection(offset);
        PoolManager.Pop(bulletPrefab, position, myTransform.rotation).
            GetComponent<Hitbox>().Initialize(myShip, damage, (LevelManager.Main.playerTransform.position - position).normalized*speed);
    }

    #endregion
}