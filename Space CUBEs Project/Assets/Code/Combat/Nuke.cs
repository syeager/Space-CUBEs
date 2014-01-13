// Steve Yeager
// 1.14.2014

using System.Collections;
using UnityEngine;

public class Nuke : Weapon
{
    #region Public Fields

    public GameObject Nuke_Prefab;
    public GameObject Explosion_Prefab;
    public Vector3 attackOffset;
    public float speed;
    public float explosionLength;
    public HitInfo hitInfo;

    #endregion

    #region Private Fields

    private Transform nuke;
    private GameObject explosion;

    #endregion


    #region Weapon Overrides

    public override void Activate(bool pressed)
    {
        // fire
        if (pressed)
        {
            nuke = ((GameObject)GameObject.Instantiate(Nuke_Prefab, myTransform.position + myTransform.TransformDirection(attackOffset), myTransform.rotation)).transform;
            StartCoroutine("MoveNuke");
        }
        // explode
        else
        {
            StopCoroutine("MoveNuke");
            explosion = (GameObject)Instantiate(Explosion_Prefab, nuke.position, nuke.rotation);
            explosion.GetComponent<Hitbox>().Initialize(myShip, hitInfo);
            power = 0f;
            Destroy(nuke.gameObject);
            InvokeAction(() => Destroy(explosion), explosionLength);
            StartCoroutine(Cooldown());
        }       
    }


    public override Weapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<Nuke>();
        comp.index = index;
        comp.cooldownSpeed = cooldownSpeed;
        comp.Nuke_Prefab = Nuke_Prefab;
        comp.Explosion_Prefab = Explosion_Prefab;
        comp.attackOffset = attackOffset;
        comp.speed = speed;
        comp.hitInfo = hitInfo;
        comp.explosionLength = explosionLength;

        return comp;
    }

    #endregion

    #region Private Methods

    private IEnumerator MoveNuke()
    {
        Vector3 vector = myShip.transform.forward;
        while (true)
        {
            nuke.position += vector * speed * deltaTime;
            yield return null;
        }
    }

    #endregion
}