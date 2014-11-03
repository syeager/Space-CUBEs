// Little Byte Games

using System.Collections;
using Annotations;
using LittleByte.Audio;
using LittleByte.Extensions;
using UnityEngine;

public class PlasmaCannon : PlayerWeapon
{
    #region Public Fields

    public PoolObject laserPrefab;
    public float damage;
    public Vector3 laserOffset;
    public float speed;

    #endregion

    #region Private Fields

    [SerializeField, UsedImplicitly]
    private AudioPlayer fireClip;

    #endregion

    #region Weapon Overrides

    public override Coroutine Activate(bool pressed, float multiplier = 1f)
    {
        if (pressed)
        {
            StartCoroutine(Fire(multiplier));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(CoolingDown(true, false));
        }

        return null;
    }


    public override PlayerWeapon Bake(GameObject parent)
    {
        var comp = parent.AddComponent<PlasmaCannon>();
        comp.index = index;
        comp.laserPrefab = laserPrefab;
        comp.cooldownTime = cooldownTime;
        comp.damage = damage;
        comp.laserOffset = laserOffset + myTransform.localPosition;
        comp.speed = speed;
        comp.fireClip = fireClip;

        return comp;
    }

    #endregion

    #region Private Methods

    private IEnumerator Fire(float multiplier)
    {
        while (true)
        {
            GameObject laser = Prefabs.Pop(laserPrefab);
            laser.transform.SetPosRot(myTransform.position + myTransform.TransformDirection(laserOffset), myTransform.rotation);
            laser.GetComponent<Hitbox>().Initialize(myShip, damage * multiplier, myTransform.forward * speed);

            AudioManager.Play(fireClip);

            ActivatedEvent.Fire(this);
            yield return StartCoroutine(CoolingDown(true, false));
        }
    }

    #endregion
}