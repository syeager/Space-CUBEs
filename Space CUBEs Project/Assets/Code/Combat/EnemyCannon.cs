// Steve Yeager
// 3.27.2014

using UnityEngine;

/// <summary>
/// Fires shot at player.
/// </summary>
public class EnemyCannon : Weapon
{
    #region Public Fields
    
    public GameObject Bullet_Prefab;
    public float speed;
    public float damage;
    public Vector3 offset;
    public float life;
    
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
        Vector3 position = myTransform.position + myTransform.TransformDirection(offset);
        PoolManager.Pop(Bullet_Prefab, position, myTransform.rotation, life).
            GetComponent<Hitbox>().Initialize(myShip, damage, (LevelManager.Main.playerTransform.position - position).normalized*speed);
    }

    #endregion
}