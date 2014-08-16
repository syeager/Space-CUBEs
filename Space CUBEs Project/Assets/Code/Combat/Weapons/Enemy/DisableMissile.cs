// Space CUBEs Project-csharp
// Author: Steve Yeager
// Created: 2014.07.16
// Edited: 2014.07.16

using System;
using Annotations;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class DisableMissile : Hitbox
{
    #region Public Fields

    public float speed;
    public float freq;
    public float amp;

    #endregion

    #region Private Fields

    private Job disabledJob;
    private float disableTime;
    private Weapon disabledWeapon;

    #endregion

    #region MonoBehaviour Overrides

    [UsedImplicitly]
    private void Awake()
    {
        GetComponent<Health>().DieEvent += OnDieHandler;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StopAllCoroutines();

        var oppHealth = (Health)other.gameObject.GetComponent(typeof(Health));
        oppHealth.RecieveHit(sender, damage);

        disabledWeapon = other.GetComponent<Player>().Weapons.weapons[UnityEngine.Random.Range(0, Player.Weaponlimit)];
        if (disabledJob != null) disabledJob.Kill();
        disabledJob = new Job(Disable());

        myPoolObject.Disable();
    }

    #endregion

    #region Hitbox Overrides

    public void Initialize(Ship sender, float damage, float disableTime, int id)
    {
        this.disableTime = disableTime;
        base.Initialize(sender, damage);
        StartCoroutine(Move(id));
    }

    #endregion

    #region Private Methods

    private IEnumerator Disable()
    {
        disabledWeapon.Disable();
        yield return new WaitForSeconds(disableTime);
        disabledWeapon.Enable();
    }


    private IEnumerator Move(int id)
    {
        Vector3 straight = myTransform.forward;
        float timer = 0f;
        if (id % 2 == 0)
        {
            timer = 0.5f;
        }

        while (true)
        {
            float time = deltaTime;
            timer += time;

            myTransform.rotation = Quaternion.LookRotation(Utility.RotateVector(straight, amp * (float)Math.Cos(freq * timer), Vector3.back), Vector3.back);
            myTransform.position += myTransform.forward * speed * time;

            yield return null;
        }
    }

    #endregion

    #region Event Handlers

    private void OnDieHandler(object sender, DieArgs args)
    {
        myPoolObject.Disable();
    }

    #endregion
}