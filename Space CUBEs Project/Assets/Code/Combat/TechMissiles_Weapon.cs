// Steve Yeager
// 3.9.2014

using UnityEngine;
using System.Collections;

public class TechMissiles_Weapon : Weapon
{
    #region Public Fields
    
    public GameObject TechMissile_Prefab;
    public float damage;
    public Vector3[] launchPositions = new Vector3[8];
    public float delay;
    public float speed;
    
    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed, float multiplier)
    {
        if (!pressed) return;
        
        StartCoroutine(Fire(multiplier));
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<TechMissiles_Weapon>();
        comp.index = index;
        comp.cooldownSpeed = cooldownSpeed;
        comp.TechMissile_Prefab = TechMissile_Prefab;
        comp.damage = damage;
        comp.launchPositions = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            comp.launchPositions[i] = launchPositions[i] + myTransform.localPosition;
        }
        comp.delay = delay;
        comp.speed = speed;

        return comp;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float multiplier)
    {
        canActivate = false;

        WaitForSeconds wait = new WaitForSeconds(delay);

        for (int i = 0; i < 8; i++)
        {
            PoolManager.Pop(TechMissile_Prefab, myTransform.TransformPoint(launchPositions[i]), myTransform.rotation).GetComponent<Hitbox>().Initialize(myShip, damage * multiplier, myTransform.forward*speed);
            yield return wait;
        }

        Activated();
        StartCoroutine(Cooldown(true));
    }

    #endregion
}