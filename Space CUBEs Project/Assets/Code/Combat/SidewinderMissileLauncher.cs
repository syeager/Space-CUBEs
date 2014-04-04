// Steve Yeager
// 3.25.2014

using UnityEngine;
using System.Collections;

/// <summary>
/// Missile with a twisted path.
/// </summary>
public class SidewinderMissileLauncher : Weapon
{
    #region Public Fields
    
    public GameObject Missile_Prefab;
    public Vector3[] missilePositions;
    public float missileDelay;
    public float missileSpeed;
    public float rotationSpeed;
    public float homingTime;
    public float damage;

    public int dummyTargets = 5;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (pressed)
        {
            StartCoroutine(Firing());
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator Firing()
    {
        WaitForSeconds wait = new WaitForSeconds(missileDelay);
        for (int i = 0; i < missilePositions.Length; i++)
        {
            yield return wait;
            PoolManager.Pop(Missile_Prefab, myTransform.position + myTransform.TransformDirection(missilePositions[i]), myTransform.rotation).
                GetComponent<SidewinderMissile>().Initialize(myShip, damage, missileSpeed, rotationSpeed, homingTime, dummyTargets, LevelManager.Main.player.transform);
        }
    }

    #endregion
}