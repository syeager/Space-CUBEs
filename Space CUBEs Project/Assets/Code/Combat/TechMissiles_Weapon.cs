﻿// Little Byte Games

using System.Collections;
using LittleByte.Extensions;
using UnityEngine;

namespace SpaceCUBEs
{
    // Space CUBEs Project-csharp
    // Author: Steve Yeager
    // Created: 2014.03.09
    // Edited: 2014.06.25

    public class TechMissiles_Weapon : PlayerWeapon
    {
        #region Public Fields

        public PoolObject missilePrefab;
        public float damage;
        public Vector3[] launchPositions = new Vector3[8];
        public float delay;
        public float speed;

        #endregion

        #region Weapon Overrides

        public override Coroutine Activate(bool pressed, float multiplier)
        {
            if (!pressed) return null;

            return StartCoroutine(Fire(multiplier));
        }

        public override PlayerWeapon Bake(GameObject parent)
        {
            var comp = parent.AddComponent<TechMissiles_Weapon>();
            comp.index = index;
            comp.cooldownTime = cooldownTime;
            comp.missilePrefab = missilePrefab;
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
                Prefabs.Pop(missilePrefab, myTransform.TransformPoint(launchPositions[i]), myTransform.rotation).GetComponent<TechMissile>().Initialize(myShip, damage * multiplier, myTransform.forward * speed);
                yield return wait;
            }

            ActivatedEvent.Fire(this);
            StartCoroutine(CoolingDown(true));
        }

        #endregion
    }
}